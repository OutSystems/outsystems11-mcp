// Mobile widget and control definitions optimized for touch and device capabilities
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI.Mobile.Widgets;

public interface IContent : IContentSignature, IMobileWidget {
    new OutSystems.Model.ISequence<IMobileWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IMobileWidget;
    IMobileWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
}

public interface IContentSignature : IMobileWidgetSignature {
    string Name { get; }
    IEnumerable<IMobileWidgetSignature> Widgets { get; }
    /// <summary>
    /// True if the widget will be repeating this content at runtime
    /// </summary>
    bool IsIterated { get; }
}

public interface IIfBranchWidget : IIfBranchWidgetSignature, IMobileWidget {
    new OutSystems.Model.ISequence<IMobileWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IMobileWidget;
    IMobileWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
}

public interface IIfBranchWidgetSignature : IMobileWidgetSignature {
    IEnumerable<IMobileWidgetSignature> Widgets { get; }
}

public interface IIfWidget : IIfWidgetSignature, IMobileWidget {
    bool Animate { get; set; }
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    string Name { get; set; }
    new IIfBranchWidget TrueBranch { get; }
    new IIfBranchWidget FalseBranch { get; }
    void SetCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IIfWidgetSignature : IMobileWidgetSignature {
    OutSystems.Model.Expressions.IExpression Condition { get; }
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    IIfBranchWidgetSignature TrueBranch { get; }
    IIfBranchWidgetSignature FalseBranch { get; }
}

/// <summary>
/// Marker interface used to identify layout widget descriptors in the Mobile UI framework.
/// This is implemented by widgets defined with the attribute IsLayout=true.
/// </summary>
public interface ILayoutWidgetDescriptor {
}

public interface IMobileBlockInstanceWidget : IMobileBlockInstanceWidgetSignature, IMobileWidget {
    new OutSystems.Model.Enumerations.DesignMode DesignMode { get; set; }
    string Name { get; set; }
    new OutSystems.Model.UI.Mobile.IMobileBlockSignature SourceBlock { get; set; }
    new IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    IEnumerable<OutSystems.Model.UI.IEventHandler> EventHandlers { get; }
    new OutSystems.Model.ISequence<IPlaceholderContentWidget> PlaceholdersContent { get; }
}

public interface IMobileBlockInstanceWidgetSignature : IMobileWidgetSignature {
    OutSystems.Model.Enumerations.DesignMode DesignMode { get; }
    OutSystems.Model.UI.Mobile.IMobileBlockSignature SourceBlock { get; }
    IEnumerable<OutSystems.Model.IArgumentSignature> Arguments { get; }
    IEnumerable<IPlaceholderContentWidgetSignature> PlaceholdersContent { get; }
}

public interface IMobileWidget : OutSystems.Model.UI.IWidget, IMobileWidgetSignature {
    OutSystems.Model.UI.IWidgetDefinitionSignature Definition { get; }
}

public interface IMobileWidgetSignature : OutSystems.Model.UI.IWidgetSignature {
}

public interface IPlaceholderContentWidget : IMobileWidget, IPlaceholderContentWidgetSignature {
    new OutSystems.Model.ISequence<IMobileWidget> Widgets { get; }
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IMobileWidget;
    IMobileWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
}

public interface IPlaceholderContentWidgetSignature : IMobileWidgetSignature {
    IPlaceholderWidgetSignature Placeholder { get; }
    IEnumerable<IMobileWidgetSignature> Widgets { get; }
}

public interface IPlaceholderWidget : IMobileWidget, IPlaceholderWidgetSignature {
    new OutSystems.Model.Enumerations.Align Align { get; set; }
    new string Description { get; set; }
    new OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; set; }
    new OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; set; }
    new string Height { get; set; }
    new string MarginLeft { get; set; }
    new string MarginTop { get; set; }
    new string Name { get; set; }
    new string Width { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.UI.Mobile.Events.IEvent> Events { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new OutSystems.Model.ISequence<IMobileWidget> Widgets { get; }
    void SetStyleClasses(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.UI.Mobile.Events.IEvent CreateEvent();
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    T CreateWidget<T>(string name = null, OutSystems.Model.IKey key = null) where T: IMobileWidget;
    IMobileWidget CreateWidget(Type type, string name = null, OutSystems.Model.IKey key = null);
    IMobileWidget CreateWidget(OutSystems.Model.UI.IWidgetDefinitionSignature widgetDefinition, string name = null, OutSystems.Model.IKey key = null);
}

public interface IPlaceholderWidgetSignature : IMobileWidgetSignature {
    OutSystems.Model.Enumerations.Align Align { get; }
    string Description { get; }
    OutSystems.Model.Enumerations.EffectiveMarginLeftType EffectiveMarginLeft { get; }
    OutSystems.Model.Enumerations.EffectiveWidthType EffectiveWidth { get; }
    string Height { get; }
    string MarginLeft { get; }
    string MarginTop { get; }
    string Name { get; }
    OutSystems.Model.Expressions.IExpression StyleClasses { get; }
    string Width { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
    IEnumerable<IMobileWidgetSignature> Widgets { get; }
}

public interface ITextWidget : IMobileWidget, ITextWidgetSignature {
    string Name { get; set; }
    new string Text { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    void SetStyleClasses(OutSystems.Model.Expressions.ExpressionDefinition value);
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface ITextWidgetSignature : IMobileWidgetSignature {
    OutSystems.Model.Expressions.IExpression StyleClasses { get; }
    string Text { get; }
    IEnumerable<OutSystems.Model.IExtendedPropertySignature> ExtendedProperties { get; }
}

