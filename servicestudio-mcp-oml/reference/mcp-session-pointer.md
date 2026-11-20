# MCP session pointer, `omlReset`, and `omlMerge`

Service Studio's in-process MCP host keeps a **per-module session pointer** at
the path of the last successful `applyModelApiCode` save. Successive tool calls
observe that pointer and reuse it as their input — so a chain of edits flows
through the host without the agent threading any intermediate paths. The first
call after the host has no pointer (fresh boot, open module just changed, or
right after `omlReset` or `omlMerge`) snapshots the live module from Service
Studio onto disk and uses that snapshot as the starting input.

The pointer is host-only state, kept per open module. The agent neither reads nor sets it explicitly —
but on a clean save `applyModelApiCode` returns the pointer's new path as
`mutatedOmlPath`, which is what you hand to `omlMerge`.

## The edit session holds the pointer

Writes need an **edit session**: `startOmlEditSession` checks out a working copy of the
module, and the pointer is that working copy for as long as the session is open. Every
read and apply on the module uses it. Merges don't end the session or reset the copy.
Only `endOmlEditSession` does, and so does `omlReset` when there are unmerged edits.
Lifecycle and rules: [`../SKILL.md`](../SKILL.md) § 2.1.

Outside a session, reads snapshot the live module and reuse the snapshot while the module
is unchanged. Parallel reads at the moment a new snapshot is taken race and fail with
`OML not found` ([`../SKILL.md`](../SKILL.md) § 2).

## Advance rules

The pointer only advances after `applyModelApiCode` runs successfully. "Success"
means **the host-allocated out file exists AND `exceptionMessage` is empty.**
Code that does not compile never runs, so it returns an MCP tool error instead
of a result and cannot advance the pointer. Read tools never advance it.

| Outcome of `applyModelApiCode` | `exceptionMessage` | `mutatedOmlPath` | Pointer | Next call reads from |
|---|---|---|---|---|
| Clean run | empty | path | **advances** | the new out file (auto-chain) |
| Runtime exception failure | populated (or surfaced as an MCP tool error) | `""` | unchanged | last successful out file, or the live module if no prior success |
| Compile error failure | *no result — an MCP tool error* | — | unchanged | last successful out file, or the live module if no prior success |
| Silent no-op (lambda never references `eSpace`) | empty | path | advances — to an **unmodified** OML | the silent-no-op path |
| Saved-but-invalid (`validationMessages` has a `type: "Error"` entry) | empty | path | advances — to an **invalid** OML | the invalid OML |

A failed call never corrupts the chain — the next attempt picks up from the
last-good state. Read tools (`getDataModel`, `getScreen`, …) follow the same
pointer so they see exactly what the last successful mutation produced. The
silent no-op and saved-but-invalid cases are where the pointer advances to a
save that doesn't reflect a valid intended change — defend by always mutating
`eSpace` and by scanning `validationMessages` for a `type: "Error"` entry (see
[`lambda-contract.md`](lambda-contract.md)).

> **`applyModelApiCode` returns `mutatedOmlPath`** — the saved path on a clean
> run, `""` otherwise. (The old `omlSaved` boolean is gone.) It mirrors the
> pointer's new position and is what you pass to `omlMerge`. Still verify a
> mutation actually landed by reading the model back (silent-no-op trap).

When the pointer references a file that no longer exists on disk, the host drops
the stale entry and re-snapshots the live module on the next call. Stale out files from superseded chain steps are
best-effort deleted as the pointer advances.

## `omlReset`

Host-only MCP tool that drops the pointer for the module named in `eSpaceName`.
Calling it forces the next tool invocation to re-snapshot that live module from
Service Studio.

**A reset can't be undone.** The discarded working copy stays on disk at the last
`mutatedOmlPath` until the next Service Studio startup wipes
`%TEMP%\ServiceStudio.MCPServer`, but `omlMerge` refuses it. `omlMerge` only accepts
the working copy the module currently tracks, and the reset removed that, so the call
fails with `INVALID_ARGUMENT`: `'mutatedOmlPath' does not match the working copy
currently tracked for module '…'.` This comes from reading the server code
(`OmlMergeTool`, 2026-11-20); it hasn't been verified live. **Merge anything the user
might want back before you reset.**

### Request

```jsonc
// agent → MCP server (call tool omlReset)
{ "input": { "eSpaceName": "MyModule" }, "sessionToken": "<token>" }
```

`eSpaceName` is **required**: an open module's `tabId`, `key` or `name` from
`listOpenModules` (the `name` from `listApps` on early contract 1 builds). See
[`module-discovery.md`](module-discovery.md).

### Response

```jsonc
{
  "discardedChanges": true,
  "eSpaceName":       "MyModule"
}
```

- `discardedChanges` — **`true` means an unmerged chain was thrown away.** `false`
  means there was nothing to throw away: the module has no chain in flight, so the
  call did nothing at all. `false` is **not** a failure — a failure comes back as
  an error (see below). Safe to report to the user either way.
