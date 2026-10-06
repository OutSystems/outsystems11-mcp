// Traditional Web widget and control definitions (server-rendered, View State based)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI.Web.Widgets;

public interface IButtonWidget : IButtonWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new string Example { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string Height { get; set; }
    new bool IsDefault { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IBuiltinEvent OnClick { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetLabel(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IButtonWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    string Example { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string Height { get; }
    bool IsDefault { get; }
    OutSystems.Model.Expressions.IExpression Label { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface ICellWidget : ICellWidgetSignature, IWebWidget {
    new Nullable<int> ColumnSpan { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string Height { get; set; }
    string Name { get; set; }
    new string StyleClasses { get; set; }
    new OutSystems.Model.Enumerations.VerticalAlignment VerticalAlignment { get; set; }
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface ICellWidgetSignature : IWebWidgetSignature {
    Nullable<int> ColumnSpan { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string Height { get; }
    string StyleClasses { get; }
    OutSystems.Model.Enumerations.VerticalAlignment VerticalAlignment { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

public interface ICheckBoxWidget : ICheckBoxWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IOnChange OnChange { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ICheckBoxWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface IComboBoxWidget : IWebWidget {
    OutSystems.Model.Expressions.IExpression IsMandatory { get; }
    OutSystems.Model.Data.IEntityAttributeSignature SourceAttribute { get; set; }
    OutSystems.Model.Data.IServerEntitySignature SourceEntity { get; set; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    void SetIsMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
}

/// <summary>
/// Traditional Web container widget. Holds a Widgets sequence and exposes CreateWidget<T> to build the child tree (Text, Expression, Link, Input, ...).
/// </summary>
public interface IContainerWidget : IContainerWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.Align Align { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string Height { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IOnClick OnClick { get; }
    new string StyleClasses { get; set; }
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    void SetDisplay(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IContainerWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.Align Align { get; }
    OutSystems.Model.Expressions.IExpression Display { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string Height { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
    string Width { get; }
}

public interface IContent : IContentSignature, IWebWidget {
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IContentSignature : IWebWidgetSignature {
    string Name { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

public interface IEditRecordWidget : IEditRecordWidgetSignature, IWebWidget {
    new string CellHeight { get; set; }
    new Nullable<int> CellSpacing { get; set; }
    new OutSystems.Model.Enumerations.DisplayColumns DisplayColumns { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveCaptionWidth { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveValueWidth { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string LabelStyle { get; set; }
    new string LabelWidth { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature MasterEntity { get; }
    string Name { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature RecordDefinition { get; set; }
    new OutSystems.Model.ISequence<IRowWidget> Rows { get; }
    OutSystems.Model.Expressions.IExpression SourceRecord { get; }
    new string StyleClasses { get; set; }
    new string ValueStyle { get; set; }
    new string VaueWidth { get; set; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    IRowWidget CreateRow(string name = null, OutSystems.Model.IKey key = null);
    void SetSourceRecord(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IEditRecordWidgetSignature : IWebWidgetSignature {
    string CellHeight { get; }
    Nullable<int> CellSpacing { get; }
    OutSystems.Model.Enumerations.DisplayColumns DisplayColumns { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveCaptionWidth { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveValueWidth { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string LabelStyle { get; }
    string LabelWidth { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    IEnumerable<IRowWidgetSignature> Rows { get; }
    string StyleClasses { get; }
    string ValueStyle { get; }
    string VaueWidth { get; }
    string Width { get; }
}

public interface IExpressionWidget : IExpressionWidgetSignature, IWebWidget {
    bool EscapeContent { get; set; }
    new string Example { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    string Name { get; set; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IExpressionWidgetSignature : IWebWidgetSignature {
    string Example { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Value { get; }
}

public interface IIfBranchWidget : IIfBranchWidgetSignature, IWebWidget {
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IIfBranchWidgetSignature : IWebWidgetSignature {
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

public interface IIfWidget : IIfWidgetSignature, IWebWidget {
    OutSystems.Model.Expressions.IExpression Condition { get; }
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    new IIfBranchWidget FalseBranch { get; }
    string Name { get; set; }
    new IIfBranchWidget TrueBranch { get; }
    void SetCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IIfWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    IIfBranchWidgetSignature FalseBranch { get; }
    IIfBranchWidgetSignature TrueBranch { get; }
}

public interface IInputFilenameWidget : IInputFilenameWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IInputFilenameWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface IInputPasswordWidget : IInputPasswordWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    Nullable<int> MaxLength { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IOnChange OnChange { get; }
    OutSystems.Model.Enumerations.PasswordType PasswordType { get; set; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IInputPasswordWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface IInputWidget : IInputWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    Nullable<int> MaxLength { get; set; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression NullValue { get; }
    OutSystems.Model.UI.Web.Events.IOnChange OnChange { get; }
    new string StyleClasses { get; set; }
    new Nullable<int> TextLines { get; set; }
    OutSystems.Model.Enumerations.InputType Type { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetNullValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetPrompt(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IInputWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    OutSystems.Model.Expressions.IExpression Prompt { get; }
    string StyleClasses { get; }
    Nullable<int> TextLines { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface ILinkWidget : ILinkWidgetSignature, IWebWidget {
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new bool IsDefault { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.ILinkOnClick OnClick { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.Expressions.IExpression Title { get; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Web.Events.ILinkOnClick CreateOnClick();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTitle(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ILinkWidgetSignature : IWebWidgetSignature {
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    bool IsDefault { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

/// <summary>
/// Traditional Web ListRecords widget. Bind its data with SetSourceRecordList(...) (an expression string, e.g. "GetFeedPosts.List") and provide an empty message via SetEmptyMessage(...).
/// </summary>
public interface IListRecordsWidget : IListRecordsWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    OutSystems.Model.Expressions.IExpression EmptyMessage { get; }
    string EmptyMessageStyle { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new OutSystems.Model.Enumerations.ListRecordsLineSeparator LineSeparator { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature MasterEntity { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression SourceRecordList { get; }
    OutSystems.Model.Expressions.IExpression StartIndex { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.Types.IListType Type { get; }
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    void SetEmptyMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetLineCount(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetSourceRecordList(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStartIndex(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IListRecordsWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    OutSystems.Model.Expressions.IExpression LineCount { get; }
    OutSystems.Model.Enumerations.ListRecordsLineSeparator LineSeparator { get; }
    string StyleClasses { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

public interface IPlaceholderContentWidget : IPlaceholderContentWidgetSignature, IWebWidget {
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IPlaceholderContentWidgetSignature : IWebWidgetSignature {
    IPlaceholderWidgetSignature Placeholder { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
}

public interface IPlaceholderWidget : IPlaceholderWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.Align Align { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string Height { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    new string Name { get; set; }
    new string StyleClasses { get; set; }
    new OutSystems.Model.ISequence<IWebWidget> Widgets { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IWebWidget;
    IWebWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IPlaceholderWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.Align Align { get; }
    string Description { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string Height { get; }
    bool IsAjaxRefreshed { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string Name { get; }
    string StyleClasses { get; }
    IEnumerable<IWebWidgetSignature> Widgets { get; }
    string Width { get; }
}

public interface IRadioButtonWidget : IRadioButtonWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IOnChange OnChange { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
    OutSystems.Model.Expressions.IExpression Value { get; }
    OutSystems.Model.Expressions.IExpression Variable { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IRadioButtonWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string StyleClasses { get; }
    OutSystems.Model.Expressions.IExpression Visible { get; }
    string Width { get; }
}

public interface IRowWidget : IRowWidgetSignature, IWebWidget {
    new OutSystems.Model.ISequence<ICellWidget> Cells { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    ICellWidget CreateCell(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface IRowWidgetSignature : IWebWidgetSignature {
    IEnumerable<ICellWidgetSignature> Cells { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IShowRecordWidget : IShowRecordWidgetSignature, IWebWidget {
    new string CellHeight { get; set; }
    new Nullable<int> CellSpacing { get; set; }
    new OutSystems.Model.Enumerations.DisplayColumns DisplayColumns { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveCaptionWidth { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveValueWidth { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string LabelStyle { get; set; }
    new string LabelWidth { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature MasterEntity { get; }
    string Name { get; set; }
    new OutSystems.Model.ISequence<IRowWidget> Rows { get; }
    OutSystems.Model.Expressions.IExpression SourceRecord { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature Type { get; }
    new string ValueStyle { get; set; }
    new string VaueWidth { get; set; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    IRowWidget CreateRow(string name = null, OutSystems.Model.IKey key = null);
    void SetSourceRecord(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IShowRecordWidgetSignature : IWebWidgetSignature {
    string CellHeight { get; }
    Nullable<int> CellSpacing { get; }
    OutSystems.Model.Enumerations.DisplayColumns DisplayColumns { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveCaptionWidth { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveValueWidth { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string LabelStyle { get; }
    string LabelWidth { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    IEnumerable<IRowWidgetSignature> Rows { get; }
    string StyleClasses { get; }
    string ValueStyle { get; }
    string VaueWidth { get; }
    string Width { get; }
}

public interface ITableRecordsWidget : ITableRecordsWidgetSignature, IWebWidget {
    new string CellHeight { get; set; }
    new Nullable<int> CellSpacing { get; set; }
    new IRowWidget DataRow { get; }
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    OutSystems.Model.Expressions.IExpression EmptyMessage { get; }
    new string EvenLineStyle { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new IRowWidget HeaderRow { get; }
    new string HeaderStyle { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    OutSystems.Model.Types.IRecordTypeSignature MasterEntity { get; }
    string Name { get; set; }
    new string OddLineStyle { get; set; }
    new bool ShowHeader { get; set; }
    OutSystems.Model.Expressions.IExpression SourceRecordList { get; }
    OutSystems.Model.Expressions.IExpression StartIndex { get; }
    new string StyleClasses { get; set; }
    OutSystems.Model.Types.IListType Type { get; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    void SetEmptyMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetLineCount(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetSourceRecordList(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStartIndex(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ITableRecordsWidgetSignature : IWebWidgetSignature {
    string CellHeight { get; }
    Nullable<int> CellSpacing { get; }
    IRowWidgetSignature DataRow { get; }
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    string EvenLineStyle { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    IRowWidgetSignature HeaderRow { get; }
    string HeaderStyle { get; }
    OutSystems.Model.Expressions.IExpression LineCount { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string OddLineStyle { get; }
    bool ShowHeader { get; }
    string StyleClasses { get; }
    string Width { get; }
}

public interface ITableWidget : ITableWidgetSignature, IWebWidget {
    new OutSystems.Model.Enumerations.Align Align { get; set; }
    new string CellHeight { get; set; }
    new Nullable<int> CellSpacing { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new string Height { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    string Name { get; set; }
    new OutSystems.Model.ISequence<IRowWidget> Rows { get; }
    new string StyleClasses { get; set; }
    new string Width { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    IRowWidget CreateRow(string name = null, OutSystems.Model.IKey key = null);
}

public interface ITableWidgetSignature : IWebWidgetSignature {
    OutSystems.Model.Enumerations.Align Align { get; }
    string CellHeight { get; }
    Nullable<int> CellSpacing { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string Height { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    IEnumerable<IRowWidgetSignature> Rows { get; }
    string StyleClasses { get; }
    string Width { get; }
}

public interface ITextWidget : ITextWidgetSignature, IWebWidget {
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    string Name { get; set; }
    new string StyleClasses { get; set; }
    new string Text { get; set; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface ITextWidgetSignature : IWebWidgetSignature {
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    string StyleClasses { get; }
    string Text { get; }
}

public interface IWebBlockInstanceWidget : IWebBlockInstanceWidgetSignature, IWebWidget {
    new IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    IEnumerable<OutSystems.Model.UI.IEventHandler> EventHandlers { get; }
    string Name { get; set; }
    OutSystems.Model.UI.Web.Events.IOnNotify OnNotify { get; }
    new OutSystems.Model.ISequence<IPlaceholderContentWidget> PlaceholdersContent { get; }
    new OutSystems.Model.UI.Web.IWebBlockSignature SourceBlock { get; set; }
    OutSystems.Model.IObject ValidationParent { get; set; }
}

public interface IWebBlockInstanceWidgetSignature : IWebWidgetSignature {
    IEnumerable<OutSystems.Model.IArgumentSignature> Arguments { get; }
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    IEnumerable<IPlaceholderContentWidgetSignature> PlaceholdersContent { get; }
    OutSystems.Model.UI.Web.IWebBlockSignature SourceBlock { get; }
}

/// <summary>
/// Base interface for every Traditional Web widget. CreateWidget<T>(...) requires T : IWebWidget; passing the Reactive ServiceStudio.Plugin.NRWidgets.IContainer fails with CS0311.
/// </summary>
public interface IWebWidget : IWebWidgetSignature, OutSystems.Model.UI.IWidget {
}

public interface IWebWidgetSignature : OutSystems.Model.UI.IWidgetSignature {
}

