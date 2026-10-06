// Change an aggregate's join condition to "a" = "b"
// Context: GetPersons aggregate in the MyAction server action. The aggregate joins the Person and Country entities

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var join = getPersons.AsDatabaseAggregate.Joins.ElementAt(0);
    join.SetCondition("\"a\" = \"b\"");
}
