# Traditional Web — descent patterns + verb selection

Companion to [`../SKILL.md`](../SKILL.md) § 4.1. Covers everything you need
to read and *mutate existing* **Traditional Web** modules through the MCP
surface. For **creating** new Web screens, blocks, widget trees, the
Preparation + Aggregate data-fetch flow, ListRecords binding, and the
`IWebScreen.JavaScript` hook, see
[`traditional-ui-creation.md`](traditional-ui-creation.md).
Reactive and Mobile remain documented in
[`patterns-by-element.md`](patterns-by-element.md) and
[`model-api-tips.md`](model-api-tips.md) — read those first if you're
working on a Reactive / Mobile module.

## When does this apply

Call `listApps` first. If the module you care about has `moduleType`
equal to `"Traditional"` or `"TraditionalLibrary"`, the patterns in
this file apply. For `"Reactive"`, `"Mobile"`, or their library
variants, use the Mobile-/Reactive-shaped recipes in
[`patterns-by-element.md`](patterns-by-element.md).

The 8-value `moduleType` enum is:
`Reactive` / `Mobile` / `Traditional` / `ReactiveLibrary` / `MobileLibrary` /
`TraditionalLibrary` / `Service` / `Unknown`.

## Read verbs — the 10 local Traditional variants

The non-Traditional read verbs (`getScreen` / `getScreenNames` /
`getWebBlock` / `getWebBlockNames` / `getTheme` / `getThemeNames` /
`getEmailTemplate` / `getEmailTemplateNames` / `getExternalSite` /
`getExternalSiteNames`) are Mobile-bound — they walk `eSpace.MobileFlows`
and pass `IMobile*` type parameters, and the external-site reads resolve the
unqualified `IExternalSite` to the Mobile interface — so they can't see
Traditional Web objects. See [`verb-reference.md`](verb-reference.md) for the
per-verb details. The Traditional variants below target the
`OutSystems.Model.UI.Web` tree instead:

| Verb | Args | Returns |
|---|---|---|
| `getScreenNamesTraditional` | — | JSON `[{ Name, Description, UIFlow, IsReferenced }]` over `eSpace.WebFlows` + references |
| `getWebBlockNamesTraditional` | — | Same shape, over `IWebBlockSignature` |
| `getScreenTraditional` | `objectName: string`, `includeJson: "Never"` | Markdown-fenced **Model API C# code** — full screen tree |
| `getWebBlockTraditional` | `objectName`, `includeJson` | Same Model-API-code shape over `IWebBlockSignature` |
| `getThemeNamesTraditional` | — | JSON `[{ Name, Description, IsReferenced }]` (no `UIFlow` — themes are eSpace-level) |
| `getThemeTraditional` | `objectName`, `includeJson` | Markdown-fenced **Model API C# code** — full theme tree |
| `getEmailTemplateNamesTraditional` | — | JSON `[{ Name, Description, UIFlow, IsReferenced }]` over WebFlows (email templates live inside `IWebFlow.Nodes`) |
| `getEmailTemplateTraditional` | `objectName`, `includeJson` | Same Model-API-code shape over `IWebEmail` |
| `getExternalSiteNamesTraditional` | — | JSON `[{ Name, Description, UIFlow, IsReferenced }]` over WebFlows |
| `getExternalSiteTraditional` | `objectName`, `includeJson` | Same Model-API-code shape over `OutSystems.Model.UI.Web.IExternalSite` |

`includeJson` is required on the code-returning single-object reads; the host
forces it to `"Never"` — pass `"Never"`.

### Output shape — important

All 5 single-object verbs (`getScreenTraditional`, `getWebBlockTraditional`,
`getThemeTraditional`, `getEmailTemplateTraditional`,
`getExternalSiteTraditional`) return **Model API C# code**, not JSON — the same
C# dialect as the Reactive/Mobile read verbs and the same dialect you emit into
`applyModelApiCode`, so you **can** pattern-match the output directly when
composing a mutation.

