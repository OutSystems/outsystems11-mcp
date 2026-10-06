// Create two site properties: RefreshIntervalSeconds (Integer, default 30) and ApiBaseUrl (Text, secret)
// Context: The module has no relevant pre-existing elements

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    // A numeric site property with a default value
    var refreshInterval = eSpace.CreateSiteProperty(false, "RefreshIntervalSeconds");
    refreshInterval.DataType = eSpace.IntegerType;
    refreshInterval.SetDefaultValue("30");
    refreshInterval.Description = "Interval in seconds between data refresh cycles.";

    // A secret text site property (e.g. for API keys)
    var apiBaseUrl = eSpace.CreateSiteProperty(false, "ApiBaseUrl");
    apiBaseUrl.DataType = eSpace.TextType;
    apiBaseUrl.SetDefaultValue(null);
    apiBaseUrl.Description = "Base URL of the external API.";
    apiBaseUrl.IsSecret = true;
}
