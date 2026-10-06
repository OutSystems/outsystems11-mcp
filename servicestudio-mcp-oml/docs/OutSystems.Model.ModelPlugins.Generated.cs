// Model plugin extension points (registration, lifecycle hooks, and custom artifact providers)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.ModelPlugins;

public interface IModelPlugin : OutSystems.Model.Applications.IModule {
    string AssemblyName { get; set; }
    Version AssemblyVersion { get; set; }
    string ReactModuleName { get; set; }
    string ReactBundleName { get; set; }
    string StylesheetModuleName { get; set; }
    IEnumerable<IWidgetDefinition> WidgetDefinitions { get; }
    IEnumerable<OutSystems.Model.ModelPlugins.UI.Mobile.ITheme> Themes { get; }
    byte[] Icon { get; set; }
    Guid SignatureHash { get; }
    byte[] SignatureBinary { get; }
    IWidgetDefinition CreateWidgetDefinition(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.ModelPlugins.UI.Mobile.ITheme CreateTheme(string name = null, OutSystems.Model.IKey key = null);
}

public interface IModelPluginObject : OutSystems.Model.IModelObject {
    IModelPlugin GetPlugin();
}

public interface IModelPluginUpdateInformation {
    Version NewVersion { get; }
    string NewReferenceRevision { get; set; }
    string NewReferenceVersion { get; set; }
    string NewSpecificVersionHash { get; set; }
    string DeveloperId { get; set; }
}

public interface IRequiredModelPlugin : OutSystems.Model.IObject {
    string AssemblyName { get; }
    Version AssemblyVersion { get; }
    OutSystems.Model.IKey PluginKey { get; }
    Guid SpecificVersionHash { get; }
    Guid UniqueId { get; }
}

public interface IShareableModelPluginObject : OutSystems.Model.IShareable, IModelPluginObject {
}

public interface IShareableModelPluginObject<ConcreteT, SignatureT> : IShareableModelPluginObject where ConcreteT: IModelPluginObject where SignatureT: OutSystems.Model.IObjectSignature {
}

public interface IWidgetDefinition : IShareableModelPluginObject<IWidgetDefinition, OutSystems.Model.UI.IWidgetDefinitionSignature> {
    string Name { get; set; }
    string Description { get; set; }
    bool HasValidationProperties { get; set; }
    bool IsValidationAggregator { get; set; }
    OutSystems.Model.ISequence<IWidgetDefinitionProperty> Properties { get; }
    IEnumerable<IWidgetDefinitionPlaceholder> Placeholders { get; }
    IEnumerable<IWidgetDefinitionRequiredResource> RequiredResources { get; }
    IWidgetDefinitionProperty CreateProperty(string name = null, OutSystems.Model.IKey key = null);
    IWidgetDefinitionPlaceholder CreatePlaceholder(string name = null, OutSystems.Model.IKey key = null);
}

public interface IWidgetDefinitionPlaceholder : IModelPluginObject {
    string Description { get; set; }
    string Name { get; set; }
    bool IsIterated { get; set; }
    string SourceProperty { get; set; }
}

public interface IWidgetDefinitionProperty : IModelPluginObject {
    string Name { get; set; }
    string Description { get; set; }
    OutSystems.Model.Enumerations.ModelPlugins.ModelPropertyKind PropertyKind { get; set; }
    OutSystems.Model.Enumerations.ModelPlugins.LambdaExpressionTarget LambdaExpressionTarget { get; set; }
    string LambdaExpressionSourceProperty { get; set; }
    string DefaultValue { get; set; }
    bool IsMandatory { get; set; }
    OutSystems.Model.Enumerations.ModelPlugins.PropertyRuntimeType RuntimeType { get; set; }
}

public interface IWidgetDefinitionRequiredResource : IModelPluginObject {
    string FileName { get; }
}

public class ModelPluginAttribute : Attribute {
    /// <summary>
    /// Returns true if this plugin is distributed with ODC Studio and thus is always present,
    /// and false if this is a plugin that must be loaded dynamically if required by a particular
    /// module.
    /// </summary>
    public bool IsBundledWithTheIDE { get; set; }
    public string ReactModuleName { get; set; }
    public string ReactBundleName { get; set; }
    public string StylesheetModuleName { get; set; }
    public string IconResourceName { get; set; }
    public string Description { get; set; }
}

/// <summary>
/// This class simplifies the process of keeping a dictionary where for a given key we may have multiple values, one
/// for each model plugin.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class ModelPluginAwareDictionary<TKey, TValue> {
    public IEnumerable<TValue> Values { get; }
    public void Add(TKey key, TValue value) {}
    public TValue GetValue(TKey key, IEnumerable<IRequiredModelPlugin> allowedPlugins) => default;
    public TValue GetValue(TKey key, IEnumerable<ValueTuple<string, Version>> allowedPlugins) => default;
    public bool TryGetValue(TKey key, IEnumerable<IRequiredModelPlugin> allowedPlugins, ref TValue value) => default;
    public bool TryGetValue(TKey key, IEnumerable<ValueTuple<string, Version>> allowedPlugins, ref TValue value) => default;
    public void Remove(TKey key) {}
    public IEnumerable<TValue> GetValues(IEnumerable<IRequiredModelPlugin> allowedPlugins) => default;
    public IEnumerable<TValue> GetValues(IEnumerable<ValueTuple<string, Version>> allowedPlugins) => default;
    public IEnumerable<TValue> GetValues(string assemblyName, Version assemblyVersion) => default;
}

public class ModelPluginInformation {
    public OutSystems.Model.IKey PluginKey { get; }
    public string AssemblyName { get; }
    public Version AssemblyVersion { get; }
    public Guid SpecificVersionHash { get; }
}

public class ModelPluginUpdateInformation : IModelPluginUpdateInformation {
    public Version NewVersion { get; }
    public string NewReferenceRevision { get; set; }
    public string NewReferenceVersion { get; set; }
    public string DeveloperId { get; set; }
    public string NewSpecificVersionHash { get; set; }
}

public class ResolveModelPluginEventArgs : EventArgs {
    public ModelPluginInformation PluginInformation { get; }
}

