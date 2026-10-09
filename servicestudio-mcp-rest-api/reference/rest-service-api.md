# The REST Service Model API surface

Everything here is fully-qualified as it must appear in `applyModelApiCode` code
(remember: `imports` can't include `ServiceStudio.Plugin.RESTService`, so no
`using` shortcuts — see SKILL.md § 1.2). Reflected from
`ServiceStudio.Plugin.RESTService.Generated.cs` and
`ServiceStudio.Plugin.RESTService.Enumerations.Generated.cs` in the
`servicestudio-mcp-oml` skill's `docs/` — those files are the source of truth if
this drifts; this file exists because the `servicestudio-mcp-oml` SKILL.md and
reference files never mention this namespace at all.

## Creating the integration

There is no `eSpace.RestServices` collection or `eSpace.CreateRestService(...)`.
Use the generic integration factory on `IESpace`:

```csharp
OutSystems.Model.Logic.Integrations.T CreateIntegration<T>(string name = null, IKey key = null)
    where T: OutSystems.Model.Logic.Integrations.IIntegration;
```

```csharp
var api = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("ShippingCompanyAPI");
```

## `IRestService` (the API itself)

```csharp
public interface IRestService : OutSystems.Model.Logic.Integrations.IService {
    string BaseURL { get; set; }              // read-only in practice; derived from the module
    string URL { get; set; }                  // base URL of all methods — derived, don't set
    ServiceStudio.Plugin.RESTService.Enumerations.HttpSecurity HttpSecurity { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.Authentication Authentication { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.InternalAccess InternalAccess { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.ShowDocumentation ShowDocumentation { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.CrossSiteRequests CrossSiteRequests { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.Interoperability Interoperability { get; set; }
    IEnumerable<IRestServiceAction> Actions { get; }
    IRestServiceAction CreateAction(string name = null, IKey key = null);
    // OnRequestCallback / OnResponseCallback + their Create* methods exist too,
    // for request/response interceptors — out of scope for a plain CRUD API.
}
```

## `IRestServiceAction` (one HTTP method)

```csharp
public interface IRestServiceAction : OutSystems.Model.Logic.Integrations.IExposedAction {
    // IExposedAction : IAction, so this has everything a server action has:
    // Nodes, CreateNode<T>, CreateInputParameter, CreateOutputParameter,
    // CreateLocalVariable — plus an ALREADY-PRESENT Start node and End node
    // (see SKILL.md § 1.3 — do not create new ones).
    string URL { get; set; }                              // derived — don't set
    string URLPath { get; set; }                           // SET THIS — must start with '/'
    ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod HTTPMethod { get; set; }
    new IEnumerable<IRestServiceActionInput> InputParameters { get; }
    new IEnumerable<IRestServiceActionOutput> OutputParameters { get; }
    new IRestServiceActionInput CreateInputParameter(string name = null, IKey key = null);
    new IRestServiceActionOutput CreateOutputParameter(string name = null, IKey key = null);
}
```

## `IRestServiceActionInput` / `IRestServiceActionOutput`

```csharp
public interface IRestServiceActionInput : OutSystems.Model.IInputParameter {
    ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn ReceiveIn { get; set; }  // URL / Header / Body
    string ReceiveAs { get; set; }             // wire name if different from the param name
    ServiceStudio.Plugin.RESTService.Enumerations.IsSensitive IsSensitive { get; set; }
}

public interface IRestServiceActionOutput : OutSystems.Model.IOutputParameter {
    ServiceStudio.Plugin.RESTService.Enumerations.SendIn SendIn { get; set; }        // Body / Header
    string SendAs { get; set; }
    ServiceStudio.Plugin.RESTService.Enumerations.IsSensitive IsSensitive { get; set; }
}
```

A `{Id}` path placeholder in `URLPath` (e.g. `"/ShippingCompanies/{Id}"`) needs a
matching input parameter named `Id` with `ReceiveIn = ReceiveIn.URL` — the name
match is how Service Studio binds the placeholder to the parameter.

## Enumerations (`ServiceStudio.Plugin.RESTService.Enumerations`)

```csharp
public enum Authentication { None, Basic, Custom }
public enum AuthenticationCallbackType { None, Custom, Basic, OAuth2 }
public enum CallbackType { Unknown, OnRequest, OnResponse }
public enum CrossSiteRequests { Allow, Restrict }
public enum DefaultValueBehavior { Send, DontSend }
public enum HTTPMethod { GET, PUT, POST, DELETE, PATCH }
public enum HttpSecurity { None, SSL }
public enum InternalAccess { No, Yes }
public enum Interoperability { No, Yes }              // "Yes" = O11 consumer, methods don't count as AOs
public enum IsSensitive { Sensitive, NotSensitive }
public enum ReceiveIn { URL, Header, Body }
public enum SendIn { Body, Header }
public enum ShowDocumentation { No, Yes }
```

## Entity CRUD action signatures (for wiring `IExecuteServerActionNode`)

These live on `IServerEntitySignature` / `IEntitySignature`, not on the REST
plugin — but you need them to build the flows. Confirmed by direct `runQuery`
against a live module (see SKILL.md § 2) rather than assumed from convention,
because the `Get` action's output name is a common wrong guess:

| Property | Returns | Input param(s) | Output param(s) |
|---|---|---|---|
| `entity.GetAction` | `IEntityActionSignature` | `Id` | **`Record`** |
| `entity.CreateAction` | `IEntityActionSignature` | `Source` | `Id` |
| `entity.UpdateAction` | `IEntityActionSignature` | `Source` | — |
| `entity.DeleteAction` | `IEntityActionSignature` | `Id` | — |
| `entity.CreateOrUpdateAction` | `IEntityActionSignature` | `Source` | `Id` |
| `entity.DeleteAllAction` | `IEntityActionSignature` | — | — |

`IEntityActionSignature : IActionSignature`, so it's a drop-in value for
`IExecuteServerActionNode.Action` — see `servicestudio-mcp-oml`'s
`examples/AddServerActionCreateOrUpdate.cs` and `AddServerActionDelete.cs` for
the base `SetArgumentValue` pattern this skill reuses inside a REST action's
flow instead of a plain server action's.
