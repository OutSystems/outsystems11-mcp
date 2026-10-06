// Add a date picker widget to the PersonDetail screen to select the account's creation date. The date picker is placed at the end of the form, just before the Save button.
// Context: Movie database management application with Person server entity, among others.

using OutSystems.Model;
using OutSystems.Model.Expressions;

eSpace => {
    // Navigate to the MainFlow of the mobile application
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");

    // Get the PersonDetail screen where we'll add the Date Of Death field
    var personDetail = mainFlow
        .Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>()
        .Named("PersonDetail");

    // Get the layout block instance (top-level layout structure with menu)
    var layoutTopMenuInstance = personDetail
        .Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>()
        .Single();

    // Get the MainContent placeholder from the layout
    var placeholderContentWidget = layoutTopMenuInstance.PlaceholdersContent.FirstOrDefault(
        p => p.Placeholder?.Name == "MainContent"
    );

    // Get the Form1 widget that contains the person detail fields
    var form1 = placeholderContentWidget
        .Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IForm>()
        .Named("Form1");

    // Create a new container widget to hold the Date Of Death field and its label
    var container = form1.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();

    // Create a label widget to display "Date Of Death" text above the input field
    var label = container.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();

    // Get the text widget inside the label and set its content
    var textWidget = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)label.Widgets.First();
    textWidget.Text = "Date Of Death";

    // Create an input widget for the Date Of Death with a specific name identifier
    var input_DateOfDeath = container.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>(
        "Input_DateOfDeath"
    );

    // Set the enabled condition - only allow editing if user has admin role
    input_DateOfDeath.SetEnabled("CheckOSMDb_DPC_AdminRole()");

    // Configure the input type as Date to display a date picker
    input_DateOfDeath.InputType = ServiceStudio
        .Plugin
        .NRWidgets
        .Enumerations
        .InputType
        .Date;

    // Set the field as optional (not mandatory)
    input_DateOfDeath.SetMandatory("False");

    // Remove the max length restriction (not applicable for date inputs)
    input_DateOfDeath.MaxLength = null;

    // Bind the input to the DateOfDeath attribute from the Person entity
    input_DateOfDeath.SetVariable("GetPersonById.List.Current.Person.DateOfDeath");

    // Get the built-in OnChange event for potential future use
    var builtinEvent = input_DateOfDeath.OnChange;

    // Link the label to the input field for accessibility and proper form behavior
    label.TargetWidget = input_DateOfDeath;

    // Get the last container in the form (which contains the Save button)
    var lastContainer = (ServiceStudio.Plugin.NRWidgets.IContainer)form1.Widgets.ElementAt(^1);

    // Move the new container just before the last container (Save button)
    container.MoveBeforeSibling(lastContainer);
}
