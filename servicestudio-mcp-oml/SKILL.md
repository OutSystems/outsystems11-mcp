---
name: servicestudio-mcp-oml
description: >-
  Provides the C# Model API contract for Service Studio's in-process MCP
  Server — full `eSpace => { ... }` lambdas (no `eSpace.Save(...)`) that mutate
  the open `.oml` module via the `applyModelApiCode` MCP tool. Covers the
  full-lambda contract, the Model API surface (entities, attributes, screens,
  server / client actions, structures, roles), the sandbox,
  the per-module session pointer that auto-chains edits, the camelCase tool
  catalogue (`getDataModel`, `getScreen`, `runQuery`, `getValidationMessages`,
  `listApps`, …), the session token + `input` envelope, and the host-only
  `omlReset` / `omlMerge` / `omlRefreshReferences` / `omlPublish`
  tools. Use when calling `applyModelApiCode`, working with the Service Studio
  MCP oml tools, editing an open module via Service Studio MCP, editing or
  modifying `.oml`, writing Model API code, or asking to add an entity /
  attribute via the Model API. Not for runtime OutSystems platform debugging —
  strictly design-time OML mutation through the Model API.
---

# OutSystems Model API for `applyModelApiCode`

This skill teaches you to write C# Model API **lambdas** for the
`applyModelApiCode` MCP tool exposed by Service Studio's in-process MCP Server.
It also points you at an example corpus, namespace doc files, and end-to-end
integration walkthroughs. **Read this file fully** — the invocation contract
differs from ODC's published examples in one important way (see § 1).

> **Naming note.** This MCP server's tools use **camelCase, no `oml_` prefix**
> (`applyModelApiCode`, `getDataModel`, `omlMerge`, `listApps`, …). Earlier
> versions of this skill used `oml_`-prefixed snake_case names like
> `oml_apply_model_api_code` — those **no longer exist**; every tool name here
> is the exact camelCase name the host advertises.

> **Beta Feature (write tools).** The write capabilities this skill teaches — mutating
> `applyModelApiCode` lambdas, `omlMerge`, `omlReset`, `omlRefreshReferences` and
> `omlPublish` — are a Beta Feature: a non-final OutSystems capability provided to collect
> customer feedback. They can change significantly, including through breaking changes, or be
> discontinued. **The first time you use a write tool in a conversation, tell the user it's a
> Beta Feature and share https://www.outsystems.com/legal/beta-features-agreement.** The read tools are Generally Available and need no notice.

## Contents

