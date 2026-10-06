// Create a web block named ProductPopup with input parameters (ShowPopup, ProductId, CanEdit), events (OnClose, OnSave), a screen aggregate to fetch the product, and a Save screen action that triggers the OnSave event
// Context: The module has a server entity named Product in MainFlow

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");
    var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");

    // Create the block
    var productPopup = mainFlow.CreateBlock("ProductPopup");
    productPopup.Description = "Popup block for editing or viewing a product.";
    productPopup.Public = false;

    // Input parameters that the parent screen passes to control the popup
    var showPopup = productPopup.CreateInputParameter("ShowPopup");
    showPopup.DataType = eSpace.BooleanType;

    var productId = productPopup.CreateInputParameter("ProductId");
    productId.DataType = product.IdentifierType;

    var canEdit = productPopup.CreateInputParameter("CanEdit");
    canEdit.DataType = eSpace.BooleanType;

    // Events that the popup fires to notify the parent screen
    var onClose = productPopup.CreateEvent("OnClose");
    onClose.IsMandatory = true;

    var onSave = productPopup.CreateEvent("OnSave");
    onSave.IsMandatory = true;

    // Screen aggregate to fetch the product being edited
    var getProductById = productPopup.CreateScreenAggregate(false, "GetProductById");
    getProductById.SetMaxRecords("1");
    getProductById.AsDatabaseAggregate.CreateSource(product);
    getProductById.AsDatabaseAggregate.CreateFilter("Product.Id = ProductId");

    // Save screen action: calls CreateOrUpdate and triggers the OnSave event
    var save = productPopup.CreateScreenAction("Save");

    var startNode = save.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var createOrUpdateProduct = save.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateOrUpdateProduct").ConnectedBelow(startNode);
    createOrUpdateProduct.Action = product.CreateOrUpdateAction;
    createOrUpdateProduct.SetArgumentValue(product.CreateOrUpdateAction.InputParameters.Named("Source"), "GetProductById.List.Current.Product");

    // Trigger the OnSave event to notify the parent screen
    var triggerOnSave = save.CreateNode<OutSystems.Model.Logic.Nodes.ITriggerNode>("OnSave").ConnectedBelow(createOrUpdateProduct);
    triggerOnSave.Event = onSave;

    var endNode = save.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(triggerOnSave);
}
