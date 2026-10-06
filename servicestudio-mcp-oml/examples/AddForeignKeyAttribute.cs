// Add a foreign key attribute GenreId to the Movie entity referencing the MovieGenre static entity
// Context: The module contains a server entity named Movie and a static entity named MovieGenre

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var movie = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Movie");
    var movieGenre = eSpace.Entities.OfType<OutSystems.Model.Data.IStaticEntity>().Named("MovieGenre");

    var genreId = movie.CreateAttribute("GenreId");
    genreId.DataType = movieGenre.IdentifierType;
    genreId.Label = "Genre";
    genreId.IsMandatory = true;
    genreId.DeleteRule = OutSystems.Model.Enumerations.DeleteRule.Protect;
}
