// Create a server action OrderApprove that takes an Order record, fetches it with a calculated permission check, raises an exception if the transition is not allowed, sets the status to Approved, and updates the record
// Context: The module has a server entity named Order with an OrderStatusId attribute, a static entity named OrderStatus with an Approved record, a user exception named InvalidStatusTransition, a server action named OrderCanApprove, and a folder named Order

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var orderApprove = eSpace.CreateServerAction("OrderApprove");
    orderApprove.Folder = eSpace.Folders.Named("Order");

    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");

    var source = orderApprove.CreateInputParameter("Source");
    source.DataType = order;

    var startNode = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var getOrder = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetOrder").ConnectedBelow(startNode);
    getOrder.SetMaxRecords("1");
    getOrder.AsDatabaseAggregate.CreateSource(order);
    getOrder.AsDatabaseAggregate.CreateCalculatedAttribute("CanApprove").SetValue("OrderCanApprove(Order.OrderStatusId, Order.AssignedToUserId)");
    getOrder.AsDatabaseAggregate.CreateFilter("Order.Id = Source.Id");

    var ifNode = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(getOrder);
    ifNode.SetCondition("GetOrder.List.Current.CanApprove");

    var raiseExceptionNode = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ToTheRightOf(ifNode);
    raiseExceptionNode.Exception = eSpace.UserExceptions.Named("InvalidStatusTransition");
    raiseExceptionNode.SetExceptionMessage("\"Invalid Status Transition\"");
    ifNode.FalseTarget = raiseExceptionNode;

    var assignNode = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().Below(ifNode);
    assignNode.CreateAssignment("Source.OrderStatusId", "Entities.OrderStatus.Approved");
    ifNode.TrueTarget = assignNode;

    var updateOrder = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateOrder").ConnectedBelow(assignNode);
    updateOrder.Action = order.UpdateAction;
    updateOrder.SetArgumentValue(order.UpdateAction.InputParameters.Named("Source"), "Source");

    var endNode = orderApprove.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(updateOrder);
}