- `eSpaceName` — the name of the module the reset acted on, whichever of its `tabId`,
  `key` or `name` you passed, so a call targeting one of several open modules can be
  matched to its response.

Errors:

- `MODULE_NOT_LOADED` — no open module matches `eSpaceName`. (Earlier
  versions of this server reported that case in the response body; it is now an
  error, consistent with every other tool.)
- `MODULE_AMBIGUOUS` — the key or name matches modules open in several tabs. Pass
  the `tabId` of the one you mean; the message lists them.

Resetting a module that only holds a plain snapshot of the live model is a no-op
returning `false`. There is nothing there the live module doesn't already have,
and the snapshot is reused only while the module's change-counter still matches —
so any edit you make in the IDE meanwhile still causes a re-snapshot on the next
call, reset or no reset.

### When to call it

Call `omlReset` to deliberately drop an in-flight mutation chain. Typical
situations:

- The user changes direction mid-task and wants the agent to start from the
  live module rather than from a partially-built chain.
- An `applyModelApiCode` call saved, but the result is unwanted and there is no
  clean "undo this" snippet to layer on top.
- Debugging the chain — verifying the agent works correctly against a fresh
  snapshot.

Do **not** call `omlReset` to recover from a failed `applyModelApiCode` —
failed calls already leave the pointer untouched (see the Advance rules table),
so the next attempt resumes from the last-good state automatically.

Before a reset that would discard changes the user might want, tell them they
will be gone for good, and offer to merge them first.

## `omlMerge`

Host-only MCP tool that merges the module named in `eSpaceName` with a mutated
OML on disk. Whether that opens Service Studio's Compare-and-Merge window for
the user to review, or is accepted with no dialog, is the user's own setting —
there is no argument for it and the agent has no say.

### Request

```jsonc
// agent → MCP server (call tool omlMerge)
{
  "input": {
    "eSpaceName":     "MyModule",
    "mutatedOmlPath": "C:\\…\\<runId>-out.oml"
  },
  "sessionToken": "<token>"
}
```

`eSpaceName` is **required**, same as `omlReset` — it decides which module the
mutated OML is compared against, rather than whichever module happens to be
active. Pass the same `tabId`, `key` or `name` you passed to `applyModelApiCode`. `mutatedOmlPath` is
**required** too: pass the value the last successful `applyModelApiCode`
returned.

### Response

```jsonc
{ "merged": true, "message": "..." }
```

Always check the value of the response's `merged` field to verify whether the
merge completed successfully. Don't re-call `omlMerge` with the same
`mutatedOmlPath` afterward to be sure. Outside an edit session a successful merge
also deletes that file, so a repeat call fails with a file-not-found error that says
nothing about whether the first call worked. Inside one, the file stays: it is the
working copy.

When the user reviews the merge, Studio brings the module's tab to the front so the
Compare-and-Merge window shows.

Errors:

- `MODULE_NOT_LOADED` / `MODULE_AMBIGUOUS` — as for `omlReset`.
- `INVALID_ARGUMENT` — `mutatedOmlPath` is empty, is not a file the MCP server
  itself produced, or no longer exists on disk.
- `INVALID_ARGUMENT` — `mutatedOmlPath` is not the working copy currently
  tracked for that module, or its contents no longer match what was recorded
  when the file was produced. You passed a path from an older chain, or edited
  the file yourself; re-read the `mutatedOmlPath` from the last successful
  `applyModelApiCode` response.
- `INVALID_ARGUMENT` — `mutatedOmlPath` "was removed by" or "was superseded by"
  a concurrent change to that module. Something else advanced the chain while
  you were merging. The message says what to do and it is the only recovery:
  call `applyModelApiCode` again, then merge the path *that* returns.

### Merging is the closing step

`omlMerge` requires a real, on-disk `mutatedOmlPath`, and `applyModelApiCode`
returns exactly that on every clean save. So the closing step of any mutating
task is: take the `mutatedOmlPath` from the **last** successful apply and call
`omlMerge` with it — do it without being asked (see [`../SKILL.md`](../SKILL.md)
§ 3.1). Skip it only when nothing was saved (`mutatedOmlPath: ""` on every
call), the task was read-only, or the user asked you to discard the changes
(`endOmlEditSession`). Then end the session with `endOmlEditSession`.

### `omlMerge` keeps the edit session open

A merge doesn't end the session or reset its working copy. After `merged: true` the
next `applyModelApiCode` builds on the same working copy, and its new
`mutatedOmlPath` is merged the same way, so an intermediate merge is a safe checkpoint.

`omlMerge` blocks until the merge resolves. When the user reviews it, the call does not
return until they accept, reject, or close the Compare-and-Merge window, which may take
minutes. When their setting allows it to be accepted with no dialog, it returns right
away.

When the merge doesn't succeed (`merged: false`), the working copy still holds your
edits and the file stays on disk. What to do depends on `message` (texts from the
server code, 2026-11-20):

