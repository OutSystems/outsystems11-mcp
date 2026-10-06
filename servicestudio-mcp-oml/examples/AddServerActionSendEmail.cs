// Create an email OrderShipped with a CustomerName input and a server action NotifyShipped that sends it to an Email input
// Context: Reactive module with a MainFlow UI flow (prefer a dedicated Emails flow with a light theme - TrueChange warns when a flow's theme exceeds 14KB). Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var flow = eSpace.MobileFlows.Named("MainFlow");
    var orderShipped = flow.CreateEmail("OrderShipped");
    orderShipped.SetSubject("\"Your order shipped\"");
    var customerName = orderShipped.CreateInputParameter("CustomerName");
    customerName.DataType = eSpace.TextType;
    var body = orderShipped.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    body.SetValue("\"Hi \" + CustomerName + \", your order is on its way.\"");

    var notifyShipped = eSpace.CreateServerAction("NotifyShipped");
    var email = notifyShipped.CreateInputParameter("Email");
    email.DataType = eSpace.EmailType;

    var startNode = notifyShipped.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();
    // Reactive emails are sent with the Mobile SendEmail node (OutSystems.Model.Logic.Mobile.Nodes).
    var send = notifyShipped.CreateNode<OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode>("SendOrderShipped").ConnectedBelow(startNode);
    send.Email = orderShipped;
    send.SetTo("Email");
    send.SetFrom("\"noreply@example.com\"");
    send.SetArgumentValue(orderShipped.InputParameters.Named("CustomerName"), "\"Customer\"");
    notifyShipped.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(send);
}
