// Create a ProductDetail screen with an input parameter ProductId, an OnInitialize lifecycle event that redirects to the Products screen if the Id is invalid, and an aggregate to fetch the product
// Context: The module has a server entity named Product, a screen named Products in MainFlow, a role named Manager, and a layout block named LayoutSideMenu in the Layouts flow

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");
    var product = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Product");

    // Create the screen with a required input parameter
    var productDetail = mainFlow.CreateScreen("ProductDetail");
    productDetail.Widgets.ToList().ForEach(x => x.Delete());
    productDetail.Roles.Add(eSpace.Roles.Named("Manager"));
    productDetail.SetTitle("\"Product Detail\"");

    // Screen input parameter: the Id of the product to display
    var productId = productDetail.CreateInputParameter("ProductId");
    productId.DataType = product.IdentifierType;
    productId.IsMandatory = true;

    // Screen aggregate to fetch the product by Id
    var getProduct = productDetail.CreateScreenAggregate(false, "GetProduct");
    getProduct.SetMaxRecords("1");
    getProduct.AsDatabaseAggregate.CreateSource(product);
    getProduct.AsDatabaseAggregate.CreateFilter("Product.Id = ProductId");

    // OnInitialize screen action: redirect if the ProductId is invalid
    var onInitialize = productDetail.CreateScreenAction("OnInitialize");

    var startNode = onInitialize.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var ifNode = onInitialize.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(startNode);
    ifNode.SetCondition("ProductId = NullIdentifier()");
    ifNode.Label = "Invalid Id?";

    // If invalid, redirect to the Products list screen
    var destinationNode = onInitialize.CreateNode<OutSystems.Model.Logic.Nodes.IDestinationNode>().ToTheRightOf(ifNode);
    var productsScreen = mainFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("Products");
    destinationNode.Destination = productsScreen;
    ifNode.TrueTarget = destinationNode;

    var endNode = onInitialize.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().Below(ifNode);
    ifNode.FalseTarget = endNode;

    // Wire the OnInitialize lifecycle event to the screen action
    productDetail.OnInitialize.Destination = onInitialize;
}
