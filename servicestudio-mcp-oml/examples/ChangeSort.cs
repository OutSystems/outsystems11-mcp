// Sort the data of an aggregate by the Person's name
// Context: GetPersons aggregate in the MyAction server action. The aggregate has Person as its only source, and is sorting the data by Person.BirthDate

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var sort = getPersons.AsDatabaseAggregate.Sorts.ElementAt(0);
    sort.SetAttribute("Person.Name");
}
