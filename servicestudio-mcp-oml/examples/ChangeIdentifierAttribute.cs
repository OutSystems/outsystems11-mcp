// Change the Person identifier attribute to be the Name attribute
// Context: The module contains an entity named Person with attributes Id and Name

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IClientEntity>().Named("Person");
    var name = person.Attributes.Named("Name");
    person.IdentifierAttribute = name;
}
