// Add a BirthDate attribute to the Person entity
// Context: The module contains an entity named Person with attribute Name

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
}
