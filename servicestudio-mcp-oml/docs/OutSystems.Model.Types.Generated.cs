// Type system primitives: basic types, records, structures, and their signatures
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Types;

public interface IAnonymousStructure : IRecordType {
}

public interface IAttribute : OutSystems.Model.IObject, IAttributeSignature {
    new ITypeSignature DataType { get; set; }
    new Nullable<int> Decimals { get; set; }
    new string Description { get; set; }
    new bool IsMandatory { get; set; }
    new Nullable<int> Length { get; set; }
    new string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IAttributeSignature : OutSystems.Model.IObjectSignature {
    ITypeSignature DataType { get; }
    Nullable<int> Decimals { get; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; }
    bool IsMandatory { get; }
    Nullable<int> Length { get; }
    string Name { get; }
}

public interface IBasicType : IType {
}

public interface IGenericType : IType {
    int GenericIndex { get; }
}

public interface IIdentifierType : IBasicType {
    OutSystems.Model.Data.IEntitySignature Entity { get; }
}

public interface IListType : IType {
    ITypeSignature ElementType { get; }
}

public interface IRecordType : IRecordTypeSignature, IType {
    new IEnumerable<IAttribute> Attributes { get; }
}

public interface IRecordTypeSignature : ITypeSignature {
    IEnumerable<IAttributeSignature> Attributes { get; }
}

public interface IType : OutSystems.Model.IObject, ITypeSignature {
}

public interface ITypeSignature : OutSystems.Model.IObjectSignature, IEquatable<ITypeSignature> {
    bool IsDisabled { get; }
    Guid UniqueId { get; }
    TypeKind Kind { get; }
    bool IsAssignableTo(ITypeSignature other);
}

/// <summary>
/// Describes a type's kind at a high level (e.g. List is a type kind, while List&lt;Integer&gt; is an actual type)
/// </summary>
public enum TypeKind {
    None,
    Boolean,
    Text,
    PhoneNumber,
    Email,
    Integer,
    LongInteger,
    Decimal,
    Currency,
    Date,
    Time,
    DateTime,
    BinaryData,
    Record,
    List,
    LambdaExpression,
    IntegerIdentifier,
    LongIntegerIdentifier,
    TextIdentifier,
    Object,
    CSharp,
    File,
    Basic,
    Any,
}

