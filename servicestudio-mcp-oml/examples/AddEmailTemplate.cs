// Create an email template named WelcomeEmail with input parameters for CustomerName and VerificationCode, containing a greeting, message body, and a link to the login screen
// Context: The module has a flow named Emails and a screen named Login in the Common flow

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var emails = eSpace.MobileFlows.Named("Emails");

    // Create the email template with a dynamic subject
    var welcomeEmail = emails.CreateEmail("WelcomeEmail");
    welcomeEmail.Description = "Welcome email sent to new users with a verification code.";
    welcomeEmail.SetSubject("\"Welcome to Our App!\"");

    // Input parameters for dynamic content
    var customerName = welcomeEmail.CreateInputParameter("CustomerName");
    customerName.DataType = eSpace.TextType;

    var verificationCode = welcomeEmail.CreateInputParameter("VerificationCode");
    verificationCode.DataType = eSpace.TextType;

    // Email body: a container with greeting, message, and a link
    var wrapper = welcomeEmail.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>("EmailWrapper");
    wrapper.SetStyle("\"email-max-width margin-auto\"");

    // Greeting
    var greeting = wrapper.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    greeting.SetValue("\"Hello \" + CustomerName + \"!\"");

    // Verification code display
    var codeContainer = wrapper.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    codeContainer.SetStyle("\"heading2 margin-bottom-s\"");

    var codeExpr = codeContainer.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    codeExpr.SetValue("VerificationCode");

    // Link to the login screen
    var link = wrapper.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILink>();
    link.Widgets.ToList().ForEach(x => x.Delete());

    var btnContainer = link.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    btnContainer.SetStyle("\"btn btn-primary\"");

    var btnText = btnContainer.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    btnText.Text = "Go to Login";

    var common = eSpace.MobileFlows.Named("Common");
    var loginScreen = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("Login");
    link.OnClick.Destination = loginScreen;
}
