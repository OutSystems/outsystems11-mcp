# ODC Workflow examples — NOT usable on OutSystems 11 modules

These lambdas use the `OutSystems.Model.BusinessProcesses` API (`eSpace.CreateBusinessProcess`,
`eSpace.BusinessProcesses`, global-event triggers, service-action activities). That is the
**ODC Workflows** model. On an O11 module the very first call fails at runtime:

```
exceptionMessage: Process objects can't be children of Module objects
```

(verified live against Service Studio 11's in-process MCP, 2026-09-30).

**O11 BPT processes are supported** through a different API: `eSpace.CreateProcess(...)`
(`OutSystems.Model.Processes.IProcess`) with node types from `OutSystems.Model.Processes.Nodes`,
wired with `node.Targets.Add(next)`. Use
[`../AddProcessWithHumanActivity.cs`](../AddProcessWithHumanActivity.cs) and
[`../../reference/logic-and-integrations.md`](../../reference/logic-and-integrations.md) § "BPT processes (O11)".

The files here are kept only as a record; do not use them as patterns for O11 work.
