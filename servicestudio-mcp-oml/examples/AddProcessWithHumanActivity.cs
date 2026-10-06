// Create an O11 BPT process ApproveOrder that launches when an Order is created: a Manager human activity on the ReviewOrder screen, an automatic activity that calls MarkReviewed, and a decision with Yes/No outcomes to two End nodes
// Context: Reactive O11 module with a server entity Order (Long Integer Id identifier), a role Manager, a screen ReviewOrder in MainFlow, and a server action MarkReviewed(OrderId). Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
    var manager = eSpace.Roles.Named("Manager");
    var reviewScreen = eSpace.MobileFlows.Named("MainFlow").Nodes.OfType<IMobileScreen>().Named("ReviewOrder");
    var markReviewed = eSpace.ServerActions.Named("MarkReviewed");

    // O11 processes: eSpace.CreateProcess (NOT CreateBusinessProcess, which is the ODC Workflows API).
    var process = eSpace.CreateProcess("ApproveOrder");
    process.LaunchOn.Entity = order;                                            // Launch On: Order ...
    process.LaunchOn.Action = OutSystems.Model.Processes.EntityActionKind.Create; // ... Create (auto-adds the OrderId input)
    process.SetDetail("\"Order \" + OrderId");                                   // Process Detail shown in the Taskbox

    // Process nodes are wired through Targets (Connected* helpers throw "Node doesn't have a target property").
    var start = process.CreateNode<OutSystems.Model.Processes.Nodes.IStartNode>();

    var review = process.CreateNode<OutSystems.Model.Processes.Nodes.IHumanActivityNode>("ReviewOrderTask").Below(start);
    start.Targets.Add(review);
    review.Destination = reviewScreen;
    // Setting Destination creates one argument per screen input; set them via Arguments
    // (SetScreenArgumentValue only accepts the ODC BusinessProcesses type).
    var orderIdArg = review.Arguments.FirstOrDefault(a => a.Parameter.Name == "OrderId");
    if (orderIdArg != null) orderIdArg.SetValue("OrderId");
    review.Roles.Add(manager);
    review.Roles.Remove(eSpace.RegisteredRole);                                  // new human activities default to Registered
    review.Instructions = "Review the new order and approve or reject it.";

    // Automatic activity = its own logic flow; it already contains a Start (and may contain an End).
    var record = process.CreateNode<OutSystems.Model.Processes.Nodes.IAutomaticActivityNode>("RecordReview").Below(review);
    review.Targets.Add(record);
    var recordStart = record.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().Single();
    var existingEnd = record.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().FirstOrDefault();
    var call = record.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("MarkReviewed").ConnectedBelow(recordStart);
    call.Action = markReviewed;
    call.SetArgumentValue(markReviewed.InputParameters.Named("OrderId"), "OrderId"); // process inputs are in scope
    if (existingEnd != null) call.Target = existingEnd;
    else record.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(call);

    // Decision: one named connector per outcome; the model builds the decision flow
    // (Start -> If -> one Outcome per connector) - only the If condition needs setting.
    var decision = process.CreateNode<OutSystems.Model.Processes.Nodes.IDecisionNode>("WasApproved").Below(record);
    record.Targets.Add(decision);
    var endApproved = process.CreateNode<OutSystems.Model.Processes.Nodes.IEndNode>().Below(decision);
    var endRejected = process.CreateNode<OutSystems.Model.Processes.Nodes.IEndNode>().ToTheRightOf(decision);
    decision.CreateConnector("Yes", endApproved);
    decision.CreateConnector("No", endRejected);
    decision.Nodes.OfType<OutSystems.Model.Logic.Nodes.IIfNode>().Single().SetCondition("OrderId <> NullIdentifier()");
}
