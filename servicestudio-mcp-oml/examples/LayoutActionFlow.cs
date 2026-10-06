// Lay out every node of the server action SumBigOrders on a grid: BFS from Start (If False branches and For Each bodies one column to the right), exception-handler paths in their own columns
// Context: typed Model API only (the MCP sandbox rejects reflection). Run it in the same lambda as the edit, or as a follow-up call, before omlMerge. Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var act = eSpace.ServerActions.Named("SumBigOrders");

    var start = act.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().Single();
    var depth = new Dictionary<OutSystems.Model.Logic.Nodes.IActionNode, int>();
    var col = new Dictionary<OutSystems.Model.Logic.Nodes.IActionNode, int>();
    var queue = new Queue<OutSystems.Model.Logic.Nodes.IActionNode>();
    depth[start] = 0; col[start] = 0; queue.Enqueue(start);

    while (queue.Count > 0) {
        var cur = queue.Dequeue();
        var loop = cur as OutSystems.Model.Logic.Nodes.IForEachNode;
        var iff = cur as OutSystems.Model.Logic.Nodes.IIfNode;
        foreach (var connector in cur.Connectors) {
            var t = connector.Target;
            if (t == null || depth.ContainsKey(t)) continue;   // loop-backs and merges keep their first slot
            int d = depth[cur] + 1;
            if (d > 40) continue;                              // hard cap keeps odd graphs on canvas
            bool side = (loop != null && t == loop.CycleTarget) || (iff != null && t == iff.FalseTarget);
            depth[t] = (side && loop != null) ? depth[cur] : d; // a loop body sits beside its For Each
            col[t] = col[cur] + (side ? 1 : 0);
            queue.Enqueue(t);
        }
    }

    // Exception-handler paths are not reachable from Start: give each its own column.
    int nextCol = (col.Count == 0 ? 0 : col.Values.Max()) + 2;
    foreach (var handler in act.Nodes.OfType<OutSystems.Model.Logic.Nodes.IExceptionHandlerNode>()) {
        OutSystems.Model.Logic.Nodes.IActionNode cur = handler;
        int row = 0;
        while (cur != null && !depth.ContainsKey(cur)) {
            depth[cur] = row++; col[cur] = nextCol;
            cur = cur.Connectors.Select(c => c.Target).FirstOrDefault();
        }
        nextCol++;
    }

    // Grid: 1828 x 1371 units, origin (3200, 800). Nodes never reached are left where they are.
    foreach (var n in depth.Keys) {
        n.HorizontalPosition = 3200 + col[n] * 1828;
        n.VerticalPosition = 800 + depth[n] * 1371;
    }
}
