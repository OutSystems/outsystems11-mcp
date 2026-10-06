# Module style support (Reactive / Mobile / Traditional Web)

OutSystems 11 modules come in three UI flavours — **Reactive Web**,
**Mobile**, and **Traditional Web** — plus Service / Library variants. The
in-process MCP Server surfaces the module's style on every `listApps` entry
via a `moduleType` field (called `style` in older builds).

## Detecting the style

```jsonc
// listApps response
{
  "apps": [
    {
      "key": "...",
      "name": "FloorPlan",
      "kind": "eSpace",
      "version": "...",
      "description": "...",
      "isOpen": true,
      "isReference": false,
      "style": "Traditional"   // ← see enum below
    }
  ]
}
```

The 8 `moduleType` values are:
`Reactive`, `Mobile`, `Traditional`,
`ReactiveLibrary`, `MobileLibrary`, `TraditionalLibrary`,
`Service`, `Unknown`. Library variants preserve the
parent style — `TraditionalLibrary` is a Traditional library, not a generic
"Library".

## Read verbs per style

Most `get*` verbs work uniformly across all styles (entities, structures,
server actions, roles, validations, sessions, settings, client variables,
images, etc.). A handful of UI-shaped read verbs are **Mobile-bound** in the
underlying sidecar — they walk `eSpace.MobileFlows` only and return empty /
"not found" on Traditional modules. Local Traditional variants cover the gap:

| Need | Reactive / Mobile | Traditional Web |
|---|---|---|
| List screens | `getScreenNames` | `getScreenNamesTraditional` |
| Get one screen | `getScreen` | `getScreenTraditional` |
| List web blocks | `getWebBlockNames` | `getWebBlockNamesTraditional` |
| Get one web block | `getWebBlock` | `getWebBlockTraditional` |
| List themes | `getThemeNames` | `getThemeNamesTraditional` |
| Get one theme | `getTheme` | `getThemeTraditional` |
| List email templates | `getEmailTemplateNames` | `getEmailTemplateNamesTraditional` |
| Get one email template | `getEmailTemplate` | `getEmailTemplateTraditional` |
| List external sites | `getExternalSiteNames` | `getExternalSiteNamesTraditional` |
| Get one external site | `getExternalSite` | `getExternalSiteTraditional` |

Each `*Traditional` tool's description tells you to prefer it when
`listApps.style` is `"Traditional"` or `"TraditionalLibrary"`. The Traditional
single-object verbs return **Model API C# code** (the same dialect as the
Reactive/Mobile verbs and as `applyModelApiCode`'s `code` arg), so you can
pattern-match the output directly. `IExternalSite` is style-specific — there
are two interfaces, `OutSystems.Model.UI.Mobile.IExternalSite` and
`OutSystems.Model.UI.Web.IExternalSite` — so the Traditional variant binds to
the fully-qualified Web type.

## Write verbs per style

`applyModelApiCode` is **fully style-agnostic** — write Traditional mutations
the same way you write Mobile ones. The host runs your snippet (via the
sidecar) against the snapshot using the native Model APIs, so Traditional types
render and mutate fine. The host-default imports ([`../SKILL.md`](../SKILL.md)
§ 4) make `IWebScreen`, `IWebBlock`, `IWebFlow`, `IMobileScreen`, and friends
usable without qualification.

Pattern-matching across styles:

| Concept | Reactive / Mobile | Traditional Web |
|---|---|---|
| Flow collection on `IESpace` | `eSpace.MobileFlows` | `eSpace.WebFlows` |
| Screen interface (concrete) | `IMobileScreen` | `IWebScreen` |
| Screen interface (signature) | `IMobileScreenSignature` | `IWebScreenSignature` |
| Block interface (concrete) | `IMobileBlock` | `IWebBlock` |
| Block interface (signature) | `IMobileBlockSignature` | `IWebBlockSignature` |
| Theme interface (signature) | `IMobileThemeSignature` | `IWebThemeSignature` |
| Email template interface | `IMobileEmail` | `IWebEmail` |
| External site interface | `OutSystems.Model.UI.Mobile.IExternalSite` | `OutSystems.Model.UI.Web.IExternalSite` (must be fully qualified — both interfaces named `IExternalSite`) |
| Session storage | Client variables (`eSpace.ClientVariables`) | Session variables (`eSpace.SessionVariables`) |
| Action namespace under a screen | `screen.ClientActions` (Reactive) | `screen.ScreenActions` |

Full descent recipes for Traditional (find by name, list, mutate, rename,
delete, add a session variable, walk widgets):
[`traditional-patterns.md`](traditional-patterns.md). For
**creating** Traditional Web UI — screens, blocks, widget trees, the
Preparation + Aggregate data-fetch flow, ListRecords binding, and the
`IWebScreen.JavaScript` hook — see
[`traditional-ui-creation.md`](traditional-ui-creation.md).
