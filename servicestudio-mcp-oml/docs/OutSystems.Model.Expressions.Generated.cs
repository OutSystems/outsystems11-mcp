// Expression tree and evaluation constructs (formulas, literals, function calls, conversions)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Expressions;

public class BuiltinFunctionsInfo {
    public IEnumerable<IBuiltinFunctionInfo> All { get; }
    public IBuiltinFunctionInfo Abs { get; }
    public IBuiltinFunctionInfo Mod { get; }
    public IBuiltinFunctionInfo Power { get; }
    public IBuiltinFunctionInfo Round { get; }
    public IBuiltinFunctionInfo Sqrt { get; }
    public IBuiltinFunctionInfo Trunc { get; }
    public IBuiltinFunctionInfo Max { get; }
    public IBuiltinFunctionInfo Min { get; }
    public IBuiltinFunctionInfo Sign { get; }
    public IBuiltinFunctionInfo Chr { get; }
    public IBuiltinFunctionInfo Concat { get; }
    public IBuiltinFunctionInfo EncodeHtml { get; }
    public IBuiltinFunctionInfo EncodeJavaScript { get; }
    public IBuiltinFunctionInfo EncodeSql { get; }
    public IBuiltinFunctionInfo EncodeUrl { get; }
    public IBuiltinFunctionInfo Index { get; }
    public IBuiltinFunctionInfo Length { get; }
    public IBuiltinFunctionInfo NewLine { get; }
    public IBuiltinFunctionInfo Replace { get; }
    public IBuiltinFunctionInfo Substr { get; }
    public IBuiltinFunctionInfo ToLower { get; }
    public IBuiltinFunctionInfo ToUpper { get; }
    public IBuiltinFunctionInfo Trim { get; }
    public IBuiltinFunctionInfo TrimEnd { get; }
    public IBuiltinFunctionInfo TrimStart { get; }
    public IBuiltinFunctionInfo AddDays { get; }
    public IBuiltinFunctionInfo AddHours { get; }
    public IBuiltinFunctionInfo AddMinutes { get; }
    public IBuiltinFunctionInfo AddMonths { get; }
    public IBuiltinFunctionInfo AddSeconds { get; }
    public IBuiltinFunctionInfo AddYears { get; }
    public IBuiltinFunctionInfo BuildDateTime { get; }
    public IBuiltinFunctionInfo CurrDate { get; }
    public IBuiltinFunctionInfo CurrDateTime { get; }
    public IBuiltinFunctionInfo CurrTime { get; }
    public IBuiltinFunctionInfo Day { get; }
    public IBuiltinFunctionInfo DayOfWeek { get; }
    public IBuiltinFunctionInfo DiffDays { get; }
    public IBuiltinFunctionInfo DiffHours { get; }
    public IBuiltinFunctionInfo DiffMinutes { get; }
    public IBuiltinFunctionInfo DiffSeconds { get; }
    public IBuiltinFunctionInfo Hour { get; }
    public IBuiltinFunctionInfo Minute { get; }
    public IBuiltinFunctionInfo Month { get; }
    public IBuiltinFunctionInfo NewDate { get; }
    public IBuiltinFunctionInfo NewDateTime { get; }
    public IBuiltinFunctionInfo NewTime { get; }
    public IBuiltinFunctionInfo Second { get; }
    public IBuiltinFunctionInfo Year { get; }
    public IBuiltinFunctionInfo BooleanToInteger { get; }
    public IBuiltinFunctionInfo BooleanToText { get; }
    public IBuiltinFunctionInfo DateTimeToDate { get; }
    public IBuiltinFunctionInfo DateTimeToText { get; }
    public IBuiltinFunctionInfo DateTimeToTime { get; }
    public IBuiltinFunctionInfo DateToDateTime { get; }
    public IBuiltinFunctionInfo DateToText { get; }
    public IBuiltinFunctionInfo DecimalToBoolean { get; }
    public IBuiltinFunctionInfo DecimalToInteger { get; }
    public IBuiltinFunctionInfo DecimalToIntegerValidate { get; }
    public IBuiltinFunctionInfo DecimalToLongInteger { get; }
    public IBuiltinFunctionInfo DecimalToLongIntegerValidate { get; }
    public IBuiltinFunctionInfo DecimalToText { get; }
    public IBuiltinFunctionInfo LongIntegerToInteger { get; }
    public IBuiltinFunctionInfo LongIntegerToIntegerValidate { get; }
    public IBuiltinFunctionInfo LongIntegerToIdentifier { get; }
    public IBuiltinFunctionInfo LongIntegerToText { get; }
    public IBuiltinFunctionInfo IdentifierToInteger { get; }
    public IBuiltinFunctionInfo IdentifierToLongInteger { get; }
    public IBuiltinFunctionInfo IdentifierToText { get; }
    public IBuiltinFunctionInfo IntegerToBoolean { get; }
    public IBuiltinFunctionInfo IntegerToDecimal { get; }
    public IBuiltinFunctionInfo IntegerToIdentifier { get; }
    public IBuiltinFunctionInfo IntegerToText { get; }
    public IBuiltinFunctionInfo NullDate { get; }
    public IBuiltinFunctionInfo NullIdentifier { get; }
    public IBuiltinFunctionInfo NullObject { get; }
    public IBuiltinFunctionInfo NullBinary { get; }
    public IBuiltinFunctionInfo NullFile { get; }
    public IBuiltinFunctionInfo NullTextIdentifier { get; }
    public IBuiltinFunctionInfo TextToDate { get; }
    public IBuiltinFunctionInfo TextToDateTime { get; }
    public IBuiltinFunctionInfo TextToDateTimeValidate { get; }
    public IBuiltinFunctionInfo TextToDateValidate { get; }
    public IBuiltinFunctionInfo TextToDecimal { get; }
    public IBuiltinFunctionInfo TextToDecimalValidate { get; }
    public IBuiltinFunctionInfo TextToIdentifier { get; }
    public IBuiltinFunctionInfo TextToInteger { get; }
    public IBuiltinFunctionInfo TextToLongInteger { get; }
    public IBuiltinFunctionInfo TextToIntegerValidate { get; }
    public IBuiltinFunctionInfo TextToLongIntegerValidate { get; }
    public IBuiltinFunctionInfo TextToTime { get; }
    public IBuiltinFunctionInfo TextToTimeValidate { get; }
    public IBuiltinFunctionInfo TimeToText { get; }
    public IBuiltinFunctionInfo ToObject { get; }
    public IBuiltinFunctionInfo Size { get; }
    public IBuiltinFunctionInfo MIMEType { get; }
    public IBuiltinFunctionInfo IsTemp { get; }
    public IBuiltinFunctionInfo IsOwnedByCurrentApp { get; }
    public IBuiltinFunctionInfo FormatCurrency { get; }
    public IBuiltinFunctionInfo FormatDecimal { get; }
    public IBuiltinFunctionInfo FormatPercent { get; }
    public IBuiltinFunctionInfo FormatPhoneNumber { get; }
    public IBuiltinFunctionInfo FormatText { get; }
    public IBuiltinFunctionInfo FormatDateTime { get; }
    public IBuiltinFunctionInfo EmailAddressCreate { get; }
    public IBuiltinFunctionInfo EmailAddressesConcatenate { get; }
    public IBuiltinFunctionInfo EmailAddressValidate { get; }
    public IBuiltinFunctionInfo GetApplicationServerType { get; }
    public IBuiltinFunctionInfo GetCurrentLocale { get; }
    public IBuiltinFunctionInfo GetDatabaseProvider { get; }
    public IBuiltinFunctionInfo GetUserAgent { get; }
    public IBuiltinFunctionInfo GetOwnerEspaceIdentifier { get; }
    public IBuiltinFunctionInfo GetEntryEspaceName { get; }
    public IBuiltinFunctionInfo GetEntryEspaceId { get; }
    public IBuiltinFunctionInfo GetObsoleteTenantId { get; }
    public IBuiltinFunctionInfo GetAppName { get; }
    public IBuiltinFunctionInfo AddPersonalAreaToURLPath { get; }
    public IBuiltinFunctionInfo GetBookmarkableURL { get; }
    public IBuiltinFunctionInfo GetPersonalAreaName { get; }
    public IBuiltinFunctionInfo GetOwnerURLPath { get; }
    public IBuiltinFunctionInfo GetExceptionURL { get; }
    public IBuiltinFunctionInfo GeneratePassword { get; }
    public IBuiltinFunctionInfo If { get; }
    public IBuiltinFunctionInfo IsLoadingScreen { get; }
    public IBuiltinFunctionInfo CurrentThemeIsMobile { get; }
    public IBuiltinFunctionInfo CheckRole { get; }
    public IBuiltinFunctionInfo CheckEndUserRole { get; }
    public IBuiltinFunctionInfo GetUserId { get; }
    public static IBuiltinFunctionInfo Get(string name) => default;
}

