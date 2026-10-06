// Create new server action named B, with just a Start and End nodes
// Context: Module has a single server action named A

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var b = eSpace.CreateServerAction("B");

    var startNode = b.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var endNode = b.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(startNode);
}
