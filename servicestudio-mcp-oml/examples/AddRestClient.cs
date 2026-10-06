// Create a REST client integration named ProductApi with a GET endpoint to fetch a product by key and a POST endpoint to create a product
// Context: The module has no relevant pre-existing elements

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    // Create the REST client integration with a base URL
    var productApi = eSpace.CreateIntegration<ServiceStudio.Plugin.REST.IRestClient>("ProductApi");
    productApi.BaseURL = "https://api.example.com/v1";
    productApi.Description = "REST client for the Product API.";

    // GET endpoint: fetch a product by key
    var getProduct = productApi.CreateAction("GetProduct");
    getProduct.HTTPMethod = ServiceStudio.Plugin.REST.Enumerations.HTTPMethod.GET;
    getProduct.URLPath = "/products?key={ProductKey}";
    getProduct.ResponseFormat = ServiceStudio.Plugin.REST.Enumerations.ResponseFormat.PlainText;

    var productKey = getProduct.CreateInputParameter("ProductKey");
    productKey.DataType = eSpace.TextType;
    productKey.Description = "Unique key of the product to retrieve.";
    productKey.IsMandatory = true;
    productKey.InputPlacement = ServiceStudio.Plugin.REST.Enumerations.InputPlacement.UrlPlacement;
    productKey.OriginalName = "ProductKey";

    var response = getProduct.CreateOutputParameter("Response");
    response.DataType = eSpace.TextType;
    response.Description = "The product data returned by the API.";
    response.OutputPlacement = ServiceStudio.Plugin.REST.Enumerations.OutputPlacement.BodyPlacement;

    // POST endpoint: create a product
    var createProduct = productApi.CreateAction("CreateProduct");
    createProduct.HTTPMethod = ServiceStudio.Plugin.REST.Enumerations.HTTPMethod.POST;
    createProduct.URLPath = "/products";
    createProduct.RequestFormat = ServiceStudio.Plugin.REST.Enumerations.RequestFormat.PlainText;

    var productName = createProduct.CreateInputParameter("ProductName");
    productName.DataType = eSpace.TextType;
    productName.Description = "Name of the product to create.";
    productName.IsMandatory = true;
    productName.InputPlacement = ServiceStudio.Plugin.REST.Enumerations.InputPlacement.BodyPlacement;
    productName.OriginalName = "ProductName";

    var productPrice = createProduct.CreateInputParameter("ProductPrice");
    productPrice.DataType = eSpace.DecimalType;
    productPrice.Description = "Price of the product.";
    productPrice.IsMandatory = true;
    productPrice.InputPlacement = ServiceStudio.Plugin.REST.Enumerations.InputPlacement.BodyPlacement;
    productPrice.OriginalName = "ProductPrice";
}