public enum EnvironmentEntryKind {
    ClientFunction,
    ServerFunction,
    Other,
}

/// <summary>
/// Represents the value of an yet to be created expression.
/// Due to the way expressions are represented in the OutSystems model (they're also model objects)
/// we cannot create a "disconnected" expression and then store it in some other object's property.
/// 
/// The most frequent representation for an yet to be created expression is a string (that't what
/// you edit in the expression editor window), but we have other kinds of expressions, namely type
/// conversion and record/list literals, that cannot be represented as text.
/// </summary>
public class ExpressionDefinition : IEquatable<ExpressionDefinition> {
    /// <summary>
    /// The type of the expression value, if already known.
    /// </summary>
    public OutSystems.Model.Types.ITypeSignature Type { get; }
    public static implicit operator ExpressionDefinition(string s) => default;
    public static implicit operator ExpressionDefinition(bool b) => default;
    public static implicit operator ExpressionDefinition(int i) => default;
    public static implicit operator ExpressionDefinition(long l) => default;
    public static implicit operator ExpressionDefinition(float f) => default;
    public static implicit operator ExpressionDefinition(double d) => default;
    public static implicit operator ExpressionDefinition(decimal d) => default;
    public static ExpressionDefinition Convert(object value) => default;
    /// <summary>
    /// Parses the given text into the correct ExpressionDefinition instance. It detects the special cases of record literals,
    /// list literals, and type conversions, which provide their own parsing methods:
    /// * ExpressionDefinition.RecordLiteral: "{ Name: \"Ann\" }
    /// * ExpressionDefinition.ListLiteral: "[ 1, 2, 3 ]"
    /// * ExpressionDefinition.TypeConversion: "user mapTo { FirstName: Name }"
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ExpressionDefinition Parse(string text) => default;
}

