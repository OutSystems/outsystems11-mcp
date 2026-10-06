// Create a client action ShowConfirmationMessage that takes a MessageText input and uses a JavaScript node to show a browser confirm dialog, returning whether the user confirmed
// Context: The module has no relevant pre-existing elements

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var showConfirmationMessage = eSpace.CreateClientAction("ShowConfirmationMessage");
    showConfirmationMessage.Description = "Shows a confirmation message dialog for the user.";

    var messageText = showConfirmationMessage.CreateInputParameter("MessageText");
    messageText.DataType = eSpace.TextType;

    var startNode = showConfirmationMessage.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var confirmationMessage = showConfirmationMessage.CreateNode<OutSystems.Model.Logic.Nodes.IJavaScriptNode>("ConfirmationMessage").ConnectedBelow(startNode);
    confirmationMessage.JavaScript = "$parameters.IsConfirmed = confirm($parameters.Message);";

    var message = confirmationMessage.CreateInputParameter("Message");
    message.DataType = eSpace.TextType;

    var jsIsConfirmed = confirmationMessage.CreateOutputParameter("IsConfirmed");
    jsIsConfirmed.DataType = eSpace.BooleanType;

    confirmationMessage.SetArgumentValue(message, "MessageText");

    var assignNode = showConfirmationMessage.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(confirmationMessage);
    assignNode.CreateAssignment("IsConfirmed", "ConfirmationMessage.IsConfirmed");

    var endNode = showConfirmationMessage.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignNode);

    var isConfirmed = showConfirmationMessage.CreateOutputParameter("IsConfirmed");
    isConfirmed.DataType = eSpace.BooleanType;
}
