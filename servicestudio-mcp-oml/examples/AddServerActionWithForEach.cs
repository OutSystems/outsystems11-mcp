// Create a server action LoadSampleOrders that deserializes a JSON resource into a list of Order records and creates each one in a for-each loop, with an exception handler to log errors without aborting
// Context: The module has a server entity named Order, a JSON resource named Orders_json accessible via Resources.Orders_json.Content, and a folder named SampleData

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var loadSampleOrders = eSpace.CreateServerAction("LoadSampleOrders");
    loadSampleOrders.Description = "Load sample Order data from a JSON resource.";
    loadSampleOrders.Folder = eSpace.Folders.Named("SampleData");

    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");

    var startNode = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var jsonDeserializeOrderList = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IJSONDeserializeNode>("JSONDeserializeOrderList").ConnectedBelow(startNode);
    jsonDeserializeOrderList.DataType = eSpace.GetOrCreateListType(order);
    jsonDeserializeOrderList.SetJSONString("BinaryDataToText(Resources.Orders_json.Content)");

    var forEachNode = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IForEachNode>().ConnectedBelow(jsonDeserializeOrderList);
    forEachNode.SetRecordList("JSONDeserializeOrderList.Data");

    var createOrder = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateOrder").ToTheRightOf(forEachNode);
    createOrder.Action = order.CreateAction;
    createOrder.SetArgumentValue(order.CreateAction.InputParameters.Named("Source"), "JSONDeserializeOrderList.Data.Current");
    createOrder.Target = forEachNode;
    forEachNode.CycleTarget = createOrder;

    var endNode = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().Below(forEachNode);
    forEachNode.Target = endNode;

    var allExceptions = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IExceptionHandlerNode>("AllExceptions");
    allExceptions.Exception = eSpace.AllExceptions;
    allExceptions.AbortTransaction = false;

    var endNode2 = loadSampleOrders.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(allExceptions);
}
