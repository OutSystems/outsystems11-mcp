// 204 No Content on successful PUT and DELETE (Product and Order APIs). Verified live 2026-10-14:
// PUT -> 204 and the change persisted, DELETE -> 204, GET afterwards -> 404.
// Context: the 404 guards (and for Product, the 409 in-use guard from AddStatusCodeGuards.cs) are
// already in place; this rebuilds Update<E> and Delete<E> with the same guards plus a final
// SetStatusCode(204) node before the End. The actions are deleted and recreated rather than spliced
// (SKILL.md gotcha #7). The REST output stays empty, so the body is empty.
// Exceptions ProductNotFound / OrderNotFound / ProductInUse must already exist.
// Pass imports: [].
eSpace => {
 var setStatus = eSpace.References.Named("HTTPRequestHandler").ServerActions.Named("SetStatusCode");
 var statusParam = setStatus.InputParameters.Named("StatusCode");
{
 var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");
 var nf = eSpace.UserExceptions.Named("ProductNotFound");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("ProductAPI");
 api.Actions.Named("UpdateProduct").Delete();
 api.Actions.Named("DeleteProduct").Delete();
 var u = api.CreateAction("UpdateProduct");
 u.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.PUT; u.URLPath = "/Products/{Id}";
 var uId = u.CreateInputParameter("Id"); uId.DataType = product.IdentifierType; uId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var uBody = u.CreateInputParameter("Product"); uBody.DataType = product; uBody.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
 var uStart = u.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var uFind = u.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(uStart);
 uFind.AsDatabaseAggregate.CreateSource(product); uFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
 var uIf = u.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(uFind);
 uIf.SetCondition("FindProduct.List.Empty"); uIf.Label = "Not found?";
 var uSet = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(uIf);
 uSet.Action = setStatus; uSet.SetArgumentValue(statusParam, "404"); uIf.TrueTarget = uSet;
 var uRaise = u.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(uSet);
 uRaise.Exception = nf; uRaise.SetExceptionMessage("\"Product not found\"");
 var uStamp = u.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetId").Below(uIf);
 uIf.FalseTarget = uStamp; uStamp.CreateAssignment("Product.Id", "Id");
 var uExec = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateEntity").ConnectedBelow(uStamp);
 uExec.Action = product.UpdateAction; uExec.SetArgumentValue(product.UpdateAction.InputParameters.Named("Source"), "Product");
 var uOk = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNoContent").ConnectedBelow(uExec);
 uOk.Action = setStatus; uOk.SetArgumentValue(statusParam, "204");
 var uEnd = u.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); uEnd.Below(uOk); uOk.Target = uEnd;
 var d = api.CreateAction("DeleteProduct");
 d.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.DELETE; d.URLPath = "/Products/{Id}";
 var dId = d.CreateInputParameter("Id"); dId.DataType = product.IdentifierType; dId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var dStart = d.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var dFind = d.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(dStart);
 dFind.AsDatabaseAggregate.CreateSource(product); dFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
 var dIf = d.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dFind);
 dIf.SetCondition("FindProduct.List.Empty"); dIf.Label = "Not found?";
 var dSet = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(dIf);
 dSet.Action = setStatus; dSet.SetArgumentValue(statusParam, "404"); dIf.TrueTarget = dSet;
 var dRaise = d.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet);
 dRaise.Exception = nf; dRaise.SetExceptionMessage("\"Product not found\"");
 var orderLine = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("OrderLine");
 var inUse = eSpace.UserExceptions.Named("ProductInUse");
 var dUse = d.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindUsage").Below(dIf);
 dIf.FalseTarget = dUse;
 dUse.AsDatabaseAggregate.CreateSource(orderLine); dUse.AsDatabaseAggregate.CreateFilter("OrderLine.ProductId = Id");
 var dIf2 = d.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dUse);
 dIf2.SetCondition("not FindUsage.List.Empty"); dIf2.Label = "In use?";
 var dSet2 = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetConflict").ToTheRightOf(dIf2);
 dSet2.Action = setStatus; dSet2.SetArgumentValue(statusParam, "409"); dIf2.TrueTarget = dSet2;
 var dRaise2 = d.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet2);
 dRaise2.Exception = inUse; dRaise2.SetExceptionMessage("\"Product is used by one or more order lines\"");
 var dExec = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").Below(dIf2);
 dIf2.FalseTarget = dExec;
 dExec.Action = product.DeleteAction; dExec.SetArgumentValue(product.DeleteAction.InputParameters.Named("Id"), "Id");
 var dOk = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNoContent").ConnectedBelow(dExec);
 dOk.Action = setStatus; dOk.SetArgumentValue(statusParam, "204");
 var dEnd = d.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); dEnd.Below(dOk); dOk.Target = dEnd;
}
{
 var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
 var nf = eSpace.UserExceptions.Named("OrderNotFound");
 var api = eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("OrderAPI");
 api.Actions.Named("UpdateOrder").Delete();
 api.Actions.Named("DeleteOrder").Delete();
 var u = api.CreateAction("UpdateOrder");
 u.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.PUT; u.URLPath = "/Orders/{Id}";
 var uId = u.CreateInputParameter("Id"); uId.DataType = order.IdentifierType; uId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var uBody = u.CreateInputParameter("Order"); uBody.DataType = order; uBody.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
 var uStart = u.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var uFind = u.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindOrder").ConnectedBelow(uStart);
 uFind.AsDatabaseAggregate.CreateSource(order); uFind.AsDatabaseAggregate.CreateFilter("Order.Id = Id");
 var uIf = u.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(uFind);
 uIf.SetCondition("FindOrder.List.Empty"); uIf.Label = "Not found?";
 var uSet = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(uIf);
 uSet.Action = setStatus; uSet.SetArgumentValue(statusParam, "404"); uIf.TrueTarget = uSet;
 var uRaise = u.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(uSet);
 uRaise.Exception = nf; uRaise.SetExceptionMessage("\"Order not found\"");
 var uStamp = u.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetId").Below(uIf);
 uIf.FalseTarget = uStamp; uStamp.CreateAssignment("Order.Id", "Id");
 var uExec = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateEntity").ConnectedBelow(uStamp);
 uExec.Action = order.UpdateAction; uExec.SetArgumentValue(order.UpdateAction.InputParameters.Named("Source"), "Order");
 var uOk = u.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNoContent").ConnectedBelow(uExec);
 uOk.Action = setStatus; uOk.SetArgumentValue(statusParam, "204");
 var uEnd = u.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); uEnd.Below(uOk); uOk.Target = uEnd;
 var d = api.CreateAction("DeleteOrder");
 d.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.DELETE; d.URLPath = "/Orders/{Id}";
 var dId = d.CreateInputParameter("Id"); dId.DataType = order.IdentifierType; dId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
 var dStart = d.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
 var dFind = d.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindOrder").ConnectedBelow(dStart);
 dFind.AsDatabaseAggregate.CreateSource(order); dFind.AsDatabaseAggregate.CreateFilter("Order.Id = Id");
 var dIf = d.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dFind);
 dIf.SetCondition("FindOrder.List.Empty"); dIf.Label = "Not found?";
 var dSet = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(dIf);
 dSet.Action = setStatus; dSet.SetArgumentValue(statusParam, "404"); dIf.TrueTarget = dSet;
 var dRaise = d.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet);
 dRaise.Exception = nf; dRaise.SetExceptionMessage("\"Order not found\"");
 var dExec = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").Below(dIf);
 dIf.FalseTarget = dExec;
 dExec.Action = order.DeleteAction; dExec.SetArgumentValue(order.DeleteAction.InputParameters.Named("Id"), "Id");
 var dOk = d.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNoContent").ConnectedBelow(dExec);
 dOk.Action = setStatus; dOk.SetArgumentValue(statusParam, "204");
 var dEnd = d.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First(); dEnd.Below(dOk); dOk.Target = dEnd;
}
}
