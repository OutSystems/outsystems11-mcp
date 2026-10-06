// Signature interfaces (read-only contract views for model objects, parameters, and members)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Signatures;

public class BinarySignatureInformation : SignatureInformation {
    public byte[] SignatureBinary { get; set; }
}

/// <summary>
/// Represents an Agent-to-Agent (A2A) Connection that exposes server actions
/// for consumption by external AI agents in agentic applications.
/// </summary>
/// <remarks>
/// A2A Connections can only be consumed by applications with SegmentationKind.AIAgent.
/// They expose server actions that behave like any other server action and are read-only when referenced.
/// </remarks>
public interface IA2AConnection : IModuleSignature {
    /// <summary>
    /// Creates a new server action in this A2A connection.
    /// </summary>
    /// <param name="name">The name of the action. If null, a default name will be generated.</param>
    /// <param name="key">The unique key for the action. If null, a new key will be generated.</param>
    /// <returns>The created action signature.</returns>
    IActionSignature CreateAction(string name = null, OutSystems.Model.IKey key = null);
    /// <summary>
    /// Creates a new structure in this A2A connection.
    /// </summary>
    /// <param name="name">The name of the structure. If null, a default name will be generated.</param>
    /// <param name="key">The unique key for the structure. If null, a new key will be generated.</param>
    /// <returns>The created structure signature.</returns>
    IStructureSignature CreateStructure(string name = null, OutSystems.Model.IKey key = null);
}