public static class ExpressionExtensions {
    public static ExpressionDefinition ToExpressionDefinition(this IExpression expression) => default;
    public static ExpressionDefinition ToExpressionDefinition(this OutSystems.Model.Expressions.Nodes.IExpressionNode node) => default;
    public static OutSystems.Model.IModelObject GetAssociatedObject(this IExpression expression) => default;
    public static OutSystems.Model.IModelObject GetAssociatedObject(this OutSystems.Model.Expressions.Nodes.IExpressionNode node) => default;
}

public interface IAIRecordLiteralField : IRecordLiteralField {
    string Description { get; set; }
    bool IsFilledByAI { get; set; }
}

public interface IAttributeMapping {
    /// <summary>
    /// The attribute that we are setting in this mapping.
    /// This value is calculated automatically by the model and we cannot change it.
    /// </summary>
    IExpression TargetValue { get; }
    /// <summary>
    /// The value to use to set the target value.
    /// </summary>
    IExpression SourceValue { get; }
    string Description { get; set; }
    bool IsFilledByAI { get; }
    void SetSourceValue(ExpressionDefinition value);
}

public interface IBinding : IEquatable<IBinding> {
}

public interface IBuiltinFunction : IBuiltinFunctionInfo {
    new IEnumerable<IInputParameter> InputParameters { get; }
    OutSystems.Model.Types.ITypeSignature Type { get; }
    bool IsVisibleInServerAggregates { get; }
    bool IsVisibleInClientAggregates { get; }
    bool IsVisibleInScripts { get; }
    bool IsVisibleInClientSide { get; }
    bool IsVisibleInMobileEmails { get; }
    bool IsVisibleInBusinessProcesses { get; }
}

