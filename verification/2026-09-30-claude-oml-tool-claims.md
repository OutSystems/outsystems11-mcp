# Live verification log — claude-oml-tool claims vs. Service Studio 11 in-process MCP (module PVOTest, Reactive), 2026-09-30

Legend: VERIFIED = claim holds in-process · DIFFERS = in-process behaviour differs from the claim · N/A = claim is a headless/ODC artefact · UNSUPPORTED = not reachable via the public API in-process

## A. MCP contract (vs. our SKILL.md as of 0fd0ef6)
| # | Finding | Result |
|---|---|---|
| A1 | `createSessionToken(clientName)` must be called first; returns token (user approval dialog) | NEW |
| A2 | Every tool except `listApps`/`createSessionToken` takes args wrapped in `input:{…}` plus top-level `sessionToken` (also sent as `SessionToken` header per `x-mcp-header`) | DIFFERS (skill says flat) |
| A3 | `omlPublish`, `omlRefreshReferences`, `getRoleExceptions` schemas carry no `sessionToken` — **but send it anyway**: `getRoleExceptions` rejects calls without it (D10) | NEW (corrected) |
| A4 | `listApps` returns `moduleType` (not `style`); `kind`, `isOpen`, `isReference`, `version` | DIFFERS |
| A5 | No `Mcp-Session-Id` header returned on initialize | DIFFERS (verb-reference HTTP transport) |
| A6 | Validation `type` values are capitalised: `Warning`, `Error` | DIFFERS (skill says lowercase) |
| A7 | By-name reads take `objectName` (e.g. `getServerAction {objectName}`) | confirm in verb-reference |
| A8 | **Sandbox**: `code` may only reference the Model API — `Console`, anything written `System.…`, reflection (`GetProperties`, `GetMethod`, `Invoke`…), file/process/env/registry/interop/network are rejected pre-compile with a tool error `'code' must only reference the OutSystems Model API … (found 'X')`. `GetType().Name` is allowed. | NEW — breaks every reflection recipe + Console.WriteLine debugging |
| A9 | Probe output channel: `throw new Exception("PROBE:"+s)` → text returned in `exceptionMessage`, nothing saved, pointer unmoved, and `validationMessages` for the in-memory model are STILL returned | NEW technique |
| A10 | `ServiceStudio.Model.*` internal types not referenced (CS0234) — irrelevant for BPT, which has a public API (D7) | NEW |
| A11 | `omlRefreshReferences` → `{message, hasErrors, hasWarnings}`; **drops the session pointer** (unmerged chain disappears from reads) | NEW |
| A12 | Host logs `Action Flow auto arrange completed` but nodes wired by hand stay stacked at one coordinate → manual layout still needed | VERIFIED (gate stays) |
| A13 | `omlPublish` exists (1-Click Publish; gated by a write permission in the MCP dialog; returns status `blocked` when withheld) | VERIFIED 2026-10-03 (see D13, D14) |

## B. Our own skill claims re-tested
| # | Claim in our skill | Result |
|---|---|---|
| B1 | `entity.IdentifierAttribute = attr` crashes → use reflection | DIFFERS — typed setter works (server + static entities) |
| B2 | `IsAutoNumber` — "leave it alone" | DIFFERS — `IsAutoNumber = AutoNumber.Yes` works |
| B3 | `AddStartEndNodes` via reflection is mandatory | DIFFERS — `CreateNode<IStartNode>()`/`IEndNode` works (examples already do this); reflection now blocked |
| B4 | Link text requires reflection | DIFFERS — `lnk.Widgets.OfType<ITextWidget>().First().Text = "…"` |
| B5 | Aggregate rename-back via reflection `SetPropertyValue` | now blocked by sandbox (untested alternative: `agg.Name = …`) |
| B6 | BPT examples (NewWorkflow, HumanActivity, …) | these use the ODC Workflows API — `CreateBusinessProcess` throws `Process objects can't be children of Module objects` on O11. **O11 BPT itself IS supported** via `eSpace.CreateProcess` (see D7) |
| B7 | `AddDependency` fails | VERIFIED — even for an already-resident element; yet `getDataModel` emits `AddDependency(...)` lines → read-back is not replayable verbatim |

