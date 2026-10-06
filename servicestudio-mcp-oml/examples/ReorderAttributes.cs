// Change the attributes order in the Person entity so that Email is the first attribute
// Context: The module contains an entity named Person with attributes Name and Email

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var person = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person");
    var email = person.Attributes.Named("Email");
    var name = person.Attributes.Named("Name");
    person.Attributes.SetOrderTo(new[] { email, name });
}
