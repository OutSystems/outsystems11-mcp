// Add a new filter to the aggregate of the MyAction server action with the following condition: "a" = "b"
// Context: GetPersons aggregate in the MyAction server action. The aggregate has a filter

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    getPersons.AsDatabaseAggregate.CreateFilter("\"a\" = \"b\"");
}