public interface IActionSignature : IShareableObjectSignature<IActionSignature, OutSystems.Model.Logic.IActionSignature> {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    bool IsFunction { get; set; }
    byte[] Icon { get; set; }
    OutSystems.Model.ISequence<IInputParameterSignature> InputParameters { get; }
    OutSystems.Model.ISequence<IOutputParameterSignature> OutputParameters { get; }
    IInputParameterSignature CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    IOutputParameterSignature CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IAIModelActionSignature : IActionSignature {
    string ModelVersion { get; set; }
}

public interface IAIModelConnection : IModuleSignature {
    new IEnumerable<IAIModelActionSignature> Actions { get; }
    IEntitySignature CreateEntity(string name = null, OutSystems.Model.IKey key = null);
    IStructureSignature CreateStructure(string name = null, OutSystems.Model.IKey key = null);
    IAIModelActionSignature CreateAction(string name = null, OutSystems.Model.IKey key = null);
}

public interface IBasicType : ITypeSignature {
}

public interface IEntityAttributeSignature : IObjectSignature {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    /// <summary>
    /// The attribute data type.
    /// The attribute data type must be a basic data type or an Entity Identifier.
    /// </summary>
    IBasicType DataType { get; set; }
    /// <summary>
    /// Number of decimal places.
    /// Only available when data type is Decimal (mandatory).
    /// </summary>
    Nullable<int> Decimals { get; set; }
    /// <summary>
    /// Set to true if the attribute value is required
    /// </summary>
    bool IsMandatory { get; set; }
    /// <summary>
    /// Set to true to enable the attributes Id value to be set by the system
    /// </summary>
    bool IsAutoNumber { get; set; }
    /// <summary>
    /// Maximum size of the attribute in characters.
    /// </summary>
    Nullable<int> Length { get; set; }
    /// <summary>
    /// Set the default value for the attribute
    /// </summary>
    string DefaultValue { get; set; }
}

public interface IEntitySignature : IShareableObjectSignature<IEntitySignature, OutSystems.Model.Data.IEntitySignature> {
    IEntityAttributeSignature IdentifierAttribute { get; set; }
    OutSystems.Model.ISequence<IEntityAttributeSignature> Attributes { get; }
    IIdentifierType IdentifierType { get; }
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    /// <summary>
    /// Exposes the entity as read-only, preventing the creation of the 'CreateEntity', 'CreateOrUpdateEntity', 'CreateOrUpdateAllEntity', 'UpdateEntity', 'DeleteEntity', and 'DeleteAllEntity' actions.
    /// </summary>
    bool ExposeAsReadOnly { get; set; }
    /// <summary>
    /// Prevents the creation of the 'CreateEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideCreateEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'CreateOrUpdateEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideCreateOrUpdateEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'CreateOrUpdateAllEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideCreateOrUpdateAllEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'UpdateEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideUpdateEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'GetEntity' action.
    /// </summary>
    bool HideGetEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'GetEntityForUpdate' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideGetEntityForUpdate { get; set; }
    /// <summary>
    /// Prevents the creation of the 'DeleteEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideDeleteEntity { get; set; }
    /// <summary>
    /// Prevents the creation of the 'DeleteAllEntity' action.
    /// </summary>
    /// <exception cref="T:System.InvalidOperationException">Thrown when the entity is exposed as read-only.</exception>
    bool HideDeleteAllEntity { get; set; }
    bool KeepDatabaseNulls { get; set; }
    bool IsStaticEntity { get; }
    IEntityAttributeSignature CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IExternalConnection : IModuleSignature {
    bool IsValid { get; }
    IEntitySignature CreateEntity(string name = null, OutSystems.Model.IKey key = null);
    IStaticEntitySignature CreateStaticEntity(string name = null, OutSystems.Model.IKey key = null);
    IStructureSignature CreateStructure(string name = null, OutSystems.Model.IKey key = null);
    IActionSignature CreateAction(string name = null, OutSystems.Model.IKey key = null);
    IEnumerable<OutSystems.Model.IValidationMessage> GetValidationMessages(bool includeMessagesFromDescendants = true);
}

public interface IIdentifierType : IBasicType {
    IEntitySignature Entity { get; }
}

public interface IInputParameterSignature : IShareableObjectSignature<IInputParameterSignature, OutSystems.Model.IInputParameterSignature> {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    ITypeSignature DataType { get; set; }
    bool IsMandatory { get; set; }
    string DefaultValue { get; set; }
}

public interface IListTypeSignature : ITypeSignature {
    ITypeSignature ElementType { get; }
}

public interface IMCPConnection : IModuleSignature {
    IActionSignature CreateAction(string name = null, OutSystems.Model.IKey key = null);
    IStructureSignature CreateStructure(string name = null, OutSystems.Model.IKey key = null);
}

public interface IModuleSignature : IObjectSignature {
    IBasicType TextType { get; }
    IBasicType IntegerType { get; }
    IBasicType LongIntegerType { get; }
    IBasicType DecimalType { get; }
    IBasicType BooleanType { get; }
    IBasicType DateTimeType { get; }
    IBasicType DateType { get; }
    IBasicType TimeType { get; }
    IBasicType PhoneNumberType { get; }
    IBasicType EmailType { get; }
    IBasicType BinaryDataType { get; }
    IBasicType CurrencyType { get; }
    IBasicType TextIdentifierType { get; }
    IBasicType IntegerIdentifierType { get; }
    IBasicType LongIntegerIdentifierType { get; }
    IBasicType FileType { get; }
    ITypeSignature ObjectType { get; }
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    /// <summary>
    /// This Hash is only calculated during the save operation.
    /// </summary>
    string Digest { get; }
    string CompatibilityDigest { get; }
    byte[] Icon { get; set; }
    System.Xml.Linq.XElement Signature { get; }
    Guid SignatureDigest { get; }
    byte[] SignatureBinary { get; }
    OutSystems.Model.Enumerations.ReferenceKind ModuleKind { get; }
    OutSystems.Model.Enumerations.SegmentationKind ModuleType { get; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    IEnumerable<IEntitySignature> Entities { get; }
    IEnumerable<IStructureSignature> Structures { get; }
    IEnumerable<IActionSignature> Actions { get; }
    void Save(string path);
    byte[] Save();
    IListTypeSignature GetOrCreateListType(ITypeSignature typeSignature);
}

public interface IObjectSignature : OutSystems.Model.IModelObject {
    OutSystems.Model.IKey ObjectKey { get; }
    IModuleSignature GetModuleSignature();
}

public interface IOutputParameterSignature : IShareableObjectSignature<IOutputParameterSignature, OutSystems.Model.IOutputParameterSignature> {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    ITypeSignature DataType { get; set; }
    string DefaultValue { get; set; }
}

public interface ISearchServiceConnection : IModuleSignature {
    IEntitySignature CreateEntity(string name = null, OutSystems.Model.IKey key = null);
    IStructureSignature CreateStructure(string name = null, OutSystems.Model.IKey key = null);
    IActionSignature CreateAction(string name = null, OutSystems.Model.IKey key = null);
}

public interface IShareableObjectSignature : OutSystems.Model.IShareable, IObjectSignature {
}

public interface IShareableObjectSignature<ConcreteT, SignatureT> : IShareableObjectSignature where ConcreteT: IObjectSignature where SignatureT: OutSystems.Model.IObjectSignature {
}

public interface IStaticEntityRecordSignature : IObjectSignature {
    string Name { get; set; }
    byte[] Icon { get; set; }
    OutSystems.Model.ISequence<IStaticRecordAttributeValueSignature> AttributeValues { get; }
}

public interface IStaticEntitySignature : IEntitySignature, IShareableObjectSignature<IStaticEntitySignature, OutSystems.Model.Data.IStaticEntitySignature> {
    OutSystems.Model.ISequence<IStaticEntityRecordSignature> Records { get; }
    IStaticEntityRecordSignature CreateRecord(string name = null, OutSystems.Model.IKey key = null);
}

public interface IStaticRecordAttributeValueSignature : IObjectSignature {
    IEntityAttributeSignature Attribute { get; }
    string Value { get; }
    void SetValue(string value);
}

public interface IStructureAttributeSignature : IShareableObjectSignature<IStructureAttributeSignature, OutSystems.Model.Data.IStructureAttributeSignature> {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    string NameInJSON { get; set; }
    string Label { get; set; }
    /// <summary>
    /// The attribute data type.
    /// The attribute data type must be a basic data type or an Entity Identifier.
    /// </summary>
    ITypeSignature DataType { get; set; }
    /// <summary>
    /// Number of decimal places.
    /// Only available when data type is Decimal (mandatory).
    /// </summary>
    Nullable<int> Decimals { get; set; }
    /// <summary>
    /// Set to true if the attribute value is required
    /// </summary>
    bool IsMandatory { get; set; }
    /// <summary>
    /// Maximum size of the attribute in characters.
    /// </summary>
    Nullable<int> Length { get; set; }
    /// <summary>
    /// Set the default value for the attribute
    /// </summary>
    string DefaultValue { get; set; }
}

public interface IStructureSignature : IShareableObjectSignature<IStructureSignature, OutSystems.Model.Data.IStructureSignature>, ITypeSignature {
    /// <summary>
    /// Identifies an element in the scope where it is defined, like a screen, action, or module.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    OutSystems.Model.ISequence<IStructureAttributeSignature> Attributes { get; }
    IStructureAttributeSignature CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface ITypeSignature : IObjectSignature {
}

public class ResolveModelSignatureEventArgs : EventArgs {
    public OutSystems.Model.IKey ModuleKey { get; }
    public string SpecificVersionHash { get; }
}

/// <summary>
/// An auxiliary class to the IESpace.AddDependency methods that receive a signature binary / stream in their parameters.
/// </summary>
public class SignatureInformation : SignatureMetadata {
    /// <summary>
    /// The app key of the producer that generated the signature.
    /// </summary>
    public OutSystems.Model.IKey AppKey { get; set; }
    /// <summary>
    /// The keys of the target elements within the signature. When empty the
    /// add dependency mechanism will bring the whole producer as a reference to
    /// the consumer.
    /// </summary>
    public IEnumerable<OutSystems.Model.IKey> TargetElementKeys { get; set; }
    /// <summary>
    /// The icon of the producer that generated the signature.
    /// </summary>
    public byte[] Icon { get; set; }
    /// <summary>
    /// The JQuery version of the producer that generated the signature.
    /// </summary>
    public string JQueryVersion { get; set; }
    /// <summary>
    /// The extensibility configurations of the producer that generated the signature.
    /// </summary>
    public string ExtensibilityConfigurations { get; set; }
    /// <summary>
    /// The digest of the producer's signature.
    /// </summary>
    public string Digest { get; set; }
    public string Version { get; set; }
    public string Revision { get; set; }
}

public class SignatureMetadata {
    public OutSystems.Model.IKey Key { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public OutSystems.Model.Enumerations.ReferenceKind ModuleKind { get; set; }
    public OutSystems.Model.Enumerations.SegmentationKind ModuleType { get; set; }
}

public class StreamSignatureInformation : SignatureInformation {
    public System.IO.Stream SignatureStream { get; set; }
}

