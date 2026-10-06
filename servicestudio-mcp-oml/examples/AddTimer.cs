// Create a timer named SyncDataOnPublish that runs the SyncData server action when the module is published
// Context: The module has a server action named SyncData

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var syncDataOnPublish = eSpace.CreateTimer("SyncDataOnPublish");
    syncDataOnPublish.Action = eSpace.ServerActions.Named("SyncData");
    syncDataOnPublish.Description = "Synchronizes data when the module is published.";
    syncDataOnPublish.Schedule = "WhenPublished";
    syncDataOnPublish.TimeoutInMinutes = 20;
    syncDataOnPublish.Priority = OutSystems.Model.Enumerations.Priority.Normal;
}
