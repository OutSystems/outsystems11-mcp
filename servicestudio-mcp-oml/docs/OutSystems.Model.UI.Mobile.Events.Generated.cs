// Mobile-specific UI event sources and handlers (lifecycle, device, gesture events)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.UI.Mobile.Events;

public interface IBuiltinEvent : OutSystems.Model.IObject {
    OutSystems.Model.Enumerations.ValidationBehavior BuiltInValidations { get; set; }
    OutSystems.Model.IObjectSignature Destination { get; set; }
    OutSystems.Model.Enumerations.TransitionTypeWithInheritance Transition { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
}

public interface IEvent : OutSystems.Model.IObject {
    string Event { get; set; }
    OutSystems.Model.IObject Handler { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IOnSyncCompleteEvent : ISynchronizationEvent {
}

public interface IOnSyncErrorEvent : ISynchronizationEvent {
}

public interface IOnSyncStartEvent : ISynchronizationEvent {
}

public interface ISynchronizationEvent : IUILifeCycleEvent {
    OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
}

public interface IUILifeCycleEvent : OutSystems.Model.IObject {
    OutSystems.Model.IObjectSignature Destination { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

