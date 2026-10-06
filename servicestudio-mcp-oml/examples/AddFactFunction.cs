// Add an integer input to the Math screen, and display the factorial of the number written in the input
// Context: The module contains a screen named Math which is currently empty

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var fact = eSpace.CreateClientAction("Fact");
    fact.Function = true;

    var n = fact.CreateInputParameter("n");
    n.DataType = eSpace.IntegerType;

    var startNode = fact.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var ifNode = fact.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(startNode);
    ifNode.SetCondition("n <= 1");

    var assignNode = fact.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode);
    assignNode.Target = ifNode;

    var assignment = assignNode.CreateAssignment("Result", "Result * n");

    var assignment2 = assignNode.CreateAssignment("n", "n - 1");
    ifNode.FalseTarget = assignNode;

    var endNode = fact.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().Below(ifNode);
    ifNode.TrueTarget = endNode;

    var result = fact.CreateOutputParameter("Result");
    result.DataType = eSpace.IntegerType;
    result.SetDefaultValue("1");

    var main = eSpace.MobileFlows.Named("Main");
    var math = main.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("Math");
    var input_n = math.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_n");
    input_n.SetEnabled("True");
    input_n.InputType = ServiceStudio.Plugin.NRWidgets.Enumerations.InputType.Number;
    input_n.SetMandatory("False");
    input_n.MaxLength = null;
    input_n.SetVariable("n");
    var builtinEvent = input_n.OnChange;

    var n2 = math.CreateLocalVariable("n");
    n2.DataType = eSpace.IntegerType;

    var expression = math.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression.SetValue("Fact(n)");
}