## C. claude-oml-tool claims
| # | Claim | Source | Result |
|---|---|---|---|
| C1 | Duplicate/taken names are silently renamed `X2` | odc/TRAPS, NameGuard | VERIFIED (`DoThing`→`DoThing2`) |
| C2 | Actions named like entity actions renamed | NameGuard | VERIFIED once identifier exists (`GetOrder`→`GetOrder2`, `CreateOrUpdateOrder`→`…2`) |
| C3 | Screen/action named `Widgets` reserved | NameGuard | not reproduced for server action |
| C4 | Attribute names truncated to 28 chars silently | odc/TRAPS | VERIFIED |
| C5 | One Binary Data per entity, nothing flags it | odc/TRAPS | DIFFERS on O11 — TrueChange error `InvalidEntity_OneBin` |
| C6 | Param named like an entity fails build | odc/TRAPS B2 | not flagged by TrueChange on O11 |
| C7 | Setting `ExecuteAction.Action` auto-creates Arguments; never add | CORE_INVARIANTS | VERIFIED |
| C8 | `Get<Entity>` output is `Record` | odc/TRAPS | VERIFIED |
| C9 | Exception handler must have its own End | odc/TRAPS | VERIFIED — TrueChange `InvalidFlow_BadErrHandlerPath` / `InvalidFlow_MalformedAction` |
| C10 | Writing into `GetX.Record.…` fails build | odc/TRAPS B5 | not flagged on O11 |
| C11 | ForEach wiring Cycle→body, body→ForEach | odc/TRAPS | VERIFIED (typed `CycleTarget`/`Target`), row = `<Src>.List.Current` |
| C12 | SQL/advanced query: params auto-created Argument, needs output | odc/capability | VERIFIED typed: `CreateNode<ISQLNode>`, `CreateInputParameter`, `SetArgumentValue`, `CreateOutput(entity)`, `Statement`, `SetMaxRecords` |
| C13 | Second aggregate source doesn't create join | odc/TRAPS | VERIFIED — only a `CrossJoinDataSet` WARNING |
| C14 | Default-named aggregate renamed on filter | odc/TRAPS G6 | VERIFIED (already in skill) |
| C15 | FK default Protect; O11 op default mandatory | o11/AUTHORING | Protect VERIFIED; `IsMandatory` defaults False |
| C16 | New screen gets Registered + every role | o11/PLATFORM_DELTAS 13 | VERIFIED (`Manager,Registered`); gate = remove Registered |
| C17 | No AnonymousAccess on O11; Anonymous via Permission | o11/PLATFORM_DELTAS 13 | **VERIFIED (corrected, see D9)** — public = `eSpace.AnonymousRole` in `screen.Roles`. `IMobileScreen.AnonymousAccess` exists but `omlMerge` drops it |
| C18 | Doc rule D4 "Anonymous listed ⇒ public" | o11/DOCUMENT | **VERIFIED (corrected, see D9)** |
| C19 | O11 Buttons/Links have no label Text | o11/PLATFORM_DELTAS 2k | N/A (headless) — in-process both ship a default `Text` child |
| C20 | O11Lint widget rules (unbound Variable, TableRecords no Source, Button/Link no OnClick, Expression no Value) | O11Lint.cs | already TrueChange ERRORS in-process (`RequiredPropertyValue_Property`) |
| C21 | Input Variable must be basic scalar | odc/capability | VERIFIED — `ExpressionInvalidVariableType` |
| C22 | InputType must match variable type | o11/TRAPS | VERIFIED — `CustomObjectError` "Input type is a Number but it should be Text" |
| C23 | InputType ordinals trap | o11/TRAPS | N/A — typed enum |
| C24 | Popup must be parented to MainContent | odc/capability | DIFFERS — Popup in Container accepted |
| C25 | `ShowPopup` required | odc | VERIFIED — `RequiredPropertyValue_Property: Show Popup must be set` |
| C26 | MaxLength defaults 500 | odc | `MaxLength` null in model (runtime default not tested) |
| C27 | Timer schedule string `"WhenPublished"`, empty = not scheduled | o11/TRAPS | VERIFIED as stored; `TimeoutInMinutes` default 20 (not 1200) |
| C28 | Site property without default | odc | not flagged by TrueChange — tip |
| C29 | REST expose: CustomService… chunk surgery | odc/capability | typed API works: `CreateIntegration<IRestService>`, `CreateAction`, `HTTPMethod`, inputs `ReceiveIn`; **REST methods ship with Start+End** (service actions ship 1 node; server/client/screen actions ship 0) |
| C30 | O11 BPT via `add_process` | o11/BPT | VERIFIED in-process via the public (undocumented) `OutSystems.Model.Processes` API — see D7. HA default role Registered VERIFIED; auto `<Entity>Id` input VERIFIED; decision auto-skeleton VERIFIED |
| C31 | Translations via ApplyToTextResources walk | odc/capability | read walk VERIFIED (178 resources); no typed write API; `GetOrCreateLocale` VERIFIED |
| C32 | CheckGlobalState anonymous-structure invariants | O11Lint 4–7 | not checkable — `IAnonymousStructure` exposes no members (count only) |
| C33 | Email via EmailScreen + SendEmail | odc/capability | VERIFIED typed: `flow.CreateEmail`, `Logic.Mobile.Nodes.ISendEmailNode` in server action; `FlowThemeTooBigForEmails` warning |
| C34 | Chunk surgery, digests, sig keys, implKey harvest, grafting, headless DLL init | various | N/A |

