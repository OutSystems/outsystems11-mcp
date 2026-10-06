// Action flow node types (start/end, decisions, cycles, exception handlers, calls, assignments)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Nodes;

public interface IActionNode : OutSystems.Model.IFlowNode {
    new IEnumerable<OutSystems.Model.Logic.IActionConnector> Connectors { get; }
    new IEnumerable<OutSystems.Model.Logic.IActionConnector> IncomingConnectors { get; }
}

public interface IAggregateNode : OutSystems.Model.Logic.Aggregates.IAggregate, IActionNode {
    IActionNode Target { get; set; }
}

public interface IAjaxRefreshNode : IActionNode {
    OutSystems.Model.Enumerations.AjaxRefreshAnimationEffectType AnimationEffect { get; set; }
    OutSystems.Model.Expressions.IExpression RowNumber { get; }
    IActionNode Target { get; set; }
    OutSystems.Model.IObject WidgetOrWebBlock { get; set; }
    void SetRowNumber(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IAssignNode : IActionNode {
    string Label { get; set; }
    IActionNode Target { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.IAssignment> Assignments { get; }
    OutSystems.Model.Logic.IAssignment CreateAssignment();
}

public interface IAttachFileNode : IActionNode {
    OutSystems.Model.Expressions.IExpression FileContent { get; }
    OutSystems.Model.Expressions.IExpression FileName { get; }
    OutSystems.Model.Expressions.IExpression MimeType { get; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    void SetFileContent(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetFileName(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMimeType(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ICallAgentCoreNode : IActionNode {
    OutSystems.Model.Logic.IServerActionSignature AIModel { get; set; }
    OutSystems.Model.Expressions.IExpression CallCondition { get; }
    string Description { get; set; }
    bool EnableActionCalling { get; set; }
    OutSystems.Model.Expressions.IExpression ExtraBody { get; }
    OutSystems.Model.Expressions.IExpression MaxTokens { get; }
    OutSystems.Model.Expressions.IExpression Messages { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression Stop { get; }
    IActionNode Target { get; set; }
    OutSystems.Model.Expressions.IExpression Temperature { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.IActionHandler> ActionHandlers { get; }
    OutSystems.Model.Data.IStructureSignature OutputFormat { get; set; }
    string OutputFormatDescription { get; set; }
    void SetCallCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetExtraBody(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMaxTokens(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMessages(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStop(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTemperature(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.Logic.IActionHandler CreateActionHandler(OutSystems.Model.IKey key = null);
    string GetOutputFormatAttributeDescription(OutSystems.Model.Types.IAttributeSignature attribute);
    void SetOutputFormatAttributeDescription(OutSystems.Model.Types.IAttributeSignature attribute, string description);
}

public interface ICallAgentNode : IActionNode {
    OutSystems.Model.Logic.IAgent Agent { get; set; }
    string Description { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface ICommentConnector : OutSystems.Model.Logic.IActionConnector {
    string Label { get; set; }
}

public interface ICommentNode : IActionNode {
    string CreatedBy { get; set; }
    bool IsReminder { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Text { get; set; }
    new IEnumerable<ICommentConnector> Connectors { get; }
    ICommentConnector CreateConnector(IActionNode target);
}

public interface IDecisionOutcomeNode : IActionNode {
}

public interface IDestinationNode : IActionNode {
    OutSystems.Model.IObjectSignature Destination { get; set; }
    OutSystems.Model.Enumerations.TransitionTypeWithInheritance Transition { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IDownloadNode : IActionNode {
    OutSystems.Model.Expressions.IExpression FileContent { get; }
    OutSystems.Model.Expressions.IExpression FileName { get; }
    /// <summary>
    /// Text literal or expression specifying the media type of the file.
    /// Example values:
    /// – "application/x-msexcel";
    /// – "application/msword";
    /// – "application/pdf";
    /// – "image/gif";
    /// – "text/html";
    /// – "video/avi";
    /// – "audio/wav".
    /// </summary>
    OutSystems.Model.Expressions.IExpression MimeType { get; }
    /// <summary>
    /// Set to true to allow the user to open or save the file.
    /// </summary>
    bool SaveToDisk { get; set; }
    void SetFileContent(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetFileName(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMimeType(string value);
    void SetMimeType(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IEndNode : IActionNode {
}

public interface IExcelToRecordListNode : IActionNode {
    OutSystems.Model.Types.IListType DataType { get; set; }
    string Description { get; set; }
    OutSystems.Model.Expressions.IExpression FileContent { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression SheetName { get; }
    IActionNode Target { get; set; }
    void SetFileContent(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetSheetName(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IExceptionHandlerNode : IActionNode {
    bool AbortTransaction { get; set; }
    OutSystems.Model.Logic.IException Exception { get; set; }
    bool LogError { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
}

public interface IExecuteClientActionNode : IActionNode {
    OutSystems.Model.Logic.IActionSignature Action { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IExecuteServerActionNode : IActionNode {
    OutSystems.Model.Logic.IActionSignature Action { get; set; }
    OutSystems.Model.Enumerations.ExecuteActionAnimationEffectType AnimationEffect { get; set; }
    string Name { get; set; }
    Nullable<int> ServerRequestTimeout { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IForEachNode : IActionNode {
    string Label { get; set; }
    OutSystems.Model.Expressions.IExpression MaximumIterations { get; }
    OutSystems.Model.Expressions.IExpression RecordList { get; }
    OutSystems.Model.Expressions.IExpression StartIndex { get; }
    IActionNode Target { get; set; }
    IActionNode CycleTarget { get; set; }
    IEnumerable<IActionNode> NodesWithinCycle { get; }
    void SetMaximumIterations(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetRecordList(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStartIndex(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SwapConnectors();
}

public interface IIfNode : IActionNode {
    OutSystems.Model.Expressions.IExpression Condition { get; }
    string Label { get; set; }
    IActionNode TrueTarget { get; set; }
    IActionNode FalseTarget { get; set; }
    void SetCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SwapConnectors();
}

public interface IJavaScriptNode : IActionNode {
    string CreatedBy { get; set; }
    string Description { get; set; }
    bool IsAsynchronous { get; }
    string JavaScript { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IOutputParameter CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IJSONDeserializeNode : IActionNode {
    OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    OutSystems.Model.Enumerations.JSONDateFormat DateFormat { get; set; }
    string Description { get; set; }
    OutSystems.Model.Expressions.IExpression JSONString { get; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    void SetJSONString(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IJSONSerializeNode : IActionNode {
    OutSystems.Model.Expressions.IExpression Data { get; }
    OutSystems.Model.Enumerations.JSONDateFormat DateFormat { get; set; }
    string Description { get; set; }
    string Name { get; set; }
    bool SerializeDefaultValues { get; set; }
    IActionNode Target { get; set; }
    void SetData(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IMessageNode : IActionNode {
    string Label { get; set; }
    OutSystems.Model.Expressions.IExpression Message { get; }
    IActionNode Target { get; set; }
    OutSystems.Model.Enumerations.MessageType Type { get; set; }
    void SetMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IRaiseExceptionNode : IActionNode {
    OutSystems.Model.Logic.IException Exception { get; set; }
    OutSystems.Model.Expressions.IExpression ExceptionMessage { get; }
    void SetExceptionMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IRecordListToExcelAttribute {
    OutSystems.Model.Types.IAttributeSignature Attribute { get; }
    OutSystems.Model.Types.IAttributeSignature AttributeParent { get; }
}

public interface IRecordListToExcelNode : IActionNode {
    string Description { get; set; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression RecordList { get; }
    IActionNode Target { get; set; }
    IEnumerable<string> AttributeSelection { get; }
    IEnumerable<IRecordListToExcelAttribute> AttributeSelectionDetails { get; }
    void SetRecordList(OutSystems.Model.Expressions.ExpressionDefinition value);
    void AddAttributeSelection(string attribute);
    void AddAttributeSelection(OutSystems.Model.Types.IAttributeSignature attribute);
    void AddAttributeSelection(OutSystems.Model.Types.IAttributeSignature attributeParent, OutSystems.Model.Types.IAttributeSignature attribute);
    void RemoveAttributeSelection(string attribute);
    void RemoveAttributeSelection(OutSystems.Model.Types.IAttributeSignature attribute);
    void RemoveAttributeSelection(OutSystems.Model.Types.IAttributeSignature attributeParent, OutSystems.Model.Types.IAttributeSignature attribute);
}

public interface IRefreshDataNode : IActionNode {
    OutSystems.Model.IObject DataSource { get; set; }
    OutSystems.Model.Expressions.IExpression MaxRecords { get; }
    OutSystems.Model.Expressions.IExpression StartIndex { get; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    void SetMaxRecords(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStartIndex(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ISemanticSearchNode : IActionNode {
    string Description { get; set; }
    OutSystems.Model.Expressions.IExpression Filters { get; }
    OutSystems.Model.Expressions.IExpression MinimumScore { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression PageNumber { get; }
    OutSystems.Model.Expressions.IExpression PageSize { get; }
    OutSystems.Model.Types.ITypeSignature ResultListType { get; }
    OutSystems.Model.Expressions.IExpression SearchQuery { get; }
    OutSystems.Model.Types.IRecordTypeSignature Source { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.Data.ISearchableAttributeSignature> SearchAttributes { get; }
    void SetFilters(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMinimumScore(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetPageNumber(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetPageSize(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetSearchQuery(OutSystems.Model.Expressions.ExpressionDefinition value);
    void AddSearchableAttributes(OutSystems.Model.Data.ISearchableAttributeSignature[] attributes);
    void RemoveSearchableAttributes(OutSystems.Model.Data.ISearchableAttributeSignature[] attributes);
}

public interface ISendEmailNode : IActionNode {
    OutSystems.Model.Expressions.IExpression Bcc { get; }
    OutSystems.Model.Expressions.IExpression Cc { get; }
    OutSystems.Model.Expressions.IExpression From { get; }
    bool LogContent { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    OutSystems.Model.Expressions.IExpression To { get; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    void SetBcc(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetCc(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetFrom(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTo(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ISQLNode : IActionNode {
    Nullable<int> CacheInMinutes { get; set; }
    string CreatedBy { get; set; }
    string Description { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.Expressions.IExpression MaxRecords { get; }
    string Name { get; set; }
    string Statement { get; set; }
    IActionNode Target { get; set; }
    Nullable<int> Timeout { get; set; }
    OutSystems.Model.Types.IListType Type { get; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.ISQLInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.ISQLOutput> Outputs { get; }
    void SetMaxRecords(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.Logic.ISQLInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.ISQLOutput CreateOutput(OutSystems.Model.Types.IRecordTypeSignature entityOrStructure, string name = null);
}

public interface IStartNode : IActionNode {
    IActionNode Target { get; set; }
}

public interface ISwitchCondition : OutSystems.Model.Logic.IActionConnector {
    OutSystems.Model.Expressions.IExpression Value { get; }
    string Label { get; set; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ISwitchNode : IActionNode {
    string Label { get; set; }
    OutSystems.Model.ISequence<ISwitchCondition> Conditions { get; }
    IActionNode OtherwiseTarget { get; set; }
    ISwitchCondition CreateCondition();
}

public interface ITriggerGlobalNode : IActionNode {
    OutSystems.Model.Processes.IGlobalEvent Event { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface ITriggerNode : IActionNode {
    string Description { get; set; }
    OutSystems.Model.Logic.ITriggerableEvent Event { get; set; }
    string Name { get; set; }
    IActionNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

