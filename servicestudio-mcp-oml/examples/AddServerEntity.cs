// Add a server entity named Person with attributes Id and Name

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.CreateServerEntity("Person");
    person.Label = "Person";
    person.LabelPlural = "Persons";

    var id = person.CreateAttribute("Id");
    id.DataType = eSpace.IntegerType;
    id.Label = "Id";
    person.IdentifierAttribute = id;

    var name = person.CreateAttribute("Name");
    name.DataType = eSpace.TextType;
    name.Label = "Name";
    name.Length = 50;
}
