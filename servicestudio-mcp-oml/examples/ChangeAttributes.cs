// Change Person.Name max length to 30. Make Person.BirthDate mandatory.
// Context: The module contains an entity named Person with attributes Name and BirthDate

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    var name = person.Attributes.Named("Name");
    name.Length = 30;
    var birthDate = person.Attributes.Named("BirthDate");
    birthDate.IsMandatory = true;
}