public interface IBuiltinFunctionBinding : IByNameBinding {
    IBuiltinFunction Function { get; set; }
}

/// <summary>
/// Provides basic information about a built-in function.
/// More detailed information, including resolved types in the context of an eSpace, is provided by
/// <seealso cref="T:OutSystems.Model.Expressions.IBuiltinFunction" />, acessible via <seealso cref="P:OutSystems.Model.IESpace.BuiltinFunctions" />.
/// </summary>
public interface IBuiltinFunctionInfo {
    string Name { get; }
    IEnumerable<IInputParameterInfo> InputParameters { get; }
    OutSystems.Model.Types.TypeKind TypeKind { get; }
    string Description { get; }
    
    public interface IInputParameterInfo {
        string Name { get; }
        OutSystems.Model.Types.TypeKind TypeKind { get; }
        bool IsMandatory { get; }
        string Description { get; }
    }
}

public interface IBuiltinFunctions {
    IEnumerable<IBuiltinFunction> All { get; }
    IBuiltinFunction Abs { get; }
    IBuiltinFunction Mod { get; }
    IBuiltinFunction Power { get; }
    IBuiltinFunction Round { get; }
    IBuiltinFunction Sqrt { get; }
    IBuiltinFunction Trunc { get; }
    IBuiltinFunction Max { get; }
    IBuiltinFunction Min { get; }
    IBuiltinFunction Sign { get; }
    IBuiltinFunction Chr { get; }
    IBuiltinFunction Concat { get; }
    IBuiltinFunction EncodeHtml { get; }
    IBuiltinFunction EncodeJavaScript { get; }
    IBuiltinFunction EncodeSql { get; }
    IBuiltinFunction EncodeUrl { get; }
    IBuiltinFunction Index { get; }
    IBuiltinFunction Length { get; }
    IBuiltinFunction NewLine { get; }
    IBuiltinFunction Replace { get; }
    IBuiltinFunction Substr { get; }
    IBuiltinFunction ToLower { get; }
    IBuiltinFunction ToUpper { get; }
    IBuiltinFunction Trim { get; }
    IBuiltinFunction TrimEnd { get; }
    IBuiltinFunction TrimStart { get; }
    IBuiltinFunction AddDays { get; }
    IBuiltinFunction AddHours { get; }
    IBuiltinFunction AddMinutes { get; }
    IBuiltinFunction AddMonths { get; }
    IBuiltinFunction AddSeconds { get; }
    IBuiltinFunction AddYears { get; }
    IBuiltinFunction BuildDateTime { get; }
    IBuiltinFunction CurrDate { get; }
    IBuiltinFunction CurrDateTime { get; }
    IBuiltinFunction CurrTime { get; }
    IBuiltinFunction Day { get; }
    IBuiltinFunction DayOfWeek { get; }
    IBuiltinFunction DiffDays { get; }
    IBuiltinFunction DiffHours { get; }
    IBuiltinFunction DiffMinutes { get; }
    IBuiltinFunction DiffSeconds { get; }
    IBuiltinFunction Hour { get; }
    IBuiltinFunction Minute { get; }
    IBuiltinFunction Month { get; }
    IBuiltinFunction NewDate { get; }
    IBuiltinFunction NewDateTime { get; }
    IBuiltinFunction NewTime { get; }
    IBuiltinFunction Second { get; }
    IBuiltinFunction Year { get; }
    IBuiltinFunction BooleanToInteger { get; }
    IBuiltinFunction BooleanToText { get; }
    IBuiltinFunction DateTimeToDate { get; }
    IBuiltinFunction DateTimeToText { get; }
    IBuiltinFunction DateTimeToTime { get; }
    IBuiltinFunction DateToDateTime { get; }
    IBuiltinFunction DateToText { get; }
    IBuiltinFunction DecimalToBoolean { get; }
    IBuiltinFunction DecimalToInteger { get; }
    IBuiltinFunction DecimalToIntegerValidate { get; }
    IBuiltinFunction DecimalToLongInteger { get; }
    IBuiltinFunction DecimalToLongIntegerValidate { get; }
    IBuiltinFunction DecimalToText { get; }
    IBuiltinFunction LongIntegerToInteger { get; }
    IBuiltinFunction LongIntegerToIntegerValidate { get; }
    IBuiltinFunction LongIntegerToIdentifier { get; }
    IBuiltinFunction LongIntegerToText { get; }
    IBuiltinFunction IdentifierToInteger { get; }
    IBuiltinFunction IdentifierToLongInteger { get; }
    IBuiltinFunction IdentifierToText { get; }
    IBuiltinFunction IntegerToBoolean { get; }
    IBuiltinFunction IntegerToDecimal { get; }
    IBuiltinFunction IntegerToIdentifier { get; }
    IBuiltinFunction IntegerToText { get; }
    IBuiltinFunction NullDate { get; }
    IBuiltinFunction NullIdentifier { get; }
    IBuiltinFunction NullObject { get; }
    IBuiltinFunction NullBinary { get; }
    IBuiltinFunction NullFile { get; }
    IBuiltinFunction NullTextIdentifier { get; }
    IBuiltinFunction TextToDate { get; }
    IBuiltinFunction TextToDateTime { get; }
    IBuiltinFunction TextToDateTimeValidate { get; }
    IBuiltinFunction TextToDateValidate { get; }
    IBuiltinFunction TextToDecimal { get; }
    IBuiltinFunction TextToDecimalValidate { get; }
    IBuiltinFunction TextToIdentifier { get; }
    IBuiltinFunction TextToInteger { get; }
    IBuiltinFunction TextToLongInteger { get; }
    IBuiltinFunction TextToIntegerValidate { get; }
    IBuiltinFunction TextToLongIntegerValidate { get; }
    IBuiltinFunction TextToTime { get; }
    IBuiltinFunction TextToTimeValidate { get; }
    IBuiltinFunction TimeToText { get; }
    IBuiltinFunction ToObject { get; }
    IBuiltinFunction Size { get; }
    IBuiltinFunction MIMEType { get; }
    IBuiltinFunction IsTemp { get; }
    IBuiltinFunction IsOwnedByCurrentApp { get; }
    IBuiltinFunction FormatCurrency { get; }
    IBuiltinFunction FormatDecimal { get; }
    IBuiltinFunction FormatPercent { get; }
    IBuiltinFunction FormatPhoneNumber { get; }
    IBuiltinFunction FormatText { get; }
    IBuiltinFunction FormatDateTime { get; }
    IBuiltinFunction EmailAddressCreate { get; }
    IBuiltinFunction EmailAddressesConcatenate { get; }
    IBuiltinFunction EmailAddressValidate { get; }
    IBuiltinFunction GetApplicationServerType { get; }
    IBuiltinFunction GetCurrentLocale { get; }
    IBuiltinFunction GetDatabaseProvider { get; }
    IBuiltinFunction GetUserAgent { get; }
    IBuiltinFunction GetOwnerEspaceIdentifier { get; }
    IBuiltinFunction GetEntryEspaceName { get; }
    IBuiltinFunction GetEntryEspaceId { get; }
    IBuiltinFunction GetObsoleteTenantId { get; }
    IBuiltinFunction GetAppName { get; }
    IBuiltinFunction AddPersonalAreaToURLPath { get; }
    IBuiltinFunction GetBookmarkableURL { get; }
    IBuiltinFunction GetPersonalAreaName { get; }
    IBuiltinFunction GetOwnerURLPath { get; }
    IBuiltinFunction GetExceptionURL { get; }
    IBuiltinFunction GeneratePassword { get; }
    IBuiltinFunction If { get; }
    IBuiltinFunction IsLoadingScreen { get; }
    IBuiltinFunction CurrentThemeIsMobile { get; }
    IBuiltinFunction CheckRole { get; }
    IBuiltinFunction CheckEndUserRole { get; }
    IBuiltinFunction GetUserId { get; }
    IBuiltinFunction Get(string name);
}

