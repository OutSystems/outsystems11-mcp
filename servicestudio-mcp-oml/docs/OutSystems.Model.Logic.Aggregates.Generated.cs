// Aggregate query definitions (logical representation of data retrieval with filters, sorts, and groups)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Aggregates;

public interface IAggregate : OutSystems.Model.IObject {
    Nullable<int> CacheInMinutes { get; set; }
    string Description { get; set; }
    bool IsClientSide { get; }
    OutSystems.Model.Expressions.IExpression MaxRecords { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression StartIndex { get; }
    Nullable<int> Timeout { get; set; }
    OutSystems.Model.Types.ITypeSignature Type { get; }
    OutSystems.Model.ISequence<IImplicitParameter> ImplicitParameters { get; }
    OutSystems.Model.Logic.Aggregates.Full.IFullAggregate AsFullAggregate { get; }
    OutSystems.Model.Logic.Aggregates.Database.IDatabaseAggregate AsDatabaseAggregate { get; }
    void SetMaxRecords(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetStartIndex(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IAggregatedAttribute : OutSystems.Model.IObject {
    OutSystems.Model.Enumerations.AggregationType AggregationType { get; set; }
    OutSystems.Model.Expressions.IExpression Attribute { get; }
    OutSystems.Model.Types.ITypeSignature DataType { get; }
    string Name { get; set; }
    void SetAttribute(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ICalculatedAttribute : OutSystems.Model.IObject {
    OutSystems.Model.Types.ITypeSignature DataType { get; }
    string Name { get; set; }
    OutSystems.Model.Expressions.IExpression Value { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IDataSource : OutSystems.Model.IModelObject {
}

public interface IFilter : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Condition { get; }
    void SetCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IGroupByAttribute : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Attribute { get; }
    string Name { get; set; }
    void SetAttribute(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IImplicitParameter : OutSystems.Model.IObject {
    string InferredName { get; }
    OutSystems.Model.Expressions.IExpression TestValue { get; }
    OutSystems.Model.Types.ITypeSignature Type { get; }
    OutSystems.Model.Expressions.IExpression Value { get; }
    void SetTestValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface ISort : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Attribute { get; }
    bool IsDynamic { get; set; }
    OutSystems.Model.Enumerations.Sort SortDirection { get; set; }
    void SetAttribute(OutSystems.Model.Expressions.ExpressionDefinition value);
}

