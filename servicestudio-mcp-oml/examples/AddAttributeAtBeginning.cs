// Create a new attribute named BirthDate in the Person entity. Add it as the first attribute.
// Context: The module contains an entity named Person with attributes Name and Address

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    var birthDate = person.CreateAttribute("BirthDate");
    birthDate.DataType = eSpace.DateType;
    birthDate.Label = "Birth Date";
    birthDate.MoveToNewAbsoluteIndex(0);
}
