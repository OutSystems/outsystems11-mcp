// Change an aggregate's filter condition to "a" = "b"
// Context: GetPersons aggregate in the MyAction server action. The aggregate has a filter

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var filter = getPersons.AsDatabaseAggregate.Filters.ElementAt(0);
    filter.SetCondition("\"a\" = \"b\"");
}