public interface IByNameBinding : IBinding {
    string Name { get; set; }
}

public interface IElementOfTextWithReferenceElements {
}

/// <summary>
/// This interface represents an expression environment, i.e., the set of identifiers that can be used
/// when writing an expression.
/// 
/// The environment for an expression is object and property dependent: different (object,property) pairs
/// will have different entries. E.g., the environment in an assignment in a server action includes, among
/// others, "local" elements such as the action's input and output parameters, local variables, and outputs
/// from previous nodes, but also "global" elements such as site properties, session variables, and user
/// and builtin functions.
/// </summary>
public interface IEnvironment {
    /// <summary>
    /// The elements that exist in this environment
    /// </summary>
    IEnumerable<IEnvironmentEntry> Entries { get; }
    /// <summary>
    /// Lookup an environment entry by (compound) name. The name can be either a single identifier (e.g. UserId)
    /// or a compound identifier (e.g GetUsers.List.Current.User.Name).
    /// </summary>
    /// <param name="name">The name of the entry we're looking for</param>
    /// <param name="kind">The entry kind</param>
    /// <returns>An environment entry, or null if none is found with the provided name and kind</returns>
    IEnvironmentEntry Lookup(string name, EnvironmentEntryKind kind = EnvironmentEntryKind.Other);
}

