// Add a decision node to MyProcess workflow, with True as its condition
// Context: MyProcess workflow consists of a Start node connected to an End node

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var start = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>().Named("Start");
    var decision = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IDecisionNode>("Decision").ConnectedBelow(start);
    decision.Label = "Decision";

    var outcome = decision.CreateOutcome();
    outcome.Label = "Outcome1";
    var endNode = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().At(3196, 2573);
    outcome.Target = endNode;
    outcome.SetValue("True");

    var outcome2 = decision.CreateOutcome();
    outcome2.Label = "Outcome2";

    var endNode2 = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().ToTheRightOf(decision);
    endNode2.Label = "End";
    outcome2.Target = endNode2;
    endNode.VerticalPosition += 1759;
}
