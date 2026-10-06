### Introduction

The Model API is an API written in C# to manipulate the OutSystems model (a graph-like structure for the OutSystems language).
It consists of a set of .Net Framework dlls that provide a low level API to read / create / change OutSystems Solutions, Applications, and eSpaces.
The Model API allows you to automate the operations that you would need to do manually in Service Studio.

You can find the available namespaces listed below. To get more information about a given namespace, open the matching `.cs` file in [`../docs/`](../docs/) (e.g. `OutSystems.Model.Data.Generated.cs` for the `OutSystems.Model.Data` namespace).

### Namespaces

The available namespaces in the Model API are:

- `OutSystems.Model` — root namespace; `IESpace`, primitive types (`TextType`, `IntegerType`, …), node-positioning helpers (`Above` / `Below` / `ToTheLeftOf` / `ToTheRightOf` and their `Connected*` variants), `IFlowNode`, `IObject`
- `OutSystems.Model.Applications` — application-level objects
- `OutSystems.Model.BusinessProcesses` / `.Nodes` — **ODC Workflows** (`CreateBusinessProcess`); fails on O11 modules
- `OutSystems.Model.Processes` — O11 BPT processes (`IProcess`, `IProcessTrigger`, `EntityActionKind` via `eSpace.CreateProcess`), plus timers and global events. `IProcess` is **missing from the generated docs** — see [`logic-and-integrations.md`](logic-and-integrations.md) § "BPT processes (O11)"
- `OutSystems.Model.Processes.Nodes` — O11 BPT node types (`IStartNode`, `IHumanActivityNode`, `IAutomaticActivityNode`, `IDecisionNode`, `IWaitNode`, …) — not in the generated docs either
- `OutSystems.Model.Data` — entities, attributes, structures, identifier attributes, foreign keys (`IServerEntity`, `IClientEntity`, `IStaticEntity`, `IAttribute`, `IStructure`, `ISiteProperty`)
- `OutSystems.Model.Data.ChunkingMethods` — entity-data chunking strategies
- `OutSystems.Model.Data.Nodes` — data-flow node types
- `OutSystems.Model.Enumerations` — enums (`AutoNumber`, `EntityType`, etc.) — auto-imported by default
- `OutSystems.Model.Enumerations.ModelPlugins` — plugin-specific enums
- `OutSystems.Model.Expressions` — expression AST + builders — auto-imported by default
- `OutSystems.Model.Expressions.Nodes` — expression node types
- `OutSystems.Model.Factory` — model creation helpers — auto-imported by default
- `OutSystems.Model.Factory.ChangeSet` — change-set primitives
- `OutSystems.Model.Logic` — logic-layer root: actions, exceptions, roles
- `OutSystems.Model.Logic.Aggregates` — Aggregate base types
- `OutSystems.Model.Logic.Aggregates.Database` — DB-backed aggregates
- `OutSystems.Model.Logic.Aggregates.Full` — full aggregate definitions (filters, sorts, joins)
- `OutSystems.Model.Logic.Integrations` — REST / SOAP / SAP integration definitions
- `OutSystems.Model.Logic.Integrations.SOAP` — SOAP-specific integration types
- `OutSystems.Model.Logic.Mobile.Nodes` — mobile-specific logic node types (e.g. `OutSystems.Model.Mobile.Node.ISendEmailNode`)
- `OutSystems.Model.Logic.Nodes` — logic-flow node types (`IIfNode`, `IForEachNode`, `ISwitchNode`, `IAssignNode`, `IExecuteActionNode`, `ICommentNode`, …)
- `OutSystems.Model.ModelPlugins` — plugin extension points
- `OutSystems.Model.ModelPlugins.UI.Mobile` — UI mobile plugin extensions
- `OutSystems.Model.Plugin.NRWidgets.IconResources` — icon resources for NR widgets
- `OutSystems.Model.Processes` — see the O11 BPT entry above (processes, timers, global events)
- `OutSystems.Model.Signatures` — `IXxxSignature` interfaces (read-only public-element variants — see `model-api-tips.md`)
- `OutSystems.Model.Types` — primitive type interfaces — auto-imported by default
- `OutSystems.Model.UI` — UI root: theme, multilingual, generic UI flow nodes (`IUIFlowNode`, `IImage` for static images)
- `OutSystems.Model.UI.Mobile` — mobile / reactive screens, blocks, flows (`IMobileScreen`, `IMobileFlow`, `IMobileBlock`, `IComment` for UI comments, `IScreenAggregate`)
- `OutSystems.Model.UI.Mobile.Events` — mobile event definitions
- `OutSystems.Model.UI.Mobile.Widgets` — mobile widget types (`IMobileWidget`, all built-in widget interfaces)
- `OutSystems.Model.Versioning` — versioning primitives
- `ServiceStudio.Plugin.NRWidgets` — Service Studio non-reactive widget plugin (`IImage` UI widget — distinct from `OutSystems.Model.UI.IImage`, `ISwitch` widget — distinct from `OutSystems.Model.Logic.Node.ISwitchNode`)
- `ServiceStudio.Plugin.NRWidgets.Enumerations` — NR-widget enums
- `ServiceStudio.Plugin.REST` — REST plugin types
- `ServiceStudio.Plugin.REST.Enumerations` — REST plugin enums
- `ServiceStudio.Plugin.REST.ModelDefinition` — REST model definition types
- `ServiceStudio.Plugin.REST.ModelDefinition.Types` — REST model definition primitive types
- `ServiceStudio.Plugin.REST.Parser` — REST parser primitives
- `ServiceStudio.Plugin.RESTService` — RESTService runtime types
- `ServiceStudio.Plugin.RESTService.Enumerations` — RESTService enums
- `ServiceStudio.Plugin.SOAP` — SOAP plugin types
- `ServiceStudio.Plugin.SOAP.Enumerations` — SOAP plugin enums
- `ServiceStudio.Plugin.SOAP.SOAPTelemetry` — SOAP telemetry types

For curated tips and gotchas (eSpace vs Module, IMobileScreen naming, Signature types, node positioning, duplicate-interface-name traps), see [`model-api-tips.md`](model-api-tips.md).

For worked C# examples that use these namespaces, see [`../examples/`](../examples/) (full `eSpace => { ... }` lambdas).

For full per-namespace docstrings (interfaces, enums, extension methods), open the matching `.Generated.cs` file in [`../docs/`](../docs/).

### Default imports

These 7 namespaces are always in scope inside an `applyModelApiCode` lambda — no `imports` entry needed (the sidecar pre-imports them):

- `System`
- `System.Linq`
- `OutSystems.Model`
- `OutSystems.Model.Enumerations`
- `OutSystems.Model.Expressions`
- `OutSystems.Model.Factory`
- `OutSystems.Model.Types`

Anything else from the list above must be passed in `imports` explicitly.
