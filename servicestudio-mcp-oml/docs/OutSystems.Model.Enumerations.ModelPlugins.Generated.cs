// Enumeration types specifically used by model plugins
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Enumerations.ModelPlugins;

public enum LambdaExpressionTarget {
    NotApplicable,
    AttributeValue,
    AttributeName,
}

public enum ModelPropertyKind {
    StringProperty,
    BooleanProperty,
    IntegerProperty,
    EnumProperty,
    StyleProperty,
    VariableProperty,
    RecordProperty,
    RecordListProperty,
    WidgetReferenceProperty,
    ImageReferenceProperty,
    ExpressionProperty,
    TypeReferenceProperty,
    BuiltInProperty,
    LambdaExpressionProperty,
    BuiltInExpressionProperty,
}

public enum PropertyRuntimeType {
    NotApplicable,
    Boolean,
    Integer,
    LongInteger,
    String,
    Time,
    Date,
    DateTime,
    Decimal,
    Currency,
    Email,
    PhoneNumber,
    LegacyStyle,
    Style,
    Enum,
    Variable,
    Record,
    List,
    ObjectReference,
    Expression,
    WidgetReference,
    TypeReference,
    BuiltInProperty,
    ImageReference,
    BinaryData,
    File,
    LambdaExpression,
    EntityIdentifier,
    EntityTextIdentifier,
    EntityLongIdentifier,
    Any,
}

