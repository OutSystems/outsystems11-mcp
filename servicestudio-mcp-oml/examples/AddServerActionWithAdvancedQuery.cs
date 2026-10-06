// Create a server action SumBigOrders that runs an SQL (advanced query) node filtered by a MinAmount input, loops the result with a For Each, sums Order.Amount into a Total output, and handles AllExceptions on its own End
// Context: The module has a server entity named Order with a Decimal attribute Amount. Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");

    var sumBigOrders = eSpace.CreateServerAction("SumBigOrders");
    var minAmount = sumBigOrders.CreateInputParameter("MinAmount");
    minAmount.DataType = eSpace.DecimalType;
    var total = sumBigOrders.CreateOutputParameter("Total");
    total.DataType = eSpace.DecimalType;

    // Server actions are created with NO nodes - add the Start yourself.
    var startNode = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    // SQL node: declare the parameter, bind its argument, declare the output entity, then the statement.
    var getBigOrders = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.ISQLNode>("GetBigOrders").ConnectedBelow(startNode);
    var sqlMinAmount = getBigOrders.CreateInputParameter("MinAmount");
    sqlMinAmount.DataType = eSpace.DecimalType;
    getBigOrders.SetArgumentValue(sqlMinAmount, "MinAmount");
    getBigOrders.CreateOutput(order);
    getBigOrders.Statement = "SELECT {Order}.* FROM {Order} WHERE {Order}.[Amount] >= @MinAmount";

    // For Each: CycleTarget = first body node; the last body node's Target = the For Each; Target = exit path.
    var forEachNode = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IForEachNode>().ConnectedBelow(getBigOrders);
    forEachNode.SetRecordList("GetBigOrders.List");
    var accumulate = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(forEachNode);
    accumulate.CreateAssignment("Total", "Total + GetBigOrders.List.Current.Order.Amount");
    accumulate.Target = forEachNode;
    forEachNode.CycleTarget = accumulate;

    var endNode = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().Below(forEachNode);
    forEachNode.Target = endNode;

    // Exception handler: its path must finish on its OWN End (sharing the main End is a TrueChange error).
    var allExceptions = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IExceptionHandlerNode>("AllExceptions");
    allExceptions.Exception = eSpace.AllExceptions;
    allExceptions.AbortTransaction = false;
    var onError = sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(allExceptions);
    onError.CreateAssignment("Total", "-1");
    sumBigOrders.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(onError);
}
