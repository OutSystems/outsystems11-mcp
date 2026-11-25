// Status-code guards beyond 404: 201 Created on POST, 409 Conflict when a delete is blocked by a
// Protect delete rule, and 400 validation inside the composite create (with the header insert rolled
// back). Verified live 2026-10-14 (Product / Order / OrderLine on O11 11.55).
// Context: AddRestApiWithNotFound.cs has already been applied (ProductAPI / OrderAPI exist with the
// 404 guards and the ProductNotFound exception), the structure CreateOrderRequest exists
// (AddCompositeCreateEndpoint.cs), and the module references HTTPRequestHandler.
// What it does:
//   - CreateProduct / CreateOrder: SetStatusCode(201) after the create, then End.
//   - DeleteProduct: after the 404 guard, look for OrderLine rows with ProductId = Id and answer
//     409 + raise ProductInUse when any exist (without it the Protect rule surfaces as a 500).
//   - CreateOrderWithLines: 400 + raise InvalidOrder when OrderLines is empty, and 400 when a line's
//     ProductId does not exist (checked per line inside the ForEach). The raise rolls the request
//     transaction back: after a failed call no order header was left behind (checked by comparing
//     the order list before and after).
// Re-applying: CreateUserException throws if the exception exists; swap to eSpace.UserExceptions.Named.
// Every rebuilt action is deleted and recreated (SKILL.md gotcha #7), never spliced.
// Pass imports: [].
eSpace => {
 var setStatus = eSpace.References.Named("HTTPRequestHandler").ServerActions.Named("SetStatusCode");
 var statusParam = setStatus.InputParameters.Named("StatusCode");
 var inUse = eSpace.CreateUserException("ProductInUse");
 var invalid = eSpace.CreateUserException("InvalidOrder");
 var notFound = eSpace.UserExceptions.Named("ProductNotFound");
{
 var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("ProductAPI");
 api.Actions.Named("CreateProduct").Delete();
 var c = api.CreateAction("CreateProduct");
 c.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST; c.URLPath = "/Products";
 var cIn = c.CreateInputParameter("Product"); cIn.DataType = product; cIn.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
 var cStart = c.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var cExec = c.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateEntity").ConnectedBelow(cStart);
 cExec.Action = product.CreateAction; cExec.SetArgumentValue(product.CreateAction.InputParameters.Named("Source"), "Product");
 var cAsg = c.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(cExec);
 cAsg.CreateAssignment("Id", "CreateEntity.Id");
 var cSt = c.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetCreated").ConnectedBelow(cAsg);
 cSt.Action = setStatus; cSt.SetArgumentValue(statusParam, "201");
 var cEnd = c.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); cEnd.Below(cSt); cSt.Target = cEnd;
 var cOut = c.CreateOutputParameter("Id"); cOut.DataType = product.IdentifierType; cOut.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;
}
{
 var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("OrderAPI");
 api.Actions.Named("CreateOrder").Delete();
 var c = api.CreateAction("CreateOrder");
 c.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST; c.URLPath = "/Orders";
 var cIn = c.CreateInputParameter("Order"); cIn.DataType = order; cIn.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
 var cStart = c.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var cExec = c.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateEntity").ConnectedBelow(cStart);
 cExec.Action = order.CreateAction; cExec.SetArgumentValue(order.CreateAction.InputParameters.Named("Source"), "Order");
 var cAsg = c.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(cExec);
 cAsg.CreateAssignment("Id", "CreateEntity.Id");
 var cSt = c.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetCreated").ConnectedBelow(cAsg);
 cSt.Action = setStatus; cSt.SetArgumentValue(statusParam, "201");
 var cEnd = c.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); cEnd.Below(cSt); cSt.Target = cEnd;
 var cOut = c.CreateOutputParameter("Id"); cOut.DataType = order.IdentifierType; cOut.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;
}
{
 var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");
 var orderLine = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("OrderLine");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("ProductAPI");
 api.Actions.Named("DeleteProduct").Delete();
 var del = api.CreateAction("DeleteProduct");
 del.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.DELETE; del.URLPath = "/Products/{Id}";
 var dId = del.CreateInputParameter("Id"); dId.DataType = product.IdentifierType; dId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var dStart = del.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var dFind = del.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(dStart);
 dFind.AsDatabaseAggregate.CreateSource(product); dFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
 var dIf = del.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dFind);
 dIf.SetCondition("FindProduct.List.Empty"); dIf.Label = "Not found?";
 var dSet = del.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(dIf);
 dSet.Action = setStatus; dSet.SetArgumentValue(statusParam, "404"); dIf.TrueTarget = dSet;
 var dRaise = del.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet);
 dRaise.Exception = notFound; dRaise.SetExceptionMessage("\"Product not found\"");
 var dUse = del.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindUsage").Below(dIf);
 dIf.FalseTarget = dUse;
 dUse.AsDatabaseAggregate.CreateSource(orderLine); dUse.AsDatabaseAggregate.CreateFilter("OrderLine.ProductId = Id");
 var dIf2 = del.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dUse);
 dIf2.SetCondition("not FindUsage.List.Empty"); dIf2.Label = "In use?";
 var dSet2 = del.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetConflict").ToTheRightOf(dIf2);
 dSet2.Action = setStatus; dSet2.SetArgumentValue(statusParam, "409"); dIf2.TrueTarget = dSet2;
 var dRaise2 = del.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet2);
 dRaise2.Exception = inUse; dRaise2.SetExceptionMessage("\"Product is used by one or more order lines\"");
 var dExec = del.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").Below(dIf2);
 dIf2.FalseTarget = dExec; dExec.Action = product.DeleteAction;
 dExec.SetArgumentValue(product.DeleteAction.InputParameters.Named("Id"), "Id");
 var dEnd = del.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); dEnd.Below(dExec); dExec.Target = dEnd;
}
{
 var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
 var orderLine = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("OrderLine");
 var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");
 var req = eSpace.Structures.Named("CreateOrderRequest");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("OrderAPI");
 api.Actions.Named("CreateOrderWithLines").Delete();
 var a = api.CreateAction("CreateOrderWithLines");
 a.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST; a.URLPath = "/Orders/WithLines";
 var rIn = a.CreateInputParameter("Request"); rIn.DataType = req; rIn.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
 var total = a.CreateLocalVariable("TotalAmount"); total.DataType = eSpace.DecimalType;
 var newId = a.CreateLocalVariable("NewOrderId"); newId.DataType = order.IdentifierType;
 var newLine = a.CreateLocalVariable("NewLine"); newLine.DataType = orderLine;
 var start = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var init = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("InitTotal").ConnectedBelow(start);
 init.CreateAssignment("TotalAmount", "0");
 var ifEmpty = a.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(init);
 ifEmpty.SetCondition("Request.OrderLines.Empty"); ifEmpty.Label = "No lines?";
 var set1 = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetBadRequest").ToTheRightOf(ifEmpty);
 set1.Action = setStatus; set1.SetArgumentValue(statusParam, "400"); ifEmpty.TrueTarget = set1;
 var raise1 = a.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(set1);
 raise1.Exception = invalid; raise1.SetExceptionMessage("\"An order needs at least one order line\"");
 var execOrder = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateOrderHeader").Below(ifEmpty);
 ifEmpty.FalseTarget = execOrder;
 execOrder.Action = order.CreateAction; execOrder.SetArgumentValue(order.CreateAction.InputParameters.Named("Source"), "Request.Order");
 var asgId = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetNewOrderId").ConnectedBelow(execOrder);
 asgId.CreateAssignment("NewOrderId", "CreateOrderHeader.Id");
 var fe = a.CreateNode<OutSystems.Model.Logic.Nodes.IForEachNode>().ConnectedBelow(asgId);
 fe.SetRecordList("Request.OrderLines");
 var findP = a.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindLineProduct").ToTheRightOf(fe);
 findP.AsDatabaseAggregate.CreateSource(product); findP.AsDatabaseAggregate.CreateFilter("Product.Id = Request.OrderLines.Current.ProductId");
 fe.CycleTarget = findP;
 var ifP = a.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(findP);
 ifP.SetCondition("FindLineProduct.List.Empty"); ifP.Label = "Unknown product?";
 var set2 = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetBadRequest2").ToTheRightOf(ifP);
 set2.Action = setStatus; set2.SetArgumentValue(statusParam, "400"); ifP.TrueTarget = set2;
 var raise2 = a.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(set2);
 raise2.Exception = invalid; raise2.SetExceptionMessage("\"An order line references a product that does not exist\"");
 var prep = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("PrepareLine").Below(ifP);
 ifP.FalseTarget = prep;
 prep.CreateAssignment("NewLine", "Request.OrderLines.Current");
 prep.CreateAssignment("NewLine.OrderId", "NewOrderId");
 prep.CreateAssignment("NewLine.LineTotal", "Request.OrderLines.Current.Quantity * Request.OrderLines.Current.UnitPrice");
 var execLine = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateLine").ConnectedBelow(prep);
 execLine.Action = orderLine.CreateAction; execLine.SetArgumentValue(orderLine.CreateAction.InputParameters.Named("Source"), "NewLine");
 var acc = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("AccumulateTotal").ConnectedBelow(execLine);
 acc.CreateAssignment("TotalAmount", "TotalAmount + NewLine.LineTotal");
 acc.Target = fe;
 var fin = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("PrepareOrderUpdate").Below(fe);
 fin.CreateAssignment("Request.Order.Id", "NewOrderId");
 fin.CreateAssignment("Request.Order.TotalAmount", "TotalAmount");
 fe.Target = fin;
 var upd = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateOrderTotal").ConnectedBelow(fin);
 upd.Action = order.UpdateAction; upd.SetArgumentValue(order.UpdateAction.InputParameters.Named("Source"), "Request.Order");
 var outAsg = a.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetOutput").ConnectedBelow(upd);
 outAsg.CreateAssignment("Id", "NewOrderId");
 var created = a.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetCreated").ConnectedBelow(outAsg);
 created.Action = setStatus; created.SetArgumentValue(statusParam, "201");
 var end = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); end.Below(created); created.Target = end;
 var o = a.CreateOutputParameter("Id"); o.DataType = order.IdentifierType; o.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;
}
}