The 5 `*Names` verbs return the same shape as their Mobile counterparts —
`Name`, `Description`, `IsReferenced` (plus `UIFlow` for screen/web-block/
email-template/external-site names, which iterate `WebFlows`; absent on
theme names, since themes are eSpace-level via `GetAllDescendantsOfType`
and have no enclosing flow).

### ExternalSite — the special-case dual-interface gotcha

`IExternalSite` is the only Traditional interface in this catalogue that
has the same name as a Mobile sibling: `OutSystems.Model.UI.Mobile.IExternalSite`
and `OutSystems.Model.UI.Web.IExternalSite`. The Mobile-bound `getExternalSite`
verb resolves the unqualified `IExternalSite` to Mobile when both namespaces are
in scope, producing a verb that can never see Traditional external sites. The
`*Traditional` verbs use the fully-qualified `OutSystems.Model.UI.Web.IExternalSite`
to bypass the ambiguity.

### When to fall back to upstream

`getSerializedObjectByNameAndType` works on Traditional today
when called with a fully-qualified `objectType`:

```jsonc
{
  "objectName": "Dashboard",
  "objectType": "OutSystems.Model.UI.Web.IWebScreen"
}
```

This serialized path returns **JSON**, so it produces a **different shape** than
`getScreenTraditional`, which returns Model API code. Prefer the Traditional verb
when you want the C# dialect to pattern-match for a mutation (and to avoid
threading the type FQN); reach for `getSerializedObjectByNameAndType` only when
you specifically want the raw JSON view of an object.

## Write side — `applyModelApiCode` is style-agnostic

The host runs your snippet inside the Service Studio process using
its native Model APIs, which handle Traditional
types correctly. The host's default `imports` include
`OutSystems.Model.UI.Web` (see [`../SKILL.md`](../SKILL.md) § 4), so
`IWebScreen` / `IWebBlock` / `IWebFlow` etc. work unqualified with no
agent-supplied `imports`.

### Common descent patterns

The snippets below are the **statements that go inside the lambda** — wrap them
in `eSpace => { ... }` when you send them as the `code` arg, with no explicit
`eSpace.Save(...)` (the host appends Save for you). See
[`lambda-contract.md`](lambda-contract.md).

#### Find a screen by name

```csharp
var screen = eSpace.WebFlows
    .SelectMany(f => f.Nodes)
    .OfType<IWebScreen>()
    .First(s => s.Name == "Dashboard");
```

`GetAllDescendantsOfType<IWebScreen>()` on the eSpace also works and walks
references too if you want them; restrict to `eSpace.WebFlows` for local
screens only.

#### List all screens via Model API (no verb)

When you need the full IModelObject (not just a name list) without one
roundtrip per screen:

```csharp
var screens = eSpace.WebFlows
    .SelectMany(f => f.Nodes)
    .OfType<IWebScreen>()
    .ToList();
// `screens` is now in scope; iterate and read .Name / .Description / etc.
```

#### Walk a screen's ScreenActions

