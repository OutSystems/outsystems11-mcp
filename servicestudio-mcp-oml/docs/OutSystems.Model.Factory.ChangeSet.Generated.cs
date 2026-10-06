// ChangeSet descriptor extensions for additional child collections and properties in model object definitions
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Factory.ChangeSet;

/// <summary>
/// Some Model API interfaces expose extra child collections e.g. Widgets collections exposing single placeholder Widgets children
/// that are not part of the class descriptor itself but can be used on ModelObjectDefinition operations
/// </summary>
public interface IDescriptorThatSupportsAdditionalChildCollections {
}

/// <summary>
/// Some Model API interfaces expose extra properties e.g. IAliasedSource Source property
/// that are not part of the class descriptor itself but can be used on ModelObjectDefinition operations
/// </summary>
public interface IDescriptorThatSupportsAdditionalProperties {
}

