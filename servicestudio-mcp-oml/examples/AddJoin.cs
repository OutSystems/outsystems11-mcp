// Add the Country entity to the GetPersons aggregate
// Context: GetPersons aggregate in the MyAction server action. The aggregate has Person as its only source. The data model includes also a Country entity. The Person entity has a foreign key to the Country entity via the CountryId field

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var country2 = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Country");
    var country = getPersons.AsDatabaseAggregate.CreateSource(country2, "Country");
    var join = getPersons.AsDatabaseAggregate.CreateJoin();
    var person = getPersons.AsDatabaseAggregate.Sources.Named("Person");
    join.LeftSource = person;
    join.RightSource = country;
    join.JoinType = JoinType.Inner;
    join.SetCondition("Person.CountryId = Country.Id");
}
