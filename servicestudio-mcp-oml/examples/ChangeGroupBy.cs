// Rename the Year "group by" attribute of the GetPersons aggregate to xpto
// Context: GetPersons aggregate in the MyAction server action. The aggregate has Person as its only source. It is grouping data by the year of the person's birth date.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myAction = eSpace.ServerActions.Named("MyAction");
    var getPersons = myAction.Nodes.OfType<OutSystems.Model.Logic.Nodes.IAggregateNode>().Named("GetPersons");
    var year = getPersons.AsDatabaseAggregate.GroupByAttributes.Named("Year");
    year.Name = "xpto";
}