/// <summary>
/// Represents an entry in an expression environment
/// </summary>
public interface IEnvironmentEntry {
    /// <summary>
    /// The entry name
    /// </summary>
    string Name { get; }
    /// <summary>
    /// The entry full name. It differs from the Name only when this is an entry whose root
    /// element is defined in the expression environment.
    /// </summary>
    string FullName { get; }
    /// <summary>
    /// The parent entry. Null if this entry exists as a direct child of the expression environment.
    /// </summary>
    IEnvironmentEntry Parent { get; }
    /// <summary>
    /// True if the entry represents a read-only element
    /// </summary>
    bool IsReadOnly { get; }
    /// <summary>
    /// True if the entry represents a built-in or user defined function.
    /// </summary>
    bool IsFunction { get; }
    /// <summary>
    /// The type of the entry. May be null for entries that are used only as "aggregator"
    /// entries, such as "Site." or "Session."
    /// </summary>
    OutSystems.Model.Types.ITypeSignature Type { get; }
    /// <summary>
    /// The model object associated with this entry. May be null for entries that are used only as "aggregator"
    /// entries, such as "Site." or "Session." or for runtime properties such as "List"
    /// </summary>
    OutSystems.Model.IObjectSignature Object { get; }
    /// <summary>
    /// The child entries. Non-empty whenever the current entry corresponds to a record type, in which
    /// case the children are the record attributes.
    /// </summary>
    IEnumerable<IEnvironmentEntry> Children { get; }
    string Description { get; }
    /// <summary>
    /// Lookup a sub-entry by (compound) name. The name can be either a single identifier (e.g. UserId)
    /// or a compound identifier (e.g Current.User.Name).
    /// </summary>
    /// <param name="name">The name of the entry we're looking for</param>
    /// <param name="kind">The entry kind</param>
    /// <returns>An environment entry, or null if none is found with the provided name and kind</returns>
    IEnvironmentEntry Lookup(string name, EnvironmentEntryKind kind);
}