Traditional screens have a `ScreenActions` collection (NOT
`ClientActions` — that's the Reactive name):

```csharp
var screen = eSpace.WebFlows
    .SelectMany(f => f.Nodes)
    .OfType<IWebScreen>()
    .First(s => s.Name == "Plan_Edit");
foreach (var action in screen.ScreenActions) {
    // action is a screen-scoped action with its own flow.
}
```

#### Find a web block by name

```csharp
var block = eSpace.WebFlows
    .SelectMany(f => f.GetAllDescendantsOfType<IWebBlock>())
    .First(b => b.Name == "Footer");
```

#### Find a session variable

Session variables are eSpace-level (not per-screen):

```csharp
var username = eSpace.SessionVariables.Named("Username");
username.Description = "Logged-in user identifier";
```

`eSpace.SessionVariables.Named(...)` throws on missing — guard with
`FirstOrDefault(v => v.Name == "...")` when uncertain.

### Common mutations

#### Rename a screen

```csharp
var screen = eSpace.WebFlows
    .SelectMany(f => f.Nodes)
    .OfType<IWebScreen>()
    .First(s => s.Name == "Dashboard");
screen.Name = "Overview";
```

The host saves the mutated OML; follow up with `omlMerge` to surface
the diff in Service Studio.

#### Set a screen's description

```csharp
var screen = eSpace.WebFlows
    .SelectMany(f => f.Nodes)
    .OfType<IWebScreen>()
    .First(s => s.Name == "Dashboard");
screen.Description = "Top-level KPI dashboard.";
```

#### Add a session variable

```csharp
// Signature: CreateSessionVariable(bool isReadOnly = false, string name = null, IKey key = null).
// The type is set afterwards. (The old form CreateSessionVariable("X", DataType.Text) does not
// compile: CS0103 'DataType' does not exist. Verified 2026-10-03 on a Traditional module.)
var userCulture = eSpace.CreateSessionVariable(false, "UserCulture");
userCulture.DataType = eSpace.TextType;
```

The created variable's `DefaultValue` / `Description` are settable
via subsequent assignments.

## Gotchas

- **`IWebBlock` vs `IMobileBlock` naming**: the Mobile-bound verb is
  misleadingly named `getWebBlock` — that name is not a hint that it walks
  `IWebBlock`; it's Mobile-bound. Use `getWebBlockTraditional` for actual
  Traditional web blocks.
- **`screen.Screens` doesn't exist on `IWebFlow`**. Walk via
  `flow.Nodes.OfType<IWebScreen>()` or
  `flow.GetAllDescendantsOfType<IWebScreen>()`. The Mobile equivalent
  `IMobileFlow.Screens` doesn't have a Traditional analogue.
- **`eSpace.ModuleType` doesn't exist on `IESpace`.** Use the `moduleType`
  field on `listApps` instead — the verb selection already encodes the
  style, so a runtime check is rarely needed.
- **`Signature` types catch both local and referenced objects.**
  `IWebScreenSignature` matches the concrete local `WebScreen` AND
  `ReferenceWebScreen`. Use `IWebScreen` (no `Signature`) when you want
  only local screens.
- **The 5 single-object Traditional verbs
  (`getScreenTraditional`, `getWebBlockTraditional`,
  `getThemeTraditional`, `getEmailTemplateTraditional`,
  `getExternalSiteTraditional`) return Model API code**, the same
  dialect as `applyModelApiCode`'s `code` arg — you can pattern-match
  it directly when composing a mutation. The descent
  recipes above are still the cleanest starting point for mutations
  (e.g. `eSpace.GetAllDescendantsOfType<IWebTheme>()` for themes,
  `eSpace.WebFlows.SelectMany(f => f.GetAllDescendantsOfType<IWebEmail>())`
  for email templates, `eSpace.GetAllDescendantsOfType<OutSystems.Model.UI.Web.IExternalSite>()`
  for external sites).
- **`IExternalSite` is ambiguous** in any context that imports both
  `OutSystems.Model.UI.Mobile` and `OutSystems.Model.UI.Web`. Always
  qualify it as `OutSystems.Model.UI.Web.IExternalSite` (or
  `OutSystems.Model.UI.Mobile.IExternalSite` if you mean the other
  variant) inside `applyModelApiCode` lambdas. The host's default
  imports include both namespaces, so the bare name will not compile.

## Output recap

The 5 single-object `*Traditional` verbs (`getScreenTraditional`,
`getWebBlockTraditional`, `getThemeTraditional`, `getEmailTemplateTraditional`,
`getExternalSiteTraditional`) return Model API C# code — the same dialect as
their Reactive/Mobile counterparts, but Traditional-aware. The write side uses
the same `applyModelApiCode` lambda and the descent recipes above.