## D. Additional findings during the skill update
| # | Finding | Result |
|---|---|---|
| D1 | Sandbox pre-parser rejects C# 12 collection expressions (`[a, b]`) → `'code' is not valid C#: Invalid expression term '['`; `new[] { … }` and tuples are fine; `ExpressionDefinition.Parse("[ { Value: 1, Label: \"a\" } ]")` yields a `ListLiteral` | NEW |
| D2 | `CreateRole("X")` adds X to the Roles of every existing screen | NEW |
| D3 | Typed replacements verified: `eSpace.RoleExceptions.First(re => re.Role.Name == …)`, `IJSONSerializeNode.SetData(...)`, `agg.Name = …`, typed BFS over `node.Connectors` + `HorizontalPosition`/`VerticalPosition` | VERIFIED |
| D4 | Manual `Target` / `TrueTarget` / `FalseTarget` / `CycleTarget` on freshly created nodes created connectors (our old "Target silently no-ops" trap did not reproduce) | DIFFERS |
| D5 | `getSettings` renders `IsSecret = true/false`; `getIntegrations` renders REST `Authentication` | VERIFIED |
| D6 | Sweep: all 81 examples run through `applyModelApiCode` after removing `using System*` — 0 sandbox rejections; 1 pre-existing compile error (`AddDashboardScreenWithDonutChart.cs`, undefined `modelServices`) fixed by resolving the block through `eSpace.References`; remaining runtime stops are missing scratch-module context (expected) | VERIFIED |
| D7 | **O11 BPT authoring works** (corrected after user challenge — initial verdict tested only `CreateBusinessProcess`). `eSpace.CreateProcess(name)` → `OutSystems.Model.Processes.IProcess`; `LaunchOn.Entity` / `LaunchOn.Action = EntityActionKind.Create` (auto-creates `OrderId` input, Start named "New Order"); `SetDetail(expr)`; nodes in `OutSystems.Model.Processes.Nodes` (`IStartNode`, `IEndNode`, `IHumanActivityNode`, `IAutomaticActivityNode`, `IDecisionNode`, `IWaitNode`, `IConditionalStartNode`, `IExecuteProcessNode`, `ICommentNode`, `ISendEmailNode`); wiring `node.Targets.Add(next)` (`Connected*` throws `Node doesn't have a target property`); HA `Destination`, `Roles` (default Registered), `Instructions`, `OnReady`/`OnClose` callbacks; automatic activity is a logic flow that ships with a Start; `IDecisionNode.CreateConnector(name, target)` auto-builds Start→If→Outcome per connector. Saved Launch-on-Create process with HA + automatic activity + Yes/No decision: 0 TrueChange errors. Generated actions `LaunchApproveOrder` (→ ProcessId) and `Close ApproveOrderTask` (ActivityId → NextHumanActivityId). None of this is in the generated docs/. | VERIFIED |
| D8 | Found in a live skill test (BPT approval process on PVOTest, merged): `SetScreenArgumentValue` does not work on the O11 `IHumanActivityNode` (extension only accepts the ODC BusinessProcesses type, `CS1929`); pass screen inputs with `ha.Arguments.First(a => a.Parameter.Name == "X").SetValue(expr)`. `OnReady` / `OnClose` are null until `CreateOnReady()` / `CreateOnClose()`, which return an empty flow (needs Start/End). `SetUser`, `SetDueDate`, `Description`, `CreateOutputParameter`, `CloseOn` confirmed by calling them. Lesson: the member-mapping helper only counted CS1061/CS0117 as missing, so extension-method mismatches (CS1929) were reported as present | VERIFIED |
| D9 | **Screen access corrected** (found in a live skill test). Setting `screen.AnonymousAccess = true` + `omlMerge` (`merged: true`) left the live screen with `AnonymousAccess = False`. Adding `eSpace.AnonymousRole` to `screen.Roles` + merge → live roles `Anonymous|Registered|…` (Service Studio adds Registered), and the user confirmed the Anonymous checkbox in Service Studio. Template public screens (Login, InvalidPermissions) carry Anonymous in Roles. The earlier conclusion was drawn from the `AnonymousAccess_NoPermissionsNoRoles` warning, which only reflects a module with no roles. Lesson: verify on the live module after omlMerge, not only the saved copy | VERIFIED |
| D10 | Found in a live documentation run (Decommissioner, read-only): `getRoleExceptions` without `sessionToken` → `-32002 This tool requires a valid session token`; with it → works (empty). `runQuery Root { Screens {…} }` → "Schema is not configured correctly to fetch field 'Screens'" (the tool's own description uses it); `Entities`, `References`, `Processes`, `BusinessProcesses`, `Name` work. `listApps` returned only the open modules, no reference rows. A real screen read-back reached ~255 K characters. Site-property defaults held staff email addresses (personal data, not secrets) | VERIFIED |
| D11 | Incorporated from a separate field-testing session (2026-10-01, verified there, not re-run here): expression text via `ToString()`; `GetType().GetInterfaces()` passes the sandbox (corrects the lambda-contract table); `merged:false` handling and the `Theme.IconLibrary` merge rejection on older builds; `IRecordListToExcelNode` output not referenceable; REST URL placeholders need mandatory inputs; consumed REST `BaseURL` / client-owned DTO structures; cron step syntax rejected; multiple `Delete` cascade paths fail at publish with 0 TrueChange errors; OutSystemsCharts and OutSystems UI block details; chained-join `LeftSource` rule | VERIFIED (other session) |
| D12 | Incorporated from further field-testing sessions (2026-10-02/03, verified there, not re-run here): **reverses D11's CSS advice** — edit the theme stylesheet (3/3 published) with `IconLibrary` left null and the sidecar's `InvalidIconLibrary` error ignored; block/screen stylesheet edits to non-empty sheets often do not publish (confirm with curl on the served CSS); 2-character loss after the Theme Editor block (start with a guard rule); theme CSS applies to blank-layout pages too; widget sequences reorder via `ISequence` (`MoveToStart`, `MoveBeforeSibling`, `SetOrderTo`); `CustomStyle` does not render on buttons and `!important` is dropped from extended-property styles; OSUI login/side-menu selectors; puppeteer screenshot loop | VERIFIED (other sessions) |
| D13 | **`omlPublish` and Traditional Web, tested live 2026-10-03.** `omlPublish` without `sessionToken` → `-32002`; on a closed module → "Module 'PVOTest' is not open in Service Studio. Open it before calling omlPublish."; on `OrderManagement` (unchanged) → `{published:true, status:"success", reason:"", messages:[Uploading, Compiling, Deploying, Done]}` in 16 s. Traditional (`BoatsBot`, throw-probes, nothing saved): `getScreenNamesTraditional` / `getWebBlockNamesTraditional` work and the Reactive `getScreenNames` returns `[]`; `CreateScreen`, `SetTitle`, container/text/expression/link widgets, `CreatePreparation` + aggregate, ListRecords, `OnClick.Destination`, `IsFrequentDestination` (clears `DisconnectedDestination`) all work; unqualified `ITextWidget` → CS0311. **Pre-existing bug fixed:** `CreateSessionVariable("X", DataType.Text)` → CS0103; correct form `CreateSessionVariable(false, "X")` then `.DataType = eSpace.TextType` | VERIFIED |
| D14 | `omlPublish` with the permission withheld (user changed the MCP Server dialog setting): immediate `{published:false, status:"blocked", reason:"Publishing requires a write permission the user has not granted. Ask the user to publish from Service Studio, or to allow automatic merge and publish in the MCP Server dialog.", messages:[]}`; nothing published. The permission is the dialog's "automatic merge and publish" setting | VERIFIED |

Method: probes are minimal `applyModelApiCode` lambdas against a scratch Reactive module (`PVOTest`), reporting through `throw new Exception("PROBE:" + s)`; saved states were read back with the `get*` tools and `getValidationMessages`. Nothing was merged or published; the chain was `omlReset` at the end. Traditional Web was not re-tested (no Traditional module open).
