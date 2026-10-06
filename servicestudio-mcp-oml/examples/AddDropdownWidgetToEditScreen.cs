// Add a dropdown widget to the AccountEdit screen to select the country of the account. The dropdown is populated with the list of countries from the Country static entity. The new widget is placed in the 4th position of the second column of the form.
// Context: Order management application with Account and CountryRegion server entities, among others.

using OutSystems.Model;
using OutSystems.Model.Expressions;

eSpace => {
    // Navigate to the MainFlow of the mobile application
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");

    // Get the AccountEdit screen where we'll add the Country dropdown
    var accountEdit = mainFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("AccountEdit");

    // Navigate through the widget hierarchy to find the correct location for the dropdown
    // Start with the layout block instance (top-level layout structure)
    var layoutTopInstance = accountEdit.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();

    // Get the MainContent placeholder from the layout
    var placeholderContentWidget = layoutTopInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");

    // Get the container widget within MainContent
    var container = placeholderContentWidget.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IContainer>().Single();

    // Get the first Columns2 block instance (two-column layout)
    var columns2Instance = container.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();

    // Get the Column1 placeholder from the Columns2 block
    var placeholderContentWidget2 = columns2Instance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Column1");

    // Get the Form1 widget that contains the account edit fields
    var form1 = placeholderContentWidget2.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IForm>().Named("Form1");

    // Get the second widget in the form (a container)
    var container2 = (ServiceStudio.Plugin.NRWidgets.IContainer)form1.Widgets.ElementAt(1);

    // Get the nested Columns2 block instance inside the form
    var columns2Instance2 = container2.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();

    // Get the Column2 placeholder where we'll add the Country dropdown
    var placeholderContentWidget3 = columns2Instance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Column2");

    // Create a new container widget to hold the Country dropdown and its label
    var container3 = placeholderContentWidget3.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    // Apply a base margin-top style to the container for spacing
    container3.SetStyle("\"margin-top-base\"");

    // Create a label widget to display "Country" text above the dropdown
    var label = container3.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Set custom style to remove bottom margin from the label
    label.CustomStyle = "margin-bottom: 0px;";

    // Get the text widget inside the label and set its content
    var textWidget = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)label.Widgets.First();
    textWidget.Text = "Country";

    // Create an If widget to conditionally display the dropdown
    // This ensures data is loaded before showing the dropdown
    var ifWidget = container3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IIfWidget>();
    // Set the condition: only show dropdown when both country regions and account data are fetched
    ifWidget.SetCondition("GetCountryRegions.IsDataFetched and GetAccountById.IsDataFetched");
    // Set design mode to show the true branch (the dropdown) in preview
    ifWidget.DesignMode = OutSystems.Model.Enumerations.DesignMode.ShowTrueOrPreview;

    // Create a DropdownSearch widget instance inside the If widget's true branch
    var dropdownSearchInstance = ifWidget.TrueBranch.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();

    // Get reference to the OutSystemsUI module that contains the DropdownSearch block
    var outSystemsUI = eSpace.References.Named("OutSystemsUI");
    // Navigate to the Interaction flow in OutSystemsUI
    var interaction = outSystemsUI.MobileFlows.Named("Interaction");
    // Get the DropdownSearch block signature
    var dropdownSearch = interaction.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("DropdownSearch");
    // Set the DropdownSearch block as the source for this instance
    dropdownSearchInstance.SourceBlock = dropdownSearch;

    // Configure the OptionsList parameter - this populates the dropdown options
    var optionsList = dropdownSearch.InputParameters.Named("OptionsList");
    // Map the country regions list to the dropdown format
    // Value: CountryRegion.Id, Label: Country2.Name
    // WARNING: `new ExpressionDefinition.TypeConversion(...)` compiles but HARD-CRASHES the in-process MCP
    // sidecar (empty stdout + empty mutatedOmlPath, uncatchable) — verified 2026-07-28 on the chart
    // DataPointList binding in AddDashboardScreenWithDonutChart.cs. Over MCP, bind via the parser instead.
    // (The parser form below is the documented replacement; it has not been re-verified for DropdownSearch
    // specifically, so read back with getScreen after applying it.)
    dropdownSearchInstance.SetArgumentValue(optionsList, ExpressionDefinition.Parse("GetCountryRegions.List mapTo { Value: CountryRegion.Id, Label: Country2.Name }"));

    // Configure the StartingSelection parameter - this sets the initially selected country
    var startingSelection = dropdownSearch.InputParameters.Named("StartingSelection");
    // Set the current account's country as the initial selection
    // List/record literals are written as parser text: C# 12 collection expressions ([...]) are rejected by the MCP sandbox pre-parser.
    dropdownSearchInstance.SetArgumentValue(startingSelection, ExpressionDefinition.Parse("[ { Value: GetAccountById.List.Current.Account.AccountCountryId, Label: GetAccountById.List.Current.Country2.Name } ]"));

    // Configure the Prompt parameter - this sets the search placeholder text
    var prompt = dropdownSearch.InputParameters.Named("Prompt");
    dropdownSearchInstance.SetArgumentValue(prompt, "\"Search by Country\" + Dummy");

    // Wire up the OnChanged event handler
    // This event fires when the user selects a different country
    var eventHandler = dropdownSearchInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "OnChanged");
    // Get the DropdownSearch_Country screen action that handles country selection
    var dropdownSearch_Country = accountEdit.ScreenActions.Named("DropdownSearch_Country");
    // Set the screen action as the event handler
    eventHandler.Handler = dropdownSearch_Country;
    // Map the SelectedOptionList parameter to pass the selected country to the handler
    var selectedOptionList = dropdownSearch_Country.InputParameters.Named("SelectedOptionList");
    eventHandler.SetArgumentValue(selectedOptionList, "SelectedOptionList");

    // Get the Initialized event handler (not currently used but available for future logic)
    var eventHandler2 = dropdownSearchInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "Initialized");

    // Move the container to position 3 in the widget order to ensure proper placement in the form
    container3.MoveToNewAbsoluteIndex(3);
}
