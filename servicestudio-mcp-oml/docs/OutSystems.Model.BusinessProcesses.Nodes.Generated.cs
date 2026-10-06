// Node types used inside business process flows (start/end, human & automatic activities, control and parallelism)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.BusinessProcesses.Nodes;

public interface AbstractProcessNode : IBusinessProcessNodeWithLabelAndDescription {
}

// Automatic server-side step executing a service action without human intervention
public interface IAutomaticActivityNode : IBusinessProcessNode {
    OutSystems.Model.Logic.IServiceActionSignature ActionToTrigger { get; set; }
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    IBusinessProcessNode Target { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IBusinessProcessNode : OutSystems.Model.IFlowNode {
    new IEnumerable<OutSystems.Model.BusinessProcesses.IBusinessProcessConnector> Connectors { get; }
    new IEnumerable<OutSystems.Model.BusinessProcesses.IBusinessProcessConnector> IncomingConnectors { get; }
}

public interface IBusinessProcessNodeWithLabelAndDescription {
    string Label { get; set; }
    string Description { get; set; }
}

public interface ICommentConnector : OutSystems.Model.BusinessProcesses.IBusinessProcessConnector {
    string Label { get; set; }
}

public interface ICommentNode : IBusinessProcessNode {
    new IEnumerable<ICommentConnector> Connectors { get; }
    string Comment { get; set; }
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    ICommentConnector CreateConnector(IBusinessProcessNode target);
}

// Alternative start is triggered only when all its conditions evaluate to true.
public interface IConditionalStartNode : IBusinessProcessNode {
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    OutSystems.Model.Processes.IGlobalEventSignature StartFlowOn { get; set; }
    IBusinessProcessNode Target { get; set; }
    OutSystems.Model.ISequence<IEventCondition> Conditions { get; }
    IEventCondition CreateCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
}

// Branching point evaluating outcomes (ordered); Otherwise acts as default branch
public interface IDecisionNode : IBusinessProcessNode {
    OutSystems.Model.BusinessProcesses.IOutcome Otherwise { get; }
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    IEnumerable<OutSystems.Model.BusinessProcesses.IActivityDataItem> ActivityData { get; }
    OutSystems.Model.ISequence<OutSystems.Model.BusinessProcesses.IOutcome> Outcomes { get; }
    OutSystems.Model.BusinessProcesses.IActivityDataItem AddAction(OutSystems.Model.Logic.IServiceActionSignature action);
    OutSystems.Model.BusinessProcesses.IOutcome CreateOutcome();
}

public interface IEndNode : IBusinessProcessNode {
    string Description { get; set; }
    string Label { get; set; }
    IForkNode ParentParallelNode { get; }
}

public interface IEventCondition : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Value { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

// Splits execution into multiple parallel paths (paired with a Join node)
public interface IForkNode : IBusinessProcessNode {
    string Description { get; set; }
    IJoinNode JoinNode { get; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    OutSystems.Model.ISequence<OutSystems.Model.BusinessProcesses.IPath> Paths { get; }
    OutSystems.Model.BusinessProcesses.IPath CreatePath();
}

// Unconditional jump to another node (used for refactoring / readability)
public interface IGoToNode : IBusinessProcessNode {
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    IBusinessProcessNode Target { get; set; }
}

// Human task step requiring user interaction; supports role assignment and lifecycle conditions
public interface IHumanActivityNode : IBusinessProcessNode {
    OutSystems.Model.Enumerations.AssignMode AssignMode { get; set; }
    OutSystems.Model.Expressions.IExpression AssignTo { get; }
    OutSystems.Model.Processes.IGlobalEventSignature CloseOn { get; set; }
    string Description { get; set; }
    OutSystems.Model.UI.Mobile.IMobileScreenSignature DestinationScreen { get; set; }
    OutSystems.Model.Expressions.IExpression DisplayMessage { get; }
    string Label { get; set; }
    string Name { get; set; }
    OutSystems.Model.Processes.IGlobalEventSignature OpenOn { get; set; }
    IForkNode ParentParallelNode { get; }
    OutSystems.Model.Processes.IGlobalEventSignature ReassignOn { get; set; }
    OutSystems.Model.Processes.IGlobalEventSignature ReleaseOn { get; set; }
    ICollection<OutSystems.Model.Logic.IRoleSignature> Roles { get; }
    IBusinessProcessNode Target { get; set; }
    Nullable<int> TimeoutBasicTime { get; set; }
    OutSystems.Model.Expressions.IExpression TimeoutCustomTime { get; }
    OutSystems.Model.Enumerations.TimeoutMode TimeoutMode { get; set; }
    OutSystems.Model.Enumerations.TimeUnit TimeoutUnit { get; set; }
    IEnumerable<OutSystems.Model.BusinessProcesses.IActivityDataItem> ActivityData { get; }
    OutSystems.Model.ISequence<IEventCondition> CloseOnConditions { get; }
    OutSystems.Model.ISequence<IEventCondition> OpenOnConditions { get; }
    OutSystems.Model.ISequence<IEventCondition> ReassignOnConditions { get; }
    OutSystems.Model.ISequence<IEventCondition> ReleaseOnConditions { get; }
    IEnumerable<OutSystems.Model.IArgument> ScreenArguments { get; }
    OutSystems.Model.BusinessProcesses.IActivityDataItem AddAction(OutSystems.Model.Logic.IServiceActionSignature action);
    IEventCondition CreateOpenOnCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
    IEventCondition CreateReleaseOnCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
    IEventCondition CreateReAssignOnCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
    IEventCondition CreateCloseOnCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
    void SetAssignTo(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetDisplayMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTimeoutCustomTime(OutSystems.Model.Expressions.ExpressionDefinition value);
}

// Synchronization point that waits for all parallel paths from a matching Fork
public interface IJoinNode : IBusinessProcessNode {
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParallelNode { get; }
    IForkNode ParentParallelNode { get; }
    IBusinessProcessNode Target { get; set; }
}

// Default entry point of the business process
public interface IStartNode : IBusinessProcessNode {
    string Description { get; set; }
    OutSystems.Model.Enumerations.Frequency Frequency { get; set; }
    OutSystems.Model.Expressions.IExpression InstanceLabel { get; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    OutSystems.Model.Processes.IGlobalEventSignature StartProcessOn { get; set; }
    IBusinessProcessNode Target { get; set; }
    OutSystems.Model.Enumerations.TriggerMode TriggerMode { get; set; }
    OutSystems.Model.Processes.ISchedule ScheduleConfiguration { get; }
    void SetInstanceLabel(OutSystems.Model.Expressions.ExpressionDefinition value);
}

// Immediately terminates the process instance (no further nodes executed)
public interface ITerminateNode : IBusinessProcessNode {
    string Description { get; set; }
    string Label { get; set; }
    IForkNode ParentParallelNode { get; }
}

// Pauses execution until a timeout or external resume condition
public interface IWaitActivityNode : IBusinessProcessNode {
    OutSystems.Model.Processes.IGlobalEventSignature CloseOnEvent { get; set; }
    string Description { get; set; }
    string Label { get; set; }
    string Name { get; set; }
    IForkNode ParentParallelNode { get; }
    IBusinessProcessNode Target { get; set; }
    Nullable<int> TimeoutBasicTime { get; set; }
    OutSystems.Model.Expressions.IExpression TimeoutCustomTime { get; }
    OutSystems.Model.Enumerations.TimeoutMode TimeoutMode { get; set; }
    OutSystems.Model.Enumerations.TimeUnit TimeoutUnit { get; set; }
    OutSystems.Model.ISequence<IEventCondition> Conditions { get; }
    IEventCondition CreateCondition(OutSystems.Model.Expressions.ExpressionDefinition condition);
    void SetTimeoutCustomTime(OutSystems.Model.Expressions.ExpressionDefinition value);
}