public interface IExpression : OutSystems.Model.IObject {
    OutSystems.Model.Types.ITypeSignature Type { get; }
    /// <summary>
    /// Allows to manipulate the expression as text.
    /// 
    /// Note that setting the text of an expression may result in a new expression object
    /// being created, and the old one being deleted. Thus, do not assume that you can keep
    /// using an expression after you have changed its text. You must get the actual expression
    /// again from its parent.
    /// </summary>
    string Text { get; set; }
    /// <summary>
    /// The expanded text of the expression, in case the expression exists inside an aggregate and
    /// a user-defined function is being called in the expression.
    /// For all other cases returns the same value as <seealso cref="P:OutSystems.Model.Expressions.IExpression.Text" />
    /// </summary>
    string ExpandedText { get; }
    /// <summary>
    /// The environment at this expression
    /// </summary>
    IEnvironment Environment { get; }
    /// <summary>
    /// Returns the root of the expression syntactic tree. Use it no navigate on the expression
    /// on a structure way, i.e. without having to try to "interpret" its text.
    /// </summary>
    OutSystems.Model.Expressions.Nodes.IExpressionNode Root { get; }
    /// <summary>
    /// Returns the root of the expanded expression syntactic tree, in case the expression exists inside an aggregate and
    /// a user-defined function is being called in the expression.
    /// For all other cases returns the same value as <seealso cref="P:OutSystems.Model.Expressions.IExpression.Root" />
    /// </summary>
    OutSystems.Model.Expressions.Nodes.IExpressionNode ExpandedRoot { get; }
    /// <summary>
    /// Applies the provided substitutions on the expression and simplify it if possible.
    /// The entries in the dictionary must follow some rules:
    /// - the key must be a variable (input parameter, output parameter, local variable)
    /// - the value must be a non-null literal of the same type as the corresponding variable
    /// 
    /// As an example, suppose that we have a boolean input parameter named "ShowLogin" and an expression
    /// whose text is "ShowLogin or UserId = NullIdentifier()". The resulting expression when calling
    /// the SubstituteAndSimplify method with a mapping containing only ShowLogin will be:
    /// - if ShowLogin is mapped to True, then the expression becomes "True"
    /// - if ShowLogin is mapped to False, then the expression becomes "UserId = NullIdentifier()"
    /// </summary>
    /// <param name="substitutions">A mapping providing a non-null literal value for each
    /// variable that we want to substitute in the expression.</param>
    void SubstituteAndSimplify(Dictionary<OutSystems.Model.IObjectSignature, object> substitutions);
    IMutableExpression AsMutableExpression();
}

public interface IListLiteralExpression : IExpression {
    OutSystems.Model.ISequence<IListLiteralItem> Items { get; }
    IListLiteralItem CreateItem();
}

public interface IListLiteralItem : OutSystems.Model.IObject {
    IExpression Value { get; }
    void SetValue(ExpressionDefinition value);
}