- [When to Use](#when-to-use)
- [§ 0 — Session token and argument envelope](#0-session-token-and-argument-envelope)
- [§ 1 — The full-lambda contract](#1-the-full-lambda-contract--read-this-first)
- [§ 1.1 — The code sandbox](#11-the-code-sandbox)
- [§ 2 — Reading before writing](#2-reading-before-writing)
- [§ 3 — The `applyModelApiCode` invocation shape](#3-the-applymodelapicode-invocation-shape)
- [§ 3.1 — Finalising — auto-merge after the last mutation](#31-finalising--auto-merge-after-the-last-mutation)
- [§ 3.2 — Refreshing references and publishing](#32-refreshing-references-and-publishing)
- [§ 4 — Default imports](#4-default-imports)
- [§ 4.1 — Module style support](#41-module-style-support)
- [§ 5 — Common patterns (pointers)](#5-common-patterns-pointers)
- [§ 6 — Where to dig deeper](#6-where-to-dig-deeper)
- [§ 7 — Integration walkthroughs](#7-integration-walkthroughs)
- [§ 8 — Known gaps](#8-known-gaps)
- [Common Mistakes](#common-mistakes)
- [§ 9 — Security caveat](#9-security-caveat)

## When to Use

**Use this skill when:**

- Calling `applyModelApiCode` to mutate the open `.oml` module.
- Calling any of the `get*` read tools, `runQuery`, `listApps`,
  `getValidationMessages`, or the host-only `omlReset` / `omlMerge` tools.
- Writing C# Model API code — entities, attributes, screens, server / client
  actions, structures, roles, integrations — that targets `IESpace`.
- Reasoning about the per-module session pointer, the silent-no-op trap, or
  how the host wraps your lambda.

**Do NOT use for:**

- Runtime OutSystems platform debugging (this is strictly design-time OML
  mutation).
- Driving the standalone tool directly. The MCP host bundles and spawns the
  sidecar per call, but the contract you write to is the MCP tool's, described here.
- ODC-side OML editing — those tools wrap differently and surface a
  different verb catalogue.

## 0. Session token and argument envelope

1. **Call `createSessionToken` once, first**, passing `clientName` (your agent's
   name — shown to the user in Service Studio's approval dialog). The result is a
   token string. Keep it for the whole agent session and **reuse it after a
   reconnect** — never request a second one for the same session.
2. **Pass `sessionToken` as a top-level argument on every other call** —
   including `omlPublish`, `omlRefreshReferences` and `getRoleExceptions`:
   their schemas don't declare it, but send it anyway: `getRoleExceptions` rejects a call without it ("This tool requires a valid session token", verified 2026-10-01).
3. **Every tool except `listApps` takes its arguments wrapped in `input`:**

```jsonc
// applyModelApiCode
{ "input": { "eSpaceName": "MyModule", "code": "eSpace => { … }", "imports": [] }, "sessionToken": "<token>" }
// listApps — the only tool with no input object
{ "sessionToken": "<token>" }
```

Full per-tool shapes: [`reference/verb-reference.md`](reference/verb-reference.md).

## 1. The full-lambda contract — read this first

The `code` argument you pass to `applyModelApiCode` is a **full
`Action<IESpace>` lambda** — `eSpace => { … }` — but **without** the
`eSpace.Save(...)` call. The host rewrites your lambda before forwarding it to
the bundled sidecar's `applyModelApiCode` verb: it strips the trailing `}`
from your `code`, appends `eSpace.Save("<host-allocated-path>")`, and
re-closes the lambda. Specifically:

```csharp
// what the host literally builds
<your code, minus its final '}'>
eSpace.Save("<host-allocated-out-path>");
}
```

So when you call the tool, pass the complete lambda **ending in `}`**, with no
save:

```jsonc
// GOOD — full lambda, ends in '}', no Save call.
{
  "code": "eSpace => { var key = eSpace.Key.CreateNewKeyBasedOnThis(new object[] { \"Customer\" }); eSpace.CreateServerEntity(\"Customer\", key); }"
}
```

```jsonc
// BAD — calls eSpace.Save yourself. The host appends its own Save against a
// host-controlled path; yours is redundant, and because the host strips your
// final '}' the wrap can end up malformed.
{
  "code": "eSpace => { eSpace.CreateServerEntity(\"Customer\", key); eSpace.Save(\"out.oml\"); }"
}
```

```jsonc
// BAD — body-only statements, no 'eSpace => { ... }' wrapper. The host strips
// your last character and appends 'eSpace.Save(...);}', producing a dangling
// '}' with no lambda head → the compiler rejects it. (This was the OLD contract;
// it is now inverted.)
{
  "code": "eSpace.CreateServerEntity(\"Customer\", key);"
}
```

Four rules follow directly from how the host rewrites your code:

1. **Pass a full `eSpace => { ... }` lambda.** The host no longer wraps a bare
   body — it only injects the save and re-closes. A body-only snippet leaves a
   dangling `}` and fails to compile.
2. **Use the literal identifier `eSpace`** for the parameter. The injected
   `eSpace.Save(...)` is hardcoded to that name — `m`, `app`, `model`, or
   anything else fails with `CS0103: The name 'X' does not exist in the current context`.
3. **Your `code` must end with `}` and nothing after it** — no trailing
   newline, whitespace, or semicolon. The host removes the **last character**
   assuming it is the closing brace; a stray character there corrupts the wrap.
4. **Do NOT call `eSpace.Save(...)`.** The host appends one against a path it
   controls. The agent never names the output path.

Full discussion of the wrap mechanics + the silent-no-op trap:
[`reference/lambda-contract.md`](reference/lambda-contract.md).

## 1.1 The code sandbox

The host screens `code` and `imports` **before compiling** and rejects anything
outside the OutSystems Model API with a tool error:

- **`code`** must not mention `Console`, any `System.…`-qualified name
  (`System.Text.StringBuilder`, `System.Enum.GetNames`, …), or reflection
  (`GetProperty`, `GetMethod`, `Invoke`, …) — nor file, process, environment,
  registry, interop or network access. → `'code' must only reference the
  OutSystems Model API … (found 'X')`. Unqualified basics still work: `List<T>`,
  `Exception`, `string.Join`, LINQ, `Action<…>`, `obj.GetType().Name`.
- **`imports`** may only contain `OutSystems.Model` or its sub-namespaces →
  otherwise `Import 'X' is not allowed`. Write other types fully qualified in the
  code instead — `ServiceStudio.Plugin.NRWidgets.IInput` and
  `ServiceStudio.Plugin.RESTService.IRestService` are allowed there.
- Service Studio's internal `ServiceStudio.Model.*` types are not available.
- The pre-parser rejects C# 12 **collection expressions** (`[a, b]`) with
  `'code' is not valid C#: Invalid expression term '['` — write `new[] { a, b }`.

**To see values, throw them.** There is no `Console` output any more. Build a
string and end the lambda with `throw new Exception("PROBE:" + s);` — the text
comes back verbatim in `exceptionMessage`, nothing is saved, the session
pointer does not move, and `validationMessages` for the in-memory model are
**still returned** — so a throw-probe also previews TrueChange for an edit
without committing it. Template: [`examples/ProbeModelWithException.cs`](examples/ProbeModelWithException.cs).

Older recipes that reached members by reflection have typed replacements;
see [`reference/lambda-contract.md`](reference/lambda-contract.md) § "Sandbox".

**Reading an existing expression's text** (verified 2026-10-01): call `ToString()`
on any expression property (`widget.Value`, `widget.Visible`, `image.ImageContent`,
or a block-instance `arg.Value`). It returns `"<expression text> [Value[…]]"`;
cut the string at the first `" ["`. With this you can rewrite existing expressions
in place: read the text, transform it, then call `SetValue`, `SetArgumentValue` or
`SetVisible`. Run a probe that prints a few values first, because the suffix
differs by property (`[ValueExpression[…]]`, `[Visible[…]]`).
`obj.GetType().GetInterfaces()` also passes the sandbox. Use it to find which
interfaces a widget actually implements when a cast or member fails to compile.

## 2. Reading before writing

Before mutating an OML, **always** call `getDataModel` first. Its output is the
existing data model rendered as the same Model API dialect you have to emit —
pattern-match against it.

| Tool | When to use |
|---|---|
| `getDataModel` | Entities, attributes, foreign keys, identifier attributes (this module + entities reachable through references) |
| `getStructures` | All structures and their attributes |
| `getActionNames` + `getServerAction` / `getClientAction` / `getServiceAction` | `getActionNames` takes `objectType: "server" \| "client" \| "service"` to list top-level action names, then fetch one by name |
| `getScreenNames` + `getScreen` | Screen layout, widgets, screen actions |
| `getWebBlockNames` + `getWebBlock` | Web blocks |
| `getRoleNames` + `getRole` | Roles |
| `getClientVariables` / `getSessionVariables` / `getSettings` | Collection-only reads |
| `getEvents` / `getEventHandlers` / `getIntegrations` / `getTimers` / `getUserExceptions` / `getRoleExceptions` / `getLocales` | Other top-level element collections |
| `getException` / `getImage` / `getResource` / `getScript` / `getEmailTemplate` / `getExternalSite` / `getTheme` | Single named element |
| `getSerializedObject` / `getSerializedObjectByKey` / `getSerializedObjectByNameAndType` | Raw JSON serialization of an object (by current pointer, by key, or by name + fully-qualified type) |
| `runQuery` | Generic ModelQL query against the eSpace — use when no specific read tool fits. Query always starts with `Root { … }` |
| `getValidationMessages` | TrueChange validation messages against the current pointer's OML; optional `filter` of `info` / `warning` / `error` |
| `listApps` | The open module + its loaded references, each with `moduleType` (see § 4.1) |

Read tools auto-snapshot the live module on the first call and subsequently
follow the session pointer, so they always see the state of the last successful
mutation (see [`reference/mcp-session-pointer.md`](reference/mcp-session-pointer.md)).
Full catalogue with input shapes: [`reference/verb-reference.md`](reference/verb-reference.md).

> **`includeJson` is a required argument on the code-returning read tools**
> (`getDataModel`, `getStructures`, the `get<Element>` by-name reads, etc. —
> anything whose input is `GetModelObjectCodeInput` / `GetModelObjectByNameCodeInput`).
> Its declared values are `"Never"`, `"Always"`, `"IfReferenced"`, but **the
> host overrides whatever you send and forces `"Never"`** before calling the
> sidecar — so the value is effectively ignored, yet the field must still be
> present or the MCP call fails to deserialize. **Always pass `"Never"`.** The
> name-list tools (`getScreenNames`, `getRoleNames`, …), `listApps`,
> `runQuery`, and `getValidationMessages` take no `includeJson`. By-name reads
> (`getServerAction`, `getScreen`, …) name the element with `objectName`.

> **Read-back code is not replayable verbatim.** `getDataModel` and friends emit
> referenced elements as `eSpace.AddDependency(Services.ModelServices.ParseGlobalKey(…))`
> lines, but `AddDependency` throws inside `applyModelApiCode` even for an
> already-resident element (§ 8). Copy the *shape* of the calls, and resolve
> referenced elements through `eSpace.References.Named("…")`.

## 3. The `applyModelApiCode` invocation shape

```jsonc
// agent → MCP server (call tool applyModelApiCode)
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { /* statements that mutate eSpace */ }",
    "imports": ["OutSystems.Model.Data"]   // required field; may be []; OutSystems.Model.* only (§ 1.1)
  },
  "sessionToken": "<token from createSessionToken>"
}
```

What the host does behind that call:

1. Snapshots the open module to `%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml`.
2. Allocates `<runId>-out.oml` as the save destination.
3. Rewrites your lambda — strips the trailing `}`, appends
   `eSpace.Save("<runId>-out.oml")`, re-closes — and forwards it (plus merged
   `imports` and the input path) to the sidecar's `applyModelApiCode` verb over
   stdio JSON-RPC.
4. If the out file was written and no exception was reported, advances the
   per-module session pointer to the out file so the next read sees your
   change.

Response (returned directly as the tool result):

```jsonc
{
  "exceptionMessage":   "",  // string — runtime error text; non-empty means the code compiled and ran but threw, so nothing was saved
  "stdoutOutput":       "",  // string — captured stdout (your code cannot write to it: Console is sandboxed out, § 1.1)
  "stderrOutput":       "",  // string — captured stderr (host log lines, e.g. "Action Flow auto arrange completed")
  "validationMessages": [],  // ValidationMessage[] — TrueChange errors/warnings against the resulting model; non-empty does NOT mean the save failed
  "mutatedOmlPath":     ""   // string — path to the saved OML on a clean run; "" when nothing was saved. Hand it to omlMerge (§ 3.1).
}
```

Each `validationMessages` entry is an object, not a line of text:

```jsonc
{
  "id":        "",  // string — identifier of the validation rule
  "type":      "",  // string — "Info" | "Warning" | "Error" (capitalised) ← branch on this, case-insensitively
  "message":   "",  // string — short description
  "detail":    "",  // string — long form
  "ownerKey":  "",  // string — key of the element the message is about
  "ownerPath": "",  // string — readable path to that element
  "ownerType": ""   // string — that element's kind
}
```

> **`mutatedOmlPath` is populated only on a clean save** (no exception and the
> out file was written); it is `""` otherwise. It is your success signal and the
> path you hand to `omlMerge` (§ 3.1). There is still no `hasValidationErrors` /
> `validationErrorCount` field, but you no longer scan text for it — filter
> `validationMessages` on `type` equal to `"Error"` (case-insensitive). Runtime failures surface through
> `exceptionMessage` (or, if the sidecar returns a JSON-RPC error envelope, as
> an MCP tool error such as `OML_TOOL_VERB_FAILURE` / `OML_TOOL_PROTOCOL_ERROR`).
> **There is no `compilationErrors` field.** Code that does not compile never
> ran, so it comes back as an **MCP tool error** (`OML_TOOL_CODE_REJECTED`)
> rather than as a result — the compiler diagnostics are in that error's
> message, one per line, each prefixed `compilationErrors:` (or
> `restrictionErrors:` in the restricted sandbox):
>
> ```
> [-32602] Script compilation failed
> compilationErrors: (2,20): error CS1061: 'IESpace' does not contain a definition for 'NoSuchModelApiMember'
> ```
>
> Long diagnostic sets are truncated at 4000 characters with a trailing note
> giving the total count. Saved-but-invalid models surface as a clean result
> whose `validationMessages` contains a `type: "Error"` entry.

> **`OML_TOOL_HOST_FAILURE` (`[Error -32099] Oml Tool cannot be used outside
> OutSystems Service Studio.`) is NOT a code problem — do not retry or rewrite
> the lambda.** It only runs under an official (signed) Service Studio; surface
> it as an environment/deployment issue, not a flaw in your Model API code.

**Outcome table:**

| Outcome | Channel | `exceptionMessage` | `validationMessages` | `mutatedOmlPath` | Session pointer |
|---|---|---|---|---|---|
| Clean run | result | empty | `[]` (or warnings only) | path | advances |
| Runtime exception failure | result (or an MCP tool error) | populated | — | `""` | unchanged |
| **Compile error failure** (malformed C#) | **MCP tool error** — no result at all | — | — | — | unchanged |
| **Sandbox rejection** (`Console`, `System.…`, reflection, or a non-`OutSystems.Model` import — § 1.1) | **MCP tool error** — no result at all | — | — | — | unchanged |
| **Throw-probe** (deliberate `throw new Exception("PROBE:…")`, § 1.1) | result | your probe text | in-memory model's messages | `""` | unchanged |
| **Silent no-op** (lambda compiles + saves but never touches `eSpace` — e.g. `eSpace => { var n = eSpace.Name; }`) | result | empty | `[]` | path | advances — but to an **unmodified** OML |
| **Saved but invalid** (clean compile + save, but the model has a TrueChange error) | result | empty | has a `type: "Error"` entry | path | advances — but to an **invalid** OML |
| **Runner process-crash** (an unsupported API member kills the sidecar mid-run) | result | **empty** | `[]` | `""` | unchanged |

> **⚠️ The process-crash row wastes the most time.** An unsupported API member
> kills the sidecar mid-run, giving **empty everything** — including
> `stdoutOutput`, so breadcrumb prints can't localise it. Unlike a runtime
> failure (which populates `exceptionMessage`) or a compile failure (which
> never reaches the result channel at all), a crash is silent on
> every channel; it is **not** "no module open." Recovery: bisect, don't
> re-read your code — `eSpace => { throw new Exception("PROBE:ok"); }` coming
> back with `PROBE:ok` proves the session is
> healthy, then re-issue with one risky member per call until it dies. Full
> treatment + known crash members: [`reference/lambda-contract.md`](reference/lambda-contract.md).

The **silent no-op** is the only outcome where the response looks "successful"
but the OML wasn't actually mutated. Defend against it by always actually
mutating `eSpace` in the lambda body and by reading the result back via
`getDataModel` (or the matching `get*`) after mutating — if your change isn't
there, the lambda never touched `eSpace`.

**Saved-but-invalid is the second deceptive outcome.** A lambda can compile,
mutate `eSpace`, and save — so `exceptionMessage` is empty and the pointer
advances — while the saved model still contains a TrueChange **error**.
**Always check `validationMessages` for a `type: "Error"` entry even when the
run looked clean** — a non-empty array alone isn't enough, since
warnings-and-info-only messages are normal. A frequent trigger: hard-coding an
aggregate name the host silently auto-renamed after a navigation-style filter
was added to a default-named aggregate. Recovery recipes:
[`reference/patterns-by-element.md`](reference/patterns-by-element.md) §§
"Recovering from a saved-but-invalid action" and "Aggregate in a server
action (auto-rename trap)".

Use the response as your recovery signal: a `CS1061` in the tool error's
diagnostics means a wrong method/property name; a `type: "Error"` entry in
`validationMessages` means the validator caught something. Traditional-Web widget errors (`CS0104`, `CS0311`) have
targeted fixes in [`reference/traditional-ui-creation.md`](reference/traditional-ui-creation.md);
pointer mechanics in [`reference/mcp-session-pointer.md`](reference/mcp-session-pointer.md).

## 3.1 Finalising — auto-merge after the last mutation

**Once the final `applyModelApiCode` call that satisfies the user's request
returns a non-empty `mutatedOmlPath` — and every touched flow has cleared the
layout completion gate below — call `omlMerge` with that path.** This is the
default closing step of any task that mutates the OML — do it without being
asked, but not before the gate passes (see "Flow layout is a completion gate"
below; merging a visually tangled flow is not "done"). Without it the
host-allocated out OML stays on disk and the user never sees the diff in
Service Studio.

```jsonc
// call tool omlMerge — last action of any mutating task
{ "input": { "eSpaceName": "<the module you mutated>", "mutatedOmlPath": "<the most recent non-empty mutatedOmlPath>" }, "sessionToken": "<token>" }
// → { "merged": true, "message": "..." }
```

> **`merged` is the definitive, final success signal — not just "a window opened."**
> Whether the merge opens Service Studio's Compare-and-Merge window for the user
> or is accepted with no dialog is the **user's setting**, not something you pass
> or can influence. Either way the call returns only once the merge has resolved,
> and `merged: true` means it is **done** — the change is in the module. Always
> check that field rather than inferring success from the call returning. Do not
> re-call `omlMerge` with the same `mutatedOmlPath` "to be sure": a successful
> merge deletes that file as a side effect, so a repeat call fails with a
> file-not-found error that says nothing about whether the *first* call worked.
> Trust `merged` on the first response and stop.

> **`eSpaceName` is required**, same as `omlReset` — it decides which open module the
> mutated OML is compared against. Omitting it fails the call.
>
> **Envelope note.** Like every tool except `listApps`, `omlMerge` takes its
> arguments inside `input` plus the top-level `sessionToken` (§ 0):
> `{ "input": { "eSpaceName": ..., "mutatedOmlPath": ... }, "sessionToken": ... }`.

`omlMerge` merges the supplied OML into the named module and blocks until that
resolves. Depending on the user's setting, it either opens the Compare-and-Merge
window for them to review — which may take minutes — or accepts the changes with
no dialog and returns right away.

**Once resolved, it also clears that module's session pointer**, so the next
tool call re-snapshots the live module — which by then holds whatever the user
accepted. No follow-up `omlReset` is needed to get back in sync, but the chain
does *not* continue past a merge: a later `applyModelApiCode` starts a new
chain off the live module. Merge once at the end rather than merging
intermediate states.

The pointer is dropped once the merge resolves — so a rejected or closed merge
also leaves the agent on the live module. Nothing is lost when the merge
doesn't succeed (the OML stays on disk at the `mutatedOmlPath` you passed),
but **keep that path** in case the user wants to bring the work back. See
[`reference/mcp-session-pointer.md`](reference/mcp-session-pointer.md).

> **`merged: false` with "The merge failed. See the log for details."** The real
> reason appears only in a Service Studio dialog. No log file is written that you
> can read. Do not retry with the same file.
>
> 1. Ask the user what the dialog says.
> 2. `omlReset` the module.
> 3. Re-apply the changes against the live module, without whatever caused the failure.
> 4. Merge the new `mutatedOmlPath`.
>
> **Known cause (verified 2026-10-01):** setting `IMobileTheme.IconLibrary`. Older
> Service Studio builds reject the whole merge with *"Update Service Studio to merge
> the module. The feature Theme Icon Library is not supported in this version."*
> The trap: editing `Theme.StyleSheet` makes the sidecar report an `InvalidIconLibrary`
> error ("Allowed libraries are: FontAwesome4.0, Phosphor2.0"), and setting
> `IconLibrary` to clear it is what breaks the merge.
>
> **What works (verified 2026-10-03, 3 of 3 edits):** edit `Theme.StyleSheet`, leave
> `IconLibrary` null, and **ignore that one sidecar error**. The error comes from the
> sidecar's newer validator. Service Studio's own validation of the merged module is
> clean, `omlMerge` returns `merged:true`, and every theme stylesheet edit published:
> the served `css/<App>.<Theme>.css` changed hash each time. That includes repeated
> edits of a non-empty theme stylesheet, which block stylesheets did not handle reliably
> (§ 8). **The theme is the preferred place for app-wide CSS.** Treat
> `InvalidIconLibrary` as expected when the only change is the theme stylesheet. Any
> other Error still blocks the merge.
>
> **Two traps when appending to the theme stylesheet (verified 2026-10-03):**
> 1. **The first 2 characters after the Theme Editor block are dropped.** That block
>    ends with the `:root { … }` written by Theme Editor. Whatever you append right after
>    it loses 2 characters, probably from a line-ending normalisation. A leading `/*` is
>    lost, which leaves a stray `… */`, which then silently kills the next rule; in our
>    case that was the whole `:root` token block. Start your section with a throwaway
>    guard rule, e.g. `.ro-theme-start { }`, and avoid `/* =====` banners, which Theme
>    Editor uses for its own markers.
> 2. **The theme loads on every screen, including Login and other blank-layout pages.**
>    CSS moved from a layout block (loaded only on screens using that layout) or from a
>    single screen now applies everywhere. Scope page-specific rules, e.g.
>    `body:has(.login-screen) …`, and check the blank-layout pages after moving CSS.
>    OSUI's blank layout uses the classes `.layout.blank`, not `.layout-blank`.

### Flow layout is a completion gate — do it before you merge

**Every action (server or client) that gained new or spliced nodes must get a
full visual layout pass** — a BFS walk from the start node assigning
coordinates level by level, `Connected*` helpers preferred over manually
setting `Target`, and a splice-link read-back — before the task is
considered complete. A flow that is logically correct but visually tangled
is not done; treat this as a hard precondition of the auto-merge above, not
an optional polish step. The host's own "Action Flow auto arrange" (logged on
stderr) does **not** position hand-wired nodes. Full mechanics: [`reference/model-api-tips.md`](reference/model-api-tips.md)
§ "Manipulating Nodes in Logic Flows"; a ready typed layout lambda:
[`examples/LayoutActionFlow.cs`](examples/LayoutActionFlow.cs).

**Skip the auto-merge only when:**

- The task was read-only — no `applyModelApiCode` fired, so there is no
  `mutatedOmlPath`.
- The user asked you to discard the chain — call `omlReset` instead.
- Every `applyModelApiCode` call returned `mutatedOmlPath: ""` (compile error,
  runtime exception, nothing saved) — there is nothing to merge.

To deliberately discard an in-flight chain, call `omlReset eSpaceName=<module>`
— it drops that module's pointer and re-snapshots the live module on the next
read. Failed `applyModelApiCode` calls don't need it; the pointer stays put on
its own.

**Reset is reversible.** It drops the pointer, not the file: the discarded OML
stays at the `mutatedOmlPath` the last apply returned, and passing that to
`omlMerge` brings the changes back. `omlReset` doesn't hand the path back to
you, so hold onto it before resetting — recovery is available until Service
Studio restarts. `discardedChanges: true` means a chain was actually thrown
away; `false` means there was nothing in flight (not a failure).

## 3.2 Refreshing references and publishing

Two host tools act on the **live** module in Service Studio, not on the pointer chain.
Their schemas don't declare `sessionToken`; send it anyway (§ 0):

- **`omlRefreshReferences`** — `{ "input": { "eSpaceName": … }, "sessionToken": … }` → `{ message, hasErrors, hasWarnings }`.
  Refreshes every reference to its latest version (Manage Dependencies → *Refresh All*).
  It does **not** add a new dependency. **It drops the session pointer**: afterwards, reads
  and writes start again from the live module, so an unmerged chain disappears from view
  (its last `mutatedOmlPath` is still on disk and can be merged back). Refresh **before**
  starting a chain, or merge first.
- **`omlPublish`** — `{ "input": { "eSpaceName": … }, "sessionToken": … }`. 1-Click Publishes the open module
  and waits for the result. It needs a write permission the user grants in the MCP Server
  dialog; while that is withheld it returns status `blocked`. **Only call it when the user
  explicitly asks to publish** — never as an automatic closing step.
  Verified 2026-10-03: it publishes the **live** module (merge first; an unmerged chain is not
  published), it requires the session token, and it fails at once with "Module 'X' is not open
  in Service Studio. Open it before calling omlPublish." if the module isn't open. A clean
  publish of a small module took 16 s and returned
  `{ "published": true, "status": "success", "reason": "", "messages": ["Uploading", "Compiling", "Deploying", "Done"] }`.
  Check `published` / `status`; `reason` explains a failure. When the user has not allowed
  **automatic merge and publish** in the MCP Server dialog it returns immediately, publishes
  nothing, and answers
  `{ "published": false, "status": "blocked", "reason": "Publishing requires a write permission the user has not granted. Ask the user to publish from Service Studio, or to allow automatic merge and publish in the MCP Server dialog.", "messages": [] }`
  (verified 2026-10-03). Then tell the user, and let them publish from Service Studio or change
  the setting. Don't retry.

## 4. Default imports

**Host-added defaults** (3, prepended automatically — don't list them in
`imports`): `OutSystems.Model.UI` (`IScreen`, `IBlock`, `IUIFlowNode`),
`OutSystems.Model.UI.Web` (Traditional Web: `IWebScreen`, `IWebBlock`,
`IWebFlow`), `OutSystems.Model.UI.Mobile` (Reactive/Mobile: `IMobileScreen`,
`IMobileBlock`).

The bundled sidecar additionally pre-imports `System`, `System.Linq`,
`OutSystems.Model`, `.Enumerations`, `.Expressions`, `.Factory`, `.Types` —
so `eSpace.WebFlows.SelectMany(f => f.Nodes).OfType<IWebScreen>()` and
`eSpace.MobileFlows.SelectMany(...).OfType<IMobileScreen>()` both compile
without any agent-supplied `imports`. Other **`OutSystems.Model.*`** namespaces
(e.g. `OutSystems.Model.Data`, `OutSystems.Model.Logic.Nodes`) may be passed via
`imports` — the host dedup-appends them after the defaults. **Nothing else is
accepted** (§ 1.1): never pass `System`, `System.Linq` (already pre-imported) or
`ServiceStudio.Plugin.*` — fully qualify those types in the code instead.
(`imports` is required on the wire; `[]` is fine.)

## 4.1 Module style support

OutSystems 11 modules come in three UI flavours — **Reactive Web**,
**Mobile**, and **Traditional Web** — plus Service / Library variants. The
in-process MCP Server surfaces the module's style on every `listApps` hit
via a `moduleType` field (formerly `style`; values such as `Reactive`, `Mobile`,
`Traditional`, `ReactiveLibrary`, `MobileLibrary`, `TraditionalLibrary`,
`Service`, `Unknown` — `Reactive` verified live 2026-09-30).

Most `get*` verbs work uniformly across styles; a handful of UI-shaped reads
are **Mobile-bound** and return empty on Traditional modules — use the
`*Traditional` variant instead (`getScreenNamesTraditional`,
`getScreenTraditional`, …) whenever `moduleType` is `"Traditional"` /
`"TraditionalLibrary"`. `applyModelApiCode` itself is **fully style-agnostic**
— write Traditional mutations the same way you write Mobile ones, using the
`IWebScreen` / `IWebBlock` / `IWebFlow` family (host-default-imported, § 4)
in place of `IMobileScreen` / `IMobileBlock` / `IMobileFlow`.

Full style-detection payload shape, the complete `*Traditional` verb table,
and the Reactive-vs-Traditional interface/concept mapping (flow collection,
screen/block signatures, session vs. client variables, …):
[`reference/module-style-support.md`](reference/module-style-support.md).

## 5. Common patterns (pointers)

The lambda sketches for each element type — server entity, attribute,
client/server action, screen + widget, node positioning in flows — live in
[`reference/patterns-by-element.md`](reference/patterns-by-element.md).
Read that first; pattern-match against the snippet that fits your task,
then verify with `getDataModel` after applying.

For building a **Reactive Web** screen from scratch — layout scaffold, KPI tiles
from the `Card` block, data-bound `List` rows, dynamic inline styles / CSS-bar
charts, and the real-chart-vs-fallback decision — see
[`reference/reactive-ui-creation.md`](reference/reactive-ui-creation.md) (Traditional Web UI creation is
the separate [`reference/traditional-ui-creation.md`](reference/traditional-ui-creation.md)).

Full worked C# examples: [`examples/`](examples/). Each file is **already** a
full `eSpace => { ... }` lambda with no `eSpace.Save` — exactly the shape the
`code` arg wants. Use the lambda verbatim and lift the file's `using`
directives (all `OutSystems.Model.*`) into `imports`. The
[`examples/odc-workflows-unsupported-on-o11/`](examples/odc-workflows-unsupported-on-o11/)
subfolder holds ODC Workflows examples that are **not** usable on O11 — for
O11 BPT use [`examples/AddProcessWithHumanActivity.cs`](examples/AddProcessWithHumanActivity.cs) (§ 8).

## 6. Where to dig deeper

| Topic | File |
|---|---|
| Full-lambda contract, host wrap mechanics, CS0103 / silent-no-op trap | [`reference/lambda-contract.md`](reference/lambda-contract.md) |
| Naming gotchas (eSpace vs Module, IMobile* vs UI, Signature types) | [`reference/model-api-tips.md`](reference/model-api-tips.md) |
| Model API namespace catalogue | [`reference/model-api-summary.md`](reference/model-api-summary.md) |
| MCP tool surface (`applyModelApiCode`, the `get*` read tools, `runQuery`, `listApps`, `getValidationMessages`, host-only `omlReset` + `omlMerge`) | [`reference/verb-reference.md`](reference/verb-reference.md) |
| Session pointer mechanics, `omlReset`, `omlMerge`, `mutatedOmlPath` | [`reference/mcp-session-pointer.md`](reference/mcp-session-pointer.md) |
| Module style detection payload, full `*Traditional` verb table, Reactive-vs-Traditional interface mapping | [`reference/module-style-support.md`](reference/module-style-support.md) |
| Lambda sketches per element type (entity, attribute, action, screen, node positioning); **grouped-aggregate reference-by-alias trap**; **silent renames**, default nodes per action kind, screen access (Anonymous role, role gating) | [`reference/patterns-by-element.md`](reference/patterns-by-element.md) |
| **Logic beyond basic flows** — SQL (advanced query) nodes, For Each, exception handlers, exposed REST APIs, email sending, timers, site properties, locales, **O11 BPT processes** (`CreateProcess`, human/automatic activities, decisions) — verified recipes + what TrueChange does and doesn't catch | [`reference/logic-and-integrations.md`](reference/logic-and-integrations.md) |
| **Reactive Web** UI *creation* (screen scaffold, block/placeholder instancing, KPI tiles from `Card`, `List` rows, data-bound inline styles via `CreateExtendedProperty`, CSS-bar chart vs `OutSystemsCharts` dependency) | [`reference/reactive-ui-creation.md`](reference/reactive-ui-creation.md) |
| **Reactive Web** verified widget-creation API (`IList`, `IDropdown`, `IIfWidget`, block instances, screen scaffolding, joins, M2M toggle, role checks) **plus the runner-crash traps** — read before writing widget code | [`reference/reactive-widget-api.md`](reference/reactive-widget-api.md) |
| **Reactive Web** OutSystemsUI verified block catalog (~45 blocks: `Gallery`, `CardSectioned`, `Counter`, `MasterDetail`, `Tabs`, charts …), per-archetype composition recipes, and the full design-token system with exact values | [`reference/osui-verified-blocks.md`](reference/osui-verified-blocks.md) |
| **Reactive Web** design-intent UX patterns (50+ layout / form / data / motion / mobile / accessibility patterns — pseudo-code for *what good UI looks like*, not literal API) | [`reference/styleguide-ui-patterns.md`](reference/styleguide-ui-patterns.md) |
| **Traditional Web** descent recipes (`eSpace.WebFlows`, `IWebScreen` / `IWebBlock`, `screen.ScreenActions`, session variables), per-style verb selection | [`reference/traditional-patterns.md`](reference/traditional-patterns.md) |
| **Traditional→Reactive migration cleanup** (what a migrated module looks like, the *data-actions-have-no-input-params* fact, the minimal-rebind repurpose-`OnStart`+`Refresh Data` conversion recipe, `GetUser`/dead-node/null-arg error fixes, and the migration-specific runner-crash traps) — read before touching a migrated module | [`reference/traditional-to-reactive-migration-cleanup.md`](reference/traditional-to-reactive-migration-cleanup.md) |
| **Traditional Web** UI *creation* (screen, block, widget tree, Preparation + Aggregate, ListRecords binding, `JavaScript` hook; CS0104 / CS0311 prophylaxis) + the Web interface catalogue | [`reference/traditional-ui-creation.md`](reference/traditional-ui-creation.md), [`docs/OutSystems.Model.UI.Web.Generated.cs`](docs/OutSystems.Model.UI.Web.Generated.cs), [`docs/OutSystems.Model.UI.Web.Widgets.Generated.cs`](docs/OutSystems.Model.UI.Web.Widgets.Generated.cs) |
| Worked C# examples (already full `eSpace => { ... }` lambdas, no `eSpace.Save` — usable verbatim as `code`; their `using` directives map to `imports`) | [`examples/`](examples/) |
| Namespace doc files (interface/method signatures) plus `BuiltinFunctions.Generated.json` (the OutSystems expression-function catalogue — `Abs`, `Mod`, etc., used in aggregate filters and computed attributes, **not** C# methods callable from `applyModelApiCode`) | [`docs/`](docs/) |
| MCP host design choices & caveats — temp OML location & cleanup-on-restart, per-module tracking, per-connection approval, security scope, timeouts/port, on-demand server startup | [`../MCP-DESIGN-CHOICES.md`](../MCP-DESIGN-CHOICES.md) |

## 7. Integration walkthroughs

The supported Claude-Code-agent path is the Service Studio in-process MCP
Server. Start with [`integration-examples/01-add-entity-via-mcp.md`](integration-examples/01-add-entity-via-mcp.md)
— that's the canonical single-mutation walkthrough. The other transcripts cover recovery and pointer scenarios.

| Scenario | File |
|---|---|
| **Single-mutation MCP call (canonical)** | [`integration-examples/01-add-entity-via-mcp.md`](integration-examples/01-add-entity-via-mcp.md) |
| Compile-error / silent-no-op recovery loop | [`integration-examples/02-recovery-from-compile-error.md`](integration-examples/02-recovery-from-compile-error.md) |
| Multi-step edit (auto-chain via session pointer) | [`integration-examples/03-multi-step-edit.md`](integration-examples/03-multi-step-edit.md) |
| `omlReset` drop-and-restart walkthrough | [`integration-examples/04-oml-reset.md`](integration-examples/04-oml-reset.md) |
| **Traditional Web** read + mutate (style detection, local `*Traditional` verbs, Model-API-code output, single-lambda multi-mutation) | [`integration-examples/05-traditional-mutation.md`](integration-examples/05-traditional-mutation.md) |
| **FK to a System / referenced entity** (`User`) — resolve via `References.Named("(System)")` → `IServerEntitySignature` → `DataType = IdentifierType`, no `RefreshDependency` | [`integration-examples/06-fk-to-system-entity.md`](integration-examples/06-fk-to-system-entity.md) |

## 8. Known gaps

The MCP host wraps the bundled sidecar's verbs and adds the
host-only `createSessionToken`, `listApps`, `omlReset`, `omlMerge`,
`omlRefreshReferences` and `omlPublish` tools. Current gaps:

- **The generated [`docs/`](docs/) miss the O11 process (BPT) API.** It exists
  and works — `eSpace.CreateProcess` / `eSpace.Processes`
  (`OutSystems.Model.Processes.IProcess`) and the node types in
  `OutSystems.Model.Processes.Nodes` — but only a hand-verified summary is
  available: [`reference/logic-and-integrations.md`](reference/logic-and-integrations.md)
  § "BPT processes". Don't use `eSpace.CreateBusinessProcess`: that is the ODC
  Workflows API and fails on an O11 module (`Process objects can't be children
  of Module objects`); its old examples are parked in
  [`examples/odc-workflows-unsupported-on-o11/`](examples/odc-workflows-unsupported-on-o11/).
- **Translations can't be written.** `eSpace.GetOrCreateLocale(Culture.X)`
  creates a locale and `eSpace.ApplyToTextResources(...)` walks the text
  resources (read-only), but there is no typed API to set a translated value.

- **No `getModelApiDoc` / `getModelApiExamples` analogue.** The static
  [`docs/`](docs/) and [`examples/`](examples/) folders are your reference.
- **No `searchModel` equivalent.** The cross-element substring search tool was
  removed from the MCP host. Find elements by name via the matching `get*Names`
  list tool (or `getDataModel`/`getStructures`) and filter client-side, or
  write a targeted `runQuery` query instead.
- **No "rewind by N saves" verb.** The pointer only advances on success;
  `omlReset` drops the chain and re-snapshots the live module. No general
  undo, though a reset is reversible within the session — the discarded copy
  stays on disk and merges back via the last `mutatedOmlPath`.
- **Adding a new external / cross-module dependency is not supported.** A
  snippet referencing a not-yet-resident element (e.g.
  `eSpace.AddDependency(globalKey)` / `eSpace.RefreshDependency(globalKey)` for
  a Forge / system / external element) fails in the sidecar with `The event
  IModelServices.ResolveModuleSignature must be set first` — **even for an
  element that is already referenced**. Add the dependency in Service Studio
  (Manage Dependencies) first, then re-run. Referencing an *already-resident*
  dependency works (e.g. System `User` via `eSpace.References.Named("(System)")`,
  no `RefreshDependency` needed). `omlRefreshReferences` (§ 3.2) only refreshes
  existing references to their latest version.
- **The output of a `Record List To Excel` node can't be referenced in an
  expression.** `IRecordListToExcelNode` can be created, but every reference to
  its output gives `ExpressionUnknownObject`. This was tried with 20+ names,
  including `ExcelFile`, and with a default-named node (verified 2026-10-01).
  Generate CSV text in a server action and download it from the client instead.
  Excel opens it directly.
- **Theme Icon Library** cannot be set on older Service Studio builds, because
  the merge is rejected (§ 3.1). You don't need to set it: theme stylesheet edits
  merge and publish fine with `IconLibrary` left null.
- **Edits to a non-empty block or screen stylesheet are not reliably published.**
  This does not apply to the theme stylesheet: theme edits published 3 times out of 3.
  Prefer the theme for CSS (§ 3.1). Verified 2026-10-01 by downloading the served CSS
  with curl.
  - What happens: the merge reports `merged:true` and the live module holds the new text,
    but `omlPublish` still deploys the old CSS. Its version hash in
    `/<App>/moduleservices/moduleinfo` doesn't change.
  - What does publish: setting a stylesheet that was **empty**.
  - **No reliable workaround.** Clearing the stylesheet in one merge and writing it in a
    second worked once, then failed on the next try. Screen and widget changes in the same
    publish *did* deploy, which you can confirm in the compiled `scripts/<App>.<Flow>.<Screen>.mvc.js`.
    Always confirm the CSS with `curl https://<host>/<App>/css/<App>.<Flow>.<Block>.css`;
    a screenshot alone can't tell you whether it deployed. If it is stale, hand the CSS to
    the user as a file to paste into the stylesheet editor in Service Studio and publish
    from there.
- **Widget sequences can be reordered.** `IContainer.Widgets`, `ILink.Widgets` and
  `IPlaceholderContentWidget.Widgets` are `OutSystems.Model.ISequence<IMobileWidget>`,
  with `MoveToStart`, `MoveToEnd`, `MoveBeforeSibling`, `MoveAfterSibling` and
  `SetOrderTo`. Cast and move existing widgets instead of deleting and recreating them.

## Common Mistakes

Full taxonomy with response-shape diagnostics: [`reference/lambda-contract.md`](reference/lambda-contract.md).
Quick checklist (each already covered in detail above):

- Body-only statements with no `eSpace => { ... }` wrapper (§ 1, rule 1).
- `code` ending in anything other than `}` — trailing newline/space/`;` (§ 1, rule 3).
- Calling `eSpace.Save(...)` yourself (§ 1, rule 4).
- Renaming the lambda parameter away from `eSpace` (§ 1, rule 2).
- Skipping `getDataModel` (or the matching `get*`) before writing (§ 2).
- Looking up a System / referenced entity via `eSpace.Entities` instead of
  `eSpace.References.Named("(System)")` — see [`reference/patterns-by-element.md`](reference/patterns-by-element.md)
  § "FK to a System / referenced entity".
- Iterating `eSpace.Entities` and mutating whatever comes back — it also
  yields static/client entities, so a server-entity setter throws
  `InvalidOperationException`. Filter with
  `.OfType<OutSystems.Model.Data.IServerEntity>()` first (not a default import).
- Omitting `includeJson` on code-returning read tools — always pass `"Never"` (§ 2).
- Skipping the auto-merge after the final successful mutation (§ 3.1).
- Sending flat arguments, or forgetting `sessionToken` — every tool except
  `listApps` wants `{ "input": {…}, "sessionToken": … }` (§ 0). Requesting a
  new token on every reconnect instead of reusing the first one (§ 0).
- Using `Console.WriteLine`, `System.…`-qualified names or reflection, or
  passing a non-`OutSystems.Model` namespace in `imports` — rejected before
  compile; throw a `PROBE:` exception to see values (§ 1.1).
- Assuming the name you asked for is the name you got — a taken name
  (including an entity-action name like `GetOrder` once entity `Order` has an
  identifier) is silently suffixed (`GetOrder2`), and attribute names are cut
  to 28 characters. Read `.Name` back ([`reference/patterns-by-element.md`](reference/patterns-by-element.md) § "Silent renames").
- Making a Reactive screen public with `screen.AnonymousAccess = true` — `omlMerge` drops it.
  Add `eSpace.AnonymousRole` to `screen.Roles` instead, and check access on the **live** module after
  the merge ([`reference/patterns-by-element.md`](reference/patterns-by-element.md) § "Screen access").
- Calling `omlRefreshReferences` mid-chain — it drops the session pointer (§ 3.2).
  Calling `omlPublish` without being asked to publish (§ 3.2).
- Building O11 BPT processes with `CreateBusinessProcess` (the ODC Workflows
  API) instead of `eSpace.CreateProcess`, or wiring process nodes with
  `ConnectedBelow` instead of `node.Targets.Add(next)` (§ 8).
- **⚠️ TEMPORARY — blaming your own code when *every* tool call fails
  identically.** If each call returns `An error occurred invoking '<toolName>'`
  and your client is auto-populating argument fields you never sent (names
  like `inputXXXNameValPairs`), your harness's tool wrapper is corrupting the
  argument envelope — fall back to posting directly to the endpoint: [`reference/verb-reference.md`](reference/verb-reference.md)
  § "HTTP transport". Observed on the Antigravity CLI, 2026-08-03.
- Looking for a compile error in the result — it never arrives there; a
  snippet that doesn't compile comes back as an MCP tool error carrying the
  diagnostics (§ 3).
- Treating a clean response as proof of mutation — verify with a follow-up
  `get*` (the silent-no-op trap, § 3).
- Ignoring `validationMessages` — a `type: "Error"` entry means
  saved-but-invalid even though the run looked clean. It's a structured array;
  don't substring-match it for `(Error)` (§ 3).
- Retrying `omlMerge` after `merged:false` without asking what Service Studio's
  dialog said, or setting `Theme.IconLibrary`, which makes older Service Studio
  builds reject the merge (§ 3.1).
- Using an ambiguous short name. `IStartNode`, `IEndNode`, `IScreenAction`,
  `IScreenAggregate` and `IDataAction` each exist in more than one imported
  namespace. Write them in full:
  - `OutSystems.Model.Logic.Nodes.IStartNode`
  - `OutSystems.Model.Logic.Nodes.IEndNode`
  - `OutSystems.Model.UI.Mobile.IScreenAction`
  - `OutSystems.Model.UI.Mobile.IScreenAggregate`
  - `OutSystems.Model.UI.Mobile.IDataAction`
- Using `.Name` on a static-entity record. Use `.Identifier` instead, for example
  `((IStaticEntitySignature)e).Records.Select(r => r.Identifier)`. The
  expression form is `Entities.<Entity>.<Identifier>`.
- Writing timestamps in build or tracking files from memory. They drift by hours
  over a long session. Run `date`, or take the times from the session transcript.
- Placeholder tokens in your own C# helper code (for example `$` meaning "current
  row prefix") that collide with literals inside OutSystems expressions. The
  observed result was `FormatCurrency(x, "GetList.List.Current.", …)`. Use a token
  that can never appear in an expression, such as `@@ROW@@`.
- Building UI or logic for many screens in one lambda without a dry run. Run the
  lambda once with `throw new Exception("PROBE:" + summary)` as its last
  statement. This returns the TrueChange messages for the change without saving
  anything (§ 1.1). Then run it again without the throw.

## 9. Security caveat

`applyModelApiCode` runs caller-authored C#. Code executes out of process and
is terminated if it exceeds the timeout (300 s for `applyModelApiCode`).

Because the snippet is arbitrary C# that runs with the host's privileges, treat
`applyModelApiCode` as privileged: **use it only against modules you own**, and
be cautious with code derived from untrusted module content.
