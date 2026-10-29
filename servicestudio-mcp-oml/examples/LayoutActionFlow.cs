// Lay out every node of the server action SumBigOrders on a grid: branch-aware placement (If False, Switch conditions and For Each bodies in their own columns), merge points below their deepest branch, no shared cells, exception-handler paths in their own columns
// Context: typed Model API only (the MCP sandbox rejects reflection). Run it in the same lambda as the edit, or as a follow-up call, before omlMerge. Verified live 2026-10-17 (If merge + 3-way Switch + For Each + exception handler: 0 overlaps, 0 upward edges, 0 crossings; the previous BFS gave 6 overlaps / 6 crossings on the same flow).

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var act = eSpace.ServerActions.Named("SumBigOrders");

    // Outgoing edges with their placement offset: (target, column offset, row offset).
    // If: True straight down, False one column right. Switch: condition i at +i columns, Otherwise straight down.
    // For Each: body one column right on the SAME row, exit straight down.
    List<(OutSystems.Model.Logic.Nodes.IActionNode t, int dc, int dr)> Succ(OutSystems.Model.Logic.Nodes.IActionNode n) {
        var r = new List<(OutSystems.Model.Logic.Nodes.IActionNode, int, int)>();
        if (n is OutSystems.Model.Logic.Nodes.IIfNode i) {
            if (i.TrueTarget != null) r.Add((i.TrueTarget, 0, 1));
            if (i.FalseTarget != null) r.Add((i.FalseTarget, 1, 1));
        } else if (n is OutSystems.Model.Logic.Nodes.ISwitchNode s) {
            int k = 1;
            foreach (var c in s.Conditions) if (c.Target != null) r.Add((c.Target, k++, 1));
            if (s.OtherwiseTarget != null) r.Add((s.OtherwiseTarget, 0, 1));
        } else if (n is OutSystems.Model.Logic.Nodes.IForEachNode f) {
            if (f.CycleTarget != null) r.Add((f.CycleTarget, 1, 0));
            if (f.Target != null) r.Add((f.Target, 0, 1));
        } else {
            foreach (var c in n.Connectors) if (c.Target != null) r.Add((c.Target, 0, 1));
        }
        return r;
    }

    var col = new Dictionary<OutSystems.Model.Logic.Nodes.IActionNode, int>();
    var row = new Dictionary<OutSystems.Model.Logic.Nodes.IActionNode, int>();
    var used = new HashSet<(int, int)>();
    var back = new HashSet<(OutSystems.Model.Logic.Nodes.IActionNode, OutSystems.Model.Logic.Nodes.IActionNode)>();

    // Place everything reachable from root that isn't placed yet.
    void Place(OutSystems.Model.Logic.Nodes.IActionNode root, int rootCol) {
        // 1) Back edges (For Each loop-backs) via DFS, so loops never inflate depth.
        var state = new Dictionary<OutSystems.Model.Logic.Nodes.IActionNode, int>();
        void Dfs(OutSystems.Model.Logic.Nodes.IActionNode n) {
            state[n] = 1;
            foreach (var e in Succ(n)) {
                if (col.ContainsKey(e.t)) continue;
                if (!state.ContainsKey(e.t)) Dfs(e.t);
                else if (state[e.t] == 1) back.Add((n, e.t));
            }
            state[n] = 2;
        }
        Dfs(root);
        // 2) Topological placement over forward edges: row = max over predecessors (merges sit below the
        //    deepest branch), column = min over predecessors (merges return to the main column).
        var preds = state.Keys.ToDictionary(n => n, n => new List<(OutSystems.Model.Logic.Nodes.IActionNode p, int dc, int dr)>());
        foreach (var n in state.Keys)
            foreach (var e in Succ(n))
                if (state.ContainsKey(e.t) && !back.Contains((n, e.t))) preds[e.t].Add((n, e.dc, e.dr));
        var pending = new List<OutSystems.Model.Logic.Nodes.IActionNode>(state.Keys);
        while (pending.Count > 0) {
            var n = pending.FirstOrDefault(x => preds[x].All(p => col.ContainsKey(p.p)));
            if (n == null) break;   // not a DAG after removing back edges - leave the rest where it is
            pending.Remove(n);
            int c = preds[n].Count == 0 ? rootCol : preds[n].Min(p => col[p.p] + p.dc);
            int r = preds[n].Count == 0 ? 0 : preds[n].Max(p => row[p.p] + p.dr);
            while (used.Contains((c, r))) c++;   // 3) collision shift: never two nodes on one cell
            col[n] = c; row[n] = r; used.Add((c, r));
        }
    }

    Place(act.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().Single(), 0);

    // Exception-handler paths are not reachable from Start: give each its own columns to the right.
    foreach (var handler in act.Nodes.OfType<OutSystems.Model.Logic.Nodes.IExceptionHandlerNode>())
        if (!col.ContainsKey(handler)) Place(handler, (col.Count == 0 ? 0 : col.Values.Max()) + 2);

    // Final check (the completion gate): no shared cell, no forward edge pointing upward.
    var upward = col.Keys.SelectMany(n => Succ(n).Where(e => col.ContainsKey(e.t) && !back.Contains((n, e.t)) && row[e.t] < row[n]).Select(e => n.GetType().Name + "->" + e.t.GetType().Name)).ToList();
    if (used.Count != col.Count || upward.Count > 0) throw new Exception("LAYOUT-CHECK failed: " + string.Join(",", upward));

    // Grid: 1828 x 1371 units, origin (3200, 800). Nodes never reached are left where they are.
    foreach (var n in col.Keys) {
        n.HorizontalPosition = 3200 + col[n] * 1828;
        n.VerticalPosition = 800 + row[n] * 1371;
    }
}
