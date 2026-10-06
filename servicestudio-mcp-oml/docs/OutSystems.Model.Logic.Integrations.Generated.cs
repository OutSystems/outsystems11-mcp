// Integration artifacts (generic connectors, authentication configs, and endpoint definitions)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Integrations;

public interface IClient : IIntegration {
    IEnumerable<IConsumedAction> Actions { get; }
    IEnumerable<OutSystems.Model.Data.IStaticEntity> StaticEntities { get; }
}

public interface IConsumedAction : OutSystems.Model.Logic.IActionSignature {
    Nullable<int> CacheInMinutes { get; set; }
    string CreatedBy { get; set; }
    string Description { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
    OutSystems.Model.IOutputParameter CreateOutputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IExposedAction : OutSystems.Model.Logic.IAction {
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface IIntegration : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    string Description { get; set; }
    byte[] Icon { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    string Name { get; set; }
    IEnumerable<IPluginCallback> Callbacks { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    IEnumerable<OutSystems.Model.Data.IStructure> Structures { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface IPluginCallback : OutSystems.Model.Logic.IAction {
    OutSystems.Model.ISequence<OutSystems.Model.IExtendedProperty> ExtendedProperties { get; }
    OutSystems.Model.IExtendedProperty CreateExtendedProperty();
}

public interface IService : IIntegration {
    IEnumerable<IExposedAction> Actions { get; }
}

