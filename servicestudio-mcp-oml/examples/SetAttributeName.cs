// Rename the Person.Name attribute to PersonName
// Context: The module contains an entity named Person with attribute Name

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    var name = person.Attributes.Named("Name");
    name.Label = "Person Name";
    name.Name = "PersonName";
}
