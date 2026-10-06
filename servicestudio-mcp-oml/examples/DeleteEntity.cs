// Delete the Person entity
// Context: The module contains two entities: Person and Employee

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Person").Delete();
}
