// Full CRUD REST API over a public server entity, with a real HTTP 404 and the standard error body
// {"Errors":["Product not found"],"StatusCode":404} on Get / Update / Delete when the id does not
// exist. Verified live 2026-10-13 against entity "Product" (list, get, create, update, delete, 404 on
// missing ids; a non-numeric id still returns 400).
// Context: the module has a public server entity "Product" with an identifier attribute, AND a
// reference to the Forge component HTTPRequestHandler (it provides SetStatusCode; the Model API and
// (System) cannot set a response status on their own — see SKILL.md § 7.2).
// How the 404 body works: SetStatusCode(404) alone leaves the normal output record (default values)
// as the body; raising a user exception right after it makes the platform send its standard error
// JSON and KEEP the status you set.
// Template: Product -> <YourEntity>, ProductAPI -> <YourEntity>API, Products -> <YourEntities>,
// ProductNotFound -> <YourEntity>NotFound.
// Pass imports: [] — ServiceStudio.Plugin.RESTService cannot be imported.
// Re-applying: the first statements create the user exception and delete an existing integration of
// the same name (delete and recreate, SKILL.md gotcha #7). When the exception already exists, replace
// CreateUserException with eSpace.UserExceptions.Named("ProductNotFound").
eSpace => {
    var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");
    var productListType = eSpace.GetOrCreateListType(product);
    var setStatus = eSpace.References.Named("HTTPRequestHandler").ServerActions.Named("SetStatusCode");
    var statusParam = setStatus.InputParameters.Named("StatusCode");

    var notFound = eSpace.CreateUserException("ProductNotFound");
    eSpace.Integrations.OfType<ServiceStudio.Plugin.RESTService.IRestService>().Named("ProductAPI").Delete();

    var api = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("ProductAPI");
    api.Authentication = ServiceStudio.Plugin.RESTService.Enumerations.Authentication.None;
    api.HttpSecurity = ServiceStudio.Plugin.RESTService.Enumerations.HttpSecurity.SSL;
    api.InternalAccess = ServiceStudio.Plugin.RESTService.Enumerations.InternalAccess.No;
    api.ShowDocumentation = ServiceStudio.Plugin.RESTService.Enumerations.ShowDocumentation.Yes;
    api.CrossSiteRequests = ServiceStudio.Plugin.RESTService.Enumerations.CrossSiteRequests.Restrict;
    api.DefaultValueBehavior = ServiceStudio.Plugin.RESTService.Enumerations.DefaultValueBehavior.Send;
    api.Interoperability = ServiceStudio.Plugin.RESTService.Enumerations.Interoperability.No;

    /* ---------- GET /Products ---------- */
    var list = api.CreateAction("GetProducts");
    list.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;
    list.URLPath = "/Products";
    var lStart = list.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var lAgg = list.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetProductList").ConnectedBelow(lStart);
    lAgg.AsDatabaseAggregate.CreateSource(product);
    var lAsg = list.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(lAgg);
    lAsg.CreateAssignment("Products", "GetProductList.List");
    var lEnd = list.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    lEnd.Below(lAsg); lAsg.Target = lEnd;
    var lOut = list.CreateOutputParameter("Products"); lOut.DataType = productListType;
    lOut.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- GET /Products/{Id}  (404 when missing) ---------- */
    var get = api.CreateAction("GetProduct");
    get.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;
    get.URLPath = "/Products/{Id}";
    var gId = get.CreateInputParameter("Id"); gId.DataType = product.IdentifierType;
    gId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
    var gStart = get.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var gFind = get.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(gStart);
    gFind.AsDatabaseAggregate.CreateSource(product);
    gFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
    var gIf = get.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(gFind);
    gIf.SetCondition("FindProduct.List.Empty");
    gIf.Label = "Not found?";
    var gSet = get.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(gIf);
    gSet.Action = setStatus;
    gSet.SetArgumentValue(statusParam, "404");
    gIf.TrueTarget = gSet;
    var gRaise = get.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(gSet);
    gRaise.Exception = notFound;
    gRaise.SetExceptionMessage("\"Product not found\"");
    var gExec = get.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("GetEntity").Below(gIf);
    gIf.FalseTarget = gExec;
    gExec.Action = product.GetAction;
    gExec.SetArgumentValue(product.GetAction.InputParameters.Named("Id"), "Id");
    var gAsg = get.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(gExec);
    gAsg.CreateAssignment("Product", "GetEntity.Record");
    var gEnd = get.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    gEnd.Below(gAsg); gAsg.Target = gEnd;
    var gOut = get.CreateOutputParameter("Product"); gOut.DataType = product;
    gOut.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- POST /Products ---------- */
    var create = api.CreateAction("CreateProduct");
    create.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST;
    create.URLPath = "/Products";
    var cIn = create.CreateInputParameter("Product"); cIn.DataType = product;
    cIn.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
    var cStart = create.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var cExec = create.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateEntity").ConnectedBelow(cStart);
    cExec.Action = product.CreateAction;
    cExec.SetArgumentValue(product.CreateAction.InputParameters.Named("Source"), "Product");
    var cAsg = create.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(cExec);
    cAsg.CreateAssignment("Id", "CreateEntity.Id");
    var cEnd = create.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    cEnd.Below(cAsg); cAsg.Target = cEnd;
    var cOut = create.CreateOutputParameter("Id"); cOut.DataType = product.IdentifierType;
    cOut.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- PUT /Products/{Id}  (404 when missing) ---------- */
    var update = api.CreateAction("UpdateProduct");
    update.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.PUT;
    update.URLPath = "/Products/{Id}";
    var uId = update.CreateInputParameter("Id"); uId.DataType = product.IdentifierType;
    uId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
    var uBody = update.CreateInputParameter("Product"); uBody.DataType = product;
    uBody.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;
    var uStart = update.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var uFind = update.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(uStart);
    uFind.AsDatabaseAggregate.CreateSource(product);
    uFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
    var uIf = update.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(uFind);
    uIf.SetCondition("FindProduct.List.Empty");
    uIf.Label = "Not found?";
    var uSet = update.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(uIf);
    uSet.Action = setStatus;
    uSet.SetArgumentValue(statusParam, "404");
    uIf.TrueTarget = uSet;
    var uRaise = update.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(uSet);
    uRaise.Exception = notFound;
    uRaise.SetExceptionMessage("\"Product not found\"");
    var uStamp = update.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetId").Below(uIf);
    uIf.FalseTarget = uStamp;
    uStamp.CreateAssignment("Product.Id", "Id");
    var uExec = update.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateEntity").ConnectedBelow(uStamp);
    uExec.Action = product.UpdateAction;
    uExec.SetArgumentValue(product.UpdateAction.InputParameters.Named("Source"), "Product");
    var uEnd = update.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    uEnd.Below(uExec); uExec.Target = uEnd;

    /* ---------- DELETE /Products/{Id}  (404 when missing) ---------- */
    var del = api.CreateAction("DeleteProduct");
    del.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.DELETE;
    del.URLPath = "/Products/{Id}";
    var dId = del.CreateInputParameter("Id"); dId.DataType = product.IdentifierType;
    dId.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;
    var dStart = del.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var dFind = del.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("FindProduct").ConnectedBelow(dStart);
    dFind.AsDatabaseAggregate.CreateSource(product);
    dFind.AsDatabaseAggregate.CreateFilter("Product.Id = Id");
    var dIf = del.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(dFind);
    dIf.SetCondition("FindProduct.List.Empty");
    dIf.Label = "Not found?";
    var dSet = del.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("SetNotFound").ToTheRightOf(dIf);
    dSet.Action = setStatus;
    dSet.SetArgumentValue(statusParam, "404");
    dIf.TrueTarget = dSet;
    var dRaise = del.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ConnectedBelow(dSet);
    dRaise.Exception = notFound;
    dRaise.SetExceptionMessage("\"Product not found\"");
    var dExec = del.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").Below(dIf);
    dIf.FalseTarget = dExec;
    dExec.Action = product.DeleteAction;
    dExec.SetArgumentValue(product.DeleteAction.InputParameters.Named("Id"), "Id");
    var dEnd = del.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    dEnd.Below(dExec); dExec.Target = dEnd;
}
