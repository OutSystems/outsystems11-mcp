// Add a composite "create header + detail lines in one call" REST action to an EXISTING REST API
// Context: The module already has a public server entity "Order" (header) and "OrderLine" (detail,
// with an OrderId foreign key to Order), and a REST API integration named "OrderAPI" already exists
// (built via examples/AddRestApiForEntity.cs or equivalent) with plain CRUD endpoints. This example
// adds ONE more action, CreateOrder, that accepts an order header plus a list of lines, creates the
// header, creates each line stamped with the new header's Id, and rolls the lines' computed total
// back onto the header via an Update call.
//
// Template: replace Order -> <YourHeaderEntity>, OrderLine -> <YourDetailEntity>, OrderAPI -> the
// existing API's name, TotalAmount/LineTotal/Quantity/UnitPrice -> your rollup/computed fields.
// Pass imports: [] when calling applyModelApiCode with this code.
//
// Apply this AFTER the base API exists — it looks the integration up by name rather than creating
// it, so running this against a module with no "OrderAPI" integration yet will throw a runtime
// NullReferenceException-shaped exceptionMessage from .Named() finding nothing.

eSpace => {
    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
    var orderLine = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("OrderLine");
    var orderLineListType = eSpace.GetOrCreateListType(orderLine);

    // Gotcha #6: a REST method can have only ONE input with ReceiveIn = Body. The header record and
    // the lines list are two logically separate inputs, so wrap them in a structure and take that
    // structure as the single Body input instead of two separate CreateInputParameter calls.
    var createOrderRequest = eSpace.CreateStructure("CreateOrderRequest");
    var reqOrder = createOrderRequest.CreateAttribute("Order");
    reqOrder.DataType = order;
    var reqLines = createOrderRequest.CreateAttribute("OrderLines");
    reqLines.DataType = orderLineListType;

    // Look the existing API up rather than creating a new one — this action is being ADDED to it.
    var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("OrderAPI");

    var createOrder = api.CreateAction("CreateOrder");
    createOrder.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST;
    createOrder.URLPath = "/Orders";

    var requestInput = createOrder.CreateInputParameter("Request");
    requestInput.DataType = createOrderRequest;
    requestInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;

    var totalAmount = createOrder.CreateLocalVariable("TotalAmount");
    totalAmount.DataType = eSpace.IntegerType;

    var newOrderId = createOrder.CreateLocalVariable("NewOrderId");
    newOrderId.DataType = order.IdentifierType;

    var newLine = createOrder.CreateLocalVariable("NewLine");
    newLine.DataType = orderLine;

    // Gotcha #3: reuse the auto-created Start, don't create a second one.
    var start = createOrder.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();

    var assignInit = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("InitTotal").ConnectedBelow(start);
    assignInit.CreateAssignment("TotalAmount", "0");

    var execCreateOrder = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateOrderHeader").ConnectedBelow(assignInit);
    execCreateOrder.Action = order.CreateAction;
    execCreateOrder.SetArgumentValue(order.CreateAction.InputParameters.Named("Source"), "Request.Order");

    var assignNewId = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetNewOrderId").ConnectedBelow(execCreateOrder);
    assignNewId.CreateAssignment("NewOrderId", "CreateOrderHeader.Id");

    var forEachNode = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IForEachNode>().ConnectedBelow(assignNewId);
    forEachNode.SetRecordList("Request.OrderLines");

    // Cycle body: position ToTheRightOf the ForEach (matches servicestudio-mcp-oml's
    // AddServerActionWithForEach.cs), then chain the rest of the body ConnectedBelow.
    var assignLine = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("PrepareLine").ToTheRightOf(forEachNode);
    assignLine.CreateAssignment("NewLine", "Request.OrderLines.Current");
    assignLine.CreateAssignment("NewLine.OrderId", "NewOrderId");
    assignLine.CreateAssignment("NewLine.LineTotal", "Request.OrderLines.Current.Quantity * Request.OrderLines.Current.UnitPrice");

    var execCreateLine = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateLine").ConnectedBelow(assignLine);
    execCreateLine.Action = orderLine.CreateAction;
    execCreateLine.SetArgumentValue(orderLine.CreateAction.InputParameters.Named("Source"), "NewLine");

    var assignAccumulate = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("AccumulateTotal").ConnectedBelow(execCreateLine);
    assignAccumulate.CreateAssignment("TotalAmount", "TotalAmount + NewLine.LineTotal");

    // Manual wiring for the cycle entry/exit — ToTheRightOf/Below only position, they don't connect.
    forEachNode.CycleTarget = assignLine;
    assignAccumulate.Target = forEachNode;

    var assignFinal = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("PrepareOrderUpdate").Below(forEachNode);
    assignFinal.CreateAssignment("Request.Order.Id", "NewOrderId");
    assignFinal.CreateAssignment("Request.Order.TotalAmount", "TotalAmount");
    forEachNode.Target = assignFinal;

    var execUpdateOrder = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateOrderTotal").ConnectedBelow(assignFinal);
    execUpdateOrder.Action = order.UpdateAction;
    execUpdateOrder.SetArgumentValue(order.UpdateAction.InputParameters.Named("Source"), "Request.Order");

    var assignOutput = createOrder.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetOutput").ConnectedBelow(execUpdateOrder);
    assignOutput.CreateAssignment("Id", "NewOrderId");

    // Gotcha #3 again: reuse the auto-created End, don't create a second one.
    var end = createOrder.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    end.Below(assignOutput);
    assignOutput.Target = end;

    var idOutput = createOrder.CreateOutputParameter("Id");
    idOutput.DataType = order.IdentifierType;
    idOutput.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;
}
