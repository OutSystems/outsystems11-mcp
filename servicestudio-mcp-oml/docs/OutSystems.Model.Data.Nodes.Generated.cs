// Data model diagram nodes (entity relationship visualization and layout)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Data.Nodes;

public interface IEntityDiagramNode : OutSystems.Model.IFlowNode {
    IEnumerable<OutSystems.Model.Data.IEntityConnector> Relationships { get; }
    OutSystems.Model.Types.IRecordTypeSignature Entity { get; set; }
}

