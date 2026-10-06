// Expression tree nodes for building and evaluating complex formulas and conditions
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Expressions.Nodes;

public enum BinaryOperator {
    Plus,
    Minus,
    Multiply,
    Divide,
    Like,
    Equal,
    NotEqual,
    Less,
    LessOrEqual,
    Greater,
    GreaterOrEqual,
    And,
    Or,
}

public interface IArgument : IExpressionNode {
    OutSystems.Model.Expressions.IBinding Parameter { get; set; }
    bool HasName { get; set; }
    IExpressionNode Value { get; set; }
    string WhitespaceBeforeColon { get; set; }
}

public interface IBinaryOperation : IExpressionNode {
    BinaryOperator Operator { get; set; }
    IExpressionNode LeftOperand { get; set; }
    IExpressionNode RightOperand { get; set; }
}

public interface IBooleanLiteral : IExpressionNode {
    bool Value { get; set; }
}

public interface IDateLiteral : IExpressionNode {
    DateTime Value { get; set; }
}

public interface IDateTimeLiteral : IExpressionNode {
    DateTime Value { get; set; }
}

public interface IDecimalLiteral : IExpressionNode {
    decimal Value { get; set; }
}

public interface IExpressionNode {
    OutSystems.Model.Types.ITypeSignature Type { get; }
    IEnumerable<IExpressionNode> Children { get; }
    OutSystems.Model.Expressions.IExpression Owner { get; }
    string Text { get; }
    IEnumerable<OutSystems.Model.IModelObject> UsedObjects { get; }
    IExpressionNode Parent { get; }
    string WhitespaceBefore { get; set; }
    string WhitespaceAfter { get; set; }
    /// <summary>
    /// Replaces this node by the provided one in the parent element.
    /// If the node is a root element, then the expression root is replaced.
    /// 
    /// Can only be called on mutable expressions.
    /// </summary>
    /// <param name="newNode"></param>
    /// <param name="transferWhitespace"></param>
    void ReplaceBy(IExpressionNode newNode, bool transferWhitespace = true);
}

/// <summary>
/// Used only when mutating expressions. It includes the specified value as part of the expression's text.
/// Use it when it is easier to write the expression text rather than creating syntactic tree nodes.
/// </summary>
public interface IExpressionSnippet : IExpressionNode {
    /// <summary>
    /// The value to add as part of the expression's text
    /// </summary>
    string Value { get; set; }
}

public interface IFieldAccess : IExpressionNode {
    IExpressionNode Record { get; set; }
    IIdentifier Field { get; set; }
}

public interface IFunctionCall : IExpressionNode {
    OutSystems.Model.Expressions.IBinding Function { get; set; }
    IEnumerable<IArgument> Arguments { get; set; }
    string WhitespaceAfterFunctionName { get; set; }
    string WhitespaceBetweenParenthesis { get; set; }
}

public interface IIdentifier : IExpressionNode {
    OutSystems.Model.Expressions.IBinding Binding { get; set; }
}

public interface IIndexer : IExpressionNode {
    IExpressionNode List { get; set; }
    IExpressionNode Index { get; set; }
}

public interface IIntegerLiteral : IExpressionNode {
    int Value { get; set; }
}

public interface IListLiteral : IExpressionNode {
    IEnumerable<IExpressionNode> Items { get; set; }
}

public interface ILongLiteral : IExpressionNode {
    long Value { get; set; }
}

public interface IRecordLiteral : IExpressionNode {
    IEnumerable<IRecordLiteralField> Fields { get; set; }
}

public interface IRecordLiteralField : IExpressionNode {
    OutSystems.Model.Types.IAttributeSignature Attribute { get; set; }
    IExpressionNode Value { get; set; }
}

public interface ITextLiteral : IExpressionNode {
    string Value { get; set; }
}

public interface ITimeLiteral : IExpressionNode {
    DateTime Value { get; set; }
}

public interface ITypeConversion : IExpressionNode {
    IExpressionNode SourceValue { get; set; }
    OutSystems.Model.Types.ITypeSignature TargetElementType { get; set; }
    IEnumerable<ITypeConversionMapping> Mappings { get; set; }
}

public interface ITypeConversionMapping : IExpressionNode {
    IExpressionNode SourceValue { get; set; }
    IExpressionNode TargetValue { get; set; }
}

public interface IUnaryOperation : IExpressionNode {
    UnaryOperator Operator { get; set; }
    IExpressionNode Operand { get; set; }
}

public enum UnaryOperator {
    Minus,
    Not,
}

