// Full-feature aggregate constructs (including complex joins and advanced filtering)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Aggregates.Full;

public interface IAddSourceOperation : IOperation {
    string Name { get; }
    OutSystems.Model.Logic.Aggregates.IDataSource Source { get; set; }
    ICollection<OutSystems.Model.Types.IAttributeSignature> AttributesInPreview { get; }
    /// <summary>
    /// Sets the visibility of an attribute in the preview.
    /// </summary>
    /// <param name="attribute">The attribute to show or hide</param>
    /// <param name="isVisible">True to show the attribute, false to hide it</param>
    void SetAttributeVisibility(OutSystems.Model.Types.IAttributeSignature attribute, bool isVisible);
    /// <summary>
    /// Gets the visibility state of an attribute in the preview.
    /// </summary>
    /// <param name="attribute">The attribute to check</param>
    /// <returns>True if the attribute is visible, false otherwise</returns>
    bool GetAttributeVisibility(OutSystems.Model.Types.IAttributeSignature attribute);
    /// <summary>
    /// Sets the display order of an attribute in the preview.
    /// Lower values appear first.
    /// </summary>
    /// <param name="attribute">The attribute to reorder</param>
    /// <param name="displayOrder">The display order value</param>
    void SetAttributeDisplayOrder(OutSystems.Model.Types.IAttributeSignature attribute, int displayOrder);
    /// <summary>
    /// Gets the display order of an attribute in the preview.
    /// </summary>
    /// <param name="attribute">The attribute to check</param>
    /// <returns>The display order value</returns>
    int GetAttributeDisplayOrder(OutSystems.Model.Types.IAttributeSignature attribute);
}

public interface IAliasedSource : OutSystems.Model.IObject {
    string Name { get; set; }
    IOperation SourceOperation { get; set; }
}

public interface ICombineSourcesOperation : IOperation {
    string Name { get; set; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.ICalculatedAttribute> CalculatedAttributes { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.IFilter> Filters { get; }
    OutSystems.Model.ISequence<IJoin> Joins { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.ISort> Sorts { get; }
    IEnumerable<IAliasedSource> Sources { get; }
    OutSystems.Model.Logic.Aggregates.ICalculatedAttribute CreateCalculatedAttribute(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IFilter CreateFilter(OutSystems.Model.IKey key = null);
    IJoin CreateJoin(OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.ISort CreateSort(OutSystems.Model.IKey key = null);
    IAliasedSource CreateSource(string name = null, OutSystems.Model.IKey key = null);
    IAliasedSource CreateSource(OutSystems.Model.Logic.Aggregates.IDataSource dataSource, string name = null);
}

public interface IFullAggregate : OutSystems.Model.Logic.Aggregates.IAggregate {
    IOperation RootOperation { get; set; }
    IEnumerable<IOperation> Operations { get; }
    IAddSourceOperation CreateAddSourceOperation(OutSystems.Model.Logic.Aggregates.IDataSource dataSource);
    ICombineSourcesOperation CreateCombineSourcesOperation();
    IGroupByOperation CreateGroupByOperation();
}

public interface IGroupByOperation : IOperation {
    IOperation SourceOperation { get; set; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.IAggregatedAttribute> AggregatedAttributes { get; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.IGroupByAttribute> GroupByAttributes { get; }
    OutSystems.Model.Logic.Aggregates.IAggregatedAttribute CreateAggregatedAttribute(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IGroupByAttribute CreateGroupByAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IJoin : OutSystems.Model.IObject {
    OutSystems.Model.Expressions.IExpression Condition { get; }
    OutSystems.Model.Enumerations.JoinType JoinType { get; set; }
    IAliasedSource LeftSource { get; set; }
    IAliasedSource RightSource { get; set; }
    void SetCondition(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IOperation : OutSystems.Model.IObject {
}

