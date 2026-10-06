// Add a new Dashboard screen to the MainFlow that displays a Donut Chart showing the distribution of movies by genre. The chart retrieves data from an aggregate that counts the number of movies per genre.
// Context: Movie database management application with Movie server entity and MovieGenre static entity, among others.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;

eSpace => {
    // --- DEPENDENCY RESOLUTION ---

    // AddDependency / RefreshDependency are NOT supported via MCP: both throw "The event
    // IModelServices.ResolveModuleSignature must be set first", even for already-referenced elements.
    // Resolve referenced elements through eSpace.References instead. The module must already
    // depend on OutSystemsCharts (add it in Service Studio > Manage Dependencies first).
    // See SKILL.md § 8 (Known gaps).

    // System User entity (already resident).
    var _System_ = eSpace.References.Named("(System)");
    var user = _System_.Entities.OfType<OutSystems.Model.Data.IServerEntitySignature>().Named("User");

    // DonutChart block from the OutSystemsCharts reference (flow "Charts").
    var donutChart = eSpace.References.Named("OutSystemsCharts").MobileFlows.Named("Charts")
        .Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("DonutChart");

    // --- NAVIGATION SETUP ---

    // Get references to the Common flow and Menu block for navigation setup.
    var common = eSpace.MobileFlows.Named("Common");
    var menu = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("Menu");

    // Find the container where navigation links are typically added inside the Menu's AdvancedHtml widget.
    var advancedHtml = menu.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IAdvancedHtml>().Single();
    var pageLinks = advancedHtml.Content.Widgets.OfType<ServiceStudio.Plugin.NRWidgets.IContainer>().Named("PageLinks");

    // Create a new Link widget in the navigation menu for 'Movie Genres'.
    var link = pageLinks.CreateWidget<ServiceStudio.Plugin.NRWidgets.ILink>();
    link.SetEnabled("True");

    // Set the link text to "Movie Genres".
    var textWidget = (OutSystems.Model.UI.Mobile.Widgets.ITextWidget)link.Widgets.First();
    textWidget.Text = "Movie Genres";
    var builtinEvent = link.OnClick; // Get a reference to the OnClick event handler.

    // --- SCREEN CREATION ---

    // Get the main flow and create the 'MovieGenresDashboard' screen.
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");
    var movieGenresDashboard = mainFlow.CreateScreen("MovieGenresDashboard");

    /*** Deleting Widgets that are created by default when instantiating an IMobileScreen ***/
    // Remove default widgets.
    movieGenresDashboard.Widgets.ToList().ForEach(x => x.Delete());

    // Assign the 'OSMDb_User' role for access control.
    var oSMDb_User = eSpace.Roles.Named("OSMDb_User");
    movieGenresDashboard.Roles.Add(oSMDb_User);

    // Allow anonymous access to the screen: add the Anonymous system role
    // (AnonymousAccess = true is dropped by omlMerge).
    if (!movieGenresDashboard.Roles.Contains(eSpace.AnonymousRole)) movieGenresDashboard.Roles.Add(eSpace.AnonymousRole);

    // Set the visual positioning of the screen in the flow diagram.
    movieGenresDashboard.HorizontalPosition = 9226;
    movieGenresDashboard.VerticalPosition = 3742;

    // --- DATA AGGREGATE: GetMoviesWithGenre ---

    // Create a Screen Aggregate to fetch data for the chart.
    var getMoviesWithGenre = movieGenresDashboard.CreateScreenAggregate(false, "GetMoviesWithGenre");

    // Add Movie and MovieGenre entities as sources.
    var movie2 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Movie");
    var movie = getMoviesWithGenre.AsDatabaseAggregate.CreateSource(movie2);
    var movieGenre2 = eSpace.Entities.OfType<OutSystems.Model.Data.IStaticEntity>().Named("MovieGenre");
    var movieGenre = getMoviesWithGenre.AsDatabaseAggregate.CreateSource(movieGenre2);

    // Join Movie (Left) and MovieGenre (Right) on GenreId.
    var join = getMoviesWithGenre.AsDatabaseAggregate.CreateJoin();
    join.LeftSource = movie;
    join.RightSource = movieGenre;
    join.JoinType = OutSystems.Model.Enumerations.JoinType.Inner; // Only movies with a defined genre are included.
    join.SetCondition("Movie.GenreId = MovieGenre.Id");

    // Group By the Genre Label to count movies per genre.
    getMoviesWithGenre.AsDatabaseAggregate.CreateGroupByAttribute("Label").SetAttribute("MovieGenre.Label");

    // Create an Aggregated Attribute to count the movies for each genre group.
    var count = getMoviesWithGenre.AsDatabaseAggregate.CreateAggregatedAttribute("Count");
    count.SetAttribute("MovieGenre.Id");
    count.AggregationType = AggregationType.Count; // The aggregate output will be {Label, Count}

    // --- UI WIDGET IMPLEMENTATION ---

    // Add the main layout block for the screen.
    var layoutTopMenuInstance = movieGenresDashboard.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var layouts = eSpace.MobileFlows.Named("Layouts");
    var layoutTopMenu = layouts.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("LayoutTopMenu");
    layoutTopMenuInstance.SourceBlock = layoutTopMenu;

    // Get and clear default widgets from the 'Header' placeholder.
    var placeholderContentWidget = layoutTopMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Header");
    placeholderContentWidget.Widgets.ToList().ForEach(w => w.Delete());

    // Set layout input parameters.
    var extendedClass = layoutTopMenu.InputParameters.Named("ExtendedClass");
    layoutTopMenuInstance.SetArgumentValue(extendedClass, null);
    var hasFixedHeader = layoutTopMenu.InputParameters.Named("HasFixedHeader");
    layoutTopMenuInstance.SetArgumentValue(hasFixedHeader, null);

    // Add the Menu block to the 'Header' placeholder.
    var menuInstance = placeholderContentWidget.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    menuInstance.SourceBlock = menu;
    // Configure menu highlighting (active item is the Movie Genres link).
    var activeSubItem = menu.InputParameters.Named("ActiveSubItem");
    menuInstance.SetArgumentValue(activeSubItem, null);
    var activeItem = menu.InputParameters.Named("ActiveItem");
    menuInstance.SetArgumentValue(activeItem, null);

    // Set the screen title: "Movie Genres Dashboard".
    var placeholderContentWidget2 = layoutTopMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Title");
    var textWidget2 = placeholderContentWidget2.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.ITextWidget>();
    textWidget2.Text = "Movie Genres Dashboard";

    // Get the 'MainContent' placeholder for the chart.
    var placeholderContentWidget3 = layoutTopMenuInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");

    // --- DONUT CHART WIDGET ---

    // Instantiate the DonutChart block.
    var donutChartInstance = placeholderContentWidget3.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    donutChartInstance.SourceBlock = donutChart;

    // Get and clear the default 'AddOns_Placeholder' content (for chart features like legend/styling).
    var placeholderContentWidget4 = donutChartInstance.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "AddOns_Placeholder");
    placeholderContentWidget4.Widgets.ToList().ForEach(w => w.Delete());

    // Set chart input parameters.
    var height = donutChart.InputParameters.Named("Height");
    donutChartInstance.SetArgumentValue(height, null);

    // Bind the chart's DataPointList to the Aggregate results.
    var dataPointList = donutChart.InputParameters.Named("DataPointList");
    // Mapping: Aggregate's 'Count' column -> Chart's 'Value'; Aggregate's 'Label' column -> Chart's 'Label'.
    // WARNING (verified 2026-07-28): `new ExpressionDefinition.TypeConversion(...)` compiles but HARD-CRASHES the
    // in-process MCP sidecar (empty stdout + empty mutatedOmlPath, uncatchable). Over MCP, bind via the parser instead:
    donutChartInstance.SetArgumentValue(dataPointList, ExpressionDefinition.Parse("GetMoviesWithGenre.List mapTo { Value: Count, Label: Label }"));

    var innerSize = donutChart.InputParameters.Named("InnerSize");
    donutChartInstance.SetArgumentValue(innerSize, null);

    // Event handlers (not explicitly configured here, just referenced).
    var eventHandler = donutChartInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "Initialized");
    var eventHandler2 = donutChartInstance.EventHandlers.FirstOrDefault(e => e.Event.Name == "OnDataPointClick");

    // --- CHART ADD-ONS ---

    // Add the ChartLegend block to the Donut Chart's AddOns placeholder.
    var chartLegendInstance = placeholderContentWidget4.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var outSystemsCharts = eSpace.References.Named("OutSystemsCharts");
    var addons = outSystemsCharts.MobileFlows.Named("Addons");
    var chartLegend = addons.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("ChartLegend");
    chartLegendInstance.SourceBlock = chartLegend;
    // Set legend input parameters (using default behavior for Visible, Position, Layout).
    var visible = chartLegend.InputParameters.Named("Visible");
    chartLegendInstance.SetArgumentValue(visible, null);
    var position = chartLegend.InputParameters.Named("Position");
    chartLegendInstance.SetArgumentValue(position, null);
    var layout = chartLegend.InputParameters.Named("Layout");
    chartLegendInstance.SetArgumentValue(layout, null);

    // Add the ChartSeriesStyling block (not actively styling in this code, but included).
    var chartSeriesStylingInstance = placeholderContentWidget4.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
    var chartSeriesStyling = addons.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("ChartSeriesStyling");
    chartSeriesStylingInstance.SourceBlock = chartSeriesStyling;
    var showDataPointValues = chartSeriesStyling.InputParameters.Named("ShowDataPointValues");
    chartSeriesStylingInstance.SetArgumentValue(showDataPointValues, null);

    // Set the navigation link's destination to the newly created screen.
    builtinEvent.Destination = movieGenresDashboard;
}
