// Mobile logic node types (offline sync, device actions, client storage interactions)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Mobile.Nodes;

public interface ISendEmailNode : OutSystems.Model.Logic.Nodes.IActionNode {
    OutSystems.Model.Expressions.IExpression Attachments { get; }
    OutSystems.Model.Expressions.IExpression Bcc { get; }
    OutSystems.Model.Expressions.IExpression Cc { get; }
    OutSystems.Model.UI.Mobile.IMobileEmail Email { get; set; }
    OutSystems.Model.Expressions.IExpression From { get; }
    bool LogContent { get; set; }
    string Name { get; set; }
    OutSystems.Model.Logic.Nodes.IActionNode Target { get; set; }
    OutSystems.Model.Expressions.IExpression To { get; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    void SetAttachments(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetBcc(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetCc(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetFrom(OutSystems.Model.Expressions.ExpressionDefinition value);
    void SetTo(OutSystems.Model.Expressions.ExpressionDefinition value);
}

