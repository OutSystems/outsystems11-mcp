// Change the description of the Person.Name attribute to "The name of the person"
// Context: The module contains an entity named Person with attribute Name

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    var name = person.Attributes.Named("Name");
    name.Description = "The name of the person";
}
