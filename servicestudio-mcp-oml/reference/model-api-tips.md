### 5.3 Tips and Best Practices

#### Model API naming vs. Public OutSystems Documentation naming

The Model API sometimes uses different naming conventions than the Public OutSystems Documentation:

* An OutSystems Module is referred to as an "eSpace" in the Model API and represented by `IESpace` objects (from the `OutSystems.Model` namespace).
* UI-related objects usually have "Mobile" in their interface names in the Model API, while in the Public OutSystems Documentation they are often referred to as "UI": e.g., screens are represented by `IMobileScreen` objects (from the `OutSystems.Model.UI.Mobile` namespace), UI widgets are represented by `IMobileWidget` objects (from the `OutSystems.Model.UI.Mobile.Widgets` namespace), UI flows are represented by `IMobileFlow` objects (from the `OutSystems.Model.UI.Mobile` namespace), etc.
* App Settings are referred to as "SiteProperties" in the Model API and represented by `ISiteProperty` objects (from the `OutSystems.Model.Data` namespace).

#### Per-style namespace map (Reactive / Mobile vs. Traditional Web)

The "Mobile" naming above applies to Reactive Web and Mobile modules. **Traditional Web modules use a parallel namespace tree under `OutSystems.Model.UI.Web`** with `Web`-prefixed interface names:

