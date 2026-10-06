// Native Runtime (NR) widgets plugin definitions (custom widget metadata and behaviors)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.NRWidgets;

public interface IAdvancedHtml : IAdvancedHtmlSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// HTML tag of the element. Provides a single tag. To nest other elements you need to use other HTML Element widgets.
    /// </summary>
    new string Tag { get; set; }
    string Name { get; set; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void CreateContent();
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IAdvancedHtmlSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// HTML tag of the element. Provides a single tag. To nest other elements you need to use other HTML Element widgets.
    /// </summary>
    string Tag { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IButton : IButtonSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Text literal or expression to define the confirmation message displayed after clicking this widget.
    /// </summary>
    OutSystems.Model.Expressions.IExpression ConfirmationMessage { get; }
    /// <summary>
    /// Boolean to specify if the button should submit form that is enclosed in.
    /// </summary>
    bool IsDefault { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnClick { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetConfirmationMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IButtonGroup : IButtonGroupSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IButtonGroupItem : IButtonGroupItemSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetValue(object value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IButtonGroupItemSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Value assigned to the Button Group widget variable when this item is selected.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Value { get; }
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IButtonGroupSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IButtonSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ICheckbox : ICheckboxSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ICheckboxSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IContainer : IContainerSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies the horizontal alignment of the widgets inside this widget.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.Align Align { get; set; }
    /// <summary>
    /// Internal property used to understand if the visible property changed from expression to boolean
    /// </summary>
    bool VisibleWasExpression { get; set; }
    /// <summary>
    /// Performs an animation on the widget when the visible property changes at runtime.
    /// </summary>
    bool Animate { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IContainerSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies the horizontal alignment of the widgets inside this widget.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.Align Align { get; }
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IDropdown : IDropdownSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// Specifies the list of records to show in the dropdown.
    /// </summary>
    OutSystems.Model.Expressions.IExpression List { get; }
    /// <summary>
    /// 'Text (Default)' content provides a native experience for selecting textual values (uses the select HTML tag). 'Custom' provides richer content with non-textual widgets (e.g. images) inside the dropdown (uses the div HTML tag).
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.DropdownMode DropdownMode { get; set; }
    /// <summary>
    /// Attribute of the records in the list to use as the identifier of the selected value.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Values { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetList(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetLabels(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetValues(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetEmptyValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IDropdownSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// 'Text (Default)' content provides a native experience for selecting textual values (uses the select HTML tag). 'Custom' provides richer content with non-textual widgets (e.g. images) inside the dropdown (uses the div HTML tag).
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.DropdownMode DropdownMode { get; }
    /// <summary>
    /// Attribute of the records in the list to use as the label.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Labels { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// Text literal or expression displayed in the Dropdown list that represents an empty selection. When this option is selected, the variable defined holds a default value.
    /// </summary>
    OutSystems.Model.Expressions.IExpression EmptyValue { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IExpression : IExpressionSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Text displayed in the Preview and Design modes.
    /// </summary>
    new string Example { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IExpressionSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Combination of values, operands, operators and variables or the result of a function whose value is computed at runtime.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Value { get; }
    /// <summary>
    /// Text displayed in the Preview and Design modes.
    /// </summary>
    string Example { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IForm : IFormSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IFormSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IHeaderCell : IHeaderCellSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies a list with records to populate the widget.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Source { get; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetSource(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetSortAttribute(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IHeaderCellSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies one or more style classes to apply to the table header. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// Attribute of the records to be used in the OnSort event on the table.
    /// </summary>
    OutSystems.Model.Expressions.IExpression SortAttribute { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IIcon : IIconSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Scalable vector picture to be displayed.
    /// </summary>
    new string Icon { get; set; }
    /// <summary>
    /// Size of the scalable vector picture relatively to the font-size of the first ancestor of this widget.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize IconSize { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Weight { get; set; }
    string Name { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IIconSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Scalable vector picture to be displayed.
    /// </summary>
    string Icon { get; }
    /// <summary>
    /// Size of the scalable vector picture relatively to the font-size of the first ancestor of this widget.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize IconSize { get; }
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// 
    /// </summary>
    string Weight { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IImage : IImageSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies the source of the image: Local Image, External URL, File or Binary Data.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.Type Type { get; set; }
    /// <summary>
    /// Defines the image to display when Type is set to Local Image.
    /// </summary>
    new OutSystems.Model.UI.IImageSignature Image { get; set; }
    /// <summary>
    /// Image to display when the File or Binary Data image cannot be fetched.
    /// </summary>
    new OutSystems.Model.UI.IImageSignature DefaultImage { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetUrl(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetImageContent(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IImageSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies the source of the image: Local Image, External URL, File or Binary Data.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.Type Type { get; }
    /// <summary>
    /// Defines the image to display when Type is set to Local Image.
    /// </summary>
    OutSystems.Model.UI.IImageSignature Image { get; }
    /// <summary>
    /// Defines the URL of the image to display when Type is set to External URL.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Url { get; }
    /// <summary>
    /// Defines the image to display when Type is set to File or Binary Data.
    /// </summary>
    OutSystems.Model.Expressions.IExpression ImageContent { get; }
    /// <summary>
    /// Image to display when the File or Binary Data image cannot be fetched.
    /// </summary>
    OutSystems.Model.UI.IImageSignature DefaultImage { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IInput : IInputSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Input element type. Allows a better input control and extra functionality.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.InputType InputType { get; set; }
    /// <summary>
    /// Maximum number of characters allowed.
    /// </summary>
    Nullable<int> MaxLength { get; set; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetPrompt(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IInputSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// Text hint that describes the expected value for the input. Hides when the field has focus or is no longer empty.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Prompt { get; }
    /// <summary>
    /// Input element type. Allows a better input control and extra functionality.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.InputType InputType { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ILabel : ILabelSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies the input widget that gets focus when this Label widget is tapped.
    /// </summary>
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature TargetWidget { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ILabelSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ILink : ILinkSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Text literal or expression to define the confirmation message displayed after clicking this widget.
    /// </summary>
    OutSystems.Model.Expressions.IExpression ConfirmationMessage { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnClick { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetConfirmationMessage(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ILinkSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IList : IListSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies a list with records to populate the widget.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Source { get; }
    /// <summary>
    /// Set to Yes to Animate Items on append, insert or remove.
    /// </summary>
    new bool AnimateItems { get; set; }
    /// <summary>
    /// Set to Custom to define a custom HTML tag to be used by the widget in runtime. Default mode sets tag as div.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.Mode Mode { get; set; }
    /// <summary>
    /// Wrapper HTML tag to be used by this widget in runtime.
    /// </summary>
    new string Tag { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    /// <summary>
    /// Determines whether the list blank elements are displayed or not.
    /// </summary>
    new bool ExpandedInWebEditor { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent ListItem { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnScrollEnding { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetSource(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IListItem : IListItemSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Determines whether actions are displayed or not.
    /// </summary>
    new bool ExpandedInWebEditor { get; set; }
    /// <summary>
    /// Set to Yes to have default action triggered by a full swipe left. Default action is the rightmost right action.
    /// </summary>
    bool TriggerActionOnFullSwipeLeft { get; set; }
    /// <summary>
    /// Set to Yes to have default action triggered by a full swipe right. Default action is the leftmost left action.
    /// </summary>
    bool TriggerActionOnFullSwipeRight { get; set; }
    string Name { get; set; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent LeftActions { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent RightActions { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnClick { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IListItemAction : IListItemActionSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnClick { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IListItemActionSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IListItemSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Determines whether actions are displayed or not.
    /// </summary>
    bool ExpandedInWebEditor { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature LeftActions { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature RightActions { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IListSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Set to Yes to Animate Items on append, insert or remove.
    /// </summary>
    bool AnimateItems { get; }
    /// <summary>
    /// Set to Custom to define a custom HTML tag to be used by the widget in runtime. Default mode sets tag as div.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.Mode Mode { get; }
    /// <summary>
    /// Wrapper HTML tag to be used by this widget in runtime.
    /// </summary>
    string Tag { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// Determines whether the list blank elements are displayed or not.
    /// </summary>
    bool ExpandedInWebEditor { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature ListItem { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IPopover : IPopoverSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Width of the popover box in pixels (px).
    /// </summary>
    new Nullable<int> PopoverWidth { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    /// <summary>
    /// Determines whether the pop-up is displayed or not.
    /// </summary>
    new bool ExpandedInWebEditor { get; set; }
    string Name { get; set; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent TopContent { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent BottomContent { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IPopoverSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Width of the popover box in pixels (px).
    /// </summary>
    Nullable<int> PopoverWidth { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// Determines whether the pop-up is displayed or not.
    /// </summary>
    bool ExpandedInWebEditor { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature TopContent { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature BottomContent { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IPopup : IPopupSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Boolean literal or expression to determine if the popup is displayed.
    /// </summary>
    OutSystems.Model.Expressions.IExpression ShowPopup { get; }
    /// <summary>
    /// Determines whether the popup is displayed or not.
    /// </summary>
    new bool ExpandedInWebEditor { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetShowPopup(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetShowPopup(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IPopupSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// Determines whether the popup is displayed or not.
    /// </summary>
    bool ExpandedInWebEditor { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IRadioButton : IRadioButtonSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetValue(object value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetVisible(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IRadioButtonSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Value assigned to the Radio Group widget variable when this item is selected.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Value { get; }
    /// <summary>
    /// 
    /// </summary>
    OutSystems.Model.Expressions.IExpression Visible { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IRadioGroup : IRadioGroupSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IRadioGroupSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IRowCell : IRowCellSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IRowCellSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies one or more style classes to apply to the table header. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ISwitch : ISwitchSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ISwitchSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ITableRecords : ITableRecordsSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Specifies a list with records to populate the widget.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Source { get; }
    /// <summary>
    /// Set as Yes to display the header row of the table.
    /// </summary>
    new bool ShowHeader { get; set; }
    /// <summary>
    /// Determines whether the table blank elements are displayed or not.
    /// </summary>
    new bool ExpandedInWebEditor { get; set; }
    string Name { get; set; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent HeaderRow { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Row { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnSort { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetSource(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyleHeader(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStyleRow(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ITableRecordsSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Set as Yes to display the header row of the table.
    /// </summary>
    bool ShowHeader { get; }
    /// <summary>
    /// Determines whether the table blank elements are displayed or not.
    /// </summary>
    bool ExpandedInWebEditor { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the table. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the table header. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression StyleHeader { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the table row. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression StyleRow { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature HeaderRow { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Row { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface ITextArea : ITextAreaSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Maximum number of characters allowed.
    /// </summary>
    Nullable<int> MaxLength { get; set; }
    /// <summary>
    /// Line-height of the widget.
    /// </summary>
    new int TextLines { get; set; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is editable.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Enabled { get; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetVariable(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetPrompt(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetEnabled(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetEnabled(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface ITextAreaSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Holds the value entered by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Variable { get; }
    /// <summary>
    /// Text hint that describes the expected value for the input. Hides when the field has focus or is no longer empty.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Prompt { get; }
    /// <summary>
    /// Line-height of the widget.
    /// </summary>
    int TextLines { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

public interface IUpload : IUploadSignature, OutSystems.Model.UI.Mobile.Widgets.IMobileWidget {
    /// <summary>
    /// Holds the file selected by the user.
    /// </summary>
    OutSystems.Model.Expressions.IExpression FileContent { get; }
    /// <summary>
    /// Holds the name of the file.
    /// </summary>
    OutSystems.Model.Expressions.IExpression FileName { get; }
    /// <summary>
    /// Specifies the type of UI placeholder. The placeholder hints the expected input type.
    /// </summary>
    new ServiceStudio.Plugin.NRWidgets.Enumerations.Accept Accept { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string Width { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginTop { get; set; }
    /// <summary>
    /// 
    /// </summary>
    new string MarginLeft { get; set; }
    string Name { get; set; }
    new IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidget> Widgets { get; }
    new OutSystems.Model.UI.Mobile.Widgets.IContent Content { get; }
    OutSystems.Model.UI.Mobile.Events.IBuiltinEvent OnChange { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    void SetFileContent(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetFileName(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetMandatory(bool value);
    void SetStyle(OutSystems.Model.Expressions.ExpressionDefinition value);
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.UI.Mobile.Widgets.IMobileWidget;
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(Type widgetType, string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.UI.Mobile.Widgets.IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name, OutSystems.Model.IKey key);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
}

public interface IUploadSignature : OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature {
    /// <summary>
    /// Specifies the type of UI placeholder. The placeholder hints the expected input type.
    /// </summary>
    ServiceStudio.Plugin.NRWidgets.Enumerations.Accept Accept { get; }
    /// <summary>
    /// Boolean literal or expression that defines if the widget is required.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Mandatory { get; }
    /// <summary>
    /// 
    /// </summary>
    string Width { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginTop { get; }
    /// <summary>
    /// 
    /// </summary>
    string MarginLeft { get; }
    /// <summary>
    /// Specifies one or more style classes to apply to the widget. Separate multiple values with spaces.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Style { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.Widgets.IMobileWidgetSignature> Widgets { get; }
    OutSystems.Model.UI.Mobile.Widgets.IContentSignature Content { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

