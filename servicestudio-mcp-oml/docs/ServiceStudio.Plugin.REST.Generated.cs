// REST consumption plugin interfaces (endpoints, methods, error handling, authentication)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.REST;

public interface IEndpoint {
    string Url { get; }
    string Description { get; }
}

// Action that consumes REST API endpoints to integrate with external services
public interface IRestAction : OutSystems.Model.Logic.Integrations.IConsumedAction {
    /// <summary>
    /// URL of the method relative to the Base URL of the REST API.\nIt supports using input parameters enclosed in braces, e.g.,\n/drive/v1/files/{id}?convert={convert}
    /// </summary>
    string URLPath { get; set; }
    /// <summary>
    /// HTTP verb used in the request.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.HTTPMethod HTTPMethod { get; set; }
    /// <summary>
    /// Content format of the body request. Mandatory for POST, PUT and PATCH HTTP methods.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.RequestFormat RequestFormat { get; set; }
    /// <summary>
    /// Content format of the body response.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.ResponseFormat ResponseFormat { get; set; }
    /// <summary>
    /// Maximum waiting time to get a response from the Web Service. By default is 100 seconds.
    /// </summary>
    Nullable<int> TimeoutinSeconds { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string SampleRequest { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string SampleResponse { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string DateFormats { get; set; }
    new IEnumerable<IRestActionInput> InputParameters { get; }
    new IEnumerable<IRestActionOutput> OutputParameters { get; }
    new IRestActionInput CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    new IRestActionOutput CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestActionInput : OutSystems.Model.IInputParameter {
    /// <summary>
    /// Name to use for the parameter in the request. Only available when parameter is sent in the header of the request.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.InputPlacement InputPlacement { get; set; }
    /// <summary>
    /// Original name of the input parameter that is used in the HTTP request.\nIf not specified, it has the same value as the Name.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Content-Type of the part associated with the input parameter.
    /// </summary>
    string ContentType { get; set; }
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Set to Yes to hide sensitive information (for example, passwords) in the log entry.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.IsSensitive IsSensitive { get; set; }
}

public interface IRestActionOutput : OutSystems.Model.IOutputParameter {
    /// <summary>
    /// Indicates whether the output parameter is found in the HTTP Header or Body.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.OutputPlacement OutputPlacement { get; set; }
    /// <summary>
    /// Original name of the output parameter that is used in the HTTP response.\nIf not specified, it has the same value as the Name.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Set to Yes to hide sensitive information (for example, passwords) in the log entry.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.IsSensitive IsSensitive { get; set; }
}

public interface IRestCallbackActionFlow : OutSystems.Model.Logic.Integrations.IPluginCallback {
    /// <summary>
    /// 
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.CallbackType CallbackType { get; set; }
    new IEnumerable<IRestCallbackActionFlowInputSignature> InputParameters { get; }
    new IEnumerable<IRestCallbackActionFlowOutputSignature> OutputParameters { get; }
}

public interface IRestCallbackActionFlowInputSignature : OutSystems.Model.IInputParameterSignature {
}

public interface IRestCallbackActionFlowOutputSignature : OutSystems.Model.IOutputParameterSignature {
}

public interface IRestCallbackStructureAttributeSignature : OutSystems.Model.Data.IStructureAttributeSignature {
}

public interface IRestCallbackStructureSignature : OutSystems.Model.Data.IStructureSignature {
    new IEnumerable<IRestCallbackStructureAttributeSignature> Attributes { get; }
}

public interface IRestClient : OutSystems.Model.Logic.Integrations.IClient {
    /// <summary>
    /// The Swagger 2.0 definition URL. This allows the Refresh feature to suggest the previously inserted one.
    /// </summary>
    string SwaggerDefinitionURL { get; set; }
    /// <summary>
    /// The base URL of all methods of the REST API.\nThis value can be customized at runtime in the ODC Portal.
    /// </summary>
    string BaseURL { get; set; }
    /// <summary>
    /// The type of authentication that will be used.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.AuthenticationType AuthenticationType { get; set; }
    /// <summary>
    /// Username sent in the Authorization header of all methods for Basic Authentication. Can be customized at runtime in the ODC Portal.
    /// </summary>
    string Username { get; set; }
    /// <summary>
    /// Password sent in the Authorization header of all methods for Basic Authentication. Can be customized at runtime in the ODC Portal.
    /// </summary>
    string Password { get; set; }
    /// <summary>
    /// App identifier issued by the third-party during the application registration process.
    /// </summary>
    string ClientID { get; set; }
    /// <summary>
    /// Secret issued by the third-party during the application registration process.
    /// </summary>
    string ClientSecret { get; set; }
    /// <summary>
    /// Authorization server's endpoint that the app will use to obtain an access token.
    /// </summary>
    string AccessTokenURL { get; set; }
    /// <summary>
    /// The scopes of the access request. The scopes should be separated by space.\nExample: readwrite.name read.address
    /// </summary>
    string Scopes { get; set; }
    /// <summary>
    /// Select how to send client ID and client secret.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.ClientAuthentication ClientAuthentication { get; set; }
    /// <summary>
    /// Date format used by the Web Service.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.DateFormat DateFormat { get; set; }
    /// <summary>
    /// Defines the producer of this REST API. Selecting O11 ensures all methods count as 0 application objects (AOs).
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.Interoperability Interoperability { get; set; }
    new IEnumerable<IRestAction> Actions { get; }
    new IEnumerable<IRestStaticEntity> StaticEntities { get; }
    new IEnumerable<IRestCallbackActionFlow> Callbacks { get; }
    IRestCallbackActionFlow OnBeforeRequestCallback { get; set; }
    IRestCallbackActionFlow OnAfterResponseCallback { get; set; }
    IRestAction CreateAction(string name = null, OutSystems.Model.IKey key = null);
    IRestStructure CreateStructure(string name = null, OutSystems.Model.IKey key = null);
    IRestStaticEntity CreateStaticEntity(string name = null, OutSystems.Model.IKey key = null);
    IRestCallbackActionFlow CreateOnBeforeRequestCallback();
    IRestCallbackActionFlow CreateOnBeforeRequestAdvancedCallback();
    IRestCallbackActionFlow CreateOnAfterResponseCallback();
    IRestCallbackActionFlow CreateOnAfterResponseAdvancedCallback();
}

public interface IRestMultipartStructureAttributeSignature : OutSystems.Model.Data.IStructureAttributeSignature {
}

public interface IRestMultipartStructureSignature : OutSystems.Model.Data.IStructureSignature {
    new IEnumerable<IRestMultipartStructureAttributeSignature> Attributes { get; }
}

public interface IRestPluginService : OutSystems.Model.IPluginService {
    ISwaggerInfo GetSwaggerInfo(OutSystems.Model.IESpace eSpace, string swaggerJson, Uri swaggerUri = null);
}

public interface IRestStaticEntity : OutSystems.Model.Data.IStaticEntity {
    new IEnumerable<IRestStaticEntityAttribute> Attributes { get; }
    new IRestStaticEntityAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestStaticEntityAttribute : OutSystems.Model.Data.IEntityAttribute {
}

public interface IRestStructure : OutSystems.Model.Data.IStructure {
    new IEnumerable<IRestStructureAttribute> Attributes { get; }
    new IRestStructureAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IRestStructureAttribute : OutSystems.Model.Data.IStructureAttribute {
    /// <summary>
    /// Indicates whether the attribute is sent in the request payload when the attribute is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.REST.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
}

public interface ISwaggerInfo {
    IEnumerable<IEndpoint> Endpoints { get; }
    IEnumerable<string> ExternalReferences { get; }
    bool IsValid { get; }
    string ErrorStatus { get; }
    string ApiName { get; }
}

