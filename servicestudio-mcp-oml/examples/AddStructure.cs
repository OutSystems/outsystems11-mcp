// Create a structure named OrderSummary with attributes Id (Integer), CustomerName (Text), Total (Decimal), and CreatedOn (DateTime)
// Context: The module has no relevant pre-existing elements

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    // Create a new structure (DTO / record type)
    var orderSummary = eSpace.CreateStructure("OrderSummary");
    orderSummary.Description = "Holds a summary of an order for display or export.";
    orderSummary.Public = false;

    // Add typed attributes to the structure
    var id = orderSummary.CreateAttribute("Id");
    id.DataType = eSpace.IntegerType;
    id.Label = "Id";

    var customerName = orderSummary.CreateAttribute("CustomerName");
    customerName.DataType = eSpace.TextType;
    customerName.Label = "Customer Name";
    customerName.Length = 100;

    var total = orderSummary.CreateAttribute("Total");
    total.DataType = eSpace.DecimalType;
    total.Label = "Total";
    total.IsMandatory = true;

    var createdOn = orderSummary.CreateAttribute("CreatedOn");
    createdOn.DataType = eSpace.DateTimeType;
    createdOn.Label = "Created On";
}
