// Create a server action DownloadOrdersExcel that fetches all Order records using an aggregate and converts them to an Excel file
// Context: The module has a server entity named Order with attributes Id, CustomerName, Total, and CreatedOn, and a folder named Download

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var downloadOrdersExcel = eSpace.CreateServerAction("DownloadOrdersExcel");
    downloadOrdersExcel.Folder = eSpace.Folders.Named("Download");

    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");

    var startNode = downloadOrdersExcel.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var getOrders = downloadOrdersExcel.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetOrders").ConnectedBelow(startNode);
    getOrders.AsDatabaseAggregate.CreateSource(order);

    var recordListToExcel = downloadOrdersExcel.CreateNode<OutSystems.Model.Logic.Nodes.IRecordListToExcelNode>("RecordListToExcel").ConnectedBelow(getOrders);
    recordListToExcel.SetRecordList("GetOrders.List");
    recordListToExcel.AddAttributeSelection("Order.CustomerName");
    recordListToExcel.AddAttributeSelection("Order.Total");
    recordListToExcel.AddAttributeSelection("Order.CreatedOn");

    var assignNode = downloadOrdersExcel.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(recordListToExcel);
    assignNode.CreateAssignment("OrdersExcel", "RecordListToExcel");

    var endNode = downloadOrdersExcel.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignNode);

    var ordersExcel = downloadOrdersExcel.CreateOutputParameter("OrdersExcel");
    ordersExcel.DataType = eSpace.BinaryDataType;
}