public interface IMutableExpression : IExpression {
    void Commit();
    T CloneNode<T>(T node) where T: OutSystems.Model.Expressions.Nodes.IExpressionNode;
    OutSystems.Model.Expressions.Nodes.IArgument CreateArgument(OutSystems.Model.Expressions.Nodes.IExpressionNode value, IBinding parameter = null, bool hasName = false, string whitespaceBefore = null, string whitespaceBeforeColon = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IBinaryOperation CreateBinaryOperation(OutSystems.Model.Expressions.Nodes.BinaryOperator @operator, OutSystems.Model.Expressions.Nodes.IExpressionNode leftOperand, OutSystems.Model.Expressions.Nodes.IExpressionNode rightOperand, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IBooleanLiteral CreateBooleanLiteral(bool value, string whitespaceBefore = null, string whitespaceAfter = null);
    IByNameBinding CreateByNameBinding(string name);
    IBuiltinFunctionBinding CreateBuiltinFunctionBinding(IBuiltinFunction function);
    OutSystems.Model.Expressions.Nodes.IDateLiteral CreateDateLiteral(DateTime value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IDateTimeLiteral CreateDateTimeLiteral(DateTime value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IDecimalLiteral CreateDecimalLiteral(decimal value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IExpressionSnippet CreateExpressionSnippet(string value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IFieldAccess CreateFieldAccess(OutSystems.Model.Expressions.Nodes.IExpressionNode record, OutSystems.Model.Expressions.Nodes.IIdentifier field, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IFunctionCall CreateFunctionCall(IBinding function, IEnumerable<OutSystems.Model.Expressions.Nodes.IArgument> arguments, string whitespaceBefore = null, string whitespaceAfter = null, string whitespaceAfterFunctionName = null, string whitespaceBetweenParenthesis = null);
    OutSystems.Model.Expressions.Nodes.IIdentifier CreateIdentifier(IBinding binding, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IIntegerLiteral CreateIntegerLiteral(int value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IIndexer CreateIndexer(OutSystems.Model.Expressions.Nodes.IExpressionNode list, OutSystems.Model.Expressions.Nodes.IExpressionNode index, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.IListLiteral CreateListLiteral(IEnumerable<OutSystems.Model.Expressions.Nodes.IExpressionNode> items);
    OutSystems.Model.Expressions.Nodes.ILongLiteral CreateLongLiteral(long value, string whitespaceBefore = null, string whitespaceAfter = null);
    IObjectBinding CreateObjectBinding(OutSystems.Model.IObjectSignature @object);
    OutSystems.Model.Expressions.Nodes.IRecordLiteral CreateRecordLiteral(IEnumerable<OutSystems.Model.Expressions.Nodes.IRecordLiteralField> fields);
    OutSystems.Model.Expressions.Nodes.IRecordLiteralField CreateRecordLiteralField(OutSystems.Model.Types.IAttributeSignature attribute, OutSystems.Model.Expressions.Nodes.IExpressionNode value);
    OutSystems.Model.Expressions.Nodes.ITextLiteral CreateTextLiteral(string value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.ITimeLiteral CreateTimeLiteral(DateTime value, string whitespaceBefore = null, string whitespaceAfter = null);
    OutSystems.Model.Expressions.Nodes.ITypeConversion CreateTypeConversion(OutSystems.Model.Expressions.Nodes.IExpressionNode sourceValue, IEnumerable<OutSystems.Model.Expressions.Nodes.ITypeConversionMapping> mappings);
    OutSystems.Model.Expressions.Nodes.ITypeConversionMapping CreateTypeConversionMapping(OutSystems.Model.Expressions.Nodes.IExpressionNode sourceValue, OutSystems.Model.Expressions.Nodes.IExpressionNode targetValue);
    OutSystems.Model.Expressions.Nodes.IUnaryOperation CreateUnaryOperation(OutSystems.Model.Expressions.Nodes.UnaryOperator @operator, OutSystems.Model.Expressions.Nodes.IExpressionNode operand, string whitespaceBefore = null, string whitespaceAfter = null);
    IUnknownObjectBinding CreateUnknownObjectBinding(string name);
}

public interface IObjectBinding : IBinding {
    OutSystems.Model.IObjectSignature Object { get; set; }
}

public interface IRecordLiteralExpression : IExpression {
    OutSystems.Model.ISequence<IRecordLiteralField> Fields { get; }
    void RemoveIgnoredAttributes();
}

public interface IRecordLiteralField : OutSystems.Model.IObject {
    OutSystems.Model.Types.IAttributeSignature Attribute { get; }
    IExpression Value { get; }
    void SetValue(ExpressionDefinition value);
}

public interface IReferenceElement : IElementOfTextWithReferenceElements {
    OutSystems.Model.IObjectSignature Object { get; }
}

public interface ITextBasedExpression : IExpression {
    /// <summary>
    /// Replaces all occurrences of <paramref name="oldFunction" /> by <paramref name="newFunction" />.
    /// </summary>
    /// <param name="oldFunction"></param>
    /// <param name="newFunction"></param>
    void ReplaceBuiltinFunction(IBuiltinFunction oldFunction, IBuiltinFunction newFunction);
}

public interface ITextElement : IElementOfTextWithReferenceElements {
    string Text { get; }
}

public interface ITextWithReferencedElements : OutSystems.Model.IObject {
    /// <summary>
    /// Returns the expression as Text
    /// </summary>
    string Text { get; }
    IEnumerable<IElementOfTextWithReferenceElements> Elements { get; }
}

public interface ITypeConversionExpression : IExpression {
    /// <summary>
    /// The value that we are converting
    /// </summary>
    IExpression SourceValue { get; }
    /// <summary>
    /// A list with expressions for each of the target type's attributes
    /// </summary>
    IEnumerable<IAttributeMapping> Mappings { get; }
    bool IsListConversion { get; }
    OutSystems.Model.Types.ITypeSignature SourceElementType { get; }
    OutSystems.Model.Types.ITypeSignature TargetElementType { get; }
    bool UseImplicitConversion { get; }
    void SetSourceValue(ExpressionDefinition value);
}

public interface IUnknownObjectBinding : IBinding {
    string Name { get; set; }
}