| `message` | What to do |
|---|---|
| `Service Studio is busy — …` | Ask the user to close any open dialog, then merge the same path again. |
| `Sign in to an environment in Service Studio …` | Ask the user to sign in, then merge the same path again. |
| `The merge was cancelled in the Compare and Merge window.` | The user turned it down. Ask them how to proceed, and don't merge again on your own. |
| `The merge could not auto-resolve every change, so nothing was merged.` | An automatic merge hit conflicts. Tell the user and ask them how to proceed. |
| `The merge failed. See the log for details.` | Don't retry the same file. Follow the recovery in [`../SKILL.md`](../SKILL.md) § 3.1. |

If you fix the cause with another `applyModelApiCode`, merge the new `mutatedOmlPath`
it returns. `omlMerge` accepts only the latest one.

Reads in the session keep showing the working copy after a merge, never the live
module. `merged: true` doesn't tell you whether the user left some of your changes out
in the Compare-and-Merge window. The working copy still has them, so your next merge
proposes them again. If the user says they left something out, end the session and
start a new one to work from what they accepted.

## `endOmlEditSession`, `omlReset` and `omlMerge`: which to call

| User intent | Call |
|---|---|
| "Show me these changes so I can accept them" | `omlMerge eSpaceName=<module> mutatedOmlPath=<path from the last apply>` |
| "I've merged, now keep going on the same module" | nothing: keep applying in the same session |
| "Done with this module" | `endOmlEditSession eSpaceName=<module>` after the last `merged: true` |
| "Discard these changes" | `endOmlEditSession eSpaceName=<module>`. It deletes the working copy, so it can't be undone. `omlReset` discards the same way |
| "Discard them, but I may want them back" | Nothing discards recoverably. Say so, and offer to merge them instead |
| "That reset was a mistake — put it back" | Not possible: `omlMerge` refuses the discarded file. Offer to redo the edits |
| Failed `applyModelApiCode` you want to retry | neither — the working copy is already unchanged |

## What happens on the first call after reset or the session ends

After `endOmlEditSession`, or an `omlReset` that returned `discardedChanges: true`, the
module has no edit session. A read then sees the live module again: the host serialises
it under `%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml` and feeds that path to the
sidecar. `applyModelApiCode` is refused with `No OML edit session is started. Call
startOmlEditSession before making edits.` until you call `startOmlEditSession`, which
takes a new working copy of the live module. From then on the chain advances again on
every successful save.

An `omlReset` that returned `discardedChanges: false` changed nothing, so its session is
still open: keep working in it. Calling `startOmlEditSession` there fails with `An OML
edit session is already started for this module.`

## Cross-module behaviour

**Every open module keeps its own pointer.** Chains on different modules are
independent: reading or mutating module B leaves module A's in-flight chain
untouched, and `omlReset eSpaceName=A` drops only A's chain. So a task can hold
an open chain on several modules at once, and each one needs its own closing
`omlMerge`.

Because pointers are per module, a reset targets exactly the module you name —
which is why `eSpaceName` is required. Resetting one module never disturbs
another.

> Earlier versions of this server kept state for only **one** module at a time and
> evicted (and deleted) other modules' working copies on the next access, which
> silently destroyed an unmerged chain the moment the agent touched a second
> module. That no longer happens — but it means switching modules no longer
> implicitly drops the previous module's chain either. If you want a chain gone,
> reset it explicitly.

## Summary

- The pointer is invisible to the agent — pass `code` (+ optional `imports`) to
  `applyModelApiCode`, nothing more. Read tools take only their documented args
  (`objectName` for the by-name `get<Element>` family, `includeJson` for
  code-returning reads — always `"Never"`).
- Pointer advances when the out file was written and `exceptionMessage` is
  empty; otherwise it stays put. On a clean save the
  apply response returns that path as `mutatedOmlPath` (the old `omlSaved`
  boolean is gone).
- `omlReset eSpaceName=<module>` deliberately drops that module's chain
  (host-only) and returns `{ discardedChanges, eSpaceName }`. With `true`, the
  session has ended: call `startOmlEditSession` before the next apply. The discarded
  copy is left on disk, but `omlMerge` refuses it, so a reset can't be undone.
- `omlMerge eSpaceName=<module> mutatedOmlPath=<path>` surfaces a chain into the
  IDE's merge UI (host-only) — pass it the `mutatedOmlPath` from the last
  successful apply; it's the default closing step.
- **`omlMerge` blocks until the merge resolves and keeps the edit session open**,
  so you can keep applying and merging. `endOmlEditSession` ends the session and
  deletes the working copy.
- Pointers are per module, so chains on different modules never interfere and
  each needs its own closing `omlMerge`.
- The silent-no-op and saved-but-invalid traps are the paths where the pointer
  advances to an undesired save — see [`lambda-contract.md`](lambda-contract.md).
