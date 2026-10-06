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

**Discarding a chain is recoverable within the session.** The discarded working
copy is *not* deleted — it stays on disk at the path `applyModelApiCode` already
returned as `mutatedOmlPath`. Pass that path to `omlMerge` to bring the changes
back. That is how to undo an accidental `omlReset`, so **keep the last
`mutatedOmlPath` you were given.**

### Request

```jsonc
// agent → MCP server (call tool omlReset)
{ "eSpaceName": "MyModule" }
```

`eSpaceName` is **required** — use one of the `name` values from `listApps`.

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
- `eSpaceName` — echoed back, so a call targeting one of several open modules can
  be matched to its response.

Errors:

- `MODULE_NOT_LOADED` — `eSpaceName` is not open in Service Studio. (Earlier
  versions of this server reported that case in the response body; it is now an
  error, consistent with every other tool.)

Resetting a module that only holds a plain snapshot of the live model is a no-op
returning `false`. There is nothing there the live module doesn't already have,
and the snapshot is reused only while the module's change-counter still matches —
so any edit you make in the IDE meanwhile still causes a re-snapshot on the next
call, reset or no reset.

### How long the discarded copy lasts

Until the next Service Studio startup, which wipes the MCP temp folder
(`%TEMP%\ServiceStudio.MCPServer`). So recovery is a same-session affair: if a
reset was a mistake, merge the copy back before restarting Studio. Nothing is
archived beyond that.

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

When a reset discards changes the user might want back, tell them the copy is
still on disk and that `omlMerge` with the last `mutatedOmlPath` brings it back.
That is the difference between "your work is gone" and "your work is one call
away" — but it only holds until Studio restarts, so say so.

## `omlMerge`

Host-only MCP tool that merges the module named in `eSpaceName` with a mutated
OML on disk. Whether that opens Service Studio's Compare-and-Merge window for
the user to review, or is accepted with no dialog, is the user's own setting —
there is no argument for it and the agent has no say.

### Request

```jsonc
// agent → MCP server (call tool omlMerge)
{
  "eSpaceName":     "MyModule",
  "mutatedOmlPath":     "C:\\…\\<runId>-out.oml"
}
```

`eSpaceName` is **required**, same as `omlReset` — it decides which module the
mutated OML is compared against, rather than whichever module happens to be
active. Use one of the `name` values from `listApps`. `mutatedOmlPath` is
**required** too: pass the value the last successful `applyModelApiCode`
returned.

### Response

```jsonc
{ "merged": true, "message": "..." }
```

Always check the value of the response's `merged` field to verify whether the
merge completed successfully. Don't re-call `omlMerge` with the same
`mutatedOmlPath` afterward: a successful merge deletes that file, so a repeat
call fails with a file-not-found error that says nothing about whether the
first call worked.

Errors:

- `MODULE_NOT_LOADED` — `eSpaceName` is not open in Service Studio.
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
call), the task was read-only, or the user asked you to discard the chain
(`omlReset`).

### `omlMerge` clears that module's pointer

Once the merge resolves, `omlMerge` drops the pointer for the module you named,
so the next tool call re-snapshots the live module — which by then holds
whatever the merge accepted. That is what keeps the agent in sync with the
module after a merge; you do **not** need a follow-up `omlReset`.

Two consequences worth planning around:

- **The chain does not continue past a merge.** If the task has more legs, the
  next `applyModelApiCode` starts a *new* chain off the live module. Merge once
  at the end rather than merging intermediate states, unless you actually want
  the user's accepted state as your new baseline.
- **`omlMerge` blocks until the merge resolves, then drops the pointer.**
  When the user reviews it, the call does not return until they accept, reject,
  or close the Compare-and-Merge window — which may take minutes. When their
  setting allows it to be accepted with no dialog, it returns right away. Either
  way, by the time `omlMerge` returns, the pointer for that module is already
  cleared and the agent is back on the live module — regardless of whether
  `merged` came back `true` or `false`.
  Nothing is lost on disk when the merge doesn't succeed — the mutated OML stays
  at the `mutatedOmlPath` you passed, and passing it to `omlMerge` again puts the
  same changes back in front of the user. But note that only re-*surfaces* the
  work: there is no way to point the agent back at that file, so anything further
  you build starts a new chain off the live module. **Keep the last
  `mutatedOmlPath` even after merging** — it is the only handle on work the
  user has not accepted yet.

## `omlReset` vs `omlMerge` — when to choose which

Both tools end the chain for the module they name; they differ in whether the
work is surfaced to the user on the way out.

| User intent | Call |
|---|---|
| "Discard this chain; start fresh from the live module" | `omlReset eSpaceName=<module>` |
| "Surface this chain back into the IDE so I can accept it" | `omlMerge eSpaceName=<module> mutatedOmlPath=<path from the last apply>` |
| "That reset was a mistake — put it back" | `omlMerge eSpaceName=<module> mutatedOmlPath=<path from the last apply before the reset>` |
| "I've merged, now keep going on the same module" | nothing — the next call already re-snapshots the live module |
| Failed `applyModelApiCode` you want to retry | neither — the pointer is already unchanged |

## What happens on the first call after reset or merge

After `omlReset` or `omlMerge`, the next tool call observes an empty pointer for
that module and triggers the host's snapshot path: the module is serialised under
`%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml`, that path is fed to the sidecar
as the input, and (for `applyModelApiCode`) a fresh `<runId>-out.oml` is
allocated. From that point the chain auto-advances again on every successful save.

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
  (host-only) and returns `{ discardedChanges, eSpaceName }`. The discarded copy is
  left on disk, so the drop stays reversible via `omlMerge` until Studio restarts —
  which is why you should hold on to the last `mutatedOmlPath`.
- `omlMerge eSpaceName=<module> mutatedOmlPath=<path>` surfaces a chain into the
  IDE's merge UI (host-only) — pass it the `mutatedOmlPath` from the last
  successful apply; it's the default closing step, and re-passing it after a
  reset is how an accidental reset is undone.
- **`omlMerge` blocks until the merge resolves, then clears that module's
  pointer** — so the next call re-snapshots the live module (no follow-up
  `omlReset` needed), the chain does not continue past a merge, and a rejected
  merge leaves the agent on the live module too. Keep the last `mutatedOmlPath`
  regardless.
- Pointers are per module, so chains on different modules never interfere and
  each needs its own closing `omlMerge`.
- The silent-no-op and saved-but-invalid traps are the paths where the pointer
  advances to an undesired save — see [`lambda-contract.md`](lambda-contract.md).
