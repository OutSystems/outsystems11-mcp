// Change the value of the Age calculated attribute of an aggregate to be 123
// Context: GetPersons aggregate in the MyAction server action. The aggregate has a calculated attribute named Age

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var age = getPersons.AsDatabaseAggregate.CalculatedAttributes.Named("Age");
    age.SetValue("123");
}
