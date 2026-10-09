// Expose full CRUD (List/Get/Create/Update/Delete) as a REST API over an existing public server entity
// Context: The module has a public server entity with an identifier attribute already set.
// Template: replace ShippingCompany -> <YourEntity>, ShippingCompanyAPI -> <YourEntity>API,
// ShippingCompanies (plural URL segment) -> <YourEntities>. Pass imports: [] when calling
// applyModelApiCode with this code — ServiceStudio.Plugin.RESTService cannot be imported,
// every type below is fully qualified for that reason.
//
// Before adapting this file, confirm the entity's CRUD action parameter names with the
// runQuery in SKILL.md § 2 — GetAction's output is named "Record", not the entity name,
// which is the one part of this pattern that isn't guessable from convention.

eSpace => {
    var shippingCompany = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("ShippingCompany");
    var shippingCompanyListType = eSpace.GetOrCreateListType(shippingCompany);

    var api = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("ShippingCompanyAPI");
    api.Authentication = ServiceStudio.Plugin.RESTService.Enumerations.Authentication.None;
    api.HttpSecurity = ServiceStudio.Plugin.RESTService.Enumerations.HttpSecurity.SSL;
    api.InternalAccess = ServiceStudio.Plugin.RESTService.Enumerations.InternalAccess.No;
    api.ShowDocumentation = ServiceStudio.Plugin.RESTService.Enumerations.ShowDocumentation.Yes;
    api.CrossSiteRequests = ServiceStudio.Plugin.RESTService.Enumerations.CrossSiteRequests.Restrict;
    api.DefaultValueBehavior = ServiceStudio.Plugin.RESTService.Enumerations.DefaultValueBehavior.Send;
    api.Interoperability = ServiceStudio.Plugin.RESTService.Enumerations.Interoperability.No;

    /* ---------- GET /ShippingCompanies (List) ---------- */
    var listShippingCompanies = api.CreateAction("GetShippingCompanies");
    listShippingCompanies.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;
    listShippingCompanies.URLPath = "/ShippingCompanies";

    var listStart = listShippingCompanies.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var listAggregate = listShippingCompanies.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetShippingCompanyList").ConnectedBelow(listStart);
    listAggregate.AsDatabaseAggregate.CreateSource(shippingCompany);

    var listAssign = listShippingCompanies.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(listAggregate);
    listAssign.CreateAssignment("ShippingCompanies", "GetShippingCompanyList.List");

    var listEnd = listShippingCompanies.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    listEnd.Below(listAssign);
    listAssign.Target = listEnd;

    var listOutput = listShippingCompanies.CreateOutputParameter("ShippingCompanies");
    listOutput.DataType = shippingCompanyListType;
    listOutput.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- GET /ShippingCompanies/{Id} ---------- */
    var getShippingCompany = api.CreateAction("GetShippingCompany");
    getShippingCompany.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;
    getShippingCompany.URLPath = "/ShippingCompanies/{Id}";

    var getIdInput = getShippingCompany.CreateInputParameter("Id");
    getIdInput.DataType = shippingCompany.IdentifierType;
    getIdInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;

    var getStart = getShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var getExec = getShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("GetEntity").ConnectedBelow(getStart);
    getExec.Action = shippingCompany.GetAction;
    getExec.SetArgumentValue(shippingCompany.GetAction.InputParameters.Named("Id"), "Id");

    var getAssign = getShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(getExec);
    getAssign.CreateAssignment("ShippingCompany", "GetEntity.Record");

    var getEnd = getShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    getEnd.Below(getAssign);
    getAssign.Target = getEnd;

    var getOutput = getShippingCompany.CreateOutputParameter("ShippingCompany");
    getOutput.DataType = shippingCompany;
    getOutput.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- POST /ShippingCompanies ---------- */
    var createShippingCompany = api.CreateAction("CreateShippingCompany");
    createShippingCompany.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.POST;
    createShippingCompany.URLPath = "/ShippingCompanies";

    var createInput = createShippingCompany.CreateInputParameter("ShippingCompany");
    createInput.DataType = shippingCompany;
    createInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;

    var createStart = createShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var createExec = createShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateEntity").ConnectedBelow(createStart);
    createExec.Action = shippingCompany.CreateAction;
    createExec.SetArgumentValue(shippingCompany.CreateAction.InputParameters.Named("Source"), "ShippingCompany");

    var createAssign = createShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(createExec);
    createAssign.CreateAssignment("Id", "CreateEntity.Id");

    var createEnd = createShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    createEnd.Below(createAssign);
    createAssign.Target = createEnd;

    var createOutput = createShippingCompany.CreateOutputParameter("Id");
    createOutput.DataType = shippingCompany.IdentifierType;
    createOutput.SendIn = ServiceStudio.Plugin.RESTService.Enumerations.SendIn.Body;

    /* ---------- PUT /ShippingCompanies/{Id} ---------- */
    var updateShippingCompany = api.CreateAction("UpdateShippingCompany");
    updateShippingCompany.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.PUT;
    updateShippingCompany.URLPath = "/ShippingCompanies/{Id}";

    var updateIdInput = updateShippingCompany.CreateInputParameter("Id");
    updateIdInput.DataType = shippingCompany.IdentifierType;
    updateIdInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;

    var updateBodyInput = updateShippingCompany.CreateInputParameter("ShippingCompany");
    updateBodyInput.DataType = shippingCompany;
    updateBodyInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.Body;

    var updateStart = updateShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();

    // Stamp the URL {Id} onto the body record so the URL is the source of truth, not whatever
    // Id the client happened to also put in the body. Do this at CREATE time (this node) rather
    // than splicing it in later against an already-merged action — see SKILL.md gotcha #7.
    var updateAssignId = updateShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>("SetId").ConnectedBelow(updateStart);
    updateAssignId.CreateAssignment("ShippingCompany.Id", "Id");

    var updateExec = updateShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("UpdateEntity").ConnectedBelow(updateAssignId);
    updateExec.Action = shippingCompany.UpdateAction;
    updateExec.SetArgumentValue(shippingCompany.UpdateAction.InputParameters.Named("Source"), "ShippingCompany");

    var updateEnd = updateShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    updateEnd.Below(updateExec);
    updateExec.Target = updateEnd;

    /* ---------- DELETE /ShippingCompanies/{Id} ---------- */
    var deleteShippingCompany = api.CreateAction("DeleteShippingCompany");
    deleteShippingCompany.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.DELETE;
    deleteShippingCompany.URLPath = "/ShippingCompanies/{Id}";

    var deleteIdInput = deleteShippingCompany.CreateInputParameter("Id");
    deleteIdInput.DataType = shippingCompany.IdentifierType;
    deleteIdInput.ReceiveIn = ServiceStudio.Plugin.RESTService.Enumerations.ReceiveIn.URL;

    var deleteStart = deleteShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
    var deleteExec = deleteShippingCompany.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").ConnectedBelow(deleteStart);
    deleteExec.Action = shippingCompany.DeleteAction;
    deleteExec.SetArgumentValue(shippingCompany.DeleteAction.InputParameters.Named("Id"), "Id");

    var deleteEnd = deleteShippingCompany.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
    deleteEnd.Below(deleteExec);
    deleteExec.Target = deleteEnd;
}
