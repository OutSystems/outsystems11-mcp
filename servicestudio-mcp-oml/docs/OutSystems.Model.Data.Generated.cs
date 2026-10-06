// Data modeling interfaces: entities, attributes, indexes, and client-side storage constructs (design-time metadata)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Data;

public enum EntityKind {
    Server,
    Client,
    Static,
}

public interface IClientEntity : IClientEntitySignature, IEntity, OutSystems.Model.IShareableESpaceObject<IClientEntity, IClientEntitySignature> {
    string TableNameOverride { get; set; }
    string ViewNameOverride { get; set; }
    new IRecord SampleRecord { get; }
}

public interface IClientEntitySignature : IEntitySignature {
    OutSystems.Model.Logic.IEntityActionSignature CreateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature CreateOrUpdateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature CreateOrUpdateAllAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature UpdateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature DeleteAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature DeleteAllAction { get; }
    IRecordSignature SampleRecord { get; }
}

public interface IClientVariable : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; set; }
    OutSystems.Model.IFolder Folder { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IEntity : IEntitySignature, OutSystems.Model.Types.IRecordType, OutSystems.Model.IShareableESpaceObject<IEntity, IEntitySignature> {
    new string CreatedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    new string Description { get; set; }
    new bool ExposeProcessEvents { get; set; }
    new bool ExposeReadOnly { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new IEntityAttributeSignature IdentifierAttribute { get; set; }
    new IEntityAttributeSignature IsActiveAttribute { get; set; }
    new OutSystems.Model.Enumerations.BooleanWithInheritance IsMultiTenant { get; set; }
    new bool IsRestrictedInQueries { get; set; }
    new string Label { get; set; }
    new IEntityAttributeSignature LabelAttribute { get; set; }
    new string LabelPlural { get; set; }
    new string LastModifiedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    new DateTime LastModifiedDate { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    new IEntityAttributeSignature OrderByAttribute { get; set; }
    bool Public { get; set; }
    new bool ShowTenantIdentifier { get; set; }
    new OutSystems.Model.Enumerations.UpdateEntityBehaviorType UpdateBehavior { get; set; }
    new bool UseTranslations { get; set; }
    new OutSystems.Model.ISequence<IEntityAttribute> Attributes { get; }
    IEntity ConvertTo(EntityKind targetKind);
    IEntityAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IEntityAttribute : IEntityAttributeSignature, OutSystems.Model.Types.IAttribute {
    new ISearchableAttribute AsSearchable { get; }
    new OutSystems.Model.Types.IBasicType DataType { get; set; }
    new OutSystems.Model.Enumerations.DeleteRule DeleteRule { get; set; }
    new OutSystems.Model.Enumerations.AutoNumber IsAutoNumber { get; set; }
    new bool IsSearchable { get; set; }
    new string Label { get; set; }
    new string OriginalName { get; set; }
    new string OriginalType { get; set; }
}

public interface IEntityAttributeSignature : OutSystems.Model.Types.IAttributeSignature {
    ISearchableAttributeSignature AsSearchable { get; }
    new OutSystems.Model.Types.IBasicType DataType { get; }
    OutSystems.Model.Enumerations.DeleteRule DeleteRule { get; }
    OutSystems.Model.Enumerations.AutoNumber IsAutoNumber { get; }
    bool IsSearchable { get; }
    string Label { get; }
    string OriginalName { get; }
    string OriginalType { get; }
}

public interface IEntityConnector : OutSystems.Model.IConnector {
    IEntityAttributeSignature Attribute { get; }
}

public interface IEntityDiagram : OutSystems.Model.IFlow {
    string CreatedBy { get; set; }
    OutSystems.Model.IFolder Folder { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    IEnumerable<OutSystems.Model.Data.Nodes.IEntityDiagramNode> Nodes { get; }
    OutSystems.Model.Data.Nodes.IEntityDiagramNode CreateNode(IEntitySignature entity);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Data.Nodes.IEntityDiagramNode;
    OutSystems.Model.Data.Nodes.IEntityDiagramNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IEntityIndex : IEntityIndexSignature, OutSystems.Model.IObject {
    bool AutoGenerated { get; set; }
    string CreatedBy { get; set; }
    new string Description { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    new string Name { get; set; }
    new bool Unique { get; set; }
    new OutSystems.Model.ISequence<IEntityIndexAttribute> IndexAttributes { get; }
    void AddAttribute(IEntityAttribute attribute);
}

public interface IEntityIndexAttribute : IEntityIndexAttributeSignature, OutSystems.Model.IObject {
    new IEntityAttribute Attribute { get; set; }
}

public interface IEntityIndexAttributeSignature : OutSystems.Model.IObjectSignature {
    IEntityAttributeSignature Attribute { get; }
}

public interface IEntityIndexSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    string Name { get; }
    bool Unique { get; }
    IEnumerable<IEntityIndexAttributeSignature> IndexAttributes { get; }
}

public interface IEntitySignature : OutSystems.Model.Logic.Aggregates.IDataSource, OutSystems.Model.Types.IRecordTypeSignature {
    EntityKind EntityKind { get; }
    OutSystems.Model.Logic.IEntityActionSignature GetAction { get; }
    bool Hidden { get; }
    string CreatedBy { get; }
    string Description { get; }
    bool ExposeProcessEvents { get; }
    bool ExposeReadOnly { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    IEntityAttributeSignature IdentifierAttribute { get; }
    IEntityAttributeSignature IsActiveAttribute { get; }
    OutSystems.Model.Enumerations.BooleanWithInheritance IsMultiTenant { get; }
    bool IsRestrictedInQueries { get; }
    string Label { get; }
    IEntityAttributeSignature LabelAttribute { get; }
    string LabelPlural { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    IEntityAttributeSignature OrderByAttribute { get; }
    bool ShowTenantIdentifier { get; }
    OutSystems.Model.Enumerations.UpdateEntityBehaviorType UpdateBehavior { get; }
    bool UseTranslations { get; }
    new IEnumerable<IEntityAttributeSignature> Attributes { get; }
    OutSystems.Model.Types.IIdentifierType IdentifierType { get; }
}

public interface IEntityVersion : OutSystems.Model.IObject {
}

public interface IExtensibilitySetting : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.IBasicType DataType { get; set; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; set; }
    bool IsMandatory { get; set; }
    bool IsSecret { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ILifeCycleDatabaseEvent : OutSystems.Model.IObject {
    OutSystems.Model.Processes.IGlobalEvent Destination { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface ILocale : OutSystems.Model.IObject {
    OutSystems.Model.Enumerations.Culture Culture { get; }
}

public interface IOnCreateLifeCycleDatabaseEvent : ILifeCycleDatabaseEvent {
}

public interface IOnDeleteLifeCycleDatabaseEvent : ILifeCycleDatabaseEvent {
}

public interface IOnUpdateLifeCycleDatabaseEvent : ILifeCycleDatabaseEvent {
}

public interface IRecord : IRecordSignature, OutSystems.Model.IObject {
    new IEnumerable<IRecordAttributeValue> AttributeValues { get; }
}

public interface IRecordAttributeValue : IRecordAttributeValueSignature, OutSystems.Model.IObject {
    new IEntityAttribute Attribute { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IRecordAttributeValueSignature : OutSystems.Model.IObjectSignature {
    IEntityAttributeSignature Attribute { get; }
    OutSystems.Model.Expressions.IExpression Value { get; }
}

public interface IRecordSignature : OutSystems.Model.IObjectSignature {
    IEnumerable<IRecordAttributeValueSignature> AttributeValues { get; }
}

public interface IResource : IResourceSignature, OutSystems.Model.IShareableESpaceObject<IResource, IResourceSignature> {
    new string CreatedBy { get; set; }
    new OutSystems.Model.Enumerations.DeployAction DeployAction { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    new string TargetDirectory { get; set; }
}

public interface IResourceSignature : OutSystems.Model.IObjectSignature {
    string CreatedBy { get; }
    OutSystems.Model.Enumerations.DeployAction DeployAction { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    byte[] ResourceContent { get; }
    string RuntimePath { get; }
    string TargetDirectory { get; }
}

public interface ISearchableAttribute : IEntityAttribute, ISearchableAttributeSignature {
    Nullable<OutSystems.Model.Enumerations.ChunkingMethod> ChunkingMethod { get; set; }
    int ChunkSize { get; set; }
    int ChunkOverlap { get; set; }
    int SentencesPerChunk { get; set; }
    string Separators { get; set; }
    T SetChunkingMethod<T>() where T: OutSystems.Model.Data.ChunkingMethods.IChunkingMethod;
    OutSystems.Model.Data.ChunkingMethods.IChunkingMethod GetChunkingMethod();
}

public interface ISearchableAttributeSignature : IEntityAttributeSignature {
}

public interface IServerEntity : IEntity, IServerEntitySignature, OutSystems.Model.IShareableESpaceObject<IServerEntity, IServerEntitySignature> {
    string TableNameOverride { get; set; }
    string ViewNameOverride { get; set; }
    new IEnumerable<IEntityIndex> Indexes { get; }
    IEntityVersion NewVersion { get; }
    IEntityVersion OldVersion { get; }
    IOnCreateLifeCycleDatabaseEvent OnCreate { get; }
    IOnDeleteLifeCycleDatabaseEvent OnDelete { get; }
    IOnUpdateLifeCycleDatabaseEvent OnUpdate { get; }
    new IRecord SampleRecord { get; }
    IEntityIndex CreateIndex(string name = null);
}

public interface IServerEntitySignature : IEntitySignature {
    OutSystems.Model.Logic.IEntityActionSignature CreateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature CreateOrUpdateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature UpdateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature GetForUpdateAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature DeleteAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature CreateOrUpdateAllAction { get; }
    OutSystems.Model.Logic.IEntityActionSignature DeleteAllAction { get; }
    IEnumerable<IEntityIndexSignature> Indexes { get; }
    IRecordSignature SampleRecord { get; }
}

public interface ISessionVariable : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; set; }
    OutSystems.Model.IFolder Folder { get; set; }
    bool IsReadOnly { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ISiteProperty : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.IBasicType DataType { get; set; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; set; }
    OutSystems.Model.IFolder Folder { get; set; }
    OutSystems.Model.Enumerations.BooleanWithInheritance IsMultiTenant { get; set; }
    bool IsReadOnly { get; }
    bool IsSecret { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IStaticEntity : IEntity, IStaticEntitySignature, OutSystems.Model.IShareableESpaceObject<IStaticEntity, IStaticEntitySignature> {
    new IEnumerable<IStaticEntityRecord> Records { get; }
    IStaticEntityRecord CreateRecord(string name = null, OutSystems.Model.IKey key = null);
}

public interface IStaticEntityRecord : IRecord, IStaticEntityRecordSignature {
    string CreatedBy { get; set; }
    new byte[] Icon { get; set; }
    new string Identifier { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
}

public interface IStaticEntityRecordSignature : IRecordSignature {
    byte[] Icon { get; }
    string Identifier { get; }
}

public interface IStaticEntitySignature : IEntitySignature {
    IEnumerable<IStaticEntityRecordSignature> Records { get; }
}

public interface IStructure : IStructureSignature, OutSystems.Model.Types.IRecordType, OutSystems.Model.IShareableESpaceObject<IStructure, IStructureSignature> {
    bool IsReadOnly { get; }
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    new OutSystems.Model.ISequence<IStructureAttribute> Attributes { get; }
    IStructureAttribute CreateAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IStructureAttribute : IStructureAttributeSignature, OutSystems.Model.Types.IAttribute {
    string CreatedBy { get; set; }
    new string Label { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    new string NameInJSON { get; set; }
}

public interface IStructureAttributeSignature : OutSystems.Model.Types.IAttributeSignature {
    string Label { get; }
    string NameInJSON { get; }
}

public interface IStructureSignature : OutSystems.Model.Logic.Aggregates.IDataSource, OutSystems.Model.Types.IRecordTypeSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    new IEnumerable<IStructureAttributeSignature> Attributes { get; }
}

public interface ISystemStaticEntity : IStaticEntitySignature {
}

