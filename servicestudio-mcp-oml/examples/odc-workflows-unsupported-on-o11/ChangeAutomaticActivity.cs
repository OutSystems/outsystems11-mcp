// Change the value of the automatic activity argument of a workflow to 456
// Context: MyProcess workflow consists of a Start, AutomaticActivity, and End node connected together. AutomaticActivy is calling a service action with a single parameter

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var automaticActivity = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IAutomaticActivityNode>().Named("AutomaticActivity");
    var producer = eSpace.References.Named("Producer");
    var myAction = producer.ServiceActions.Named("MyAction");
    var myParam = myAction.InputParameters.Named("MyParam");
    automaticActivity.SetArgumentValue(myParam, "456");
}
