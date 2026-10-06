// Delete the Person.BirthDate attribute
// Context: The module contains an entity named Person with attributes Name and BirthDate

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    person.Attributes.Named("BirthDate").Delete();
}
