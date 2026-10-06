// Apply the following changes in the human activity in MyProcess workflow:  - close the activity when the following condition is met: 3 = 4  - change the argument to the MyAction call to be 456  - include the Employee role as one of the roles to which the activity may be assigned to  - associate the activity with the ReviewScreen screen  - timeout the activiy after two days
// Context: MyProcess workflow consists of a Start, HumanActivity, and End nodes connected together. The module has dependencies to the following elements in the Producer module:  - MyEvent event  - MyAction service action. MyAction has a single parameter  - ApproveScreen and ReviewScreen screens  - Manager and Employee roles

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var producer = eSpace.References.Named("Producer");
    var uIFlow1 = producer.MobileFlows.Named("UIFlow1");
    var reviewScreen = uIFlow1.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreenSignature>().Named("ReviewScreen");
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var humanActivity = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IHumanActivityNode>().Named("HumanActivity");
    humanActivity.DestinationScreen = reviewScreen;
    humanActivity.SetTimeoutCustomTime("AddDays(CurrDate(), 2)");
    humanActivity.TimeoutMode = OutSystems.Model.Enumerations.TimeoutMode.Custom;
    var activityDataItem = humanActivity.ActivityData.Single();
    var myAction = producer.ServiceActions.Named("MyAction");
    var myParam = myAction.InputParameters.Named("MyParam");
    activityDataItem.SetArgumentValue(myParam, "456");

    var eventCondition = humanActivity.CreateCloseOnCondition("3 = 4");
    var employee = producer.Roles.Named("Employee");
    humanActivity.Roles.Add(employee);
}
