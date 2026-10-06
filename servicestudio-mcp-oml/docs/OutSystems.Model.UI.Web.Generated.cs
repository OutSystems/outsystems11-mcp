// Traditional Web UI flows, screens, blocks, themes, emails, and preparation constructs
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI.Web;

public interface IComment : IWebFlowNode {
    new IEnumerable<ICommentConnector> Connectors { get; }
    string CreatedBy { get; set; }
    bool IsReminder { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Text { get; set; }
    ICommentConnector CreateConnector(IWebFlowNode target);
}

public interface ICommentConnector : IWebFlowConnector {
    string Label { get; set; }
}

public interface IEntry : IWebFlowNode {
    string Description { get; set; }
    bool IsDefault { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.IUIFlowNode Target { get; set; }
}

public interface IExternalSite : IWebFlowNode {
    string CreatedBy { get; set; }
    string Description { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    /// <summary>
    /// When true, this external site is a Frequent Destination, silencing the 'Unexpected Link' warning for links that target it.
    /// </summary>
    bool IsFrequentDestination { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    string URL { get; set; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IGoToDestination : IWebFlowNode {
    OutSystems.Model.UI.IUIFlowNodeSignature Destination { get; set; }
}

/// <summary>
/// Traditional data-fetch flow that runs before a Web screen or block renders. Build its contents with CreateNode<...>() (Start / Aggregate / End nodes); this is structurally different from Mobile's screen.CreateScreenAggregate(...).
/// </summary>
public interface IPreparation : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IMetadata> Metadata { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    IEnumerable<OutSystems.Model.ITextResource> TextResources { get; }
    IEnumerable<string> TextResourcesIds { get; }
    bool UserReadOnlySessionsInScreens { get; set; }
    T CreateMetadata<T>(string name, string managedBy, OutSystems.Model.IKey key = null) where T: OutSystems.Model.IMetadata;
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
    void CreateOrUpdateTranslation(OutSystems.Model.Enumerations.Culture culture, string id, string value);
    void SetTranslationBehavior(string id, OutSystems.Model.Enumerations.TranslationBehavior behavior);
}

public interface IScreenAction : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    string Description { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    bool IsCalledByAjax { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IMetadata> Metadata { get; }
    string Name { get; set; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    IEnumerable<OutSystems.Model.ITextResource> TextResources { get; }
    IEnumerable<string> TextResourcesIds { get; }
    bool UserReadOnlySessionsInScreens { get; set; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateMetadata<T>(string name, string managedBy, OutSystems.Model.IKey key = null) where T: OutSystems.Model.IMetadata;
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
    void CreateOrUpdateTranslation(OutSystems.Model.Enumerations.Culture culture, string id, string value);
    OutSystems.Model.IOutputParameter CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
    void SetTranslationBehavior(string id, OutSystems.Model.Enumerations.TranslationBehavior behavior);
}

public interface IWebBlock : IWebBlockSignature, IWebFlowNode, OutSystems.Model.IShareableESpaceObject<IWebBlock, IWebBlockSignature>, OutSystems.Model.UI.IBlock {
    new Nullable<int> CacheInMinutes { get; set; }
    new bool HasValidationBehavior { get; set; }
    new bool IncludeRuntimeJavaScript { get; set; }
    new bool IsNotAjaxSelfRefreshable { get; set; }
    IPreparation Preparation { get; }
    IEnumerable<IScreenAction> ScreenActions { get; }
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Web.Widgets.IWebWidget> Widgets { get; }
    IPreparation CreatePreparation();
    IScreenAction CreateScreenAction(string name = null, OutSystems.Model.IKey key = null);
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Web.Widgets.IWebWidget;
}

public interface IWebBlockSignature : IWebFlowNodeSignature, OutSystems.Model.UI.IBlockSignature {
    Nullable<int> CacheInMinutes { get; }
    bool HasInputs { get; }
    bool HasNotification { get; }
    bool HasValidationBehavior { get; }
    bool IncludeRuntimeJavaScript { get; }
    bool IsNotAjaxSelfRefreshable { get; }
    new IEnumerable<OutSystems.Model.UI.Web.Widgets.IWebWidgetSignature> Widgets { get; }
}

public interface IWebDestination : OutSystems.Model.IObject {
}

public interface IWebEmail : IWebFlowNode, OutSystems.Model.UI.IEmail {
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    IPreparation Preparation { get; }
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Web.Widgets.IWebWidget> Widgets { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    IPreparation CreatePreparation();
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Web.Widgets.IWebWidget;
}

public interface IWebFlow : IWebFlowSignature, OutSystems.Model.IShareableESpaceObject<IWebFlow, IWebFlowSignature>, OutSystems.Model.UI.IUIFlow {
    new string Description { get; set; }
    new OutSystems.Model.Enumerations.HTTPSecurityWithCertificates HTTPSecurity { get; set; }
    new bool IntegratedAuthentication { get; set; }
    new bool InternalAccessOnly { get; set; }
    new IEnumerable<IWebFlowNode> Nodes { get; }
    new IWebThemeSignature Theme { get; set; }
    new IWebBlock CreateBlock(string name = null, OutSystems.Model.IKey key = null);
    new IWebEmail CreateEmail(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebFlowNode;
    new IWebScreen CreateScreen(string name = null, OutSystems.Model.IKey key = null);
    IWebScreen Instantiate(IWebScreen templateScreen, OutSystems.Model.IKey newScreenKey = null);
    IEnumerable<IWebScreen> Instantiate(IEnumerable<IWebScreen> templateScreens, IEnumerable<OutSystems.Model.IKey> newScreensKeys = null);
}

public interface IWebFlowConnector : OutSystems.Model.UI.IUIFlowConnector {
    new IWebFlowNode Source { get; }
    new IWebFlowNode Target { get; set; }
}

public interface IWebFlowNode : IWebFlowNodeSignature, OutSystems.Model.UI.IUIFlowNode {
}

public interface IWebFlowNodeSignature : OutSystems.Model.UI.IUIFlowNodeSignature {
}

public interface IWebFlowSignature : OutSystems.Model.UI.IUIFlowSignature {
    OutSystems.Model.Enumerations.HTTPSecurityWithCertificates HTTPSecurity { get; }
    bool IntegratedAuthentication { get; }
    bool InternalAccessOnly { get; }
    new IEnumerable<IWebFlowNodeSignature> Nodes { get; }
}

public interface IWebScreen : IWebFlowNode, IWebScreenSignature, OutSystems.Model.IShareableESpaceObject<IWebScreen, IWebScreenSignature>, OutSystems.Model.UI.IScreen {
    Nullable<int> CacheInMinutes { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new OutSystems.Model.Enumerations.HTTPSecurityWithCertificatesAndInheritance HTTPSecurity { get; set; }
    OutSystems.Model.Enumerations.BooleanWithInheritance IntegratedAuthentication { get; set; }
    /// <summary>
    /// When true, this screen is a Frequent Destination, which silences the 'Unexpected Link' warning for links that target it. Set it on the destination screen; there is no API to register a flow connector from the link side.
    /// </summary>
    bool IsFrequentDestination { get; set; }
    /// <summary>
    /// Client-side JavaScript injected verbatim into the rendered Traditional page. NOT auto-escaped: interpolating user-controlled data here yields stored/DOM XSS in the compiled app. Escape with EncodeJavaScript(). See reference/traditional-ui-creation.md.
    /// </summary>
    string JavaScript { get; set; }
    IPreparation Preparation { get; }
    IEnumerable<IScreenAction> ScreenActions { get; }
    ICollection<OutSystems.Model.UI.IUIFlowNode> Targets { get; }
    OutSystems.Model.Expressions.IExpression Title { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.UI.Web.Widgets.IWebWidget> Widgets { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    IPreparation CreatePreparation();
    IScreenAction CreateScreenAction(string name = null, OutSystems.Model.IKey key = null);
    new T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Web.Widgets.IWebWidget;
    void SetTitle(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IWebScreenSignature : IWebFlowNodeSignature, OutSystems.Model.UI.IScreenSignature {
    OutSystems.Model.Enumerations.HTTPSecurityWithCertificatesAndInheritance HTTPSecurity { get; }
}

public interface IWebTheme : IWebThemeSignature, OutSystems.Model.IShareableESpaceObject<IWebTheme, IWebThemeSignature>, OutSystems.Model.UI.ITheme {
    new IWebThemeSignature BaseTheme { get; set; }
    new IWebBlockSignature Email { get; set; }
    IWebFlow ExceptionHandlingFlow { get; set; }
    new OutSystems.Model.UI.IImageSignature FalseImage { get; set; }
    new IWebBlockSignature Footer { get; set; }
    new IWebBlockSignature Header { get; set; }
    new IWebBlockSignature InfoBalloon { get; set; }
    new OutSystems.Model.UI.IImageSignature InfoBalloonImage { get; set; }
    new IWebBlockSignature Layout { get; set; }
    new IWebBlockSignature Menu { get; set; }
    new bool Mobile { get; set; }
    new OutSystems.Model.UI.IOnExceptionAction OnExceptionHandler { get; set; }
    new IWebBlockSignature PopUpEditor { get; set; }
    new OutSystems.Model.UI.IImageSignature TrueImage { get; set; }
}

public interface IWebThemeSignature : OutSystems.Model.UI.IThemeSignature {
    new IWebThemeSignature BaseTheme { get; }
    IWebBlockSignature Email { get; }
    OutSystems.Model.UI.IImageSignature FalseImage { get; }
    new IWebThemeSignature FirstThemeWithGrid { get; }
    new IWebBlockSignature Footer { get; }
    new IWebBlockSignature Header { get; }
    IWebBlockSignature InfoBalloon { get; }
    OutSystems.Model.UI.IImageSignature InfoBalloonImage { get; }
    new IWebBlockSignature Layout { get; }
    new IWebBlockSignature Menu { get; }
    bool Mobile { get; }
    OutSystems.Model.UI.IOnExceptionActionSignature OnExceptionHandler { get; }
    IWebBlockSignature PopUpEditor { get; }
    OutSystems.Model.UI.IImageSignature TrueImage { get; }
}

