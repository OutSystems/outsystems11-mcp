// Expose a REST API OrdersAPI with a GET method GetOrderById that receives an Id in the URL and returns the Order record
// Context: The module has a server entity named Order. Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");

    var ordersAPI = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("OrdersAPI");

    var getOrderById = ordersAPI.CreateAction("GetOrderById");
    // REST methods are created WITH a Start and an End (server actions are not) - delete them before
    // building the flow, or TrueChange reports "More than one Start found".
    getOrderById.Nodes.ToList().ForEach(x => x.Delete());
    getOrderById.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;

    var id = getOrderById.CreateInputParameter("Id"); // ReceiveIn defaults to URL
    id.DataType = eSpace.LongIntegerType;
    var result = getOrderById.CreateOutputParameter("Order");
    result.DataType = order;

    var startNode = getOrderById.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();
    var getOrder = getOrderById.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("GetOrder").ConnectedBelow(startNode);
    getOrder.Action = order.GetAction; // auto-creates the argument - set it, never add a second one
    getOrder.SetArgumentValue(order.GetAction.InputParameters.Single(), "LongIntegerToIdentifier(Id)");
    var assign = getOrderById.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(getOrder);
    assign.CreateAssignment("Order", "GetOrder.Record.Order"); // the Get<Entity> output is named Record
    getOrderById.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assign);
}
