// Database-layer specific extensions and optimizations for aggregate queries
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Aggregates.Database;

public interface IAliasedSource : OutSystems.Model.IObject {
    string Name { get; set; }
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

public interface IDatabaseAggregate : OutSystems.Model.Logic.Aggregates.IAggregate {
    IAliasedSource MasterSource { get; }
    IEnumerable<IAliasedSource> Sources { get; }
    OutSystems.Model.ISequence<IJoin> Joins { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.IFilter> Filters { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.IFilter> FiltersInGroupBy { get; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.ICalculatedAttribute> CalculatedAttributes { get; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.ICalculatedAttribute> CalculatedAttributesInGroupBy { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.ISort> Sorts { get; }
    OutSystems.Model.ISequence<OutSystems.Model.Logic.Aggregates.ISort> SortsInGroupBy { get; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.IGroupByAttribute> GroupByAttributes { get; }
    IEnumerable<OutSystems.Model.Logic.Aggregates.IAggregatedAttribute> AggregatedAttributes { get; }
    IAliasedSource CreateSource(OutSystems.Model.Logic.Aggregates.IDataSource dataSource, string name = null, OutSystems.Model.IKey key = null);
    IJoin CreateJoin(OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IFilter CreateFilter(string condition = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IFilter CreateFilterInGroupBy(string condition = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.ICalculatedAttribute CreateCalculatedAttribute(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.ICalculatedAttribute CreateCalculatedAttributeInGroupBy(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.ISort CreateSort(OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.ISort CreateSortInGroupBy(OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IGroupByAttribute CreateGroupByAttribute(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.Logic.Aggregates.IAggregatedAttribute CreateAggregatedAttribute(string name = null, OutSystems.Model.IKey key = null);
}

public interface IJoin : OutSystems.Model.IObject {
    /// <summary>
    /// Boolean literal or expression to decide which content is displayed.
    /// </summary>
    OutSystems.Model.Expressions.IExpression Condition { get; }
    /// <summary>
    /// How the two entities are related in this (join) condition.
    /// </summary>
    OutSystems.Model.Enumerations.JoinType JoinType { get; set; }
    IAliasedSource LeftSource { get; set; }
    IAliasedSource RightSource { get; set; }
    void SetCondition(string value);
}

