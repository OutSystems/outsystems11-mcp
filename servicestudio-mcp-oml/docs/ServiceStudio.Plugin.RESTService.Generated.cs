// REST service publication plugin (exposed methods, contracts, serialization settings)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.RESTService;

public interface IRestService : OutSystems.Model.Logic.Integrations.IService {
    /// <summary>
    /// The base URL of all methods of the REST API.
    /// </summary>
    string BaseURL { get; set; }
    /// <summary>
    /// Base URL of all REST API methods.
    /// </summary>
    string URL { get; set; }
    /// <summary>
    /// Security level required for HTTP requests. Can be overridden by the security level set in the environment or infrastructure.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.HttpSecurity HttpSecurity { get; set; }
    /// <summary>
    /// Required authentication in HTTP requests. The callback action OnAuthentication is made available to implement Basic or Custom authentication.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.Authentication Authentication { get; set; }
    /// <summary>
    /// Only users with access to the internal network can access the Web Service.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.InternalAccess InternalAccess { get; set; }
    /// <summary>
    /// Set to Yes to automatically generate a documentation page at the REST API base URL.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.ShowDocumentation ShowDocumentation { get; set; }
    /// <summary>
    /// TBD
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.CrossSiteRequests CrossSiteRequests { get; set; }
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Defines the consumer of this REST API. Selecting O11 ensures all methods count as 0 application objects (AOs).
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.Interoperability Interoperability { get; set; }
    new IEnumerable<IRestServiceAction> Actions { get; }
    IRestServiceCallbackActionFlow OnRequestCallback { get; set; }
    IRestServiceCallbackActionFlow OnResponseCallback { get; set; }
    IRestServiceAction CreateAction(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceCallbackActionFlow CreateOnRequestCallback();
    IRestServiceCallbackActionFlow CreateOnResponseCallback();
}

public interface IRestServiceAction : OutSystems.Model.Logic.Integrations.IExposedAction {
    /// <summary>
    /// Relative URL where the method is made available.
    /// </summary>
    string URL { get; set; }
    /// <summary>
    /// Customized URL for the method. Use it in order to follow RESTful principles identified by your organization.
    /// </summary>
    string URLPath { get; set; }
    /// <summary>
    /// HTTP verb to access this method.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod HTTPMethod { get; set; }
    new IEnumerable<IRestServiceActionInput> InputParameters { get; }
    new IEnumerable<IRestServiceActionOutput> OutputParameters { get; }
    new IRestServiceActionInput CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    new IRestServiceActionOutput CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestServiceActionInput : OutSystems.Model.IInputParameter {
    /// <summary>
    /// Specifies if the value of the input parameter is to be received in the URL, the Header or Body of the HTTP request.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn ReceiveIn { get; set; }
    /// <summary>
    /// The parameter name received in the request, for example: Content-Language.
    /// </summary>
    string ReceiveAs { get; set; }
    /// <summary>
    /// Set to Yes to hide sensitive information (for example, passwords) in the log entry.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.IsSensitive IsSensitive { get; set; }
}

public interface IRestServiceActionOutput : OutSystems.Model.IOutputParameter {
    /// <summary>
    /// Indicates whether the output parameter is sent in the HTTP Header or Body.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.SendIn SendIn { get; set; }
    /// <summary>
    /// The parameter name sent in the response, for example: Content-Language.
    /// </summary>
    string SendAs { get; set; }
    /// <summary>
    /// Set to Yes to hide sensitive information (for example, passwords) in the log entry.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.IsSensitive IsSensitive { get; set; }
}

public interface IRestServiceAuthenticationFlow : OutSystems.Model.Logic.Integrations.IPluginCallback {
    /// <summary>
    /// 
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.AuthenticationCallbackType AuthenticationCallbackType { get; set; }
    IRestServiceActionInput CreateRestServiceActionInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceAuthenticationFlowInput CreateRestServiceAuthenticationFlowInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceCallbackActionFlowInput CreateRestServiceCallbackActionFlowInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceActionOutput CreateRestServiceActionOutput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceCallbackActionFlowOutput CreateRestServiceCallbackActionFlowOutput(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestServiceAuthenticationFlowInput : OutSystems.Model.IInputParameter {
}

public interface IRestServiceCallbackActionFlow : OutSystems.Model.Logic.Integrations.IPluginCallback {
    /// <summary>
    /// The type of callback handler for this REST service action flow.
    /// </summary>
    ServiceStudio.Plugin.RESTService.Enumerations.CallbackType CallbackType { get; set; }
    IRestServiceActionInput CreateRestServiceActionInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceAuthenticationFlowInput CreateRestServiceAuthenticationFlowInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceCallbackActionFlowInput CreateRestServiceCallbackActionFlowInput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceActionOutput CreateRestServiceActionOutput(string name = null, OutSystems.Model.IKey key = null);
    IRestServiceCallbackActionFlowOutput CreateRestServiceCallbackActionFlowOutput(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestServiceCallbackActionFlowInput : OutSystems.Model.IInputParameter {
}

public interface IRestServiceCallbackActionFlowOutput : OutSystems.Model.IOutputParameter {
}

public interface IRestServiceCallbackStructureAttributeSignature : OutSystems.Model.Data.IStructureAttributeSignature {
}

public interface IRestServiceCallbackStructureSignature : OutSystems.Model.Data.IStructureSignature {
    new IEnumerable<IRestServiceCallbackStructureAttributeSignature> Attributes { get; }
}