| Concept | Reactive / Mobile (`OutSystems.Model.UI.Mobile`) | Traditional Web (`OutSystems.Model.UI.Web`) |
|---|---|---|
| Flow collection on `IESpace` | `eSpace.MobileFlows` | `eSpace.WebFlows` |
| Flow interface | `IMobileFlow` | `IWebFlow` |
| Screen interface | `IMobileScreen` | `IWebScreen` |
| Block interface | `IMobileBlock` | `IWebBlock` |
| Theme (signature) | `IMobileThemeSignature` | `IWebThemeSignature` |
| Email template | `IMobileEmail` | `IWebEmail` (in `OutSystems.Model.UI.Web`; confirmed by `EmailScreen.Generated.cs:15` — `partial class EmailScreen : ... IWebEmail`) |
| External site | `OutSystems.Model.UI.Mobile.IExternalSite` | `OutSystems.Model.UI.Web.IExternalSite` — both interfaces are named `IExternalSite` so the FQN is mandatory whenever both namespaces are in scope (which the host's default imports guarantee — if both were imported in a single file, the unqualified reference is a CS0104 ambiguity error). The Mobile-bound `getExternalSiteNames` can miss a local Web external site for the same reason — when the unqualified `IExternalSite` reference resolves to the Mobile interface, the Web interface is unreachable; use `getExternalSiteNamesTraditional` for Traditional Web. |

`Signature` variants (e.g. `IWebScreenSignature`) catch both local concrete classes (`WebScreen`) AND `Reference*` variants from other modules. Use the non-Signature interface (`IWebScreen`) when you want only local objects.

The MCP host adds `OutSystems.Model.UI`, `OutSystems.Model.UI.Web`, and `OutSystems.Model.UI.Mobile` for `applyModelApiCode` — so all of the interfaces in the table above work unqualified inside an `applyModelApiCode` lambda. See [`../SKILL.md`](../SKILL.md) § 4 for the full default-imports list.

For Traditional descent recipes (find by name, list, mutate, walk widgets, add a session variable) see [`traditional-patterns.md`](traditional-patterns.md). The module's style is exposed on `listApps` → `moduleType` field.

#### Duplicate Interface Names

Some interfaces have the same or similar names but belong to different namespaces and serve different purposes. The following cases are noteworthy:

* `ServiceStudio.Plugin.NRWidgets.IImage` vs. `OutSystems.Model.UI.IImage`: The former represents a UI widget for displaying images (which can be static, from the database, or from a URL), while the latter represents a static image object that was added to the application.
* `OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode` vs. `OutSystems.Model.Logic.Nodes.ISendEmailNode`: The latter represents a legacy Send Email node in logic flows used in traditional web applications, while the former represents the Send Email node used in mobile and reactive applications (it has an `Email` property pointing at the `IMobileEmail`). Prefer `OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode` in Reactive/Mobile modules — verified live 2026-09-30, see [`../examples/AddServerActionSendEmail.cs`](../examples/AddServerActionSendEmail.cs).
* `ServiceStudio.Plugin.NRWidgets.ISwitch` vs. `OutSystems.Model.Logic.Node.ISwitchNode`: The former represents a UI widget for toggling between two states, while the latter represents a control structure used in logic flows (i.e., actions).
* `OutSystems.Model.UI.Mobile.IComment` vs. `OutSystems.Model.Logic.Nodes.ICommentNode`: Both represent comment nodes, but they are used in different contexts. The former is used in UI flows, while the latter is used in logic flows (i.e., actions).
* `OutSystems.Model.UI.Mobile.IScreenAggregate` vs. `OutSystems.Model.Logic.Nodes.IAggregateNode`: Both represent aggregates, but they are used in different contexts. The former is used for screen aggregates, while the latter is used to represent an aggregate used in a logic flow (i.e., action).
* `OutSystems.Model.IFlowNode` vs. `OutSystems.Model.UI.IUIFlowNode`: The former represents a generic flow node that can be part of any flow (UI or logic), while the latter specifically represents a flow node within a UI flow.
* **Web vs. Mobile widgets** — `OutSystems.Model.UI.Web.Widgets.ITextWidget` vs. `OutSystems.Model.UI.Mobile.Widgets.ITextWidget` (and likewise `IContent`, `IIfWidget`, `IIfBranchWidget`, `IPlaceholderWidget`, `IPlaceholderContentWidget`): these widget names exist in **both** the Web and Mobile widget namespaces, and the host's default imports bring both into scope — so a bare `ITextWidget` is a `CS0104` ambiguity. Inside a Traditional body, fully-qualify the Web type in `CreateWidget<T>` (e.g. `CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>()`) and do **not** add `using OutSystems.Model.UI.Mobile.Widgets;`. Relatedly, `CreateWidget<T>` constrains `T : OutSystems.Model.UI.Web.Widgets.IWebWidget`; passing a Reactive `ServiceStudio.Plugin.NRWidgets.*` widget fails `CS0311`. Full recipes: [`traditional-ui-creation.md`](traditional-ui-creation.md#widget-namespace-collisions).

When working with the Model API, always ensure you are using the correct interface from the appropriate namespace for your specific use case.

#### Manipulating Nodes in Logic Flows

In addition to making sure that the flow is logically correct, you should also ensure that the corresponding graph is displayed correctly in Studio when the user opens the action. Model API offers methods (`Above`, `Below`, `ToTheLeftOf`, and `ToTheRightOf`), available in the `OutSystems.Model` namespace, that allow you to specify the relative position of a new node with respect to an existing node. It also offers methods that allow you to simultaneously connect a node to an existing node and define its relative position to the existing node. These methods are: `ConnectedAbove`, `ConnectedBelow`, `ConnectedToTheLeftOf`, and `ConnectedToTheRightOf`, available in the `OutSystems.Model` namespace. You should prefer using these methods over manually setting the Target property of the node, since the latter only connects the node logically but does not take care of the visual representation. However, when connecting a new node to an existing node that does not have a `Target` property (e.g., `IIfNode`, `ISwitchNode`), you cannot use the `Connected*` methods and therefore still need to manually set the connection (e.g., for `IIfNode`, you need to set either the `TrueTarget` or `FalseTarget` property).

> **Layout is a completion gate, not optional polish.** Any action that gained new or spliced nodes must get a full visual layout pass before you merge — see SKILL.md § "Flow layout is a completion gate". What the gate requires, in order:
> 1. **Lay out the whole touched flow with a BFS pass** that walks from the action's Start node along each node's `Connectors` and assigns `HorizontalPosition` / `VerticalPosition` level by level. The host logs `Action Flow auto arrange completed` on stderr after a save, but that does **not** position hand-wired nodes (verified 2026-09-30: three manually targeted nodes stayed stacked at one coordinate), and `ArrangeAllNodes()` alone doesn't untangle spliced nodes either.
> 2. **Prefer the `Connected*` helpers** over manually setting `Target` — they set both the logical link and the visual position in one step. For nodes with no `Target` (e.g. `IIfNode`, `ISwitchNode`) wire `TrueTarget` / `FalseTarget` / `OtherwiseTarget` yourself, then position explicitly.
> 3. **Verify links before merging.** Read back `node.Connectors.Select(c => c.Target)` (or the relevant `*Target`) and confirm the endpoints are what you intended; report them through the throw-probe ([`lambda-contract.md`](lambda-contract.md) § "Sandbox") rather than deferring the check. (Earlier builds silently dropped `.Target` on a freshly created node; this did not reproduce on 2026-09-30 — manual `Target` / `TrueTarget` / `FalseTarget` / `CycleTarget` all created connectors — but checking costs nothing.)

##### Walking a flow — typed, no reflection

Reflection is rejected by the MCP code sandbox, and none is needed:

- `node.Connectors` — outgoing `IActionConnector`s; each has a typed `Target` (`IActionNode`). `node.IncomingConnectors` for the reverse.
- Branch-specific targets live on the node type: `IIfNode.TrueTarget` / `FalseTarget`, `ISwitchNode.OtherwiseTarget` (+ `Conditions`), `IForEachNode.CycleTarget` (loop body) / `Target` (exit).
- Positions are `IFlowNode.HorizontalPosition` / `VerticalPosition` (settable `int`s).
- `node.GetType().Name` is allowed if you need to print a node's kind (the concrete RaiseException node reports `RaiseError`; SQL reports `AdvancedQuery`; exception handlers `ErrorHandler`).

##### The validated BFS layout pass

Full lambda: [`../examples/LayoutActionFlow.cs`](../examples/LayoutActionFlow.cs) (verified live 2026-09-30 on a flow with an SQL node, a For Each and an exception handler). Its rules:

- Grid units are **1828 × 1371**, origin **(3200, 800)**.
- BFS from Start over `Connectors`; a node keeps the **first** slot it is given, so For Each loop-backs and merge points never inflate depth (a naive "relax to max depth" BFS pushes `ForEach` flows ~100k units off-canvas — observed live: depth 81+). Cap depth at 40.
- An `If`'s `FalseTarget` and a `ForEach`'s `CycleTarget` go **one column right**; a loop body sits on the For Each's row.
- **Exception-handler paths aren't reachable from Start** — lay each handler chain out in its own column to the right.
- **Skip nodes the walk never reached** rather than defaulting them to (0, 0), which stacks them on top of Start.

Applies to server/client/screen actions, service actions and exposed REST methods alike — all expose `Nodes` with typed `Connectors`.

#### Positioning Widgets inside Containers

When a new widget is added to a container, by default it is added to the end of the container's widget collection. However, in many cases, you may want to position the new widget at a specific location within the container to maintain the intended layout and functionality. The Model API provides methods to facilitate this: `MoveBeforeSibling`, `MoveAfterSibling`, `MoveToNewAbsoluteIndex`, and `MoveToNewRelativeIndex`, available in the `OutSystems.Model.IObject` interface.

#### Signature Data Types

For each interface whose corresponding object can be made public elements (e.g., entities, structures, actions, etc.) there is another interface with the same name and the suffix "Signature" (e.g., `IEntity` and `IEntitySignature`). The interface without the "Signature" suffix is used for local objects defined within the eSpace, while the "Signature" version encompasses both local objects and public elements that were imported from other eSpaces and are read-only. Interfaces without the "Signature" suffix inherit from their "Signature" counterparts.
