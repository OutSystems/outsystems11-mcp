// Factory utilities for creating, transforming, and cloning model objects via definitions
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Factory;

public static class FactoryExtensions {
    /// <summary>
    /// Transforms <paramref name="obj" /> according to definition <paramref name="newDefinition" />
    /// </summary>
    /// <param name="obj">The object to transform</param>
    /// <param name="newDefinition">New definition</param>
    /// <param name="objResolver">Resolver for objects referenced (by key) in the child definition</param>
    /// <param name="keysMustBeRespected">If true, then the keys provided in newDefinition will be 100%
    /// respected, even if that means deleting and recreating some objects</param>
    /// <returns>The transformed object. Note that a different object is returned whenever the new definition
    /// specifies a different type than the type of <paramref name="obj" /></returns>
    public static OutSystems.Model.IModelObject Transform(this OutSystems.Model.IModelObject obj, ModelObjectDefinition newDefinition, IObjectResolver objResolver, bool keysMustBeRespected = true) => default;
    /// <summary>
    /// Creates a new child object in <paramref name="parent" /> using
    /// definition <paramref name="childDefinition" />.
    /// </summary>
    /// <param name="parent">The object where to create the new child</param>
    /// <param name="childDefinition">Definition for the child</param>
    /// <param name="objResolver">Resolver for objects referenced (by key) in the child definition</param>
    /// <returns>The new child</returns>
    public static OutSystems.Model.IModelObject CreateChild(this OutSystems.Model.IModelObject parent, ModelObjectDefinition childDefinition, IObjectResolver objResolver) => default;
    /// <summary>
    /// Creates a detached object in <paramref name="parent" /> using definition <paramref name="childDefinition" />.
    /// The detached child knows its parent, but the parent doesn't know the detached child. Since the detached child
    /// is not stored in the parent's collections, it will be garbage collected automatically.
    /// </summary>
    /// <param name="parent">The object where to create the new child</param>
    /// <param name="childDefinition">Definition for the child</param>
    /// <param name="objResolver">Resolver for objects referenced (by key) in the child definition</param>
    /// <returns>A new detached object</returns>
    public static OutSystems.Model.IModelObject CreateDetachedChild(this OutSystems.Model.IModelObject parent, ModelObjectDefinition childDefinition, IObjectResolver objResolver) => default;
    /// <summary>
    /// Creates or transforms a set of objects in an eSpace in a single shot operation.
    /// </summary>
    /// <param name="eSpace">The eSpace where the objects are to be created or transformed</param>
    /// <param name="objResolver"></param>
    /// <param name="specs">A list of triplets specifying a target (parent) object, an optional existing object,
    /// and the specification of an object to be created (if the existing object is null) or transformed (if the
    /// existing object is set)</param>
    public static IEnumerable<OutSystems.Model.IModelObject> CreateOrTransformObjects(this OutSystems.Model.IESpace eSpace, IObjectResolver objResolver, IEnumerable<ValueTuple<OutSystems.Model.IObjectSignature, OutSystems.Model.IObjectSignature, ModelObjectDefinition>> specs) => default;
    /// <summary>
    /// Creates or transforms a set of objects in an eSpace in a single shot operation.
    /// </summary>
    /// <param name="eSpace">The eSpace where the objects are to be created or transformed</param>
    /// <param name="objResolver"></param>
    /// <param name="specs">A list of triplets specifying a target (parent) object, an optional existing object,
    /// and the specification of an object to be created (if the existing object is null) or transformed (if the
    /// existing object is set)</param>
    public static IEnumerable<OutSystems.Model.IModelObject> CreateOrTransformObjects(this OutSystems.Model.IESpace eSpace, IObjectResolver objResolver, ValueTuple<OutSystems.Model.IObjectSignature, OutSystems.Model.IObjectSignature, ModelObjectDefinition>[] specs) => default;
    public static ModelObjectDefinition ToModelObjectDefinition(this OutSystems.Model.IModelObject obj, ModelObjectDefinition parent = null, bool useUniqueIdForAnonymousTypes = false, bool useReadableIdentifiersInsteadOfKeys = false, bool useDeterministicOrder = false) => default;
    public static T CreateDetachedClone<T>(this T obj, IObjectResolver objResolver = null) where T: OutSystems.Model.IModelObject => default;
}

public interface IObjectResolver {
    bool TryLookup(OutSystems.Model.IKey key, ref OutSystems.Model.IModelObject obj);
    bool TryLookup(OutSystems.Model.IGlobalKey key, ref OutSystems.Model.IModelObject obj);
    IEnumerable<OutSystems.Model.IModelObject> Lookup(string name);
}

public class ModelObjectDefinition {
    public Type ObjectType { get; }
    public OutSystems.Model.IKey Key { get; set; }
    public ModelObjectDefinition Parent { get; }
}

public static class ModelObjectDefinitionExtensions {
    public static string Serialize(this ModelObjectDefinition def, Func<OutSystems.Model.IKey, string> serializeKey = null, Func<OutSystems.Model.IGlobalKey, string> serializeGlobalKey = null) => default;
    public static string Serialize(this IEnumerable<ModelObjectDefinition> defs, Func<OutSystems.Model.IKey, string> serializeKey = null, Func<OutSystems.Model.IGlobalKey, string> serializeGlobalKey = null) => default;
    public static void WriteProperty(this System.Text.Json.Utf8JsonWriter writer, string name, object value, Func<OutSystems.Model.IKey, string> serializeKey, Func<OutSystems.Model.IGlobalKey, string> serializeGlobalKey) {}
}

