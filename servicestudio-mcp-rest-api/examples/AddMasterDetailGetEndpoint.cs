// Master-detail READ: GET /Orders/{Id}/WithLines returns an order together with its lines in one
// response, 404 when the order does not exist. Verified live 2026-10-14.
// Context: OrderAPI exists, the user exception OrderNotFound exists, Order + OrderLine are public
// entities (OrderLine.OrderId is the foreign key) and the module references HTTPRequestHandler.
// Shape: a result STRUCTURE (OrderWithLines: Order record + OrderLines list) as the single Body
// output. Flow: FindOrder aggregate -> If empty (404 + raise) -> GetLines aggregate (filter
// OrderLine.OrderId = Id) -> Assign Result.Order := FindOrder.List.Current,
// Result.OrderLines := GetLines.List -> End. The output param is named Result, and the JSON body is
// the structure itself (no wrapper).
// Pass imports: [].
eSpace => {
 var setStatus = eSpace.References.Named("HTTPRequestHandler").ServerActions.Named("SetStatusCode");
 var statusParam = setStatus.InputParameters.Named("StatusCode");
{
 var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
 var orderLine = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("OrderLine");
 var nf = eSpace.UserExceptions.Named("OrderNotFound");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("OrderAPI");
 var res = eSpace.CreateStructure("OrderWithLines");
 var rO = res.CreateAttribute("Order"); rO.DataType = order;
 var rL = res.CreateAttribute("OrderLines"); rL.DataType = eSpace.GetOrCreateListType(orderLine);
 var a = api.CreateAction("GetOrderWithLines");
 a.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET; a.URLPath = "/Orders/{Id}/WithLines";
 var id = a.CreateInputParameter("Id"); id.DataType = order.IdentifierType; id.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var st = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var find = a.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindOrder").ConnectedBelow(st);
 find.AsDatabaseAggregate.CreateSource(order); find.AsDatabaseAggregate.CreateFilter("Order.Id = Id");
 var iff = a.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(find);
 iff.SetCondition("FindOrder.List.Empty"); iff.Label = "Not found?";
 var set = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(iff);
 set.Action = setStatus; set.SetArgumentValue(statusParam, "404"); iff.TrueTarget = set;
 var raise = a.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(set);
 raise.Exception = nf; raise.SetExceptionMessage("\"Order not found\"");
 var lines = a.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetLines").Below(iff);
 iff.FalseTarget = lines;
 lines.AsDatabaseAggregate.CreateSource(orderLine); lines.AsDatabaseAggregate.CreateFilter("OrderLine.OrderId = Id");
 var asg = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("BuildResult").ConnectedBelow(lines);
 asg.CreateAssignment("Result.Order", "FindOrder.List.Current");
 asg.CreateAssignment("Result.OrderLines", "GetLines.List");
 var end = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); end.Below(asg); asg.Target = end;
 var o = a.CreateOutputParameter("Result"); o.DataType = res; o.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;
}
}
