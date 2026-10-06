// Add a static entity named RequestStatus with records Pending, Accepted and Rejected

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var requestStatus = eSpace.CreateStaticEntity("RequestStatus");
    requestStatus.Label = "Request Status";
    requestStatus.LabelPlural = "Request Statuses";

    var id = requestStatus.CreateAttribute("Id");
    id.DataType = eSpace.IntegerType;
    id.Label = "Id";
    requestStatus.IdentifierAttribute = id;

    var label = requestStatus.CreateAttribute("Label");
    label.DataType = eSpace.TextType;
    label.IsMandatory = true;
    label.Label = "Label";
    label.Length = 50;
    requestStatus.LabelAttribute = label;

    var order = requestStatus.CreateAttribute("Order");
    order.DataType = eSpace.IntegerType;
    order.IsMandatory = true;
    order.Label = "Order";
    requestStatus.OrderByAttribute = order;

    var is_Active = requestStatus.CreateAttribute("Is_Active");
    is_Active.DataType = eSpace.BooleanType;
    is_Active.IsMandatory = true;
    is_Active.Label = "Is Active";
    requestStatus.IsActiveAttribute = is_Active;

    var pendingRecord = requestStatus.CreateRecord();
    pendingRecord.Identifier = "Pending";
    pendingRecord.SetAttributeValue(order, "1");
    pendingRecord.SetAttributeValue(is_Active, "True");
    pendingRecord.SetAttributeValue(id, "1");
    pendingRecord.SetAttributeValue(label, "\"Pending\"");

    var acceptedRecord = requestStatus.CreateRecord();
    acceptedRecord.Identifier = "Accepted";
    acceptedRecord.SetAttributeValue(order, "2");
    acceptedRecord.SetAttributeValue(is_Active, "True");
    acceptedRecord.SetAttributeValue(id, "2");
    acceptedRecord.SetAttributeValue(label, "\"Accepted\"");

    var rejectedRecord = requestStatus.CreateRecord();
    rejectedRecord.Identifier = "Rejected";
    rejectedRecord.SetAttributeValue(order, "3");
    rejectedRecord.SetAttributeValue(is_Active, "True");
    rejectedRecord.SetAttributeValue(id, "3");
    rejectedRecord.SetAttributeValue(label, "\"Rejected\"");
}
