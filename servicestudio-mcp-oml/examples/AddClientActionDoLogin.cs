// Create a client action DoLogin that takes Username and Password inputs, calls the system Login action, and returns Success and ErrorMessage based on the login result
// Context: The module has a reference to the (System) module

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var doLogin = eSpace.CreateClientAction("DoLogin");
    doLogin.Description = "Performs a login using the system action for the given username and password.";

    var username = doLogin.CreateInputParameter("Username");
    username.DataType = eSpace.TextType;

    var password = doLogin.CreateInputParameter("Password");
    password.DataType = eSpace.TextType;

    var startNode = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var login = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteClientActionNode>("Login").ConnectedBelow(startNode);
    var system = eSpace.References.Named("(System)");
    var loginAction = system.ClientActions.Named("Login");
    login.Action = loginAction;
    login.SetArgumentValue(loginAction.InputParameters.Named("Username"), "Username");
    login.SetArgumentValue(loginAction.InputParameters.Named("Password"), "Password");

    var ifSuccess = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(login);
    ifSuccess.SetCondition("Login.UserLoginResult.Success");
    ifSuccess.Label = "Success?";

    var assignSuccess = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifSuccess);
    assignSuccess.CreateAssignment("Success", "True");
    ifSuccess.TrueTarget = assignSuccess;

    var endSuccess = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignSuccess);

    var ifInvalidCredentials = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifSuccess);
    ifInvalidCredentials.SetCondition("Login.UserLoginResult.UserLoginFailureReason.InvalidCredentials");
    ifInvalidCredentials.Label = "Invalid credentials?";
    ifSuccess.FalseTarget = ifInvalidCredentials;

    var assignInvalidCredentials = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifInvalidCredentials);
    assignInvalidCredentials.CreateAssignment("ErrorMessage", "\"Invalid credentials.\"");
    ifInvalidCredentials.TrueTarget = assignInvalidCredentials;

    var endInvalidCredentials = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignInvalidCredentials);

    var ifTooManyAttempts = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifInvalidCredentials);
    ifTooManyAttempts.SetCondition("Login.UserLoginResult.UserLoginFailureReason.TooManyFailedLoginAttempts");
    ifTooManyAttempts.Label = "Too many failed login attempts?";
    ifInvalidCredentials.FalseTarget = ifTooManyAttempts;

    var assignTooManyAttempts = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifTooManyAttempts);
    assignTooManyAttempts.CreateAssignment("ErrorMessage", "\"Too many failed login attempts. Please try again in \" + Login.UserLoginResult.RetryAfterSeconds + \" seconds.\"");
    ifTooManyAttempts.TrueTarget = assignTooManyAttempts;

    var endTooManyAttempts = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignTooManyAttempts);

    var assignGenericError = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().Below(ifTooManyAttempts);
    assignGenericError.CreateAssignment("ErrorMessage", "\"Login operation failed.\"");
    ifTooManyAttempts.FalseTarget = assignGenericError;

    var endGenericError = doLogin.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignGenericError);

    var success = doLogin.CreateOutputParameter("Success");
    success.DataType = eSpace.BooleanType;

    var errorMessage = doLogin.CreateOutputParameter("ErrorMessage");
    errorMessage.DataType = eSpace.TextType;
}
