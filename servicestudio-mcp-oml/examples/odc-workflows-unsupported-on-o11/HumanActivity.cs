// Add a human activity node to MyProcess workflow. The activity should be assigned to the Manager role.  Associate the activity with the ApproveScreen screen.  The activity should start when the following condition is met: 1 = 2.  When starting the activity the MyAction service action must be called, with 123 as an argument.  The activity should be closed when the MyEvent event is raised.
// Context: MyProcess workflow consists of a Start node connected to an End node. The module has dependencies to the following elements in the Producer module:  - MyEvent event  - MyAction service action. MyAction has a single parameter  - ApproveScreen screen  - Manager and Employee roles

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var start = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>().Named("Start");
    var humanActivity = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IHumanActivityNode>("HumanActivity").ConnectedBelow(start);
    humanActivity.GetESpace().Roles.ToList().ForEach(x => x.Delete());
    var producer = eSpace.References.Named("Producer");
    var manager = producer.Roles.Named("Manager");
    humanActivity.Roles.Add(manager);
    humanActivity.AssignMode = OutSystems.Model.Enumerations.AssignMode.Roles;
    var myEvent = producer.Events.Named("MyEvent");
    humanActivity.CloseOn = myEvent;
    var uIFlow1 = producer.MobileFlows.Named("UIFlow1");
    var approveScreen = uIFlow1.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreenSignature>().Named("ApproveScreen");
    humanActivity.DestinationScreen = approveScreen;
    humanActivity.Label = "Human Activity";
    var endNode = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().Single();
    humanActivity.Target = endNode;

    var myAction = producer.ServiceActions.Named("MyAction");
    var activityDataItem = humanActivity.AddAction(myAction);
    var myParam = myAction.InputParameters.Named("MyParam");
    activityDataItem.SetArgumentValue(myParam, "123");

    var eventCondition = humanActivity.CreateOpenOnCondition("1 = 2");
    endNode.VerticalPosition += 1759;
}
