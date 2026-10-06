// Create a screen for listing customers with data sourced from a multi-table aggregate, enabling search and category filtering, dynamic column sorting, and pagination.
// Context: Home banking backoffice module contains an entity named Customer, among other entities.

using OutSystems.Model;
using OutSystems.Model.Enumerations;

eSpace => {
    // Get a reference to the 'MainFlow' within the current eSpace (module).
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");

    // Create the main 'Customers' screen within the mobile flow.
    var customers = mainFlow.CreateScreen("Customers");

    /*** Deleting Widgets that are created by default when instantiating an IMobileScreen ***/
    // Remove any default widgets that OutSystems typically creates on a new screen.
    customers.Widgets.ToList().ForEach(x => x.Delete());

    // Retrieve the 'BankingManager' security role from the eSpace.
    var bankingManager = eSpace.Roles.Named("BankingManager");

    // Assign the 'BankingManager' role to the 'Customers' screen, restricting access.
    customers.Roles.Add(bankingManager);

    // Set the visual positioning (X/Y coordinates) of the screen in the flow diagram (Service Studio).
    customers.HorizontalPosition = 2508;
    customers.VerticalPosition = 912;

    // --- AGGREGATE DEFINITION: GetCustomersWithAccounts ---
    // Create a new Aggregate (data query) for the screen.
    var getCustomersWithAccounts = customers.CreateScreenAggregate(false, "GetCustomersWithAccounts");

    // Map the Aggregate's MaxRecords property to a Screen Local Variable named "MaxRecords" for pagination.
    getCustomersWithAccounts.SetMaxRecords("MaxRecords");

    // Map the Aggregate's StartIndex property to a Screen Local Variable named "StartIndex" for pagination.
    getCustomersWithAccounts.SetStartIndex("StartIndex");

    // Retrieve entity references and add them as sources (tables) to the Aggregate.
    var customer2 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Customer");
    var customer = getCustomersWithAccounts.AsDatabaseAggregate.CreateSource(customer2);
    var account2 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Account");
    var account = getCustomersWithAccounts.AsDatabaseAggregate.CreateSource(account2);
    var transaction2 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Transaction");
    var transaction = getCustomersWithAccounts.AsDatabaseAggregate.CreateSource(transaction2);
    // Static entity for transaction types (e.g., IsIncrease/IsDecrease).
    var transactionType2 = eSpace.Entities.OfType<OutSystems.Model.Data.IStaticEntity>().Named("TransactionType");
    var transactionType = getCustomersWithAccounts.AsDatabaseAggregate.CreateSource(transactionType2);

    // Define Joins to link the tables together.

    // Join 1: Customer (Left) to Account (Right) - LEFT JOIN: Include all customers, even those without accounts.
    var join = getCustomersWithAccounts.AsDatabaseAggregate.CreateJoin();
    join.LeftSource = customer;
    join.RightSource = account;
    join.JoinType = OutSystems.Model.Enumerations.JoinType.Left;
    join.SetCondition("Customer.Id = Account.CustomerId");

    // Join 2: Account (Left) to Transaction (Right) - LEFT JOIN: Include all accounts, even those without transactions.
    var join2 = getCustomersWithAccounts.AsDatabaseAggregate.CreateJoin();
    join2.LeftSource = account;
    join2.RightSource = transaction;
    join2.JoinType = OutSystems.Model.Enumerations.JoinType.Left;
    join2.SetCondition("Account.Id = Transaction.AccountId");

    // Join 3: Transaction (Left) to TransactionType (Right) - INNER JOIN: Only include transactions with a valid type.
    var join3 = getCustomersWithAccounts.AsDatabaseAggregate.CreateJoin();
    join3.LeftSource = transaction;
    join3.RightSource = transactionType;
    join3.JoinType = OutSystems.Model.Enumerations.JoinType.Inner;
    join3.SetCondition("Transaction.TypeId = TransactionType.Id");

    // Calculated Attribute: Determines the transaction amount with the correct sign (positive for increase, negative for decrease).
    getCustomersWithAccounts.AsDatabaseAggregate.CreateCalculatedAttribute("AmountWithSign").SetValue("If(TransactionType.IsIncrease,Transaction.Amount,-Transaction.Amount)");

    // Filter 1: Limits the results to customers with Checking or Saving accounts only.
    getCustomersWithAccounts.AsDatabaseAggregate.CreateFilter("Account.ProductTypeId= Entities.ProductType.Checking or Account.ProductTypeId= Entities.ProductType.Saving ");

    // Filter 2 (Search filter): Allows searching by customer Name or CardID (converted to text) if a 'SearchKeyword' is provided.
    getCustomersWithAccounts.AsDatabaseAggregate.CreateFilter("SearchKeyword = \"\" or Customer.Name like \"%\"+SearchKeyword+\"%\" or  LongIntegerToText(Customer.CardID) like \"%\"+SearchKeyword+\"%\" ");

    // Filter 3 (Category filter): Filters the customers based on the 'CategoryFilter' variable ("All", "Premium", or "Standard").
    getCustomersWithAccounts.AsDatabaseAggregate.CreateFilter("CategoryFilter = \"All\" or (CategoryFilter=\"Premium\" and Customer.IsPremium) or (CategoryFilter=\"Standard\" and not Customer.IsPremium)");

    // Group By Attributes: The Aggregate is grouped by customer details to get one row per customer.
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("CardID").SetAttribute("Customer.CardID");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("ClientSince").SetAttribute("Customer.ClientSince");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("CreditScore").SetAttribute("Customer.CreditScore");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("Id").SetAttribute("Customer.Id");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("IsPremium").SetAttribute("Customer.IsPremium");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("Mobile").SetAttribute("Customer.Mobile");
    getCustomersWithAccounts.AsDatabaseAggregate.CreateGroupByAttribute("Name").SetAttribute("Customer.Name");

    // Aggregated Attribute: Calculates the sum of all transaction amounts (Total Balance).
    var amountWithSignSum = getCustomersWithAccounts.AsDatabaseAggregate.CreateAggregatedAttribute("AmountWithSignSum");
    amountWithSignSum.SetAttribute("AmountWithSign");
    amountWithSignSum.AggregationType = AggregationType.Sum;

    // Sort: Primary sort attribute based on a variable ('TableSort') for dynamic sorting.
    getCustomersWithAccounts.AsDatabaseAggregate.CreateSortInGroupBy().SetAttribute("TableSort");

    // Sort: Secondary sort by Name.
    getCustomersWithAccounts.AsDatabaseAggregate.CreateSortInGroupBy().SetAttribute("Name");

    // --- SCREEN LOCAL VARIABLES ---

    // Local variable to hold the search text input by the user.
    var searchKeyword = customers.CreateLocalVariable("SearchKeyword");
    searchKeyword.DataType = eSpace.TextType;

    // --- MENU NAVIGATION UPDATE (Fixing the Breadcrumb/Menu Link) ---
    // Get references to the 'Menu' block and its internal widgets to update a navigation link.
    var common = eSpace.MobileFlows.Named("Common");
    var menu = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("Menu");
    var advancedHtml = menu.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IAdvancedHtml>().Single();
    var pageLinks = advancedHtml.Content.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IContainer>().Named("PageLinks");
    // Selects the second link element (index 1) which is likely the one to link to 'Customers'.
    var link = (ServiceStudio.Plugin.NRWidgets.ILink)pageLinks.Widgets.ElementAt(1);
    // Set the link's destination to the newly created 'Customers' screen.
    link.OnClick.Destination = customers;

    // Local variable to hold the current sorting expression for the data table.
    var tableSort = customers.CreateLocalVariable("TableSort");
    tableSort.DataType = eSpace.TextType;

    // Local variable to manage the starting position in the list for pagination.
    var startIndex = customers.CreateLocalVariable("StartIndex");
    startIndex.DataType = eSpace.IntegerType;

    // Local variable to define the number of records to fetch per page. Default set to 5.
    var maxRecords = customers.CreateLocalVariable("MaxRecords");
    maxRecords.DataType = eSpace.IntegerType;
    maxRecords.SetDefaultValue("5");

    // Local variable to store the selected customer category filter ("All", "Premium", "Standard"). Default set to "All".
    var categoryFilter = customers.CreateLocalVariable("CategoryFilter");
    categoryFilter.DataType = eSpace.TextType;
    categoryFilter.SetDefaultValue("\"All\"");

    // --- SCREEN ACTION: OnSort ---
    // Create a screen action to handle column sorting logic when a table header is clicked.
    var onSort = customers.CreateScreenAction("OnSort");

    // Input parameter for the action, representing the attribute name to sort by.
    var sortBy = onSort.CreateInputParameter("SortBy");
    sortBy.DataType = eSpace.TextType;
    sortBy.Description = "Table column by which the aggregate will be sorted.";

    // Start node of the action flow.
    var startNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    // IF node: Checks if the table is currently sorted by the same column AND the column is not empty (i.e., clicking again to reverse sort).
    var ifNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(startNode, 1330);
    ifNode.SetCondition("TableSort = SortBy and SortBy <> \"\"");

    // ASSIGN node (False branch): Set the new sort column (Default: ASCENDING).
    var assignNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().Below(ifNode, 1330);
    assignNode.Label = "TableSort = SortBy";
    var assignment = assignNode.CreateAssignment("TableSort", "SortBy");
    ifNode.FalseTarget = assignNode;

    // ASSIGN node (True branch): Set the sort column to DESCENDING (toggles from ASC).
    var assignNode2 = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(ifNode, 1772);
    assignNode2.Label = "TableSort += DESC";
    var assignment2 = assignNode2.CreateAssignment("TableSort", "SortBy + \" DESC\"");
    ifNode.TrueTarget = assignNode2;

    // ASSIGN node: Reset the pagination StartIndex to 0 (first page) after any sort change.
    var assignNode3 = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(assignNode, 1330);
    assignNode3.Label = "StartIndex = 0";
    var assignment3 = assignNode3.CreateAssignment("StartIndex", "0");
    assignNode2.Target = assignNode3; // Both true and false branches lead here.

    // REFRESH DATA node: Re-run the Aggregate with the new sort/pagination settings.
    var refreshDataNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IRefreshDataNode>().ConnectedBelow(assignNode3, 1330);
    refreshDataNode.DataSource = getCustomersWithAccounts;
    refreshDataNode.SetMaxRecords("MaxRecords");
    refreshDataNode.SetStartIndex("StartIndex");

    // End node of the action flow.
    var endNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(refreshDataNode, 1330);

    // Comments for documentation/clarity in the action flow.
    var commentNode = onSort.CreateNode<OutSystems.Model.Logic.Nodes.ICommentNode>();
    commentNode.HorizontalPosition = 6392;
    commentNode.Text = "This on sort action was automatically generated.";
    commentNode.VerticalPosition = 800;

    var commentNode2 = onSort.CreateNode<OutSystems.Model.Logic.Nodes.ICommentNode>();
    commentNode2.HorizontalPosition = 5856;
    commentNode2.Text = "Pagination resets to first page when the sorting option is changed.";
    commentNode2.VerticalPosition = 3460;
    commentNode2.CreateConnector(assignNode3);

    // --- SCREEN ACTION: Refresh ---
    // Create a screen action primarily used for pagination and triggering data reload on filter/search changes.
    var refresh = customers.CreateScreenAction("Refresh");

    // Input parameter for the new index used for pagination.
    var newStartIndex = refresh.CreateInputParameter("NewStartIndex");
    newStartIndex.DataType = eSpace.IntegerType;
    newStartIndex.SetDefaultValue("0");
    newStartIndex.Description = "New Index used for pagination.";
    newStartIndex.IsMandatory = false;

    // Start node.
    var startNode2 = refresh.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    // ASSIGN node: Updates the screen's StartIndex variable with the new value from the input parameter.
    var assignNode4 = refresh.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(startNode2, 1729);
    assignNode4.Label = "StartIndex = NewStartIndex";
    var assignment4 = assignNode4.CreateAssignment("StartIndex", "NewStartIndex");

    // REFRESH DATA node: Re-run the Aggregate with the updated StartIndex.
    var refreshDataNode2 = refresh.CreateNode<OutSystems.Model.Logic.Nodes.IRefreshDataNode>().ConnectedBelow(assignNode4, 1330);
    refreshDataNode2.DataSource = getCustomersWithAccounts;
    refreshDataNode2.SetMaxRecords("MaxRecords");
    refreshDataNode2.SetStartIndex("StartIndex");

    // End node.
    var endNode2 = refresh.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(refreshDataNode2, 1330);

    // --- WIDGET STRUCTURE AND LAYOUT ---

    // Add the common LayoutSideMenu block to the screen.
    var layoutSideMenuInstance = customers.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var layouts = eSpace.MobileFlows.Named("Layouts");
    var layoutSideMenu = layouts.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("LayoutSideMenu");
    layoutSideMenuInstance.SourceBlock = layoutSideMenu;

    // Clear the default content from the 'Navigation' placeholder (used for the side menu content).
    var placeholderContentWidget = layoutSideMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Navigation");
    placeholderContentWidget.Widgets.ToList().ForEach(w => w.Delete());

    // Clear the default content from the 'Header' placeholder.
    var placeholderContentWidget2 = layoutSideMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Header");
    placeholderContentWidget2.Widgets.ToList().ForEach(w => w.Delete());

    // Set input parameters for the LayoutSideMenu block (using default behavior).
    var hasFixedHeader = layoutSideMenu.InputParameters.Named("HasFixedHeader");
    layoutSideMenuInstance.SetArgumentValue(hasFixedHeader, null);
    var extendedClass = layoutSideMenu.InputParameters.Named("ExtendedClass");
    layoutSideMenuInstance.SetArgumentValue(extendedClass, null);

    // Add the 'Menu' block (side menu content) to the 'Navigation' placeholder.
    var menuInstance = placeholderContentWidget.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    menuInstance.SourceBlock = menu;
    // Set input parameters for menu navigation (active item not explicitly set here).
    var activeItem = menu.InputParameters.Named("ActiveItem");
    menuInstance.SetArgumentValue(activeItem, null);
    var activeSubItem = menu.InputParameters.Named("ActiveSubItem");
    menuInstance.SetArgumentValue(activeSubItem, null);

    // Add the 'Header' block to the 'Header' placeholder.
    var headerInstance = placeholderContentWidget2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var header = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("Header");
    headerInstance.SourceBlock = header;

    // Get the 'Title' placeholder content and add the screen title text: "Customers".
    var placeholderContentWidget3 = layoutSideMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Title");
    var textWidget = placeholderContentWidget3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget.Text = "Customers";

    // Get the 'MainContent' placeholder for the main body of the screen.
    var placeholderContentWidget4 = layoutSideMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");

    // Create a main container for the search and filters area.
    var container = placeholderContentWidget4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();

    // Text label: "Search by name or id number"
    var textWidget2 = container.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget2.SetStyleClasses("\"text-neutral-8\"");
    textWidget2.Text = "Search by name or id number";

    // Container for the search bar and filter buttons, with a top margin.
    var container2 = placeholderContentWidget4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container2.SetStyle("\"margin-top-s\"");

    // Container for the search input (takes 4 columns in a grid system).
    var container3 = container2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container3.Width = "4 col";

    // --- SEARCH INPUT WIDGET ---
    // Use the 'InputWithIcon' block from OutSystemsUI.
    var inputWithIconInstance = container3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var outSystemsUI = eSpace.References.Named("OutSystemsUI");
    var interaction = outSystemsUI.MobileFlows.Named("Interaction");
    var inputWithIcon = interaction.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("InputWithIcon");
    inputWithIconInstance.SourceBlock = inputWithIcon;

    // Clear default content in the Icon placeholder.
    var placeholderContentWidget5 = inputWithIconInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Icon");
    placeholderContentWidget5.Widgets.ToList().ForEach(w => w.Delete());

    // Clear default content in the Input placeholder.
    var placeholderContentWidget6 = inputWithIconInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Input");
    placeholderContentWidget6.Widgets.ToList().ForEach(w => w.Delete());

    // Set input parameters for the InputWithIcon block.
    var alignIconRight = inputWithIcon.InputParameters.Named("AlignIconRight");
    inputWithIconInstance.SetArgumentValue(alignIconRight, null);

    // Add a custom icon block ('HBIcon') to the Icon placeholder.
    var hBIconInstance = placeholderContentWidget5.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var homeBankingResources = eSpace.References.Named("HomeBankingResources");
    var widgets = homeBankingResources.MobileFlows.Named("Widgets");
    var hBIcon = widgets.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("HBIcon");
    hBIconInstance.SourceBlock = hBIcon;

    // Set the icon name to "search".
    var placeholderContentWidget7 = hBIconInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "IconName");
    var textWidget3 = placeholderContentWidget7.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget3.Text = "search";

    // Add the actual Input widget to the Input placeholder.
    var input_SearchKeyword = placeholderContentWidget6.CreateWidget<ServiceStudio.Plugin.NRWidgets.IInput>("Input_SearchKeyword");
    input_SearchKeyword.SetEnabled("True");
    input_SearchKeyword.InputType = ServiceStudio.Plugin.NRWidgets.Enumerations.InputType.Search; // Set input type to Search
    input_SearchKeyword.SetMandatory("False");
    input_SearchKeyword.SetPrompt("\"Search by Name or ID No\""); // Set placeholder text
    input_SearchKeyword.SetStyle("\"form-control  no-border-with-shadow\"");
    input_SearchKeyword.SetVariable("SearchKeyword"); // Bind the input to the 'SearchKeyword' local variable

    // Event: Trigger the 'Refresh' action whenever the search input value changes.
    var builtinEvent = input_SearchKeyword.OnChange;
    builtinEvent.Destination = refresh;
    builtinEvent.SetArgumentValue(newStartIndex, null);

    // Container for the category filter buttons (takes 8 columns).
    var container4 = container2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container4.Width = "8 col";

    // --- CATEGORY FILTER WIDGET (Button Group) ---
    // Create the ButtonGroup widget for "All", "Premium", "Standard" selection.
    var buttonGroup1 = container4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButtonGroup>("ButtonGroup1");

    /*** Deleting Widgets that are created by default when instantiating an IButtonGroup ***/
    buttonGroup1.Widgets.ToList().ForEach(x => x.Delete());
    buttonGroup1.SetEnabled("True");
    buttonGroup1.SetMandatory("False");
    buttonGroup1.SetVariable("CategoryFilter"); // Bind the button group's value to the 'CategoryFilter' local variable
    buttonGroup1.Width = "6 col";

    // Button Group Item 1: "All"
    var buttonGroupItem1 = buttonGroup1.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButtonGroupItem>("ButtonGroupItem1");
    /*** Deleting Widgets that are created by default when instantiating an IButtonGroupItem ***/
    buttonGroupItem1.Widgets.ToList().ForEach(x => x.Delete());
    buttonGroupItem1.SetEnabled("True");
    buttonGroupItem1.SetValue("\"All\"");
    var textWidget4 = buttonGroupItem1.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget4.CustomStyle = "font-weight: normal;";
    textWidget4.Text = "All";

    // Button Group Item 2: "Premium"
    var buttonGroupItem2 = buttonGroup1.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButtonGroupItem>("ButtonGroupItem2");
    /*** Deleting Widgets that are created by default when instantiating an IButtonGroupItem ***/
    buttonGroupItem2.Widgets.ToList().ForEach(x => x.Delete());
    buttonGroupItem2.SetEnabled("True");
    buttonGroupItem2.SetValue("\"Premium\"");
    var textWidget5 = buttonGroupItem2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget5.CustomStyle = "font-weight: normal;";
    textWidget5.Text = "Premium";

    // Button Group Item 3: "Standard"
    var buttonGroupItem3 = buttonGroup1.CreateWidget<ServiceStudio.Plugin.NRWidgets.IButtonGroupItem>("ButtonGroupItem3");
    /*** Deleting Widgets that are created by default when instantiating an IButtonGroupItem ***/
    buttonGroupItem3.Widgets.ToList().ForEach(x => x.Delete());
    buttonGroupItem3.SetEnabled("True");
    buttonGroupItem3.SetValue("\"Standard\"");
    var textWidget6 = buttonGroupItem3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget6.CustomStyle = "font-weight: normal;";
    textWidget6.Text = "Standard";

    // Event: Trigger the 'Refresh' action whenever the category filter changes.
    var builtinEvent2 = buttonGroup1.OnChange;
    builtinEvent2.Destination = refresh;
    builtinEvent2.SetArgumentValue(newStartIndex, null);

    // --- DATA DISPLAY SECTION (Loading/Empty/Table) ---
    // Container to house the conditional display of the table, loading, or empty state.
    var isTableLoadingOrEmpty = placeholderContentWidget4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>("IsTableLoadingOrEmpty");
    isTableLoadingOrEmpty.SetStyle("\"margin-top-base\"");

    // IF widget 1: Checks if data has been fetched AND the resulting list is empty. (EMPTY STATE)
    var isEmpty = isTableLoadingOrEmpty.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IIfWidget>("IsEmpty");
    isEmpty.SetCondition("GetCustomersWithAccounts.IsDataFetched and GetCustomersWithAccounts.List.Empty");

    // True Branch (Data is empty): Display the Blank Slate message.
    var container5 = isEmpty.TrueBranch.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container5.SetStyle("\"table-empty margin-top-m\"");

    // Use the 'BlankSlate' block from OutSystemsUI.
    var blankSlateInstance = container5.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var content = outSystemsUI.MobileFlows.Named("Content");
    var blankSlate = content.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("BlankSlate");
    blankSlateInstance.SourceBlock = blankSlate;
    // Clear default icon.
    var placeholderContentWidget8 = blankSlateInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Icon");
    placeholderContentWidget8.Widgets.ToList().ForEach(w => w.Delete());
    // Set input parameters.
    var fullHeight = blankSlate.InputParameters.Named("FullHeight");
    blankSlateInstance.SetArgumentValue(fullHeight, null);

    // Add the 'users' icon to the Blank Slate.
    var hBIconInstance2 = placeholderContentWidget8.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    hBIconInstance2.SourceBlock = hBIcon;

    // Set the icon name to "users".
    var placeholderContentWidget9 = hBIconInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "IconName");
    var textWidget7 = placeholderContentWidget9.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget7.Text = "users";

    // Set the empty state message text.
    var placeholderContentWidget10 = blankSlateInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Content");
    var textWidget8 = placeholderContentWidget10.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget8.Text = "There aren't any customers to show.";

    // False Branch of isEmpty: Checks if data has NOT been fetched yet. (LOADING STATE/TABLE)
    var isLoading = isEmpty.FalseBranch.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IIfWidget>("IsLoading");
    isLoading.SetCondition("not GetCustomersWithAccounts.IsDataFetched");

    // True Branch (Data is loading): Display the loading state style/container.
    var container6 = isLoading.TrueBranch.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container6.SetStyle("\"list-updating\"");

    // False Branch (Data is fetched and not empty): Display the TableRecords widget.
    var tableRecords = isLoading.FalseBranch.CreateWidget<ServiceStudio.Plugin.NRWidgets.ITableRecords>();
    tableRecords.SetSource("GetCustomersWithAccounts.List"); // Source the table from the Aggregate list.

    // --- TABLE HEADERS (COLUMNS) ---
    // HeaderCell 1: Customer Name (Sortable by 'Name').
    var headerCell = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell.SetSortAttribute("Name");
    headerCell.SetSource("GetCustomersWithAccounts.List");
    var textWidget9 = headerCell.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget9.Text = "Customer Name";

    // HeaderCell 2: ID No (Sortable by 'CardID', right-aligned).
    var headerCell2 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell2.CustomStyle = "text-align: right";
    headerCell2.SetSortAttribute("CardID");
    headerCell2.SetSource("GetCustomersWithAccounts.List");
    var textWidget10 = headerCell2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget10.Text = "ID No";

    // HeaderCell 3: Client Since (Sortable by 'ClientSince', right-aligned).
    var headerCell3 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell3.CustomStyle = "text-align: right";
    headerCell3.SetSortAttribute("ClientSince");
    headerCell3.SetSource("GetCustomersWithAccounts.List");
    var textWidget11 = headerCell3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget11.Text = "Client Since";

    // HeaderCell 4: Phone No (Sortable by 'Mobile').
    var headerCell4 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell4.SetSortAttribute("Mobile");
    headerCell4.SetSource("GetCustomersWithAccounts.List");
    var textWidget12 = headerCell4.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget12.Text = "Phone No";

    // HeaderCell 5: Balance (Sortable by 'AmountWithSignSum', right-aligned).
    var headerCell5 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell5.CustomStyle = "text-align: right";
    headerCell5.SetSortAttribute("AmountWithSignSum");
    headerCell5.SetSource("GetCustomersWithAccounts.List");
    var textWidget13 = headerCell5.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget13.Text = "Balance";

    // HeaderCell 6: Credit worthiness (Sortable by 'CreditScore', right-aligned).
    var headerCell6 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell6.CustomStyle = "text-align: right";
    headerCell6.SetSortAttribute("CreditScore");
    headerCell6.SetSource("GetCustomersWithAccounts.List");
    var textWidget14 = headerCell6.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget14.Text = "Credit worthiness";

    // HeaderCell 7: Category (Sortable by 'IsPremium').
    var headerCell7 = tableRecords.HeaderRow.CreateWidget<ServiceStudio.Plugin.NRWidgets.IHeaderCell>();
    headerCell7.SetSortAttribute("IsPremium");
    headerCell7.SetSource("GetCustomersWithAccounts.List");
    var textWidget15 = headerCell7.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget15.Text = "Category";

    // --- TABLE ROW CELLS (CONTENT) ---

    // RowCell 1: Customer Name (Link to Detail screen).
    var rowCell = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var link2 = rowCell.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILink>();
    /*** Deleting Widgets that are created by default when instantiating an ILink ***/
    link2.Widgets.ToList().ForEach(x => x.Delete());
    link2.SetEnabled("True");

    // Expression displays the Customer Name.
    var expression = link2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression.SetValue("GetCustomersWithAccounts.List.Current.Name");
    // OnClick event navigates to the 'CustomerDetail' screen.
    var builtinEvent3 = link2.OnClick;
    var customerDetail = mainFlow.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("CustomerDetail");
    builtinEvent3.Destination = customerDetail;
    // Pass the current Customer Id as the input parameter to the detail screen.
    var customerId = customerDetail.InputParameters.Named("CustomerId");
    builtinEvent3.SetArgumentValue(customerId, "GetCustomersWithAccounts.List.Current.Id");

    // RowCell 2: ID No (CardID, right-aligned).
    var rowCell2 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var container7 = rowCell2.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container7.Align = ServiceStudio.Plugin.NRWidgets.Enumerations.Align.Right;
    var expression2 = container7.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression2.SetValue("GetCustomersWithAccounts.List.Current.CardID");

    // RowCell 3: Client Since (right-aligned, formatted date).
    var rowCell3 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var container8 = rowCell3.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container8.Align = ServiceStudio.Plugin.NRWidgets.Enumerations.Align.Right;
    var expression3 = container8.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression3.SetValue("FormatDateCustom(GetCustomersWithAccounts.List.Current.ClientSince)");

    // RowCell 4: Phone No (Mobile).
    var rowCell4 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var expression4 = rowCell4.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression4.SetValue("GetCustomersWithAccounts.List.Current.Mobile");

    // RowCell 5: Balance (right-aligned, formatted currency).
    var rowCell5 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var container9 = rowCell5.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
    container9.Align = ServiceStudio.Plugin.NRWidgets.Enumerations.Align.Right;
    var expression5 = container9.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>();
    expression5.SetValue("FormatCurrencyCustom(GetCustomersWithAccounts.List.Current.AmountWithSignSum)");

    // RowCell 6: Credit worthiness (Uses a custom block to visually represent the Credit Score).
    var rowCell6 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var creditCategoriesInstance = rowCell6.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var blocks = eSpace.MobileFlows.Named("Blocks");
    var creditCategories = blocks.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("CreditCategories");
    creditCategoriesInstance.SourceBlock = creditCategories;
    // Pass the current CreditScore as an input parameter to the block.
    var creditScore = creditCategories.InputParameters.Named("CreditScore");
    creditCategoriesInstance.SetArgumentValue(creditScore, "GetCustomersWithAccounts.List.Current.CreditScore");

    // RowCell 7: Category (Uses an IF widget to display either 'Premium' or 'Standard' tag).
    var rowCell7 = tableRecords.Row.CreateWidget<ServiceStudio.Plugin.NRWidgets.IRowCell>();
    var ifWidget = rowCell7.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IIfWidget>();
    ifWidget.SetCondition("GetCustomersWithAccounts.List.Current.IsPremium"); // Condition: IsPremium is True/False
    ifWidget.DesignMode = OutSystems.Model.Enumerations.DesignMode.ShowTrueOrPreview;

    // True Branch (IsPremium is True): Display 'Premium' Tag.
    var tagInstance = ifWidget.TrueBranch.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var tag = content.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("Tag"); // Use the 'Tag' block from OutSystemsUI
    tagInstance.SourceBlock = tag;
    // Set tag appearance properties, including the custom CSS class for the premium look.
    var shape = tag.InputParameters.Named("Shape");
    tagInstance.SetArgumentValue(shape, null);
    var isLight = tag.InputParameters.Named("IsLight");
    tagInstance.SetArgumentValue(isLight, null);
    var color = tag.InputParameters.Named("Color");
    tagInstance.SetArgumentValue(color, null);
    var extendedClass2 = tag.InputParameters.Named("ExtendedClass");
    tagInstance.SetArgumentValue(extendedClass2, "\"background-tag-is-premium\"");

    // Content inside the Premium Tag: Icon (star) and Text.
    var placeholderContentWidget11 = tagInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Tag");
    var hBIconInstance3 = placeholderContentWidget11.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    hBIconInstance3.SourceBlock = hBIcon;
    var classes = hBIcon.InputParameters.Named("Classes");
    hBIconInstance3.SetArgumentValue(classes, "\"margin-right-xs\"");
    var placeholderContentWidget12 = hBIconInstance3.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "IconName");
    var textWidget16 = placeholderContentWidget12.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget16.Text = "star";
    var textWidget17 = placeholderContentWidget11.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget17.CustomStyle = "font-weight: normal;";
    textWidget17.Text = "Premium";

    // False Branch (IsPremium is False): Display 'Standard' Tag.
    var tagInstance2 = ifWidget.FalseBranch.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    tagInstance2.SourceBlock = tag;
    tagInstance2.SetArgumentValue(isLight, null);
    // Apply custom CSS class for the standard look.
    tagInstance2.SetArgumentValue(extendedClass2, "\"background-tag-not-premium\"");
    tagInstance2.SetArgumentValue(color, null);
    tagInstance2.SetArgumentValue(shape, null);

    // Content inside the Standard Tag: Icon (medal) and Text.
    var placeholderContentWidget13 = tagInstance2.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Tag");
    var hBIconInstance4 = placeholderContentWidget13.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    hBIconInstance4.SourceBlock = hBIcon;
    hBIconInstance4.SetArgumentValue(classes, "\"margin-right-xs\"");
    var placeholderContentWidget14 = hBIconInstance4.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "IconName");
    var textWidget18 = placeholderContentWidget14.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget18.Text = "medal";
    var textWidget19 = placeholderContentWidget13.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget19.CustomStyle = "font-weight: normal;";
    textWidget19.Text = "Standard";

    // Event: Link the TableRecords 'OnSort' event to the custom 'OnSort' screen action.
    var builtinEvent4 = tableRecords.OnSort;
    builtinEvent4.Destination = onSort;
    // Map the table's clicked column output to the OnSort action's input parameter.
    var clickedColumn = builtinEvent4.InputParameters.Named("ClickedColumn");
    clickedColumn.DataType = eSpace.TextType;
    clickedColumn.Name = "ClickedColumn";
    builtinEvent4.SetArgumentValue(sortBy, "ClickedColumn");

    // --- PAGINATION WIDGET ---
    // Add the 'Pagination' block from OutSystemsUI below the table.
    var paginationInstance = isLoading.FalseBranch.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var navigation = outSystemsUI.MobileFlows.Named("Navigation");
    var pagination = navigation.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("Pagination");
    paginationInstance.SourceBlock = pagination;

    // Clear default content in 'Previous' and 'Next' placeholders.
    var placeholderContentWidget15 = paginationInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Previous");
    placeholderContentWidget15.Widgets.ToList().ForEach(w => w.Delete());
    var placeholderContentWidget16 = paginationInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Next");
    placeholderContentWidget16.Widgets.ToList().ForEach(w => w.Delete());

    // Set input parameters for pagination: Max Records, Total Count, and Current Start Index.
    var maxRecords2 = pagination.InputParameters.Named("MaxRecords");
    paginationInstance.SetArgumentValue(maxRecords2, "MaxRecords");
    var totalCount = pagination.InputParameters.Named("TotalCount");
    paginationInstance.SetArgumentValue(totalCount, "GetCustomersWithAccounts.Count"); // Total records in the Aggregate
    var startIndex2 = pagination.InputParameters.Named("StartIndex");
    paginationInstance.SetArgumentValue(startIndex2, "StartIndex");
    var showGoToPage = pagination.InputParameters.Named("ShowGoToPage");
    paginationInstance.SetArgumentValue(showGoToPage, null);

    // Event: Link the Pagination 'OnNavigate' event (when moving pages) to the 'Refresh' screen action.
    var eventHandler = paginationInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "OnNavigate");
    eventHandler.Handler = refresh;
    // Map the pagination block's new index output to the Refresh action's input parameter.
    eventHandler.SetArgumentValue(newStartIndex, "NewStartIndex");

    // Add the 'angle-left' icon to the Previous button placeholder.
    var icon = placeholderContentWidget15.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon.Icon = "angle-left";
    icon.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;

    // Add the 'angle-right' icon to the Next button placeholder.
    var icon2 = placeholderContentWidget16.CreateWidget<ServiceStudio.Plugin.NRWidgets.IIcon>();
    icon2.Icon = "angle-right";
    icon2.IconSize = ServiceStudio.Plugin.NRWidgets.Enumerations.IconSize.FontSize;

    // --- CUSTOMER DETAIL BREADCRUMB FIX ---
    // This final section updates the breadcrumb link on the *CustomerDetail* screen (if it exists and has the expected structure)
    // to ensure navigating back goes to the *Customers* screen.
    var layoutSideMenuBannerActionsInstance = customerDetail.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();
    var placeholderContentWidget17 = layoutSideMenuBannerActionsInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Breadcrumbs");
    var breadcrumbsInstance = placeholderContentWidget17.Widgets.OfType<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>().Single();
    var placeholderContentWidget18 = breadcrumbsInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Content");
    var breadcrumbsItemInstance = (OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget)placeholderContentWidget18.Widgets.ElementAt(0);
    var placeholderContentWidget19 = breadcrumbsItemInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Title");
    var link3 = placeholderContentWidget19.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.ILink>().Single();
    // Set the first breadcrumb item's link destination to the 'Customers' screen.
    link3.OnClick.Destination = customers;
}
