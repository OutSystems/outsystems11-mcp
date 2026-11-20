---
mode: 'agent'
tools: ['codebase', 'search', 'editFiles', 'servicestudio']
description: 'Make a change to the open OutSystems 11 module via the Service Studio Model API.'
---

You are editing the **currently open `.oml` module** in Service Studio through
the `servicestudio` MCP server. Follow the contract in
`servicestudio-mcp-oml/SKILL.md` and the `servicestudio-mcp-oml/reference/*.md` files
(`SKILL.md` wins if this prompt disagrees).

Writes are a **Beta Feature**: before your first write in this conversation, tell the user and
share https://www.outsystems.com/legal/beta-features-agreement.

Do this for the request below:

0. **Contract.** This skill is **written for MCP contract 2**; the first line of the server's
   instructions names the server's (if you can't see them, `createSessionToken`'s description
   says the same). If it's higher, offer the user the latest skill from the repository the second line names, and install
   it only if they agree. If it's lower or missing, follow
   `servicestudio-mcp-oml/reference/contracts.md`. On any contract, where a tool's live
   description differs from this skill, follow the description.
   **Session.** If you don't already hold one, call `createSessionToken` once
   and pass the token as top-level `sessionToken` on every call. Every tool
   except `listOpenModules` takes `{ "input": { … }, "sessionToken": "<token>" }`.
   Pass the module's `tabId` from `listOpenModules` as `eSpaceName` (on
   `MODULE_AMBIGUOUS`, retry with the `tabId` the user means). If the request names a
   module that isn't open, find it with `searchApps` and open it with `openModule`
   (on `ENVIRONMENT_REQUIRED`, ask the user which environment and pass it as
   `environment` to both); close it with `closeModule` when done. Early contract 1 builds (`listApps`) can't open modules:
   ask the user.
1. **Read first.** Call `getDataModel` (and any relevant `get*` read tool, e.g.
   `getServerAction`, `getScreen` with `objectName`) with
   `input: { eSpaceName, includeJson: "Never" }` and pattern-match the output
   before writing anything. Outside an edit session, make these reads one at a time:
   parallel reads on one module can fail with `OML not found: …-in.oml` (run the failed
   one again, alone).
2. **Mutate.** Call `startOmlEditSession` (`input: { eSpaceName }`) once, then
   `applyModelApiCode`, `input: { eSpaceName, code, imports }`,
   passing a full `eSpace => { ... }` lambda that ends in `}` and never calls
   `eSpace.Save`. To change an existing element, look it up
   (`eSpace.ServerActions.Named("X")`) and mutate it. Never replay `get*`
   read-back, which is creation code and makes a renamed duplicate. Prefer lifting a matching example from
   `servicestudio-mcp-oml/examples/*.cs`. Sandbox: no `Console`, `System.…` or
   reflection in `code`; `imports` only `OutSystems.Model.*` (fully qualify
   `ServiceStudio.Plugin.*` types). To inspect something, end the lambda with
   `throw new Exception("PROBE:" + s);` and read `exceptionMessage`.
3. **Verify.** Confirm `exceptionMessage` is empty, `mutatedOmlPath` is
   non-empty, and `validationMessages` has no `type: "Error"` entry.
   Re-read with a `get*` tool to confirm the change actually landed
   (silent-no-op guard) and under the name you chose (taken names are silently
   suffixed, e.g. `GetOrder2`). If *all* fields come back empty (including
   `mutatedOmlPath`), that's a runner crash, not a compile error (a compile
   error is an MCP tool error carrying diagnostics, not silence) — bisect with
   one risky member per call.
4. **Finalise.** Call `omlMerge` with
   `input: { eSpaceName, mutatedOmlPath: "<final path>" }`, then `endOmlEditSession`
   once it returned `merged: true`. Don't call `omlPublish` unless the user asked to
   publish.

The change to make:

${input:request:Describe the entity/screen/action change to apply}
