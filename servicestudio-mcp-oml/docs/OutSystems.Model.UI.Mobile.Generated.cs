// Mobile UI flows, screens, themes, and native integration constructs
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI.Mobile;

public interface IComment : IMobileFlowNode {
    string CreatedBy { get; set; }
    bool IsReminder { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Text { get; set; }
    new IEnumerable<ICommentConnector> Connectors { get; }
    ICommentConnector CreateConnector(IMobileFlowNode target);
}

public interface ICommentConnector : IMobileFlowConnector {
    string Label { get; set; }
}

public interface IDataAction : OutSystems.Model.IFlow {
    string CreatedBy { get; set; }
    OutSystems.Model.Enumerations.DataSourceFetch Fetch { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    Nullable<int> ServerRequestTimeout { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnAfterFetch { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IOutputParameter CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IExternalSite : IMobileFlowNode {
    string Description { get; set; }
    string Name { get; set; }
    string URL { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IMobileBlock : OutSystems.Model.UI.IBlock, IMobileBlockSignature, IMobileFlowNode, OutSystems.Model.IShareableESpaceObject<IMobileBlock, IMobileBlockSignature> {
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    IEnumerable<IDataAction> DataActions { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnDestroy { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnInitialize { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnParametersChanged { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnReady { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnRender { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncCompleteEvent OnSyncComplete { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncErrorEvent OnSyncError { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncStartEvent OnSyncStart { get; }
    IEnumerable<IScreenAction> ScreenActions { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    ICollection<OutSystems.Model.UI.IScriptSignature> RequiredScripts { get; }
    IEnumerable<IScreenAggregate> ScreenAggregates { get; }
    IDataAction CreateDataAction(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Events.IOnSyncCompleteEvent CreateOnSyncComplete();
    OutSystems.Model.UI.Mobile.Events.IOnSyncErrorEvent CreateOnSyncError();
    OutSystems.Model.UI.Mobile.Events.IOnSyncStartEvent CreateOnSyncStart();
    IScreenAction CreateScreenAction(string name = null, OutSystems.Model.IKey key = null);
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
    IScreenAggregate CreateScreenAggregate(bool isClientSide, string name = null, OutSystems.Model.IKey key = null);
}

public interface IMobileBlockSignature : OutSystems.Model.UI.IBlockSignature, IMobileFlowNodeSignature {
    bool HasScreenDestination { get; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    IEnumerable<OutSystems.Model.ModelPlugins.IRequiredModelPlugin> RequiredModelPlugins { get; }
}

public interface IMobileEmail : OutSystems.Model.UI.IEmail, IMobileFlowNode {
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
}

public interface IMobileFlow : OutSystems.Model.UI.IUIFlow, IMobileFlowSignature, OutSystems.Model.IShareableESpaceObject<IMobileFlow, IMobileFlowSignature> {
    new IMobileThemeSignature Theme { get; set; }
    new IEnumerable<IMobileFlowNode> Nodes { get; }
    new IMobileScreen CreateScreen(string name = null, OutSystems.Model.IKey key = null);
    new IMobileBlock CreateBlock(string name = null, OutSystems.Model.IKey key = null);
    new IMobileEmail CreateEmail(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: IMobileFlowNode;
    IMobileScreen Instantiate(IMobileScreen templateScreen, OutSystems.Model.IKey newScreenKey = null);
    IEnumerable<IMobileScreen> Instantiate(IEnumerable<IMobileScreen> templateScreens, IEnumerable<OutSystems.Model.IKey> newScreensKeys = null);
}

public interface IMobileFlowConnector : OutSystems.Model.UI.IUIFlowConnector {
    new IMobileFlowNode Source { get; }
    new IMobileFlowNode Target { get; set; }
}

public interface IMobileFlowNode : OutSystems.Model.UI.IUIFlowNode, IMobileFlowNodeSignature {
}

public interface IMobileFlowNodeSignature : OutSystems.Model.UI.IUIFlowNodeSignature {
}

public interface IMobileFlowSignature : OutSystems.Model.UI.IUIFlowSignature {
    string IconLibrary { get; }
    new IEnumerable<IMobileFlowNodeSignature> Nodes { get; }
}

public interface IMobileScreen : OutSystems.Model.UI.IScreen, IMobileFlowNode, IMobileScreenSignature, OutSystems.Model.IShareableESpaceObject<IMobileScreen, IMobileScreenSignature> {
    bool AnonymousAccess { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    bool CustomURL { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    new string PageName { get; set; }
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    OutSystems.Model.Expressions.IExpression Title { get; }
    new OutSystems.Model.Enumerations.URLStructure URLStructure { get; set; }
    IEnumerable<IDataAction> DataActions { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnDestroy { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnInitialize { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnReady { get; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnRender { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncCompleteEvent OnSyncComplete { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncErrorEvent OnSyncError { get; }
    OutSystems.Model.UI.Mobile.Events.IOnSyncStartEvent OnSyncStart { get; }
    IEnumerable<IScreenAction> ScreenActions { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    ICollection<OutSystems.Model.UI.IScriptSignature> RequiredScripts { get; }
    IEnumerable<IScreenAggregate> ScreenAggregates { get; }
    void SetTitle(OutSystems.Model.Expressions.ExpressionDefinition value);
    IDataAction CreateDataAction(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Events.IOnSyncCompleteEvent CreateOnSyncComplete();
    OutSystems.Model.UI.Mobile.Events.IOnSyncErrorEvent CreateOnSyncError();
    OutSystems.Model.UI.Mobile.Events.IOnSyncStartEvent CreateOnSyncStart();
    IScreenAction CreateScreenAction(string name = null, OutSystems.Model.IKey key = null);
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
    IScreenAggregate CreateScreenAggregate(bool isClientSide, string name = null, OutSystems.Model.IKey key = null);
}

public interface IMobileScreenSignature : OutSystems.Model.UI.IScreenSignature, IMobileFlowNodeSignature {
    string PageName { get; }
    OutSystems.Model.Enumerations.URLStructure URLStructure { get; }
}

public interface IMobileTheme : OutSystems.Model.UI.ITheme, IMobileThemeSignature, OutSystems.Model.IShareableESpaceObject<IMobileTheme, IMobileThemeSignature> {
    new IMobileThemeSignature BaseTheme { get; set; }
    new IMobileBlockSignature Footer { get; set; }
    new IMobileBlockSignature Header { get; set; }
    new string IconLibrary { get; set; }
    new IMobileBlockSignature Layout { get; set; }
    new IMobileBlockSignature Menu { get; set; }
}

public interface IMobileThemeSignature : OutSystems.Model.UI.IThemeSignature {
    new IMobileThemeSignature BaseTheme { get; }
    new IMobileThemeSignature FirstThemeWithGrid { get; }
    new IMobileBlockSignature Footer { get; }
    new IMobileBlockSignature Header { get; }
    string IconLibrary { get; }
    new IMobileBlockSignature Layout { get; }
    new IMobileBlockSignature Menu { get; }
    ValueTuple<bool, string, string, Version> IsBasedOnModelPlugin();
}

public interface IPreviousScreen : OutSystems.Model.IObject {
}

public interface IScreenAction : OutSystems.Model.Logic.IAction {
}

public interface IScreenAggregate : OutSystems.Model.Logic.Aggregates.IAggregate {
    OutSystems.Model.Enumerations.DataSourceFetch Fetch { get; set; }
    OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent OnAfterFetch { get; }
}

