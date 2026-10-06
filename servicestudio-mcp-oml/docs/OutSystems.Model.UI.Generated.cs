// Web UI artifacts: flows, screens, blocks, widgets, styles, and theming constructs
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI;

public interface IBlock : IBlockSignature, IUIFlowNode, OutSystems.Model.IShareableESpaceObject<IBlock, IBlockSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new byte[] Icon { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    new IEnumerable<IBlockEvent> Events { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    new OutSystems.Model.ISequence<IWidget> Widgets { get; }
    new string StyleSheet { get; set; }
    new string JavaScript { get; set; }
    IBlockEvent CreateEvent(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWidget;
    IWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    void SetOnRenderJavascript(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IBlockEvent : OutSystems.Model.IObject, IBlockEventSignature, OutSystems.Model.Logic.ITriggerableEvent {
    new string Description { get; set; }
    new bool IsMandatory { get; set; }
    new string Name { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IBlockEventSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    bool IsMandatory { get; }
    string Name { get; }
    IEnumerable<OutSystems.Model.IInputParameterSignature> InputParameters { get; }
}

public interface IBlockSignature : IUIFlowNodeSignature {
    string CreatedBy { get; }
    string Description { get; }
    byte[] Icon { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    IEnumerable<IBlockEventSignature> Events { get; }
    IEnumerable<OutSystems.Model.IInputParameterSignature> InputParameters { get; }
    IEnumerable<IWidgetSignature> Widgets { get; }
    string StyleSheet { get; }
    string JavaScript { get; }
    OutSystems.Model.Expressions.IExpression OnRenderJavascript { get; }
    bool Hidden { get; }
}

public interface IEmail : IUIFlowNode {
    string CreatedBy { get; set; }
    string Description { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression Subject { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    OutSystems.Model.ISequence<IWidget> Widgets { get; }
    void SetSubject(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWidget;
    IWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IEventHandler : OutSystems.Model.IObject {
    IBlockEventSignature Event { get; }
    OutSystems.Model.IObjectSignature Handler { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IGridLayout {
    IEnumerable<string> CssClasses { get; }
    string MarginLeftDimension { get; }
    string WidthDimension { get; }
}

public interface IImage : IImageSignature, OutSystems.Model.IShareableESpaceObject<IImage, IImageSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    new IEnumerable<ILocalizedImage> LocalizedImages { get; }
    void SetImageContent(byte[] binary, string originalPath);
    void SetImageContent(byte[] binary, string originalPath, ImageFormat format, int width, int height);
    ILocalizedImage CreateLocalizedImage(OutSystems.Model.Data.ILocale locale, byte[] binary, string originalPath, OutSystems.Model.IKey key = null);
}

public interface IImageSignature : OutSystems.Model.IObjectSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    int Height { get; }
    byte[] ImageContent { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    string OriginalFilename { get; }
    ImageFormat OriginalFormat { get; }
    string RuntimePath { get; }
    int Width { get; }
    IEnumerable<ILocalizedImageSignature> LocalizedImages { get; }
    bool Hidden { get; }
}

public interface ILocalizedImage : OutSystems.Model.IObject, ILocalizedImageSignature {
    OutSystems.Model.Data.ILocale Locale { get; set; }
    void SetImageContent(byte[] binary, string originalPath);
}

public interface ILocalizedImageSignature : OutSystems.Model.IObjectSignature {
    byte[] ImageContent { get; }
    ImageFormat OriginalFormat { get; }
}

public enum ImageFormat {
    Png,
    Jpg,
    Gif,
    Ico,
    Svg,
    Unknown,
}

public interface IOnExceptionAction : IOnExceptionActionSignature, OutSystems.Model.IShareableESpaceObject<IOnExceptionAction, IOnExceptionActionSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IOnExceptionActionSignature : OutSystems.Model.IObjectSignature {
    string CreatedBy { get; }
    string Description { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
}

public interface IScreen : IScreenSignature, IUIFlowNode, OutSystems.Model.IShareableESpaceObject<IScreen, IScreenSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new byte[] Icon { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    ICollection<OutSystems.Model.Logic.IRoleSignature> Roles { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.ILocalVariable> LocalVariables { get; }
    OutSystems.Model.ISequence<IWidget> Widgets { get; }
    string StyleSheet { get; set; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ILocalVariable CreateLocalVariable(string name = null, OutSystems.Model.IKey key = null);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWidget;
    IWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    void ReplaceEntity(OutSystems.Model.Data.IEntitySignature oldEntity, OutSystems.Model.Data.IEntitySignature newEntity, Dictionary<OutSystems.Model.Data.IEntityAttributeSignature, OutSystems.Model.Data.IEntityAttributeSignature> attributesMapping = null);
}

public interface IScreenSignature : IUIFlowNodeSignature {
    string CreatedBy { get; }
    string Description { get; }
    byte[] Icon { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    IEnumerable<OutSystems.Model.IInputParameterSignature> InputParameters { get; }
}

public interface IScript : IScriptSignature, OutSystems.Model.IShareableESpaceObject<IScript, IScriptSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    bool HasSyntaxErrors { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    bool Public { get; set; }
    string JavaScript { get; set; }
    new ICollection<IScriptSignature> RequiredScripts { get; }
}

public interface IScriptSignature : OutSystems.Model.IObjectSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    string RuntimePath { get; }
    IEnumerable<IScriptSignature> RequiredScripts { get; }
}

public interface ITheme : IThemeSignature, OutSystems.Model.IShareableESpaceObject<ITheme, IThemeSignature> {
    new IThemeSignature BaseTheme { get; set; }
    new int Columns { get; set; }
    new int ColumnWidth { get; set; }
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new IBlockSignature Footer { get; set; }
    new OutSystems.Model.Enumerations.GridType GridType { get; set; }
    new int GutterWidth { get; set; }
    new int GutterWidthPercentage { get; set; }
    new IBlockSignature Header { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    new IBlockSignature Layout { get; set; }
    new Nullable<int> MaxWidth { get; set; }
    new IBlockSignature Menu { get; set; }
    new Nullable<int> MinWidth { get; set; }
    bool Public { get; set; }
    new IEnumerable<OutSystems.Model.IThemeValues> ThemeValues { get; }
    /// <summary>
    /// StyleSheet may contain some parts that are generated from theme values.
    /// In these cases, final CSS is only computed when a transaction is completed or
    /// when the ESpace is saved. Thus, this property may return incomplete values
    /// transiently.
    /// </summary>
    new string StyleSheet { get; set; }
    OutSystems.Model.Expressions.ITextWithReferencedElements StyleSheetExpression { get; }
    void AddOrUpdateThemeValues(Dictionary<OutSystems.Model.Enumerations.ThemeProperty, string> properties);
}

public interface IThemeSignature : OutSystems.Model.IObjectSignature {
    IThemeSignature BaseTheme { get; }
    int Columns { get; }
    int ColumnWidth { get; }
    string CreatedBy { get; }
    string Description { get; }
    IThemeSignature FirstThemeWithGrid { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    IBlockSignature Footer { get; }
    OutSystems.Model.Enumerations.GridType GridType { get; }
    int GutterWidth { get; }
    int GutterWidthPercentage { get; }
    IBlockSignature Header { get; }
    string InvisibleStyleSheet { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    IBlockSignature Layout { get; }
    Nullable<int> MaxWidth { get; }
    IBlockSignature Menu { get; }
    Nullable<int> MinWidth { get; }
    string Name { get; set; }
    int TotalWidth { get; }
    IEnumerable<OutSystems.Model.IThemeValuesSignature> ThemeValues { get; }
    string StyleSheet { get; }
}

public interface IUIFlow : OutSystems.Model.IFlow, IUIFlowSignature, OutSystems.Model.IShareableESpaceObject<IUIFlow, IUIFlowSignature> {
    new string CreatedBy { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    IThemeSignature Theme { get; set; }
    new IOnExceptionAction OnExceptionHandler { get; }
    new IEnumerable<IUIFlowNode> Nodes { get; }
    IOnExceptionAction CreateOnExceptionHandler(string name = null);
    IScreen CreateScreen(string name = null, OutSystems.Model.IKey key = null);
    IBlock CreateBlock(string name = null, OutSystems.Model.IKey key = null);
    IEmail CreateEmail(string name = null, OutSystems.Model.IKey key = null);
    IEnumerable<IScreen> Instantiate(IEnumerable<IScreen> templateScreens, IEnumerable<OutSystems.Model.IKey> newScreensKeys = null);
    IScreen Instantiate(IScreen templateScreen, OutSystems.Model.IKey newScreenKey = null);
    IEnumerable<IEmail> Instantiate(IEnumerable<IEmail> templateEmails, IEnumerable<OutSystems.Model.IKey> newEmailsKeys = null);
    IEmail Instantiate(IEmail templateEmail, OutSystems.Model.IKey newEmailKey = null);
    void ArrangeAllNodes();
}

public interface IUIFlowConnector : OutSystems.Model.IConnector {
    new IUIFlowNode Source { get; }
    new IUIFlowNode Target { get; set; }
}

public interface IUIFlowNode : OutSystems.Model.IFlowNode, IUIFlowNodeSignature {
    new IEnumerable<IUIFlowConnector> Connectors { get; }
    new IEnumerable<IUIFlowConnector> IncomingConnectors { get; }
}

public interface IUIFlowNodeSignature : OutSystems.Model.IFlowNodeSignature {
}

public interface IUIFlowSignature : OutSystems.Model.IFlowSignature {
    string CreatedBy { get; }
    string Description { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    IOnExceptionActionSignature OnExceptionHandler { get; }
    IEnumerable<IUIFlowNodeSignature> Nodes { get; }
}

public interface IWidget : OutSystems.Model.IObject, IWidgetSignature {
    new string CustomStyle { get; set; }
    void Delete(bool preserveChildren);
}

public interface IWidgetDefinitionPlaceholderSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    bool IsIterated { get; }
    string Name { get; }
    string SourceProperty { get; }
}

public interface IWidgetDefinitionPropertySignature : OutSystems.Model.IObjectSignature {
    string DefaultValue { get; }
    string Description { get; }
    bool IsMandatory { get; }
    string LambdaExpressionSourceProperty { get; }
    OutSystems.Model.Enumerations.ModelPlugins.LambdaExpressionTarget LambdaExpressionTarget { get; }
    string Name { get; }
    OutSystems.Model.Enumerations.ModelPlugins.ModelPropertyKind PropertyKind { get; }
    OutSystems.Model.Enumerations.ModelPlugins.PropertyRuntimeType RuntimeType { get; }
}

public interface IWidgetDefinitionRequiredResourceSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    OutSystems.Model.IKey ResourceKey { get; }
}

public interface IWidgetDefinitionSignature : OutSystems.Model.IObjectSignature {
    string Description { get; }
    bool HasValidationProperties { get; }
    bool IsValidationAggregator { get; }
    string Name { get; set; }
    IEnumerable<IWidgetDefinitionPlaceholderSignature> Placeholders { get; }
    IEnumerable<IWidgetDefinitionPropertySignature> Properties { get; }
    IEnumerable<IWidgetDefinitionRequiredResourceSignature> RequiredResources { get; }
}

public interface IWidgetSignature : OutSystems.Model.IObjectSignature {
    string CustomStyle { get; }
}

public interface IWidgetWithGridProperties : IWidgetSignature {
    IGridLayout GridLayout { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string Width { get; }
}

