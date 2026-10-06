// Create a Traditional Web screen PostFeed that fetches Post records in a Preparation aggregate and lists them with a ListRecords widget
// Context: TRADITIONAL WEB module (listApps style == "Traditional"). The module has a web flow named PlansFlow, a server entity named Post (with attributes Content, CreatedOn), and an existing screen named Dashboard.
//
// NOTE — this is the Traditional Web creation example. Do NOT confuse it with examples/AddWebBlock.cs,
// which despite its name builds a MOBILE block (eSpace.MobileFlows / CreateScreenAggregate / NRWidgets).
// Traditional differs in three ways shown below: (1) eSpace.WebFlows + IWebScreen, (2) data fetch via a
// Preparation flow with an Aggregate node (NOT screen.CreateScreenAggregate), (3) widgets are
// OutSystems.Model.UI.Web.Widgets.* (NOT ServiceStudio.Plugin.NRWidgets.*).
// Widget interface signatures: docs/OutSystems.Model.UI.Web.Generated.cs +
// docs/OutSystems.Model.UI.Web.Widgets.Generated.cs. Recipes: reference/traditional-ui-creation.md.
// Security (SKILL.md § 9): applyModelApiCode runs with the host's privileges — use only against OMLs you own.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var plansFlow = eSpace.WebFlows.Named("PlansFlow");
    var post = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Post");

    // 1) Create the Traditional Web screen (IWebFlow.CreateScreen returns IWebScreen).
    var screen = plansFlow.CreateScreen("PostFeed");
    screen.SetTitle("\"News Feed\"");   // SetTitle takes an expression string; a literal is double-quoted.

    // 2) Data fetch the Traditional way: a Preparation flow containing an Aggregate node
    //    (NOT screen.CreateScreenAggregate(...), which is the Mobile API and is absent on IWebScreen).
    //    Logic-node types stay fully-qualified to avoid the Logic.Nodes vs BusinessProcesses.Nodes CS0104 clash.
    var preparation = screen.CreatePreparation();
    var startNode = preparation.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();
    var getPosts = preparation
        .CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetPosts")
        .ConnectedBelow(startNode);
    getPosts.SetMaxRecords("50");
    getPosts.AsDatabaseAggregate.CreateSource(post);
    preparation.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(getPosts);

    // 3) Widget tree. Widget types are FULLY QUALIFIED: ITextWidget exists in BOTH
    //    OutSystems.Model.UI.Web.Widgets and OutSystems.Model.UI.Mobile.Widgets (in scope by default),
    //    so a bare 'ITextWidget' is a CS0104 ambiguity. Do NOT add 'using OutSystems.Model.UI.Mobile.Widgets;'.
    var container = screen.CreateWidget<OutSystems.Model.UI.Web.Widgets.IContainerWidget>();

    var heading = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>();
    heading.Text = "Latest posts";   // ITextWidget.Text is a plain string, not an expression.

    // 4) ListRecords bound to the aggregate output. SetSourceRecordList takes an EXPRESSION STRING
    //    ("GetPosts.List"), not a typed list reference. Bind row expressions to <list>.Current.<Entity>.<Attr>.
    var list = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.IListRecordsWidget>();
    list.SetSourceRecordList("GetPosts.List");
    list.SetEmptyMessage("\"No posts yet.\"");

    var row = list.CreateWidget<OutSystems.Model.UI.Web.Widgets.IContainerWidget>();
    var content = row.CreateWidget<OutSystems.Model.UI.Web.Widgets.IExpressionWidget>();
    content.SetValue("GetPosts.List.Current.Post.Content");   // EscapeContent defaults to true (HTML-safe).

    // 5) A link to an existing screen. Marking the TARGET as a frequent destination silences the
    //    benign "Unexpected Link" TrueChange warning (there is no link-side connector API).
    var dashboard = plansFlow.Nodes.OfType<IWebScreen>().First(s => s.Name == "Dashboard");
    var link = row.CreateWidget<OutSystems.Model.UI.Web.Widgets.ILinkWidget>();
    var linkLabel = link.CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>();
    linkLabel.Text = "Open dashboard";
    link.OnClick.Destination = dashboard;
    dashboard.IsFrequentDestination = true;

    // 6) Client-side JS hook. NOT auto-escaped: escape any user-controlled data with EncodeJavaScript()
    //    before interpolating it here (SKILL.md § 3 / reference/traditional-ui-creation.md).
    screen.JavaScript = "console.log('PostFeed loaded');";
}
