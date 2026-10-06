// Add a filter to an aggregate of a server action so that only the records where the Name is John are returned.
// Context: The module contains a server action named MyAction, which in turn contains an aggregate named GetPersons. There are two database entities: Person and Country.  Person has the following attributes: Id, Name, CountryId, BirthDate. CountryId is foreign key to the Country entity. Country has the following attribute: Id, Name. The aggregate uses both the Person and Country entities.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    getPersons.AsDatabaseAggregate.CreateFilter("Person.Name = \"John\"");
}
