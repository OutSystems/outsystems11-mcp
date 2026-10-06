// Business Process Technology (BPT) interfaces: workflow structure, nodes, and connectors for human + automated activities
using System;
using System.Collections.Generic;

namespace OutSystems.Model.BusinessProcesses;

public interface IActivityDataItem : OutSystems.Model.IObjectSignature {
    OutSystems.Model.Logic.IServiceActionSignature Action { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IBusinessProcess : OutSystems.Model.IFlow {
    /// <summary>
    /// Retrieves the cyclic connectors clusters in the business process.
    /// To get the cyclic connectors clusters we use Tarjan's strongly connected component algorithm.
    /// </summary>
    IEnumerable<IEnumerable<IBusinessProcessConnector>> CyclicConnectorsClusters { get; }
    byte[] Icon { get; set; }
    IEnumerable<OutSystems.Model.BusinessProcesses.Nodes.IBusinessProcessNode> Nodes { get; }
    T CreateNode<T>(string name = null, OutSystems.Model.IKey key = null) where T: OutSystems.Model.BusinessProcesses.Nodes.IBusinessProcessNode;
    OutSystems.Model.BusinessProcesses.Nodes.IBusinessProcessNode CreateNode(Type type, string name = null, OutSystems.Model.IKey key = null);
}

public interface IBusinessProcessConnector : OutSystems.Model.IConnector {
    new OutSystems.Model.BusinessProcesses.Nodes.IBusinessProcessNode Source { get; }
    new OutSystems.Model.BusinessProcesses.Nodes.IBusinessProcessNode Target { get; set; }
}

public interface IOutcome : IBusinessProcessConnector {
    string Label { get; set; }
    OutSystems.Model.Expressions.IExpression Value { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IPath : IBusinessProcessConnector {
    string Label { get; set; }
}

