// Group the records of the GetPersons aggregate by Year (the person's birth year). Add a Count of each group in the GetPersons aggregate.
// Context: The module contains a server action named MyAction, which in turn contains an aggregate named GetPersons. There are two database entities: Person and Country.  Person has the following attributes: Id, Name, CountryId, BirthDate. CountryId is foreign key to the Country entity. Country has the following attribute: Id, Name. The aggregate uses both the Person and Country entities.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var count = getPersons.AsDatabaseAggregate.CreateAggregatedAttribute("Count");
    count.SetAttribute("Person.Id");
    count.AggregationType = AggregationType.Count;
    var year = getPersons.AsDatabaseAggregate.CreateGroupByAttribute("Year");
    year.SetAttribute("Year(Person.BirthDate)");
}
