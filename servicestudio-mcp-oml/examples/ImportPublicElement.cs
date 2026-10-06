// Import a public element from the tenant into an application
// Context: Public element with the specified asset and element keys exists in the tenant
//
// NOT SUPPORTED VIA MCP (today): adding a not-yet-referenced element through the in-process MCP
// Server fails. eSpace.AddDependency(globalKey) for an element that is not already resident throws
// "The event IModelServices.ResolveModuleSignature must be set first" in the out-of-process sidecar
// (which has no module-signature resolver); the host returns a clear "dependency not supported"
// message with mutatedOmlPath null. Add the dependency in Service Studio (Manage Dependencies) first,
// then reference it from MCP. See SKILL.md §8 (Known gaps).

using OutSystems.Model;

eSpace => {
    var assetKey = Services.ModelServices.ParseKey("2d879040-d5e0-4a01-8c98-2e2353dc8f3d");
    var elementKey = Services.ModelServices.ParseKey("7442b0c3-3206-4a62-891a-489d033867fd");
    var globalKey = Services.ModelServices.CreateGlobalKey(assetKey, elementKey);
    eSpace.AddDependency(globalKey);
}
