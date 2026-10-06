// Create a Products list screen with an inline edit popup. The screen fetches products via an aggregate, displays them in a table, and opens a ProductPopup block when a row is clicked. The popup is controlled by local boolean variables and closed via screen actions that refresh the data.
// Context: The module has a server entity named Product, a block named ProductPopup (with inputs ShowPopup, ProductId, CanEdit, ShowPopupInViewMode and events OnClose, OnDelete) in MainFlow, a role named Manager, and a layout block named LayoutSideMenu in the Layouts flow

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");
    var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");

    // --- SCREEN ---
    var products = mainFlow.CreateScreen("Products");
    products.Widgets.ToList().ForEach(x => x.Delete());
    products.Roles.Add(eSpace.Roles.Named("Manager"));

    // --- SCREEN AGGREGATE ---
    var getProducts = products.CreateScreenAggregate(false, "GetProducts");
    getProducts.SetMaxRecords("50");
    getProducts.AsDatabaseAggregate.CreateSource(product);

    // --- LOCAL VARIABLES FOR POPUP STATE ---
    var showProductPopup = products.CreateLocalVariable("ShowProductPopup");
    showProductPopup.DataType = eSpace.BooleanType;

    var showProductPopupInViewMode = products.CreateLocalVariable("ShowProductPopupInViewMode");
    showProductPopupInViewMode.DataType = eSpace.BooleanType;

    var productInEdition = products.CreateLocalVariable("ProductInEdition");
    productInEdition.DataType = product.IdentifierType;

    var productCanEdit = products.CreateLocalVariable("ProductCanEdit");
    productCanEdit.DataType = eSpace.BooleanType;

    // --- SCREEN ACTION: OnClosePopup ---
    var productOnClosePopup = products.CreateScreenAction("ProductOnClosePopup");

    var closeStart = productOnClosePopup.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var closeAssign = productOnClosePopup.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(closeStart);
    closeAssign.CreateAssignment("ShowProductPopup", "False");
    closeAssign.CreateAssignment("ProductInEdition", "NullIdentifier()");

    var refreshDataNode = productOnClosePopup.CreateNode<OutSystems.Model.Logic.Nodes.IRefreshDataNode>().ConnectedBelow(closeAssign);
    refreshDataNode.DataSource = getProducts;
    refreshDataNode.SetMaxRecords("50");

    var closeEnd = productOnClosePopup.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(refreshDataNode);

    // --- SCREEN ACTION: OnOpenPopup ---
    var productOnOpenPopup = products.CreateScreenAction("ProductOnOpenPopup");

    var productIdParam = productOnOpenPopup.CreateInputParameter("ProductId");
    productIdParam.DataType = product.IdentifierType;

    var openInViewModeParam = productOnOpenPopup.CreateInputParameter("OpenInViewMode");
    openInViewModeParam.DataType = eSpace.BooleanType;

    var canEditParam = productOnOpenPopup.CreateInputParameter("CanEdit");
    canEditParam.DataType = eSpace.BooleanType;
    canEditParam.SetDefaultValue("True");
    canEditParam.IsMandatory = false;

    var openStart = productOnOpenPopup.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var openAssign = productOnOpenPopup.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(openStart);
    openAssign.CreateAssignment("ProductCanEdit", "CanEdit");

    var openAssign2 = productOnOpenPopup.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(openAssign);
    openAssign2.CreateAssignment("ProductInEdition", "ProductId");
    openAssign2.CreateAssignment("ShowProductPopupInViewMode", "OpenInViewMode");
    openAssign2.CreateAssignment("ShowProductPopup", "True");

    var openEnd = productOnOpenPopup.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(openAssign2);

    // --- LAYOUT ---
    var layoutInstance = products.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var layouts = eSpace.MobileFlows.Named("Layouts");
    var layoutSideMenu = layouts.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("LayoutSideMenu");
    layoutInstance.SourceBlock = layoutSideMenu;

    // --- TABLE IN MAIN CONTENT ---
    var mainContent = layoutInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");

    // Create a table records widget bound to the aggregate
    var tableRecords = mainContent.CreateWidget<ServiceStudio.Plugin.NRWidgets.ITableRecords>();
    tableRecords.SetSource("GetProducts.List");
    tableRecords.SetStyle("\"table\"");
    tableRecords.SetStyleRow("\"table-row\"");

    // Header cell with column label
    var headerCell = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    var headerText = headerCell.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    headerText.Text = "Name";

    // Row cell with a link that opens the popup when clicked
    var rowCell = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var link = rowCell.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILink>();
    link.Widgets.ToList().ForEach(x => x.Delete());
    var nameExpr = link.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    nameExpr.SetValue("GetProducts.List.Current.Product.Name");
    var rowClick = link.OnClick;
    rowClick.Destination = productOnOpenPopup;
    rowClick.SetArgumentValue(productIdParam, "GetProducts.List.Current.Product.Id");
    rowClick.SetArgumentValue(openInViewModeParam, "True");

    // --- POPUP BLOCK INSTANCE IN FOOTER ---
    var footer = layoutInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Footer");

    var productPopupInstance = footer.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var productPopup = mainFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("ProductPopup");
    productPopupInstance.SourceBlock = productPopup;
    productPopupInstance.SetArgumentValue(productPopup.InputParameters.Named("ShowPopup"), "ShowProductPopup");
    productPopupInstance.SetArgumentValue(productPopup.InputParameters.Named("ProductId"), "ProductInEdition");
    productPopupInstance.SetArgumentValue(productPopup.InputParameters.Named("CanEdit"), "ProductCanEdit");
    productPopupInstance.SetArgumentValue(productPopup.InputParameters.Named("ShowPopupInViewMode"), "ShowProductPopupInViewMode");

    // Wire popup events to close handler
    productPopupInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "OnClose").Handler = productOnClosePopup;
    productPopupInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "OnDelete").Handler = productOnClosePopup;
}
