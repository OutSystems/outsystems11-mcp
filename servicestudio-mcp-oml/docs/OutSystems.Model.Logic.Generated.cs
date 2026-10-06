// Logic artifacts: actions (server/client), callbacks, roles, system events, and execution flow connectors
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic;

public interface IAction : OutSystems.Model.IFlow, IActionSignature {
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IOutputParameter CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IActionConnector : OutSystems.Model.IConnector {
    new OutSystems.Model.Logic.Nodes.IActionNode Source { get; }
    new OutSystems.Model.Logic.Nodes.IActionNode Target { get; set; }
}

public interface IActionHandler : OutSystems.Model.IObject {
    IActionSignature Action { get; set; }
    string Description { get; set; }
    IEnumerable<OutSystems.Model.IAIArgument> Arguments { get; }
}

public interface IActionSignature : OutSystems.Model.IFlowSignature {
    IEnumerable<OutSystems.Model.IInputParameterSignature> InputParameters { get; }
    IEnumerable<OutSystems.Model.IOutputParameterSignature> OutputParameters { get; }
}

public interface IAgent : OutSystems.Model.IObject {
    IServerActionSignature AIModel { get; set; }
    OutSystems.Model.Expressions.IExpression CallCondition { get; }
    string Description { get; set; }
    bool EnableActionCalling { get; set; }
    OutSystems.Model.Expressions.IExpression ExtraBody { get; }
    OutSystems.Model.IFolder Folder { get; set; }
    OutSystems.Model.Expressions.IExpression MaxTokens { get; }
    OutSystems.Model.Expressions.IExpression Messages { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression Stop { get; }
    OutSystems.Model.Expressions.IExpression Temperature { get; }
    OutSystems.Model.ISequence<IActionHandler> ActionHandlers { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.Data.IStructureSignature OutputFormat { get; set; }
    string OutputFormatDescription { get; set; }
    void SetCallCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetExtraBody(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMaxTokens(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMessages(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStop(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTemperature(OutSystems.Model.Expressions.ExpressionDefinition value);
    IActionHandler CreateActionHandler(OutSystems.Model.IKey key = null);
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    string GetOutputFormatAttributeDescription(OutSystems.Model.Types.IAttributeSignature attribute);
    void SetOutputFormatAttributeDescription(OutSystems.Model.Types.IAttributeSignature attribute, string description);
}

public interface IAppRole : IAppRoleSignature, IRole, OutSystems.Model.IShareableESpaceObject<IAppRole, IAppRoleSignature> {
    new string CreatedBy { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
}

public interface IAppRoleSignature : IRoleSignature {
    string CreatedBy { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
}

public interface IAssignment : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Value { get; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
}

// Anonymous-like reusable flow fragment invoked within an action context (holds its own locals and nodes)
public interface ICallback : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
}

// Action executed on client (browser/mobile) context; may be exposed (Public) for reuse
public interface IClientAction : IAction, IClientActionSignature, OutSystems.Model.IShareableESpaceObject<IClientAction, IClientActionSignature> {
    new OutSystems.Model.IFolder Folder { get; set; }
    new bool Function { get; set; }
    new byte[] Icon { get; set; }
    bool Public { get; set; }
}

public interface IClientActionSignature : IActionSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    bool Function { get; }
    byte[] Icon { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
}

// Signature for entity CRUD actions generated by the platform (Get/Create/Update/Delete...)
public interface IEntityActionSignature : IActionSignature {
}

public interface IException : OutSystems.Model.IObject {
}

// System event raised during client/server data synchronization
public interface IOnSyncSystemEvent : ISystemEvent, ITriggerableEvent {
}

public interface IRole : OutSystems.Model.IObject, IRoleSignature {
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new bool IsPersistent { get; set; }
}

public interface IRoleException : IException {
    IRoleSignature Role { get; }
}

public interface IRoleSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    bool IsPersistent { get; }
    string Name { get; set; }
    IActionSignature CheckRoleAction { get; }
    IActionSignature CheckRoleClientSideAction { get; }
    IActionSignature GrantRoleAction { get; }
    IActionSignature RevokeRoleAction { get; }
}

// Server-side action that executes on the OutSystems platform
public interface IServerAction : IAction, IServerActionSignature, OutSystems.Model.IShareableESpaceObject<IServerAction, IServerActionSignature> {
    Nullable<int> CacheInMinutes { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new bool Function { get; set; }
    new byte[] Icon { get; set; }
    bool Public { get; set; }
}

public interface IServerActionSignature : IActionSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    bool Function { get; }
    byte[] Icon { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
}

// Action exposed as a service endpoint that can be consumed by other modules
public interface IServiceAction : IAction, IServiceActionSignature, OutSystems.Model.IShareableESpaceObject<IServiceAction, IServiceActionSignature> {
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new byte[] Icon { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    bool Public { get; set; }
}

public interface IServiceActionSignature : IActionSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    byte[] Icon { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
}

// Input parameter specifically designed for SQL queries with inline expansion capabilities
public interface ISQLInputParameter : OutSystems.Model.IInputParameter {
    string CreatedBy { get; set; }
    new OutSystems.Model.Types.IBasicType DataType { get; set; }
    bool ExpandInline { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
}

public interface ISQLOutput : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature EntityOrStructure { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
}

public interface ISystemEvent : OutSystems.Model.IFlow {
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface ISystemException : IException {
}

public interface ISystemRole : IRole {
}

public interface ITriggerableEvent : OutSystems.Model.IModelObject {
}

public interface IUserException : IException {
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
}

