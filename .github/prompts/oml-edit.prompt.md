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

0. **Session.** If you don't already hold one, call `createSessionToken` once
   and pass the token as top-level `sessionToken` on every call. Every tool
   except `listApps` takes `{ "input": { … }, "sessionToken": "<token>" }`.
1. **Read first.** Call `getDataModel` (and any relevant `get*` read tool, e.g.
   `getServerAction`, `getScreen` with `objectName`) with
   `input: { eSpaceName, includeJson: "Never" }` and pattern-match the output
   before writing anything.
2. **Mutate** with `applyModelApiCode`, `input: { eSpaceName, code, imports }`,
   passing a full `eSpace => { ... }` lambda that ends in `}` and never calls
   `eSpace.Save`. Prefer lifting a matching example from
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
   `input: { eSpaceName, mutatedOmlPath: "<final path>" }`. Don't call
   `omlPublish` unless the user asked to publish.

The change to make:

${input:request:Describe the entity/screen/action change to apply}
