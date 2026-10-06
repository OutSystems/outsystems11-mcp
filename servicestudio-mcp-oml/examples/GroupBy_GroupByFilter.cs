// Only groups in which the Year is less than 2000 should be included in the GetPersons aggregate.
// Context: The module contains a server action named MyAction, which in turn contains an aggregate named GetPersons. There are two database entities: Person and Country.  Person has the following attributes: Id, Name, CountryId, BirthDate. CountryId is foreign key to the Country entity. Country has the following attribute: Id, Name. The aggregate uses both the Person and Country entities. The records are grouped by the Year (the person's birth year).

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    getPersons.AsDatabaseAggregate.CreateFilterInGroupBy("Year < 2000");
}
