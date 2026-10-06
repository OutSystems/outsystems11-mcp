// Create a user management screen to edit patient details with access restricted to specific roles, featuring data validation on save, an option to delete the record, and fetching existing data via an aggregate.
// Context: Healthcare portal app including an entity named Patient, among other entities.

using OutSystems.Model;
using OutSystems.Model.Enumerations;

eSpace => {
    // Get a reference to the 'PatientFlow' within the eSpace.
    var patientFlow = eSpace.MobileFlows.Named("PatientFlow");
    // Get a reference to the existing 'Patient' screen (likely the listing or detail screen).
    var patient = patientFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("Patient");

    // --- SCREEN DEFINITION: PatientEdit ---
    // Create the new 'PatientEdit' screen.
    var patientEdit = patientFlow.CreateScreen("PatientEdit");
    /*** Deleting Widgets that are created by default when instantiating an IMobileScreen ***/
    // Remove any default widgets created by the platform.
    patientEdit.Widgets.ToList().ForEach(x => x.Delete());

    // --- SECURITY ROLES ---
    // Assign multiple security roles that are allowed to access this screen.
    var admin = eSpace.Roles.Named("Admin");
    patientEdit.Roles.Add(admin);
    var manager = eSpace.Roles.Named("Manager");
    patientEdit.Roles.Add(manager);
    var patient2 = eSpace.Roles.Named("Patient");
    patientEdit.Roles.Add(patient2);
    var doctor = eSpace.Roles.Named("Doctor");
    patientEdit.Roles.Add(doctor);

    // Set the visual positioning (X/Y coordinates) of the screen in the flow diagram.
    patientEdit.HorizontalPosition = 11654;
    patientEdit.VerticalPosition = 5713;

    // --- INPUT PARAMETER ---
    // Define the input parameter for the screen, used to identify which patient record to load/edit.
    var patientId = patientEdit.CreateInputParameter("PatientId");
    // The data type is set to the Identifier type of the Patient entity.
    var patient4 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Patient");
    patientId.DataType = patient4.IdentifierType;

    // --- DATA AGGREGATE: GetPatientById ---
    // Create a Screen Aggregate to fetch the patient details for editing.
    var getPatientById = patientEdit.CreateScreenAggregate(false, "GetPatientById");
    // Limit the aggregate to return only one record.
    getPatientById.SetMaxRecords("1");
    // Add the 'Patient' entity as the source.
    var patient3 = getPatientById.AsDatabaseAggregate.CreateSource(patient4);
    // Filter the records based on the input PatientId.
    getPatientById.AsDatabaseAggregate.CreateFilter("Patient.Id = PatientId");
    // Add a custom function call as a filter for authorization/security before fetching data.
    getPatientById.AsDatabaseAggregate.CreateFilter("CanEditPatient(Patient.Id,Patient.Id)");


    // --- NAVIGATION LINK FIX (Linking from another screen) ---
    // Locate the existing 'Patient' screen's UI elements to find the button that should link to this 'PatientEdit' screen.
    var layoutSideMenuInstance = patient.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();
    var placeholderContentWidget = layoutSideMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");
    var container = placeholderContentWidget.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IContainer>().Single();
    var actions = container.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IContainer>().Named("Actions");
    var button = actions.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IButton>().Single();
    // Set the existing button's destination to the new 'PatientEdit' screen.
    button.OnClick.Destination = patientEdit;

    // --- SCREEN ACTION: Delete ---
    // Action triggered when the 'Delete' button is pressed.
    var delete = patientEdit.CreateScreenAction("Delete");

    var startNode = delete.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    // Execute Server Action: Calls the server action to perform the actual database deletion.
    var patientDelete = delete.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("PatientDelete").ConnectedBelow(startNode);
    var patientDelete2 = eSpace.ServerActions.Named("PatientDelete");
    patientDelete.Action = patientDelete2;
    // Pass the ID of the current patient to the delete action.
    var id = patientDelete2.InputParameters.Named("Id");
    patientDelete.SetArgumentValue(id, "GetPatientById.List.Current.Patient.Id");

    // Display Success Message after deletion.
    var messageNode = delete.CreateNode<OutSystems.Model.Logic.Nodes.IMessageNode>().ConnectedBelow(patientDelete);
    messageNode.SetMessage("\"Element was successfully deleted.\"");
    messageNode.Type = OutSystems.Model.Enumerations.MessageType.Success;

    // Navigate to the 'Patients' list screen after success.
    var destinationNode = delete.CreateNode<OutSystems.Model.Logic.Nodes.IDestinationNode>().ConnectedBelow(messageNode);
    var patients = patientFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("Patients");
    destinationNode.Destination = patients;
    destinationNode.Transition = OutSystems.Model.Enumerations.TransitionTypeWithInheritance.None;

    // --- SCREEN ACTION: Save ---
    // Action triggered when the 'Save' button is pressed (handles Create or Update).
    var save = patientEdit.CreateScreenAction("Save");

    var startNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    // Client Action Call: First, call a client-side action to validate specific input fields.
    var validatePatientInput = save.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteClientActionNode>("ValidatePatientInput");
    var validatePatientInput2 = eSpace.ClientActions.Named("ValidatePatientInput");
    validatePatientInput.Action = validatePatientInput2;
    // Pass the entire patient record from the Aggregate as the source for validation.
    var source = validatePatientInput2.InputParameters.Named("Source");
    validatePatientInput.SetArgumentValue(source, "GetPatientById.List.Current.Patient");
    startNode2.Target = validatePatientInput;

    // IF node: Check validation success for FullName.
    var ifNode = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(validatePatientInput, 1372);
    ifNode.SetCondition("ValidatePatientInput.ValidName.IsSuccess");
    ifNode.Label = "ValidName";

    // ASSIGN node (FullName Validation Failure): Set the FullName input field to invalid and show the error message.
    var assignNode = save.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode, 1600);
    assignNode.Label = "Set Error";
    var assignment = assignNode.CreateAssignment("Input_FullName.Valid", "False");
    var assignment2 = assignNode.CreateAssignment("Input_FullName.ValidationMessage", "ValidatePatientInput.ValidName.ErrorMessage");
    ifNode.FalseTarget = assignNode;

    // IF node: Check validation success for DateOfBirth.
    var ifNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifNode, 1371);
    ifNode2.SetCondition("ValidatePatientInput.ValidDateOfBirth.IsSuccess");
    ifNode2.Label = "ValidDateOfBirth";
    ifNode.TrueTarget = ifNode2;
    assignNode.Target = ifNode2; // Connects failure path of ValidName to the next check.

    // ASSIGN node (DateOfBirth Validation Failure): Set the DateOfBirth input field to invalid and show the error message.
    var assignNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode2, 1600);
    assignNode2.Label = "Set Error";
    var assignment3 = assignNode2.CreateAssignment("Input_DateOfBirth.Valid", "False");
    var assignment4 = assignNode2.CreateAssignment("Input_DateOfBirth.ValidationMessage", "ValidatePatientInput.ValidDateOfBirth.ErrorMessage");
    ifNode2.FalseTarget = assignNode2;

    // IF node: Check validation success for Phone Number.
    var ifNode3 = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifNode2, 1372);
    ifNode3.SetCondition("ValidatePatientInput.ValidPhone.IsSuccess");
    ifNode3.Label = "ValidPhone";
    ifNode2.TrueTarget = ifNode3;
    assignNode2.Target = ifNode3; // Connects failure path of ValidDateOfBirth to the next check.

    // ASSIGN node (Phone Validation Failure): Set the Phone Number input field to invalid and show the error message.
    var assignNode3 = save.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode3, 1600);
    assignNode3.Label = "Set Error";
    var assignment5 = assignNode3.CreateAssignment("Input_AttributePhoneNumber.Valid", "False");
    var assignment6 = assignNode3.CreateAssignment("Input_AttributePhoneNumber.ValidationMessage", "ValidatePatientInput.ValidPhone.ErrorMessage");
    ifNode3.FalseTarget = assignNode3;

    // IF node: Check validation success for Email.
    var ifNode4 = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifNode3, 1371);
    ifNode4.SetCondition("ValidatePatientInput.ValidEmail.IsSuccess");
    ifNode4.Label = "ValidEmail";
    ifNode3.TrueTarget = ifNode4;
    assignNode3.Target = ifNode4; // Connects failure path of ValidPhone to the next check.

    // ASSIGN node (Email Validation Failure): Set the Email input field to invalid and show the error message.
    var assignNode4 = save.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode4, 1600);
    assignNode4.Label = "Set Error";
    var assignment7 = assignNode4.CreateAssignment("Input_Email.Valid", "False");
    var assignment8 = assignNode4.CreateAssignment("Input_Email.ValidationMessage", "ValidatePatientInput.ValidEmail.ErrorMessage");
    ifNode4.FalseTarget = assignNode4;

    // IF node: Final check for overall form validity (including built-in platform validations).
    var ifNode5 = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().Below(ifNode4, 1371);
    ifNode5.SetCondition("Form.Valid");
    ifNode4.TrueTarget = ifNode5;
    assignNode4.Target = ifNode5; // Connects failure path of ValidEmail to the final check.

    // MESSAGE node (Form Invalid): Display a generic error message if validation failed.
    var messageNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IMessageNode>().ToTheRightOf(ifNode5, 1600);
    messageNode2.SetMessage("\"Please fix the existing validation errors.\"");
    messageNode2.Type = OutSystems.Model.Enumerations.MessageType.Error;
    ifNode5.FalseTarget = messageNode2;

    // Execute Server Action: If the form is valid, call the server action to save the patient data.
    var patientCreateOrUpdate = save.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("PatientCreateOrUpdate").Below(ifNode5, 1600);
    var patientCreateOrUpdate2 = eSpace.ServerActions.Named("PatientCreateOrUpdate");
    patientCreateOrUpdate.Action = patientCreateOrUpdate2;
    // Set UserId to Null (not relevant for patient creation in this context).
    var userId = patientCreateOrUpdate2.InputParameters.Named("UserId");
    patientCreateOrUpdate.SetArgumentValue(userId, "NullTextIdentifier()");
    // Pass the potentially modified patient record from the Aggregate's current variable.
    var source2 = patientCreateOrUpdate2.InputParameters.Named("Source");
    patientCreateOrUpdate.SetArgumentValue(source2, "GetPatientById.List.Current.Patient");
    ifNode5.TrueTarget = patientCreateOrUpdate;

    // End node for the failure branch.
    var endNode = save.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedToTheRightOf(messageNode2, 1371);

    // IF node: Check the success status returned by the server action.
    var ifNode6 = save.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(patientCreateOrUpdate, 1372);
    ifNode6.SetCondition("PatientCreateOrUpdate.IsSuccess");
    ifNode6.Label = "IsSuccess?";

    // MESSAGE node (Server Action Error): Display the specific error message from the server action.
    var messageNode3 = save.CreateNode<OutSystems.Model.Logic.Nodes.IMessageNode>().ToTheRightOf(ifNode6, 1600);
    messageNode3.Label = "Error Message";
    messageNode3.SetMessage("PatientCreateOrUpdate.ErrorMessage");
    ifNode6.FalseTarget = messageNode3;

    // MESSAGE node (Server Action Success): Display a dynamic success message ("created" or "updated").
    var messageNode4 = save.CreateNode<OutSystems.Model.Logic.Nodes.IMessageNode>().Below(ifNode6, 1371);
    messageNode4.Label = "Success Message";
    // Dynamic message based on whether PatientId was provided (Update) or not (Create).
    messageNode4.SetMessage("\"Element was successfully \"+\nIf(PatientId = NullTextIdentifier(),\"created\",\"updated\")");
    messageNode4.Type = OutSystems.Model.Enumerations.MessageType.Success;
    ifNode6.TrueTarget = messageNode4;

    // End node for the server action failure branch.
    var endNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedToTheRightOf(messageNode3, 1371);

    // Destination node (Success): Navigate back to the previous screen.
    var destinationNode2 = save.CreateNode<OutSystems.Model.Logic.Nodes.IDestinationNode>().ConnectedBelow(messageNode4, 1372);
    destinationNode2.Destination = eSpace.PreviousScreen;
    destinationNode2.Transition = OutSystems.Model.Enumerations.TransitionTypeWithInheritance.UseHistory;


    // --- UI WIDGET IMPLEMENTATION ---

    // Use the common LayoutSideMenu block for the screen structure.
    var layoutSideMenuInstance2 = patientEdit.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var layouts = eSpace.MobileFlows.Named("Layouts");
    var layoutSideMenu = layouts.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("LayoutSideMenu");
    layoutSideMenuInstance2.SourceBlock = layoutSideMenu;

    // Get and clear default content from the 'Navigation' placeholder (used for the side menu content).
    var placeholderContentWidget2 = layoutSideMenuInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Navigation");
    placeholderContentWidget2.Widgets.ToList().ForEach(w => w.Delete());

    // Set input parameters for the layout block (using default behavior).
    var hasFixedHeader = layoutSideMenu.InputParameters.Named("HasFixedHeader");
    layoutSideMenuInstance2.SetArgumentValue(hasFixedHeader, null);
    var extendedClass = layoutSideMenu.InputParameters.Named("ExtendedClass");
    layoutSideMenuInstance2.SetArgumentValue(extendedClass, null);

    // Add the 'Menu' block to the 'Navigation' placeholder.
    var menuInstance = placeholderContentWidget2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var common = eSpace.MobileFlows.Named("Common");
    var menu = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("Menu");
    menuInstance.SourceBlock = menu;
    // Set input parameters for menu navigation.
    var activeSubItem = menu.InputParameters.Named("ActiveSubItem");
    menuInstance.SetArgumentValue(activeSubItem, null);
    var activeItem = menu.InputParameters.Named("ActiveItem");
    menuInstance.SetArgumentValue(activeItem, null);

    // --- BREADCRUMBS/BACK LINK ---
    // Configure the 'Breadcrumbs' placeholder content as a back link.
    var placeholderContentWidget3 = layoutSideMenuInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Breadcrumbs");
    var link = placeholderContentWidget3.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILink>();
    /*** Deleting Widgets that are created by default when instantiating an ILink ***/
    link.Widgets.ToList().ForEach(x => x.Delete());
    link.SetEnabled("True");

    // Text widget for the '‹' icon.
    var textWidget = link.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget.CustomStyle = "vertical-align: 1px;";
    textWidget.Text = "‹";

    // Text widget for the "Back" label.
    var textWidget2 = link.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget2.CustomStyle = "margin-left: auto;";
    textWidget2.Text = " Back";

    // Set the OnClick event for the back link.
    var builtinEvent = link.OnClick;
    // Platform validation is applied, but navigation continues regardless of minor validation issues.
    builtinEvent.BuiltInValidations = OutSystems.Model.Enumerations.ValidationBehavior.ValidateAndContinue;
    // Destination is always the previous screen in the navigation history.
    builtinEvent.Destination = eSpace.PreviousScreen;
    builtinEvent.Transition = OutSystems.Model.Enumerations.TransitionTypeWithInheritance.UseHistory;

    // --- SCREEN TITLE ---
    // Get the 'Title' placeholder content.
    var placeholderContentWidget4 = layoutSideMenuInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Title");
    // Add the 'user' icon next to the title.
    var icon = placeholderContentWidget4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon.CustomStyle = "margin-right: 5px;";
    icon.Icon = "user";
    icon.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;

    // Add an AdvancedHtml widget configured as an H1 tag for the main title text.
    var screenTitle = placeholderContentWidget4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IAdvancedHtml>("ScreenTitle");
    screenTitle.Tag = "h1";

    // Dynamic Expression: Sets the title to "Edit Patient" or "New Patient" based on the PatientId input parameter.
    var expression = screenTitle.Content.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression.SetValue("If(PatientId <> NullTextIdentifier(), \"Edit\", \"New\") + \" Patient\"");

    // --- FORM LAYOUT AND FIELDS ---
    // Get the 'MainContent' placeholder for the form.
    var placeholderContentWidget5 = layoutSideMenuInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");
    // Use the OutSystemsUI Columns2 block for responsive layout (dividing the content area).
    var columns2Instance = placeholderContentWidget5.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var outSystemsUI = eSpace.References.Named("OutSystemsUI");
    var adaptive = outSystemsUI.MobileFlows.Named("Adaptive");
    var columns2 = adaptive.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("Columns2");
    columns2Instance.SourceBlock = columns2;
    // Configure responsiveness for tablet/phone.
    var tabletBehavior = columns2.InputParameters.Named("TabletBehavior");
    columns2Instance.SetArgumentValue(tabletBehavior, null);
    // On phone, all columns break to a single column stack.
    var phoneBehavior = columns2.InputParameters.Named("PhoneBehavior");
    columns2Instance.SetArgumentValue(phoneBehavior, "Entities.BreakColumns.All");
    var gutterSize = columns2.InputParameters.Named("GutterSize");
    columns2Instance.SetArgumentValue(gutterSize, null);

    // Get the first column placeholder ('Column1').
    var placeholderContentWidget6 = columns2Instance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Column1");
    // Create the main Form widget.
    var form = placeholderContentWidget6.CreateWidget<ServiceStudio.Plugin.NRWidgets.IForm>("Form");

    // 1. FULL NAME Input
    var container2 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container2.CustomStyle = "margin-top: 0px;";

    var label = container2.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Clear default label content.
    label.Widgets.ToList().ForEach(x => x.Delete());
    // Add 'heartbeat' icon to the label.
    var icon2 = label.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon2.CustomStyle = "margin-right: 5px;";
    icon2.Icon = "heartbeat";
    icon2.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;
    // Create the Input widget.
    var input_FullName = container2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_FullName");
    input_FullName.SetEnabled("True");
    input_FullName.SetMandatory("True");
    input_FullName.MaxLength = 250;
    // Bind to the Patient record fetched by the Aggregate.
    input_FullName.SetVariable("GetPatientById.List.Current.Patient.FullName");
    var builtinEvent2 = input_FullName.OnChange;
    // Link the label to the input field.
    label.TargetWidget = input_FullName;
    // Set the label text.
    var textWidget3 = label.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget3.Text = "Full Name";

    // 2. DATE OF BIRTH Input
    var container3 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container3.CustomStyle = "margin-top: 0px;";

    var label2 = container3.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Clear default label content.
    label2.Widgets.ToList().ForEach(x => x.Delete());
    // Add 'calendar' icon to the label.
    var icon3 = label2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon3.CustomStyle = "height: 21px; margin-right: 5px;";
    icon3.Icon = "calendar";
    icon3.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;
    icon3.SetStyle(null);
    // Create the Date Input widget.
    var input_DateOfBirth = container3.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_DateOfBirth");
    input_DateOfBirth.SetEnabled("True");
    input_DateOfBirth.InputType = ServiceStudio.Plugin.NRWidgets.Enumerations.InputType.Date;
    input_DateOfBirth.SetMandatory("True"); // Note: This field is marked as mandatory here.
    input_DateOfBirth.MaxLength = null;
    // Bind to the Patient record.
    input_DateOfBirth.SetVariable("GetPatientById.List.Current.Patient.DateOfBirth");
    var builtinEvent3 = input_DateOfBirth.OnChange;
    label2.TargetWidget = input_DateOfBirth;
    // Set the label text.
    var textWidget4 = label2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget4.Text = "Date of Birth";

    // 3. GENDER Input
    var container4 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container4.CustomStyle = "margin-top: 0px;";

    var label3 = container4.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Set the label text for Gender.
    var textWidget5 = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)label3.Widgets.First();
    textWidget5.Text = "Gender";
    // Create the Input widget.
    var input_Gender = container4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_Gender");
    input_Gender.SetEnabled("True");
    input_Gender.SetMandatory("False");
    // Bind to the Patient record.
    input_Gender.SetVariable("GetPatientById.List.Current.Patient.Gender");
    var builtinEvent4 = input_Gender.OnChange;
    label3.TargetWidget = input_Gender;

    // 4. PHONE NUMBER Input
    var container5 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();

    var label4 = container5.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Clear default label content.
    label4.Widgets.ToList().ForEach(x => x.Delete());
    // Add 'phone' icon to the label.
    var icon4 = label4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon4.CustomStyle = "margin-right: 5px;";
    icon4.Icon = "phone";
    icon4.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;
    icon4.SetStyle(null);
    // Create the Phone Input widget.
    var input_AttributePhoneNumber = container5.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_AttributePhoneNumber");
    input_AttributePhoneNumber.SetEnabled("True");
    input_AttributePhoneNumber.InputType = ServiceStudio.Plugin.NRWidgets.Enumerations.InputType.Phone;
    input_AttributePhoneNumber.SetMandatory("True"); // Note: This field is marked as mandatory here.
    input_AttributePhoneNumber.MaxLength = 20;
    // Bind to the Patient record.
    input_AttributePhoneNumber.SetVariable("GetPatientById.List.Current.Patient.PhoneNumber");
    var builtinEvent5 = input_AttributePhoneNumber.OnChange;
    label4.TargetWidget = input_AttributePhoneNumber;
    // Set the label text.
    var textWidget6 = label4.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget6.Text = "Phone Number";

    // 5. EMAIL Input
    var container6 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container6.CustomStyle = "margin-top: 0px;";

    var label5 = container6.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Clear default label content.
    label5.Widgets.ToList().ForEach(x => x.Delete());
    // Add 'envelope-o' (email) icon to the label.
    var icon5 = label5.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon5.CustomStyle = "margin-right: 5px;";
    icon5.Icon = "envelope-o";
    icon5.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;
    icon5.SetStyle(null);
    // Create the Email Input widget.
    var input_Email = container6.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_Email");
    input_Email.SetEnabled("True");
    input_Email.InputType = ServiceStudio.Plugin.NRWidgets.Enumerations.InputType.Email;
    input_Email.SetMandatory("False");
    input_Email.MaxLength = 256;
    // Bind to the Patient record.
    input_Email.SetVariable("GetPatientById.List.Current.Patient.Email");
    var builtinEvent6 = input_Email.OnChange;
    label5.TargetWidget = input_Email;
    // Set the label text.
    var textWidget7 = label5.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget7.Text = "Email";

    // 6. IS ACTIVE Checkbox
    var container7 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container7.CustomStyle = "margin-top: 0px;";

    var label6 = container7.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILabel>();
    // Set the label text for Is Active.
    var textWidget8 = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)label6.Widgets.First();
    textWidget8.Text = "Is Active";
    // Create the Checkbox widget.
    var checkbox1 = container7.CreateWidget<ServiceStudio.Plugin.NRWidgets.ICheckbox>("Checkbox1");
    checkbox1.SetEnabled("True");
    // Bind to the Patient record.
    checkbox1.SetVariable("GetPatientById.List.Current.Patient.IsActive");
    var builtinEvent7 = checkbox1.OnChange;
    label6.TargetWidget = checkbox1;

    // --- ACTION BUTTONS ---
    // Container for the Delete and Save buttons (right-aligned).
    var actions2 = form.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>("Actions");
    actions2.CustomStyle = "margin-top: var(--space-m); text-align: right;";

    // DELETE Button
    var deleteButton = actions2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButton>("DeleteButton");
    // Add confirmation message before executing deletion.
    deleteButton.SetConfirmationMessage("\"Are you sure you want to delete this element? This operation can't be undone.\"");
    deleteButton.SetEnabled("True");
    // Set button visibility: only show if a record is being edited (PatientId is not null).
    deleteButton.SetVisible("PatientId <> NullTextIdentifier()");
    // Set the button text.
    var textWidget9 = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)deleteButton.Widgets.First();
    textWidget9.Text = "Delete";
    // Link the OnClick event to the 'Delete' action.
    var builtinEvent8 = deleteButton.OnClick;
    builtinEvent8.Destination = delete;
    builtinEvent8.Transition = OutSystems.Model.Enumerations.TransitionTypeWithInheritance.UseHistory;

    // SAVE Button
    var save2 = actions2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButton>("Save");
    save2.SetEnabled("True");
    save2.IsDefault = true;
    save2.SetStyle("\"btn btn-primary\"");
    // Set the button text.
    var textWidget10 = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)save2.Widgets.First();
    textWidget10.Text = "Save";
    // Link the OnClick event to the 'Save' action.
    var builtinEvent9 = save2.OnClick;
    // Platform validation is applied, and navigation continues to the action regardless of minor issues (handled by the 'Save' logic).
    builtinEvent9.BuiltInValidations = OutSystems.Model.Enumerations.ValidationBehavior.ValidateAndContinue;
    builtinEvent9.Destination = save;
}
