// SOAP consumption plugin (service references, operations, WSDL-derived artifacts)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.SOAP;

public interface ISettings {
    string GetRecentUrls();
    IList<string> GetRecentUrlsList();
    void AddRecentUrl(Uri uri);
    void ClearRecentUrls();
}

public interface ISOAPAction : OutSystems.Model.Logic.Integrations.IConsumedAction {
    /// <summary>
    /// Maximum time this method waits for a synchronous Web Service request to complete and after which an exception is raised. If unspecified, the timeout observed is 100 seconds.
    /// </summary>
    Nullable<int> TimeoutInSeconds { get; set; }
    /// <summary>
    /// Name of the element as defined in the module which implements it (producer module). This property is read-only.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    Nullable<int> Cache { get; set; }
    /// <summary>
    /// Name of the method as it will appear on WCF generated proxy code. Contructed from the original name, with invalid characters removed and after nameclashing.
    /// </summary>
    string WcfName { get; set; }
    ISOAPDynamicUsernameInput CreateSOAPDynamicUsernameInput(string name = null, OutSystems.Model.IKey key = null);
    ISOAPDynamicPasswordInput CreateSOAPDynamicPasswordInput(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionInput CreateSOAPActionInput(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionInputPolymorphic CreateSOAPActionInputPolymorphic(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionInputWildcard CreateSOAPActionInputWildcard(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionOutput CreateSOAPActionOutput(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionOutputPolymorphic CreateSOAPActionOutputPolymorphic(string name = null, OutSystems.Model.IKey key = null);
    ISOAPActionOutputWildcard CreateSOAPActionOutputWildcard(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPActionInput : OutSystems.Model.IInputParameter {
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Placement of the parameter in the request.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.InputPlacement InputPlacement { get; set; }
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name of the input parameter as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the input parameter as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    /// <summary>
    /// Default value of the attribute defined in the WSDL.
    /// </summary>
    string OriginalDefaultValue { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
    /// <summary>
    /// Order of the input in the WSDL
    /// </summary>
    string Order { get; set; }
}

public interface ISOAPActionInputPolymorphic : OutSystems.Model.IInputParameter {
    /// <summary>
    /// Name of the input parameter as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the input parameter as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Placement of the parameter in the request.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.InputPlacement InputPlacement { get; set; }
}

public interface ISOAPActionInputWildcard : OutSystems.Model.IInputParameter {
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Placement of the parameter in the request.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.InputPlacement InputPlacement { get; set; }
    /// <summary>
    /// Namespace as defined in the WSDL. Used if namespace is ##targetNamespace
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// The namespaces containing the elements that can be used. If no namespace is specified, any may be used.
    /// </summary>
    string Namespace { get; set; }
    /// <summary>
    /// An indicator of how an application or XML processor should handle validation of XML documents against the elements specified by this any element.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.ProcessContents ProcessContents { get; set; }
    /// <summary>
    /// Specifies the specific kind of wildcard this object represents.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.WildcardType WildcardType { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
}

public interface ISOAPActionOutput : OutSystems.Model.IOutputParameter {
    /// <summary>
    /// Placement of the parameter in the response.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.OutputPlacement OutputPlacement { get; set; }
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name of the output parameter as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the output parameter as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
    /// <summary>
    /// Order of the output in the WSDL
    /// </summary>
    string Order { get; set; }
}

public interface ISOAPActionOutputPolymorphic : OutSystems.Model.IOutputParameter {
    /// <summary>
    /// Name of the output parameter as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the output parameter as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Placement of the parameter in the response.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.OutputPlacement OutputPlacement { get; set; }
}

public interface ISOAPActionOutputWildcard : OutSystems.Model.IOutputParameter {
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Placement of the parameter in the response.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.OutputPlacement OutputPlacement { get; set; }
    /// <summary>
    /// Namespace as defined in the WSDL. Used if namespace is ##targetNamespace
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// The namespaces containing the elements that can be used. If no namespace is specified, any may be used.
    /// </summary>
    string Namespace { get; set; }
    /// <summary>
    /// An indicator of how an application or XML processor should handle validation of XML documents against the elements specified by this any element.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.ProcessContents ProcessContents { get; set; }
    /// <summary>
    /// Specifies the specific kind of wildcard this object represents.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.WildcardType WildcardType { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
}

public interface ISOAPCallbackActionFlow : OutSystems.Model.Logic.Integrations.IPluginCallback {
    /// <summary>
    /// 
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.CallbackType CallbackType { get; set; }
}

public interface ISOAPClient : OutSystems.Model.Logic.Integrations.IClient {
    /// <summary>
    /// Authentication type for each request.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.AuthenticationType AuthenticationType { get; set; }
    /// <summary>
    /// Username sent for authenticating requests. Can be customized at runtime using Dynamic Authentication or in the consoles.
    /// </summary>
    string AuthenticationUsername { get; set; }
    /// <summary>
    /// Password sent for authenticating requests. Can be customized at runtime using Dynamic Authentication or in the consoles.
    /// </summary>
    string AuthenticationPassword { get; set; }
    /// <summary>
    /// Location where the WSDL is obtained.
    /// </summary>
    string WSDLLocation { get; set; }
    /// <summary>
    /// WSDL with the definition of the functionality provided by this SOAP Web Service.
    /// </summary>
    string WSDL { get; set; }
    /// <summary>
    /// Binding of the SOAP Web Service.
    /// </summary>
    string Binding { get; set; }
    /// <summary>
    /// Name of the element as defined in the module which implements it (producer module). This property is read-only.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Contract of the SOAP Web Service.
    /// </summary>
    string Contract { get; set; }
    /// <summary>
    /// Default Uri of the SOAP Web Service. Can be overriden.
    /// </summary>
    string DefaultUri { get; set; }
    /// <summary>
    /// MetaData of the Web Service as provided in the WSDL.
    /// </summary>
    string MetaData { get; set; }
    /// <summary>
    /// The actions imported can be dynamically authenticated.
    /// </summary>
    bool IsDynamicallyAuthenticated { get; set; }
    /// <summary>
    /// Set to Yes to use Integrated Authentication to validate user access.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.IntegratedAuthentication IntegratedAuthentication { get; set; }
    new IEnumerable<ISOAPAction> Actions { get; }
    ISOAPAction CreateAction(string name = null, OutSystems.Model.IKey key = null);
    ISOAPStructure CreateSOAPStructure(string name = null, OutSystems.Model.IKey key = null);
    ISOAPStructurePolymorphic CreateSOAPStructurePolymorphic(string name = null, OutSystems.Model.IKey key = null);
    ISOAPStaticEntity CreateSOAPStaticEntity(string name = null, OutSystems.Model.IKey key = null);
    ISOAPStaticEntityPolymorphic CreateSOAPStaticEntityPolymorphic(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPDynamicPasswordInput : OutSystems.Model.IInputParameter {
}

public interface ISOAPDynamicUsernameInput : OutSystems.Model.IInputParameter {
}

public interface ISOAPStaticEntity : OutSystems.Model.Data.IStaticEntity {
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    /// <summary>
    /// 
    /// </summary>
    bool IsAnonymous { get; set; }
    /// <summary>
    /// When the enumeration is anonymous save the parent element name.
    /// </summary>
    string ParentElement { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string XmlKind { get; set; }
    new IEnumerable<ISOAPStaticEntityAttribute> Attributes { get; }
    new ISOAPStaticEntityAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPStaticEntityAttribute : OutSystems.Model.Data.IEntityAttribute {
}

public interface ISOAPStaticEntityAttributePolymorphic : OutSystems.Model.Data.IEntityAttribute {
}

public interface ISOAPStaticEntityPolymorphic : OutSystems.Model.Data.IStaticEntity {
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    new IEnumerable<ISOAPStaticEntityAttributePolymorphic> Attributes { get; }
    new ISOAPStaticEntityAttributePolymorphic CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPStructure : OutSystems.Model.Data.IStructure {
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    bool IsAnonymous { get; set; }
    /// <summary>
    /// When the structure is anonymous save the parent element name.
    /// </summary>
    string ParentElement { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string XmlKind { get; set; }
    new IEnumerable<ISOAPStructureAttribute> Attributes { get; }
    new ISOAPStructureAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPStructureAttribute : OutSystems.Model.Data.IStructureAttribute {
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    /// <summary>
    /// Default value of the attribute defined in the WSDL.
    /// </summary>
    string OriginalDefaultValue { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string XmlKind { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
    /// <summary>
    /// Order of the attribute in the WSDL
    /// </summary>
    string Order { get; set; }
}

public interface ISOAPStructureAttributePolymorphic : OutSystems.Model.Data.IStructureAttribute {
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string OriginalType { get; set; }
    /// <summary>
    /// Default value of the attribute defined in the WSDL.
    /// </summary>
    string OriginalDefaultValue { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
}

public interface ISOAPStructureAttributeWildcard : OutSystems.Model.Data.IStructureAttribute {
    /// <summary>
    /// Send the input parameter value in the response payload if it is holding its default value.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.DefaultValueBehavior DefaultValueBehavior { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// Minimum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MinOccurrencies { get; set; }
    /// <summary>
    /// Maximum number of times this element can appear as defined in the WSDL.
    /// </summary>
    string MaxOccurrencies { get; set; }
    /// <summary>
    /// Set to Yes to allow elements to accept nil values.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.Nillable Nillable { get; set; }
    /// <summary>
    /// Namespace as defined in the WSDL. Used if namespace is ##targetNamespace
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// The namespaces containing the elements that can be used. If no namespace is specified, any may be used.
    /// </summary>
    string Namespace { get; set; }
    /// <summary>
    /// An indicator of how an application or XML processor should handle validation of XML documents against the elements specified by this any element.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.ProcessContents ProcessContents { get; set; }
    /// <summary>
    /// Specifies the specific kind of wildcard this object represents.
    /// </summary>
    ServiceStudio.Plugin.SOAP.Enumerations.WildcardType WildcardType { get; set; }
    /// <summary>
    /// Full hierarchy of the type.
    /// </summary>
    string FullPath { get; set; }
}

public interface ISOAPStructurePolymorphic : OutSystems.Model.Data.IStructure {
    /// <summary>
    /// Namespace as defined in the WSDL.
    /// </summary>
    string OriginalNamespace { get; set; }
    /// <summary>
    /// Name as defined in the WSDL.
    /// </summary>
    string OriginalName { get; set; }
    /// <summary>
    /// Description of the Web Service as provided in the WSDL.
    /// </summary>
    string OriginalDescription { get; set; }
    /// <summary>
    /// 
    /// </summary>
    string XmlKind { get; set; }
    new IEnumerable<ISOAPStructureAttributePolymorphic> Attributes { get; }
    new ISOAPStructureAttributePolymorphic CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

