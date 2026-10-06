// Create a Country entity with attributes Id, Name, and CountryCode. Create a Person entity with attributes Name, BirthDate, Email, and CountryId. CountryId is a foreign key to the Country entity.

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
    id.IsAutoNumber = OutSystems.Model.Enumerations.AutoNumber.Yes;
    id.Label = "Id";
    person.IdentifierAttribute = id;

    var name = person.CreateAttribute("Name");
    name.DataType = eSpace.TextType;
    name.Label = "Name";
    name.Length = 50;

    var birthDate = person.CreateAttribute("BirthDate");
    birthDate.DataType = eSpace.DateType;
    birthDate.Label = "Birth Date";

    var email = person.CreateAttribute("Email");
    email.DataType = eSpace.EmailType;
    email.Label = "Email";

    var countryId = person.CreateAttribute("CountryId");
    countryId.Label = "Country";

    var country = eSpace.CreateServerEntity("Country");
    country.Label = "Country";
    country.LabelPlural = "Countries";

    var id2 = country.CreateAttribute("Id");
    id2.DataType = eSpace.IntegerType;
    id2.IsAutoNumber = OutSystems.Model.Enumerations.AutoNumber.Yes;
    id2.Label = "Id";

    var name2 = country.CreateAttribute("Name");
    name2.DataType = eSpace.TextType;
    name2.Label = "Name";
    name2.Length = 50;

    var countryCode = country.CreateAttribute("CountryCode");
    countryCode.DataType = eSpace.TextType;
    countryCode.Label = "Country Code";
    countryCode.Length = 50;
    country.IdentifierAttribute = id2;
    countryId.DataType = country.IdentifierType;
}
