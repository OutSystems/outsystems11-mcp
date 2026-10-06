// Core model abstractions: modules (eSpaces), dependencies, flow infrastructure, arguments, and shared base interfaces
using System;
using System.Collections.Generic;

namespace OutSystems.Model;

public enum Border {
    NotDefined,
    None,
    Soft,
    Rounded,
}

public class DefaultValues {
}

public static class ESpaceServicesExtensions {
    /// <summary>
    /// Gets the PlatformServerVersion of the eSpace.
    /// </summary>
    /// <param name="eSpace">The eSpace</param>
    /// <returns></returns>
    public static OutSystems.Model.Enumerations.PlatformServerVersion GetPlatformServerVersion(this IESpace eSpace) => default;
}

public class ExtensionConstants {
}

public enum FileStorageStrategy {
    InMemory,
    Temp,
    ApplicationData,
}

public enum FontFamily {
    NotDefined,
    SystemFont,
    OpenSans,
    Roboto,
    Lato,
    PTSerif,
    Ubuntu,
}

public interface IAbstractionsManager : IObject {
    string DigestAfterHeal { get; }
    /// <summary>
    /// Resets the information of the types of objects that have been modified.
    /// Can only be called if transactions are disabled for the eSpace, normally . If transactionsare enabled then the <seealso cref="E:OutSystems.Model.IAbstractionsManager.Heal" />
    /// event must be subscribed instead.
    /// </summary>
    void OnHealCompleted();
}

public interface IAIArgument : IObject {
    OutSystems.Model.Types.ITypeSignature ConcreteParameterType { get; }
    string Description { get; set; }
    bool IsFilledByAI { get; set; }
    OutSystems.Model.Expressions.IExpression Value { get; }
    IInputParameterSignature Parameter { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IArgument : IArgumentSignature, IObject {
    OutSystems.Model.Types.ITypeSignature ConcreteParameterType { get; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IArgumentSignature : IObjectSignature {
    OutSystems.Model.Expressions.IExpression Value { get; }
    IInputParameterSignature Parameter { get; }
}

public interface IConnector : IObject {
    IFlowNode Source { get; }
    IFlowNode Target { get; set; }
}

public interface IDependency {
    string Name { get; }
    IKey Key { get; }
    string VersionDigest { get; }
    string SignatureDigest { get; }
    OutSystems.Model.Enumerations.SegmentationKind SegmentationKind { get; }
    string Revision { get; }
    IEnumerable<IDependency> Dependencies { get; }
}

public interface IDependencyGraph {
    IEnumerable<IDependency> Dependencies { get; }
    /// <summary>
    /// Returns a conflict-resolved version of the dependency graph, where dependencies with multiple versions are updated to use the version closest to the root of the graph.
    /// </summary>
    IDependencyGraph GetGraphWithoutConflicts();
}

public interface IESpace : IObject, OutSystems.Model.Applications.IModule, IOMLComponent {
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; set; }
    OutSystems.Model.Enumerations.DBMSType DBMS { get; set; }
    OutSystems.Model.UI.Mobile.IMobileThemeSignature DefaultMobileTheme { get; set; }
    OutSystems.Model.UI.IUIFlowNodeSignature DefaultScreen { get; set; }
    OutSystems.Model.Enumerations.TransitionType DefaultTransition { get; set; }
    bool HideWidgetsDistributedWithIDE { get; set; }
    byte[] Icon { get; set; }
    string InvalidCurrency { get; set; }
    string InvalidDate { get; set; }
    string InvalidDateTime { get; set; }
    string InvalidDecimal { get; set; }
    string InvalidEmail { get; set; }
    string InvalidInteger { get; set; }
    string InvalidNumericPassword { get; set; }
    string InvalidPhone { get; set; }
    string InvalidText { get; set; }
    string InvalidTime { get; set; }
    string InvisibleStyleSheet { get; }
    bool IsInIsolated { get; set; }
    bool IsMultiTenant { get; set; }
    bool IsUserProvider { get; set; }
    OutSystems.Model.Enumerations.JQueryVersion JQueryVersion { get; set; }
    string MandatoryInput { get; set; }
    Nullable<int> RetryInterval { get; set; }
    bool RetrySynchronizations { get; set; }
    Nullable<int> ServerRequestTimeout { get; set; }
    OutSystems.Model.UI.IUIFlowNodeSignature SplashScreen { get; set; }
    bool SynchronizeOnOnline { get; set; }
    bool SynchronizeOnResume { get; set; }
    string UpgradeComplete { get; set; }
    string UpgradeFailed { get; set; }
    string UpgradeFailedOnDataModel { get; set; }
    string UpgradeFailedOnResources { get; set; }
    string UpgradeRequired { get; set; }
    string UpgradeRequiredWithDataLoss { get; set; }
    bool UseCookies { get; set; }
    bool UseDefaultThemeExceptionHandler { get; set; }
    bool UsesCache { get; }
    OutSystems.Model.Enumerations.WebScreenRenderingMode WebScreenRenderingMode { get; set; }
    string WebServicesNamespace { get; set; }
    IAbstractionsManager AbstractionsManager { get; }
    IEnumerable<OutSystems.Model.Logic.IAgent> Agents { get; }
    IEnumerable<OutSystems.Model.Types.IAnonymousStructure> AnonymousStructures { get; }
    IEnumerable<OutSystems.Model.BusinessProcesses.IBusinessProcess> BusinessProcesses { get; }
    IEnumerable<OutSystems.Model.Logic.IClientAction> ClientActions { get; }
    IEnumerable<OutSystems.Model.Data.IClientVariable> ClientVariables { get; }
    IEnumerable<OutSystems.Model.Data.IEntity> Entities { get; }
    IEnumerable<OutSystems.Model.Data.IEntityDiagram> EntityDiagrams { get; }
    IEnumerable<OutSystems.Model.Data.IExtensibilitySetting> ExtensibilitySettings { get; }
    IEnumerable<IFolder> Folders { get; }
    IEnumerable<OutSystems.Model.Processes.IGlobalEventHandler> GlobalEventHandlers { get; }
    IEnumerable<OutSystems.Model.Processes.IGlobalEvent> GlobalEvents { get; }
    IEnumerable<OutSystems.Model.UI.IImage> Images { get; }
    IEnumerable<OutSystems.Model.Types.IListType> ListTypes { get; }
    IEnumerable<OutSystems.Model.Data.ILocale> Locales { get; }
    IMobileConfigurations MobileConfigurations { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.IMobileFlow> MobileFlows { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.IMobileTheme> MobileThemes { get; }
    OutSystems.Model.UI.Mobile.IPreviousScreen PreviousScreen { get; }
    IEnumerable<IReference> References { get; }
    IEnumerable<OutSystems.Model.ModelPlugins.IRequiredModelPlugin> RequiredModelPlugins { get; }
    IEnumerable<OutSystems.Model.Data.IResource> Resources { get; }
    IEnumerable<OutSystems.Model.Logic.IRoleException> RoleExceptions { get; }
    IEnumerable<OutSystems.Model.Logic.IAppRole> Roles { get; }
    IEnumerable<OutSystems.Model.UI.IScript> Scripts { get; }
    IEnumerable<OutSystems.Model.Logic.IServerAction> ServerActions { get; }
    IEnumerable<OutSystems.Model.Logic.IServiceAction> ServiceActions { get; }
    IEnumerable<OutSystems.Model.Data.ISessionVariable> SessionVariables { get; }
    IEnumerable<OutSystems.Model.Data.ISiteProperty> SiteProperties { get; }
    IEnumerable<OutSystems.Model.Logic.Integrations.SOAP.ISOAPClient> SOAPClients { get; }
    IEnumerable<OutSystems.Model.Logic.Integrations.SOAP.ISOAPService> SOAPServices { get; }
    IEnumerable<OutSystems.Model.Data.IStructure> Structures { get; }
    OutSystems.Model.Logic.ISystemEvent OnBeginWebRequest { get; }
    OutSystems.Model.Logic.ISystemEvent OnSessionStart { get; }
    OutSystems.Model.Logic.ISystemEvent OnApplicationReady { get; }
    OutSystems.Model.Logic.ISystemEvent OnApplicationResume { get; }
    OutSystems.Model.Logic.ISystemEvent OnSync { get; }
    IEnumerable<OutSystems.Model.Processes.ITimer> Timers { get; }
    IEnumerable<OutSystems.Model.Logic.IUserException> UserExceptions { get; }
    OutSystems.Model.Enumerations.SegmentationKind Kind { get; }
    bool IsTemplateBundle { get; set; }
    bool ReadOnlyMode { get; }
    Version LastUpgradeVersion { get; }
    IKey ClonedFromKey { get; }
    OutSystems.Model.Types.IBasicType TextType { get; }
    OutSystems.Model.Types.IBasicType IntegerType { get; }
    OutSystems.Model.Types.IBasicType LongIntegerType { get; }
    OutSystems.Model.Types.IBasicType DecimalType { get; }
    OutSystems.Model.Types.IBasicType BooleanType { get; }
    OutSystems.Model.Types.IBasicType DateTimeType { get; }
    OutSystems.Model.Types.IBasicType DateType { get; }
    OutSystems.Model.Types.IBasicType TimeType { get; }
    OutSystems.Model.Types.IBasicType PhoneNumberType { get; }
    OutSystems.Model.Types.IBasicType EmailType { get; }
    OutSystems.Model.Types.IBasicType BinaryDataType { get; }
    OutSystems.Model.Types.IBasicType FileType { get; }
    OutSystems.Model.Types.IBasicType CurrencyType { get; }
    OutSystems.Model.Types.IType ObjectType { get; }
    OutSystems.Model.Types.IBasicType TextIdentifierType { get; }
    OutSystems.Model.Types.IBasicType IntegerIdentifierType { get; }
    OutSystems.Model.Types.IBasicType LongIntegerIdentifierType { get; }
    OutSystems.Model.Types.IType UnknownRecordType { get; }
    OutSystems.Model.Types.IType UnknownListType { get; }
    OutSystems.Model.Types.IGenericType GenericRecordType { get; }
    OutSystems.Model.Types.IGenericType GenericListType { get; }
    OutSystems.Model.Expressions.IBuiltinFunctions BuiltinFunctions { get; }
    bool IsInMigrationsMergeModulesProcess { get; set; }
    Guid MigrationProcessKey { get; }
    bool SkipDependencyValidation { get; set; }
    bool SkipESpaceValidation { get; set; }
    OutSystems.Model.Expressions.ITextWithReferencedElements ExtensibilityConfigurations { get; }
    string JavaScript { get; set; }
    ICollection<OutSystems.Model.UI.IScriptSignature> RequiredScripts { get; }
    OutSystems.Model.Logic.IRoleSignature AnonymousRole { get; }
    OutSystems.Model.Logic.IRoleSignature RegisteredRole { get; }
    IEnumerable<OutSystems.Model.Logic.Integrations.IIntegration> Integrations { get; }
    OutSystems.Model.Logic.ISystemException AllExceptions { get; }
    OutSystems.Model.Logic.ISystemException UserException { get; }
    OutSystems.Model.Logic.ISystemException DatabaseException { get; }
    OutSystems.Model.Logic.ISystemException SecurityException { get; }
    OutSystems.Model.Logic.ISystemException InvalidLoginException { get; }
    OutSystems.Model.Logic.ISystemException NotRegisteredException { get; }
    OutSystems.Model.Logic.ISystemException CommunicationException { get; }
    OutSystems.Model.Logic.ISystemException AbortActivityChangeException { get; }
    OutSystems.Model.UI.IOnExceptionAction GlobalExceptionHandler { get; set; }
    /// <summary>
    /// The digest of the content of all fragments (except Versions, ReferersData and ReferersForCompilerData)
    /// as well as some header values of the app at the time of the last save:
    /// <list type="bullet">
    /// <item>LastUpgradeVersion</item>
    /// <item>Version</item>
    /// <item>HashAlgorithmVersion</item>
    /// <item>ElementsOrderVersion</item>
    /// <item>CompilationUnitsListVersion</item>
    /// <item>ActivationCode</item>
    /// <item>ProductId</item>
    /// <item>ProductName</item>
    /// <item>IsOpenSourceProduct</item>
    /// </list>
    /// The returned value will not change if the app is updated.
    /// For deeper clarification, see the OmlHeader.RefreshContentHash method.
    /// </summary>
    Guid ContentDigest { get; }
    /// <summary>
    /// The digest of the elements whose changes causes a broken reference. Value may change as the loaded app is modified.
    /// </summary>
    Guid SignatureCompatibilityDigest { get; }
    /// <summary>
    /// Locked references to this espace will use this digest. This uses Digest (GeneralHash),
    /// except when there was some upgrade during its last load. In that case it will use DigestBeforeUpgrades.
    /// </summary>
    Guid DigestForLibrariesVersioning { get; }
    IEnumerable<IESpaceVersion> Versions { get; }
    IEnumerable<OutSystems.Model.Types.IBasicType> BasicTypes { get; }
    IEnumerable<OutSystems.Model.Data.IStructureSignature> SystemStructures { get; }
    IEnumerable<OutSystems.Model.Data.IStaticEntitySignature> SystemStaticEntities { get; }
    IEnumerable<OutSystems.Model.Logic.ISystemException> SystemExceptions { get; }
    IEnumerable<OutSystems.Model.Logic.IRoleSignature> SystemRoles { get; }
    /// <summary>
    /// Full graph of dependencies
    /// </summary>
    IDependencyGraph DependencyGraph { get; }
    /// <summary>
    /// This returns the mobile frameworks that are supported by a Mobile Library eSpace,
    /// as well as if Capacitor support has been manually overridden by the user.
    /// </summary>
    IMobileFrameworksConfiguration MobileFrameworksConfiguration { get; }
    IEnumerable<IObjectSignature> AddDependencies(IEnumerable<IGlobalKey> elements, bool fixSpecificVersion = false);
    IEnumerable<IObjectSignature> AddDependency(OutSystems.Model.Signatures.SignatureInformation signatureInformation, Func<IKey, OutSystems.Model.Signatures.SignatureInformation> fetchAdditionalSignature = null, bool fixSpecificVersion = false);
    IEnumerable<IObjectSignature> AddDependency(IEnumerable<OutSystems.Model.Signatures.SignatureInformation> signatureInformation, Func<IKey, OutSystems.Model.Signatures.SignatureInformation> fetchAdditionalSignature = null, bool fixSpecificVersion = false);
    IEnumerable<IObjectSignature> AddDependency(OutSystems.Model.Signatures.SignatureInformation signatureInformation, Func<IKey, System.Threading.Tasks.Task<OutSystems.Model.Signatures.SignatureInformation>> fetchAdditionalSignature, bool fixSpecificVersion = false);
    IEnumerable<IObjectSignature> AddDependency(IEnumerable<OutSystems.Model.Signatures.SignatureInformation> signatureInformation, Func<IKey, System.Threading.Tasks.Task<OutSystems.Model.Signatures.SignatureInformation>> fetchAdditionalSignature, bool fixSpecificVersion = false);
    T CreateIntegration<T>(string name = null, IKey key = null) where T: OutSystems.Model.Logic.Integrations.IIntegration;
    void Close();
    void Lock(string secretKey, ModuleOperations forbiddenOperations = ModuleOperations.All);
    void Unlock(string secretKey);
    bool IsAllowed(ModuleOperations operations, string secretKey = null);
    /// <summary>
    /// Merges a set of objects from one eSpace into another.
    /// The provided objects must all be from the same eSpace, which cannot be
    /// "this" eSpace.
    /// 
    /// For each object a corresponding object in this eSpace is either created
    /// or updated.
    /// 
    /// Any object referred by <code>objects</code> but not contained there will
    /// be created if it doesn't exist (provided that options is null or contains
    /// <code>MergeOption.IncludeDependencies</code> for the object). If it
    /// already exists it will not be updated.
    /// </summary>
    /// <param name="objects">The objects to merge</param>
    /// <param name="options">If set, provides the merge options for the objects being merged.
    /// By default, the options for an object are <code>MergeOption.IncludeChildren | MergeOption.IncludeDependencies</code>.</param>
    /// <param name="pairings">If set, provides a pairing of objects from the source eSpace and the target eSpace. This will
    /// override the Merge algorithm's pairing rules, allowing to match pairs of objects explicitly. The objects must be of the same
    /// type</param>
    /// <param name="desiredKeys">If set, provides explict keys to be used for new target objects to be created</param>
    /// <returns>The resulting objects</returns>
    IEnumerable<IObjectSignature> Merge(IEnumerable<IObjectSignature> objects, Dictionary<IObjectSignature, MergeOption> options = null, IEnumerable<ValueTuple<IObjectSignature, IObjectSignature>> pairings = null, IEnumerable<ValueTuple<IObjectSignature, IKey>> desiredKeys = null);
    /// <summary>
    /// Creates a byte array of serialized objects from an enumerable array of ObjectSignature objects
    /// </summary>
    /// <param name="objects">An enumerable array of ObjectSignature objects</param>
    /// <returns>Byte array of serialized objects</returns>
    byte[] Serialize(IEnumerable<IObjectSignature> objects);
    /// <summary>
    /// Creates an enumerable array of ObjectSignature objects from a byte array of serialized objects
    /// including the target object provided. This method is also responsible in figuring out if
    /// the objects can be deserialized into the provided target.
    /// </summary>
    /// <param name="objects">A byte array of serialized objects</param>
    /// <returns>An enumerable array of ObjectSignature objects</returns>
    IEnumerable<IObjectSignature> DeserializeInto(byte[] objects, IObjectSignature target);
    /// <summary>
    /// Creates any missing metadata in all the objects in the eSpace (including the eSpace itself),
    /// according to the currently set initial metadata.
    /// 
    /// I.e., this method goes over each object <code>obj</code> that can contain metadata and
    /// creates new metadata for each value returned by <code>eSpace.GetInitialMetadataFor&lt;&gt;</code>
    /// that is missing in the object.
    /// 
    /// Use this method if you have called any of the <code>CreateInitial[Type]MetadataFor</code>
    /// methods in a non-empty eSpace and you want the existing objects to have that metadata.
    /// </summary>
    void CreateMissingMetadata();
    IFolder CreateFolder(OutSystems.Model.Enumerations.ESpaceTreeFolder parentFolder, string name = null, IKey key = null);
    OutSystems.Model.Data.ILocale GetOrCreateLocale(OutSystems.Model.Enumerations.Culture culture);
    /// <summary>
    /// Saves the text resources of all the elements in the eSpace into the provided output folder.
    /// For the Excel format a single file is produced, while for ResX a separate file is produced for each culture.
    /// </summary>
    /// <param name="outputFolder">Path to output folder</param>
    /// <param name="format">Desired resources format</param>
    void SaveTextResources(string outputFolder, OutSystems.Model.Enumerations.TextResourcesFileFormat format = OutSystems.Model.Enumerations.TextResourcesFileFormat.Excel);
    /// <summary>
    /// Loads text resources from the specified file.
    /// </summary>
    /// <param name="filePath">Path to input file</param>
    void LoadTextResources(string filePath);
    /// <summary>
    /// Applies the specified action to all text resources in this module and its descendants.
    /// This method traverses the entire object tree.
    /// </summary>
    /// <param name="action">Action to apply to each text resource. Parameters are: object, resource name, type, default value</param>
    void ApplyToTextResources(Action<IObjectSignature, string, OutSystems.Model.Enumerations.TextResourceType, string> action);
    /// <summary>
    /// Executes the <paramref name="action" /> in detached mode, i.e. any model object created while executing
    /// the action will be a detached object.
    /// Note that these objects do not need to be direct children of the eSpace.
    /// </summary>
    /// <param name="action"></param>
    void DoInDetachedMode(Action action);
    /// <summary>
    /// Executes the <paramref name="function" /> in detached mode, i.e. any model object created while executing
    /// the function will be a detached object.
    /// Note that these objects do not need to be direct children of the eSpace.
    /// </summary>
    /// <param name="function"></param>
    T DoInDetachedMode<T>(Func<T> function);
    /// <summary>
    /// Performs a partial save operation on the ESpace, returning only the modified fragments of the oml.
    /// </summary>
    /// <returns>Binary representation of the ESpace.</returns>
    byte[] SaveDelta();
    /// <summary>
    /// Converts module while skipping validations. Use this only for troubleshooting purposes
    /// </summary>
    /// <returns>The module's binary</returns>
    byte[] GetBytesUnsafe();
    IESpace Reload(string secretKey = null, string productKey = null);
    /// <summary>
    /// Reloads the eSpace (a new instance is returned) with the listed model plugins upgraded to the
    /// specified versions.
    /// 
    /// Currently in place upgrades of model plugins (i.e. in the loaded eSpace instance) is not supported.
    /// Undoing this operation is not allowed; the undo history for the returned eSpace is empty.
    /// </summary>
    /// <param name="pluginsToUpgrade"></param>
    /// <returns></returns>
    IESpace ReloadWithUpgradedModelPlugins(IEnumerable<ValueTuple<OutSystems.Model.ModelPlugins.IRequiredModelPlugin, OutSystems.Model.ModelPlugins.IModelPluginUpdateInformation>> pluginsToUpgrade, string secretKey = null, string productKey = null);
    /// <summary>
    /// Gets the ESpace documentation in xml format, this includes information about the oml and the objects present in it
    /// </summary>
    /// <returns>String representation of the ESpace Documentation.</returns>
    string GetDocumentation();
    /// <summary>
    /// Calculates the full dependencies graph. It recursively fetches those dependencies that do not specify their full dependency graph
    /// in order to firstly calculate this property for them and then obtain the espace dependency graph as the addition of its references
    /// dependencies graphs
    /// </summary>
    /// <param name="getDependencies">Callback to obtain the references list of the espace with the provided key</param>
    /// <returns></returns>
    IDependencyGraph UpdateDependencyGraph(Func<ValueTuple<IKey, string, string>, IEnumerable<ValueTuple<IKey, string, OutSystems.Model.Enumerations.SegmentationKind, string, string, string>>> getDependencies);
    OutSystems.Model.Data.IClientVariable CreateClientVariable(string name = null, IKey key = null);
    OutSystems.Model.Data.IEntityDiagram CreateEntityDiagram(string name = null, IKey key = null);
    OutSystems.Model.Data.IExtensibilitySetting CreateExtensibilitySetting(string name = null, IKey key = null);
    OutSystems.Model.Processes.IGlobalEvent CreateGlobalEvent(string name = null, IKey key = null);
    OutSystems.Model.Processes.IGlobalEventHandler CreateGlobalEventHandler(string name = null, IKey key = null);
    IMobileConfigurations CreateMobileConfiguration();
    OutSystems.Model.UI.Mobile.IMobileFlow CreateMobileFlow(string name = null, IKey key = null);
    OutSystems.Model.UI.Mobile.IMobileTheme CreateMobileTheme(string name = null, IKey key = null);
    OutSystems.Model.Logic.ISystemEvent CreateOnApplicationReady();
    OutSystems.Model.Logic.ISystemEvent CreateOnApplicationResume();
    OutSystems.Model.Logic.ISystemEvent CreateOnBeginWebRequest();
    OutSystems.Model.Logic.ISystemEvent CreateOnSessionStart();
    OutSystems.Model.Logic.IOnSyncSystemEvent CreateOnSync();
    OutSystems.Model.Logic.IAppRole CreateRole(string name = null, IKey key = null);
    OutSystems.Model.UI.IScript CreateScript(string name = null, IKey key = null);
    OutSystems.Model.Logic.IServerAction CreateServerAction(string name = null, IKey key = null);
    OutSystems.Model.Logic.IServiceAction CreateServiceAction(string name = null, IKey key = null);
    OutSystems.Model.Logic.Integrations.SOAP.ISOAPService CreateSOAPService(string name = null, IKey key = null);
    OutSystems.Model.Data.IStructure CreateStructure(string name = null, IKey key = null);
    OutSystems.Model.Processes.ITimer CreateTimer(string name = null, IKey key = null);
    OutSystems.Model.Logic.IUserException CreateUserException(string name = null, IKey key = null);
    OutSystems.Model.Types.IListType GetOrCreateListType(OutSystems.Model.Types.ITypeSignature elementType);
    OutSystems.Model.Types.IAnonymousStructure GetOrCreateAnonymousStructure(IEnumerable<Tuple<string, OutSystems.Model.Types.ITypeSignature>> attributes);
    OutSystems.Model.Types.IAnonymousStructure GetOrCreateAnonymousStructure(Tuple<string, OutSystems.Model.Types.ITypeSignature>[] attributes);
    OutSystems.Model.Types.ITypeSignature GetOrCreateType(string typeName);
    OutSystems.Model.Data.IServerEntity CreateServerEntity(string name = null, IKey key = null);
    OutSystems.Model.Data.IClientEntity CreateClientEntity(string name = null, IKey key = null);
    OutSystems.Model.Data.IStaticEntity CreateStaticEntity(string name = null, IKey key = null);
    OutSystems.Model.UI.IImage CreateImage(byte[] binary, string originalPath, string name = null, IKey key = null);
    OutSystems.Model.UI.IImage CreateImage(byte[] binary, string originalPath, OutSystems.Model.UI.ImageFormat format, int width, int height, string name = null, IKey key = null);
    OutSystems.Model.UI.IUIFlow CreateUIFlow(string name = null, IKey key = null);
    OutSystems.Model.Data.ISiteProperty CreateSiteProperty(bool isReadOnly = false, string name = null, IKey key = null);
    OutSystems.Model.Data.ISessionVariable CreateSessionVariable(bool isReadOnly = false, string name = null, IKey key = null);
    OutSystems.Model.Data.IResource CreateResource(byte[] binary, string name = null, IKey key = null);
    void SetExtensibilityConfigurations(string expression);
    /// <summary>
    /// Tool that was used to load the current espace (must have been set, as a parameter, during load or creation).
    /// </summary>
    OutSystems.Model.Enumerations.CreatedByTool GetWorkingTool();
    IESpace Clone(string activationCode = null, string secretKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, IModelObject parent = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None);
    IObjectSignature AddDependency(IESpace sourceESpace);
    IObjectSignature AddDependency(IShareable sourceObj, bool fixSpecificVersion = false);
    SignatureT AddDependency<ConcreteT, SignatureT>(IShareableESpaceObject<ConcreteT, SignatureT> sourceObj, bool fixSpecificVersion = false) where ConcreteT: SignatureT, IObject where SignatureT: IObjectSignature;
    SignatureT AddDependency<ConcreteT, SignatureT>(OutSystems.Model.ModelPlugins.IShareableModelPluginObject<ConcreteT, SignatureT> sourceObj) where ConcreteT: OutSystems.Model.ModelPlugins.IModelPluginObject where SignatureT: IObjectSignature;
    SignatureT AddDependency<ConcreteT, SignatureT>(OutSystems.Model.Signatures.IShareableObjectSignature<ConcreteT, SignatureT> sourceObj) where ConcreteT: OutSystems.Model.Signatures.IObjectSignature where SignatureT: IObjectSignature;
    void RefreshDependencies(IShareable[] sourceElements);
    void RefreshDependencies(IEnumerable<IShareable> sourceElements);
    void RefreshDependencies(IESpace[] sourceESpaces);
    void RefreshDependencies(IEnumerable<IESpace> sourceESpaces, bool updateSpecificVersion = false);
    void RefreshDependencies(IEnumerable<IReference> references);
    void RefreshDependencies(IReference reference);
    void RemoveUnusedDependencies();
    IReference AddDependency(OutSystems.Model.ModelPlugins.IModelPlugin plugin);
    IReference AddDependency(OutSystems.Model.Signatures.IExternalConnection externalConnection);
    IReference AddDependency(OutSystems.Model.Signatures.IAIModelConnection connector);
    IReference AddDependency(OutSystems.Model.Signatures.ISearchServiceConnection connector);
    IReference AddDependency(OutSystems.Model.Signatures.IMCPConnection connection);
    IReference AddDependency(OutSystems.Model.Signatures.IA2AConnection connection);
    bool RefreshDependencies(IEnumerable<IModuleSignature> producerModuleSignatures, IKey ownerApplicationKey, string ownerDatabaseName);
    IReference GetSignature();
    byte[] GetSignatureBinary();
    OutSystems.Model.Signatures.SignatureInformation GetSignatureInformation(IEnumerable<IKey> targetElementKeys);
    void RefreshDependency(IGlobalKey element, bool updateSpecificVersion = false);
    void RefreshDependencies(IEnumerable<IGlobalKey> elements, bool updateSpecificVersion = false);
    void RefreshDependencies(OutSystems.Model.Signatures.SignatureInformation signatureInformation, Func<IKey, OutSystems.Model.Signatures.SignatureInformation> fetchAdditionalSignature = null, bool updateSpecificVersion = false);
    void RefreshDependencies(IEnumerable<OutSystems.Model.Signatures.SignatureInformation> signatureInformation, Func<IKey, OutSystems.Model.Signatures.SignatureInformation> fetchAdditionalSignature = null, bool updateSpecificVersion = false);
    void RefreshDependencies(OutSystems.Model.Signatures.SignatureInformation signatureInformation, Func<IKey, System.Threading.Tasks.Task<OutSystems.Model.Signatures.SignatureInformation>> fetchAdditionalSignature, bool updateSpecificVersion = false);
    void RefreshDependencies(IEnumerable<OutSystems.Model.Signatures.SignatureInformation> signatureInformation, Func<IKey, System.Threading.Tasks.Task<OutSystems.Model.Signatures.SignatureInformation>> fetchAdditionalSignature, bool updateSpecificVersion = false);
    IObjectSignature AddDependency(IGlobalKey element, bool fixSpecificVersion = false);
    OutSystems.Model.Logic.IAgent CreateAgent(string name = null, IKey key = null);
    OutSystems.Model.BusinessProcesses.IBusinessProcess CreateBusinessProcess(string name = null, IKey key = null);
    OutSystems.Model.Logic.IClientAction CreateClientAction(string name = null, IKey key = null);
}

public interface IESpaceVersion {
    Guid Digest { get; }
    DateTime SaveDate { get; }
}

public interface IExtendedProperty : IExtendedPropertySignature, IObject {
    new string Property { get; set; }
    void SetValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IExtendedPropertySignature : IObjectSignature {
    string Property { get; }
    OutSystems.Model.Expressions.IExpression Value { get; }
}

public interface IFlow : IFlowSignature, IObject {
    string Description { get; set; }
}

public interface IFlowNode : IFlowNodeSignature, IObject {
    IEnumerable<IConnector> Connectors { get; }
    int HorizontalPosition { get; set; }
    IEnumerable<IConnector> IncomingConnectors { get; }
    bool IsDisabled { get; set; }
    int VerticalPosition { get; set; }
}

public interface IFlowNodeSignature : IObjectSignature {
}

public interface IFlowSignature : IObjectSignature {
    string Name { get; set; }
}

public interface IFolder : IFolderSignature, IShareableESpaceObject<IFolder, IFolderSignature> {
    new string CreatedBy { get; set; }
    new string Description { get; set; }
    new string LastModifiedBy { get; set; }
    new DateTime LastModifiedDate { get; set; }
    new IEnumerable<IObject> Entries { get; }
}

public interface IFolderSignature : IObjectSignature {
    string CreatedBy { get; }
    string Description { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    OutSystems.Model.Enumerations.ESpaceTreeFolder ParentFolder { get; }
    IEnumerable<IObjectSignature> Entries { get; }
}

/// <summary>
/// A global key uniquely identifies an object accross different modules. Objects in different modules
/// can have the same key - that's actually by design, and is usually the result of Merge or Clone operations,
/// which preserve the keys of the source objects whenever possible.
/// 
/// A global key combines the module key with the object key, resulting in an identifier that is
/// guaranteed to not be repeated in different modules.
/// </summary>
public interface IGlobalKey : IEquatable<IGlobalKey> {
    /// <summary>
    /// The key of the module
    /// </summary>
    IKey ModuleKey { get; }
    /// <summary>
    /// The key of the object
    /// </summary>
    IKey ObjectKey { get; }
    string SerializeToString();
}

public interface IHeader {
    bool RequiresUpgrades { get; }
    bool RequiresModelPlugins { get; }
    bool RequiresRecover { get; }
    bool RequiresElementsReordering { get; }
    string DependencyGraph { get; }
    IEnumerable<OutSystems.Model.Enumerations.MobileFramework> MobileFrameworks { get; }
}

public interface IInputParameter : IInputParameterSignature, IObject {
    new OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    new string Name { get; set; }
    /// <summary>
    /// Set to true to require for a value to be set.
    /// </summary>
    new bool IsMandatory { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    new string Description { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IInputParameterSignature : IObjectSignature {
    OutSystems.Model.Types.ITypeSignature DataType { get; }
    string Name { get; }
    /// <summary>
    /// Initial value of this element. If undefined, the default value of the data type is used.
    /// </summary>
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    /// <summary>
    /// Set to true to require for a value to be set.
    /// </summary>
    bool IsMandatory { get; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; }
}

/// <summary>
/// Identifies a model object uniquely inside a module.
/// </summary>
public interface IKey : IEquatable<IKey>, IComparable<IKey> {
    /// <summary>
    /// If the key is a local key, this property returns the parent key. Otherwise, returns null.
    /// E.g., for the local key &lt;EntityKey&gt;.#IdentifierType, the ParentKey is &lt;EntityKey&gt;.
    /// </summary>
    IKey ParentKey { get; }
    string SerializeToString(bool useDatabaseFormat = true);
    /// <summary>
    /// Use this method to create a new key deterministically from this key. The extra info is
    /// used to create the new key.
    /// 
    /// This method gurantees that for the same set of extraInfo you'll get the same result, thus
    /// you need to guarantee the uniqueness of extraInfo if calling this method more than once.
    /// 
    /// As an example, consider that you need to generate exactly one client entity from a given server entity.
    /// In that case we can calculate the client entity's key from the server entity's key using the
    /// code below:
    /// 
    /// <code>
    /// IServerEntity serverEntity;
    /// ...
    /// var clientEntityKey = serverEntity.ObjectKey.CreateNewKeyBasedOnThis("Client entity");
    /// </code>
    /// </summary>
    /// <param name="extraInfo">The extra info to include in order to calculate a new key.</param>
    /// <returns></returns>
    IKey CreateNewKeyBasedOnThis(object[] extraInfo);
    /// <summary>
    /// Returns a new key where the parent key is replaced by <paramref name="parentKey" />.
    /// If the key is not a local key, it returns the same key.
    /// </summary>
    /// <param name="parentKey"></param>
    /// <returns></returns>
    IKey WithNewParentKey(IKey parentKey);
}

public interface ILocalVariable : IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IMobileConfigurations : IObject {
    INativeSplashScreenConfiguration NativeSplashScreenConfiguration { get; }
}

public interface IMobileFrameworksConfiguration {
    IEnumerable<OutSystems.Model.Enumerations.MobileFramework> MobileFrameworks { get; }
    bool IsCapacitorSupportOverridden { get; set; }
}

public interface IModelObject : OutSystems.Model.Versioning.IVersionedObject {
    string DisplayName { get; }
    /// <summary>
    /// The object's key. Keys are unique inside a given module, but may be repeated accross modules.
    /// </summary>
    IKey Key { get; }
    /// <summary>
    /// The object's global key. Global keys uniquely identify objects.
    /// </summary>
    IGlobalKey GlobalKey { get; }
    /// <summary>
    /// The parent of the object. It may be null for top level elements:
    /// - solutions
    /// - applications, eSpaces, and extensions created on their own
    /// </summary>
    IModelObject Parent { get; }
    /// <summary>
    /// Returns the object's direct children.
    /// </summary>
    IEnumerable<IModelObject> Children { get; }
    /// <summary>
    /// The objects that refer to this one.
    /// </summary>
    IEnumerable<IModelObject> Referrers { get; }
    /// <summary>
    /// True if the object is detached
    /// </summary>
    bool IsDetached { get; }
    /// <summary>
    /// The ModelAPI interface for this object. Note that GetType() always returns a class,
    /// which for the Model API is an internal class and not very useful for decision purposes.
    /// 
    /// GetInterface(), however, returns one of the Model API interfaces, namely the most
    /// specific one implemented by the object.
    /// </summary>
    /// <returns></returns>
    Type GetInterface();
}

/// <summary>
/// Represents a weak reference to a model object in a way that still allows the object to be looked up
/// by providing its containing eSpace
/// </summary>
public interface IModelObjectAddress {
    /// <summary>
    /// Tries to lookup an object inside an eSpace given its address
    /// </summary>
    /// <param name="eSpace"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool TryLookup(IESpace eSpace, ref IModelObject obj);
    /// <summary>
    /// Serializes the address into a string
    /// </summary>
    /// <returns></returns>
    string SerializeToString();
}

/// <summary>
/// Represents a weak reference to a model object.
/// 
/// Intra-references (i.e. pointers inside the model itself) are represented directly as strong references.
/// E.g., IArgument.Parameter is of type IInputParameterSignature and directly references the parameter.
/// 
/// IModelObjectIdentifier should be used whenever you need to store a pointer to a model object that can
/// still be correctly de-referenced after undo/redo operations. E.g., if you store a direct pointer to
/// an object and then delete the object and undo the delete operation, the pointer will no longer reference
/// the correct object, since the Undo operation created a new object in memory (even though it is meant to
/// be an equivalent object, it is not the same exact instance as before).
/// 
/// By using  IModelObjectIdentifier instead the correct pointer will be returned after the Undo operation.
/// 
/// </summary>
public interface IModelObjectIdentifier {
    /// <summary>
    /// Returns the reference's target. If the object has been deleted and the delete operation
    /// hasn't been undone then the property returns null
    /// </summary>
    IModelObject Target { get; }
}

/// <summary>
/// Type-safe version of <see cref="T:OutSystems.Model.IModelObjectIdentifier" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IModelObjectIdentifier<T> : IModelObjectIdentifier where T: IModelObject {
    new T Target { get; }
}

public interface IModelServices {
    IEnumerable<IESpace> LoadedESpaces { get; }
    /// <summary>
    /// Returns all known Model API interraces
    /// </summary>
    IEnumerable<Type> ModelInterfaces { get; }
    IESpace LoadESpace(string omlPath, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, byte[] partialOml = null, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="omlPath"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    T LoadComponent<T>(string omlPath, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="omlPath"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    T LoadComponent<T>(string omlPath, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <param name="omlPath"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    IOMLComponent LoadComponent(string omlPath, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <param name="omlPath"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    IOMLComponent LoadComponent(string omlPath, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="oml"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    T LoadComponent<T>(byte[] oml, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="oml"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    T LoadComponent<T>(byte[] oml, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <param name="oml"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    IOMLComponent LoadComponent(byte[] oml, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <param name="oml"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="upgradeInformation"></param>
    /// <returns></returns>
    IOMLComponent LoadComponent(byte[] oml, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    IESpace LoadESpace(string omlPath, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, byte[] partialOml = null, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false);
    IESpace LoadESpace(byte[] oml, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, byte[] partialOml = null, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false);
    IESpace LoadESpace(byte[] oml, System.Threading.CancellationToken cancellationToken, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None, string secretKey = null, string productKey = null, IServerInformationForUpgrade upgradeInformation = null, bool enableTransactions = false, byte[] partialOml = null, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false);
    IESpace CreateESpace(OutSystems.Model.Enumerations.SegmentationKind kind, string name = null, string activationCode = null, bool isTemplatesBundleESpace = false, IKey key = null, OutSystems.Model.Enumerations.PlatformServerVersion targetPlatform = OutSystems.Model.Enumerations.PlatformServerVersion.O11, bool enableTransactions = false, Action<IESpace> setup = null, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="kind"></param>
    /// <param name="name"></param>
    /// <param name="activationCode"></param>
    /// <param name="isTemplatesBundleESpace"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    T Create<T>(OutSystems.Model.Enumerations.SegmentationKind kind, string name = null, string activationCode = null, bool isTemplatesBundleESpace = false, IKey key = null, bool enableTransactions = false, Action<T> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, bool trackChanges = false) where T: IOMLComponent;
    IESpace CloneESpace(byte[] oml, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, OutSystems.Model.Enumerations.LoadFlags loadFlags = OutSystems.Model.Enumerations.LoadFlags.None);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="oml"></param>
    /// <param name="activationCode"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="clonedESpaceKey"></param>
    /// <returns></returns>
    T Clone<T>(byte[] oml, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    IESpace CloneESpace(string omlPath, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    byte[] CloneESpaceWithoutLoad(byte[] oml, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, string name = null, string description = null, byte[] icon = null);
    byte[] CloneESpaceWithoutLoad(string omlPath, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict, string name = null, string description = null, byte[] icon = null);
    /// <summary>
    /// This feature is not yet fully finished and may change at any time.
    /// </summary>
    /// <typeparam name="T">This Generic Type corresponds to all inheritors of IOMLComponent</typeparam>
    /// <param name="omlPath"></param>
    /// <param name="activationCode"></param>
    /// <param name="secretKey"></param>
    /// <param name="productKey"></param>
    /// <param name="clonedESpaceKey"></param>
    /// <returns></returns>
    T Clone<T>(string omlPath, string activationCode = null, string secretKey = null, string productKey = null, IKey clonedESpaceKey = null, bool enableTransactions = false, OutSystems.Model.Enumerations.CreatedByTool tool = OutSystems.Model.Enumerations.CreatedByTool.Unspecified, string developerId = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict) where T: IOMLComponent;
    /// <summary>
    /// This method will return the header of the given oml.
    /// </summary>
    /// <param name="oml"></param>
    /// <returns></returns>
    IHeader GetHeader(byte[] oml);
    /// <summary>
    /// This method will return the header of the given oml.
    /// </summary>
    /// <param name="oml"></param>
    /// <returns></returns>
    IHeader GetHeader(System.IO.Stream oml);
    OutSystems.Model.ModelPlugins.IModelPlugin LoadModelPlugin(string xifPath, bool includeResources = true, FileStorageStrategy resourceStorageStrategy = FileStorageStrategy.InMemory, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.ModelPlugins.IModelPlugin LoadModelPlugin(byte[] xif, bool includeResources = true, FileStorageStrategy resourceStorageStrategy = FileStorageStrategy.InMemory, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// Create a model plugin by providing the high code package in zip format.
    /// </summary>
    /// <param name="package"></param>
    /// <param name="validationMessages"></param>
    /// <returns></returns>
    OutSystems.Model.ModelPlugins.IModelPlugin CreateModelPlugin(byte[] package, ref IList<IValidationMessage> validationMessages, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// Create a model plugin by providing the high code main assembly.
    /// Note that this is a bare-bones model plugin that only contains the model definitions but no resources
    /// (not even the provided assembly)
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="validationMessages"></param>
    /// <returns></returns>
    OutSystems.Model.ModelPlugins.IModelPlugin GetOrCreateModelPlugin(System.Reflection.Assembly assembly, ref IList<IValidationMessage> validationMessages, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    /// <summary>
    /// Create a model plugin by providing the assembly name and version. Lookup for the plugin's assembly
    /// is provided by the execution context - for standalone uses, the dlls are looked for in the
    /// ModelPlugins subdirectory; for ODC Studio uses, they are additionally fetched from the platform server
    /// if a local cached copy is not found.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="assemblyVersion"></param>
    /// <param name="validationMessages"></param>
    /// <returns></returns>
    OutSystems.Model.ModelPlugins.IModelPlugin GetOrCreateModelPlugin(string assemblyName, Version assemblyVersion, ref IList<IValidationMessage> validationMessages, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IModuleSignature LoadModuleSignature(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IModuleSignature LoadModuleSignature(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IExternalConnection LoadExternalConnection(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IExternalConnection LoadExternalConnection(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IExternalConnection CreateExternalConnection(string name = "Connection", IKey key = null, bool enableTransactions = false, Action<OutSystems.Model.Signatures.IExternalConnection> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IAIModelConnection CreateAIModelConnection(string name = "Connector", IKey key = null, bool enableTransactions = false, Action<OutSystems.Model.Signatures.IAIModelConnection> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IAIModelConnection LoadAIModelConnection(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IAIModelConnection LoadAIModelConnection(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.ISearchServiceConnection CreateSearchServiceConnection(string name = "Connector", IKey key = null, bool enableTransactions = false, Action<OutSystems.Model.Signatures.ISearchServiceConnection> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.ISearchServiceConnection LoadSearchServiceConnection(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.ISearchServiceConnection LoadSearchServiceConnection(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IMCPConnection CreateMCPConnection(string name = "Connection", IKey key = null, bool enableTransactions = false, Action<OutSystems.Model.Signatures.IMCPConnection> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IMCPConnection LoadMCPConnection(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IMCPConnection LoadMCPConnection(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IA2AConnection CreateA2AConnection(string name = "Connection", IKey key = null, bool enableTransactions = false, Action<OutSystems.Model.Signatures.IA2AConnection> setup = null, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IA2AConnection LoadA2AConnection(string signaturePath, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    OutSystems.Model.Signatures.IA2AConnection LoadA2AConnection(byte[] signature, OutSystems.Model.Signatures.SignatureMetadata signatureMetadata = null, bool enableTransactions = false, OutSystems.Model.Versioning.CommitStrategy commitStrategy = OutSystems.Model.Versioning.CommitStrategy.ReexecuteOnConflict);
    IKey ParseKey(string value);
    IKey NewKey();
    IKey NewKeyBasedOn(object[] info);
    IGlobalKey ParseGlobalKey(string value);
    bool TryParseKey(string value, ref IKey key);
    bool TryParseGlobalKey(string value, ref IGlobalKey key);
    IGlobalKey CreateGlobalKey(IKey moduleKey, IKey objectKey);
    IModelObjectAddress ParseObjectAddress(string value);
    string GenerateSecretKey();
    T GetPluginService<T>() where T: IPluginService;
    /// <summary>
    /// Returns a valid version of the provided name, replacing / removing all invalid characters according to the
    /// naming rules of the OutSystems language.
    /// </summary>
    /// <param name="name">A possible invalid name</param>
    /// <returns>A fixed and valid version of the name</returns>
    string FixName(string name);
    /// <summary>
    /// Gets the activation code of a binary file, which must be an oml, xif, oap, or oap.
    /// </summary>
    /// <param name="fileContent"></param>
    /// <param name="error"></param>
    /// <returns>The activation code. If the file is not of a recognized format, return null</returns>
    string GetActivationCode(byte[] fileContent, ref string error);
    /// <summary>
    /// Performs an automatic 3-way merge if possible, i.e. if no conflicts are detected and the model plugins
    /// can be merged without resulting in inconsistent versions.
    /// If there is at least one conflict then the method does nothing.
    /// </summary>
    /// <param name="myESpace">The eSpace into which changes will be brought</param>
    /// <param name="otherESpace">The eSpace from where changes will be brought</param>
    /// <param name="baseESpace">A common base version between myESpace and otherESpace. For better results
    /// this should be the most recent common version from which myESpace and otherESpace were created.
    /// Note that we don't actually check if baseESpace is indeed a common base version.</param>
    /// <param name="pairings">If set, provides a pairing of objects from the source eSpace and the target eSpace. This will
    /// override the Merge algorithm's pairing rules, allowing to match pairs of objects explicitly. The objects must be of the same
    /// type</param>
    /// <param name="matchByStructureInsteadOfIdentity">If false (the default) the objects are matched by their
    /// key; if true, objects are matched structuraly and by name</param>
    /// <returns>True if no conflict was detected and merge was successful, false otherwise</returns>
    bool Merge(IESpace myESpace, IESpace otherESpace, IESpace baseESpace, IEnumerable<ValueTuple<IObjectSignature, IObjectSignature, IObjectSignature>> pairings = null, bool matchByStructureInsteadOfIdentity = false);
    /// <summary>
    /// Performs an automatic 3-way merge if possible, i.e. if no conflicts are detected and the model plugins
    /// can be merged without resulting in inconsistent versions.
    /// If there is at least one conflict then the method does nothing.
    /// </summary>
    /// <param name="myESpace">The eSpace into which changes will be brought</param>
    /// <param name="otherESpace">The eSpace from where changes will be brought</param>
    /// <param name="baseESpace">A common base version between myESpace and otherESpace. For better results
    /// this should be the most recent common version from which myESpace and otherESpace were created.
    /// Note that we don't actually check if baseESpace is indeed a common base version.</param>
    /// <param name="reasonForFailure">Set if merge cannot proceed with the reason why</param>
    /// <param name="pairings">If set, provides a pairing of objects from the source eSpace and the target eSpace. This will
    /// override the Merge algorithm's pairing rules, allowing to match pairs of objects explicitly. The objects must be of the same
    /// type</param>
    /// <param name="matchByStructureInsteadOfIdentity">If false (the default) the objects are matched by their
    /// key; if true, objects are matched structuraly and by name</param>
    /// <returns>True if no conflict was detected and merge was successful, false otherwise</returns>
    bool Merge(IESpace myESpace, IESpace otherESpace, IESpace baseESpace, ref string reasonForFailure, IEnumerable<ValueTuple<IObjectSignature, IObjectSignature, IObjectSignature>> pairings = null, bool matchByStructureInsteadOfIdentity = false);
    /// <summary>
    /// Moves an object to a given new parent
    /// </summary>
    /// <param name="source">The object that will be moved</param>
    /// <param name="newParent">The new parent of the object that will be moved</param>
    /// <returns>The moved object that is inside the new parent</returns>
    IObject MoveObject(IObject source, IObject newParent);
    /// <summary>
    /// Returns true if <paramref name="type" /> is a Model API interface
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool IsModelInterface(Type type);
    void SetLogger(Microsoft.Extensions.Logging.ILogger logger, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = null);
    bool IsFromDynamicallyLoadedModelPlugin(object obj);
    bool IsFromDynamicallyLoadedModelPlugin(object obj, ref string assemblyName, ref Version assemblyVersion);
}

public interface IModuleSignature {
    IKey Key { get; }
    IKey ApplicationKey { get; }
    OutSystems.Model.Enumerations.ReferenceKind ModuleKind { get; }
    OutSystems.Model.Enumerations.SegmentationKind ModuleType { get; }
    string Name { get; }
    string Description { get; }
    byte[] Icon { get; }
    string JQueryVersion { get; }
    string ExtensibilityConfigurations { get; }
    string DatabaseName { get; }
    Guid SignatureDigest { get; }
    byte[] SignatureBinary { get; }
}

public interface INativeSplashScreenConfiguration : IObject {
    string BackgroundColor { get; set; }
    OutSystems.Model.UI.IImageSignature BrandingImage { get; set; }
    OutSystems.Model.UI.IImageSignature CustomBrandingImage { get; set; }
    OutSystems.Model.UI.IImageSignature CustomSplashScreen { get; set; }
    OutSystems.Model.Enumerations.ScreenTransition ExitAnimation { get; set; }
    string LoadingBackgroundColor { get; set; }
    OutSystems.Model.UI.IImageSignature LoadingBrandingImage { get; set; }
    string LoadingMessage { get; set; }
    string LoadingMessageColor { get; set; }
    OutSystems.Model.Enumerations.LoadingMessagePosition LoadingMessagePosition { get; set; }
    OutSystems.Model.Enumerations.LoadingScreenType LoadingScreenType { get; set; }
    OutSystems.Model.Enumerations.LoadingPatternType LoadingSpinner { get; set; }
    string LoadingSpinnerColor { get; set; }
}

public interface INeoApp : IOMLComponent {
}

public interface INeoLibrary : IOMLComponent {
}

public interface IObject : IObjectSignature {
    /// <summary>
    /// Returns the position of the object in its child collection.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    /// <returns></returns>
    int IndexInParent { get; }
    /// <summary>
    /// True if <see cref="P:OutSystems.Model.IObject.CanMoveBefore" /> can be called.
    /// </summary>
    bool CanMoveBefore { get; }
    /// <summary>
    /// True if <see cref="P:OutSystems.Model.IObject.CanMoveAfter" /> can be called.
    /// </summary>
    bool CanMoveAfter { get; }
    /// <summary>
    /// Copies an object into this object.
    /// </summary>
    /// <param name="obj">The object to copy</param>
    /// <returns>The created copy</returns>
    IObjectSignature Copy(IObjectSignature obj);
    /// <summary>
    /// Copies a list of objects into this object.
    /// </summary>
    /// <param name="objects">The objects to copy</param>
    /// <returns>The created copies</returns>
    IEnumerable<IObjectSignature> Copy(IEnumerable<IObjectSignature> objects);
    /// <summary>
    /// Creates a new object of type <typeparamref name="T" /> in the same collection as this object.
    /// If the collection is a sequence then the new object is placed right after this one.
    /// 
    /// This method is not applicable to top-level elements (IApplication, ISolution, IESpace, and IExtension)
    /// </summary>
    /// <typeparam name="T">The type of object to create. Must be of a type compatible with the collection,
    /// otherwise an exception will be raised.</typeparam>
    /// <param name="name">The name of the new object</param>
    /// <param name="key">The key of the new object</param>
    /// <returns>The new object</returns>
    T CreateSibling<T>(string name = null, IKey key = null) where T: IModelObject;
    /// <summary>
    /// Creates a new object of type <paramref name="type" /> in the same collection as this object.
    /// If the collection is a sequence then the new object is placed right after this one.
    /// 
    /// This method is not applicable to top-level elements (IApplication, ISolution, IESpace, and IExtension)
    /// </summary>
    /// <param name="type">The type of object to create. Must be of a type compatible with the collection,
    /// otherwise an exception will be raised.</param>
    /// <param name="name">The name of the new object</param>
    /// <param name="key">The key of the new object</param>
    /// <returns>The new object</returns>
    IModelObject CreateSibling(Type type, string name = null, IKey key = null);
    /// <summary>
    /// Duplicates an object into this object.
    /// </summary>
    /// <param name="obj">The object to copy</param>
    /// <returns>The created copy</returns>
    IObjectSignature Duplicate(IObjectSignature obj);
    /// <summary>
    /// Duplicates a list of objects into this object.
    /// </summary>
    /// <param name="objects">The objects to copy</param>
    /// <returns>The created copies</returns>
    IEnumerable<IObjectSignature> Duplicate(IEnumerable<IObjectSignature> objects);
    /// <summary>
    /// Moves the object back to the end of its child collection.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveToEnd();
    /// <summary>
    /// Moves the object back one position.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveBefore();
    /// <summary>
    /// Moves the object forward one position.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveAfter();
    /// <summary>
    /// Moves the object to a given position.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveToNewAbsoluteIndex(int newAbsoluteIndex);
    /// <summary>
    /// Moves the object to the position after the given sibling.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveAfterSibling(IObject sibling);
    /// <summary>
    /// Moves the object to the position before the given sibling.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveBeforeSibling(IObject sibling);
    /// <summary>
    /// Moves the object to a given relative position.
    /// This requires the object's child collection to be a sequence.
    /// </summary>
    void MoveToNewRelativeIndex(int newRelativeIndex);
    /// <summary>
    /// True if <see cref="M:OutSystems.Model.IObject.CanMoveToNewAbsoluteIndex(System.Int32)" /> can be called.
    /// </summary>
    bool CanMoveToNewAbsoluteIndex(int newAbsoluteIndex);
    /// <summary>
    /// True if <see cref="M:OutSystems.Model.IObject.CanMoveToNewRelativeIndex(System.Int32)" /> can be called.
    /// </summary>
    bool CanMoveToNewRelativeIndex(int newRelativeIndex);
}

public interface IObjectSignature : IModelObject {
    IKey ObjectKey { get; }
    bool IsValid { get; }
    string Digest { get; }
    /// <summary>
    /// A readable identifier (or null, if none exists) by which the object can be referred to
    /// in changeset definitions.
    /// </summary>
    string ReadableIdentifier { get; }
    IESpace GetESpace();
    IEnumerable<IValidationMessage> GetValidationMessages(bool includeMessagesFromDescendants = true);
    IEnumerable<T> GetAllDescendantsOfType<T>() where T: IObjectSignature;
    void ReplaceReferences(IObjectSignature previousObject, IObjectSignature newObject);
}

public interface IObjectWithMatch : IObjectSignature {
}

public interface IObjectWithNodes : IObject {
    IEnumerable<OutSystems.Model.Logic.Nodes.IActionNode> Nodes { get; }
    T CreateNode<T>(string name = null, IKey key = null) where T: OutSystems.Model.Logic.Nodes.IActionNode;
    OutSystems.Model.Logic.Nodes.IActionNode CreateNode(Type type, string name = null, IKey key = null);
}

/// <summary>
/// Interface that represents model elements that have an original name.
/// </summary>
/// <remarks>
/// This is not implemented by all model elements that have an original name yet.
/// </remarks>
public interface IObjectWithOriginalName : IObjectSignature {
    string OriginalName { get; }
}

public interface IOMLComponent {
    bool Is<T>();
}

public interface IOutputFormat : IObject {
    string Description { get; set; }
    OutSystems.Model.Data.IStructureSignature Type { get; set; }
    IEnumerable<IOutputFormatAttribute> OutputFormatAttributes { get; }
    IOutputFormatAttribute CreateOutputFormatAttribute(IKey key = null);
}

public interface IOutputFormatAttribute : IObject {
    OutSystems.Model.Types.IAttributeSignature Attribute { get; set; }
    string Description { get; set; }
}

public interface IOutputParameter : IObject, IOutputParameterSignature {
    new OutSystems.Model.Types.ITypeSignature DataType { get; set; }
    new string Description { get; set; }
    new string Name { get; set; }
    void SetDefaultValue(OutSystems.Model.Expressions.ExpressionDefinition value);
}

public interface IOutputParameterSignature : IObjectSignature {
    OutSystems.Model.Types.ITypeSignature DataType { get; }
    OutSystems.Model.Expressions.IExpression DefaultValue { get; }
    string Description { get; }
    string Name { get; }
}

public interface IPluginService {
    void StandaloneInitialize();
}

public interface IReference : IObject {
    string CreatedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; set; }
    string DependencyGraph { get; set; }
    string Description { get; set; }
    OutSystems.Model.Enumerations.SegmentationKind ESpaceType { get; set; }
    Guid Hash { get; set; }
    byte[] Icon { get; set; }
    bool IsMobilePlugin { get; set; }
    OutSystems.Model.Enumerations.JQueryVersion JQueryVersion { get; set; }
    OutSystems.Model.Enumerations.ReferenceKind Kind { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    OutSystems.Model.ModelPlugins.IRequiredModelPlugin ModelPlugin { get; }
    /// <summary>
    /// The key of the producer module
    /// </summary>
    IKey ModuleKey { get; }
    OutSystems.Model.Enumerations.SegmentationKind ModuleType { get; set; }
    string Name { get; set; }
    string ReactBundleName { get; set; }
    string ReactModuleName { get; set; }
    string Revision { get; }
    /// <summary>
    /// The digest before upgrades of the producer from which this reference was generated.
    /// </summary>
    string SpecificVersionHash { get; }
    string StylesheetModuleName { get; set; }
    string Version { get; }
    IEnumerable<OutSystems.Model.Logic.IClientActionSignature> ClientActions { get; }
    IEnumerable<OutSystems.Model.Data.IEntitySignature> Entities { get; }
    IEnumerable<IFolderSignature> Folders { get; }
    IEnumerable<OutSystems.Model.Processes.IGlobalEventSignature> GlobalEvents { get; }
    IEnumerable<OutSystems.Model.UI.IImageSignature> Images { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.IMobileFlowSignature> MobileFlows { get; }
    IEnumerable<OutSystems.Model.UI.Mobile.IMobileThemeSignature> MobileThemes { get; }
    IEnumerable<OutSystems.Model.Data.IResourceSignature> Resources { get; }
    IEnumerable<OutSystems.Model.Logic.IAppRoleSignature> Roles { get; }
    IEnumerable<OutSystems.Model.UI.IScriptSignature> Scripts { get; }
    IEnumerable<OutSystems.Model.Logic.IServerActionSignature> ServerActions { get; }
    IEnumerable<OutSystems.Model.Logic.IServiceActionSignature> ServiceActions { get; }
    IEnumerable<OutSystems.Model.Data.IStructureSignature> Structures { get; }
    IEnumerable<OutSystems.Model.UI.IWidgetDefinitionSignature> WidgetDefinitions { get; }
    bool IsFixedVersion { get; }
    /// <summary>
    /// Returns this reference as a dependency graph node with its dependencies.
    /// Returns null if its dependency graph has not been calculated yet. In this cases,
    /// use UpdateDependencyGraph to calculate this reference dependency graph.
    /// </summary>
    IDependency AsDependency { get; }
    IEnumerable<OutSystems.Model.Enumerations.MobileFramework> MobileFrameworks { get; }
    IEnumerable<OutSystems.Model.Processes.IGlobalEvent> Events { get; }
    /// <summary>
    /// Calculates the full dependencies graph. It firstly fetches this reference producer module and then recursively fetches
    /// its references that do not specify their full dependency graph in order to firstly calculate this property for them and then
    /// obtain the reference dependency graph as the addition of this reference producer module references dependencies graphs
    /// </summary>
    /// <param name="getDependencies">Callback to obtain the references list of the espace with the provided key</param>
    /// <returns></returns>
    IDependencyGraph UpdateDependencyGraph(Func<ValueTuple<IKey, string, string>, IEnumerable<ValueTuple<IKey, string, OutSystems.Model.Enumerations.SegmentationKind, string, string, string>>> getDependencies);
}

public interface ISequence : System.Collections.IEnumerable {
    void SetOrderTo(IEnumerable<IModelObject> order);
}

public interface ISequence<T> : ISequence, IEnumerable<T> {
    void SetOrderTo(IEnumerable<T> order);
    void MoveToStart(T element);
    void MoveToEnd(T element);
    void MoveBeforeSibling(T sibling, T element);
    void MoveAfterSibling(T sibling, T element);
}

public interface IServerInformationForUpgrade {
    string ServerDatabaseProviderKey { get; }
    IDictionary<IKey, string> ExternalEntityDatabaseProviders { get; }
}

public interface IShareable : IModelObject {
}

public interface IShareableESpaceObject : IShareable, IObject {
}

public interface IShareableESpaceObject<ConcreteT, SignatureT> : IShareableESpaceObject where ConcreteT: SignatureT, IObject where SignatureT: IObjectSignature {
}

public interface IThemeValues : IObject, IThemeValuesSignature {
    new string PropertyDisplayName { get; set; }
    new string PropertyName { get; set; }
    new string PropertyValue { get; set; }
}

public interface IThemeValuesSignature : IObjectSignature {
    string PropertyDisplayName { get; }
    string PropertyName { get; }
    string PropertyValue { get; }
}

public interface ITopLevelReferenceElement : IModelObject {
    Guid FullSignatureHash { get; set; }
    Guid CompatibilitySignatureHash { get; set; }
}

public interface ITranslation : IModelObject {
    OutSystems.Model.Enumerations.Culture Culture { get; }
    string Value { get; set; }
}

public interface IValidationMessage {
    string Id { get; }
    ValidationMessageType Type { get; }
    string Message { get; }
    string Detail { get; }
    string InContextDetail { get; }
    bool Hidden { get; set; }
    IObjectSignature Owner { get; }
    int HelpRef { get; }
    /// <summary>
    /// Warning: this will return the underlying model property name.
    /// </summary>
    IEnumerable<string> Properties { get; }
}

public enum MergeOption {
    None,
    IncludeChildren,
    IncludeDependencies,
}

public static class MethodExtensions {
    /// <summary>
    /// Determines whether a solution has system components
    /// </summary>
    /// <param name="fileByteArray">Application binary file</param>
    /// <returns></returns>
    public static bool HasSystemComponents(this byte[] fileByteArray) => default;
    public static bool IsSytemApplication(this byte[] fileByteArray) => default;
    public static OutSystems.Model.Enumerations.FileType GetFileType(this byte[] fileByteArray) => default;
    public static OutSystems.Model.Enumerations.SegmentationKind GetESpaceKind(this byte[] fileByteArray) => default;
    /// <summary>
    /// Tests if the given operations are allowed in an eSpace.
    /// Throws <c>InvalidOperationException</c> exception if the module kind is not eSpace
    /// </summary>
    /// <param name="eSpace">The binary representation of the eSpace</param>
    /// <param name="operations">The operations to test</param>
    /// <returns>true if <c>operations</c> is allowed for this module, false otherwise</returns>
    public static bool IsModuleOperationAllowed(this byte[] eSpace, ModuleOperations operations) => default;
    /// <summary>
    /// Tests if the given operations are allowed in an eSpace when using the provided secret key.
    /// Throws <c>InvalidOperationException</c> exception if the module kind is not eSpace.
    /// Throws <c>ArgumentException</c> exception if the provided key is not correct.
    /// </summary>
    /// <param name="eSpace">The binary representation of the eSpace</param>
    /// <param name="operations">The operations to test</param>
    /// <param name="secretKey">The key with which the module was locked with</param>
    /// <returns>true if <c>operations</c> is allowed for this module using the given secret key, false otherwise</returns>
    public static bool IsModuleOperationAllowed(this byte[] eSpace, ModuleOperations operations, string secretKey) => default;
    /// <summary>
    /// Gets version and LastUpgradeVersion of a given file.
    /// Throws <c>InvalidOperationException</c> exception if the file kind is not an Application, Solution, Espace or Extension.
    /// </summary>
    /// <param name="file">The binary representation of an Application, Solution, Espace or Extension</param>
    public static ValueTuple<Version, Version> GetVersions(this byte[] file) => default;
    /// <summary>
    /// Check if the application has any independent module (i.e any module that has no 'parent' application).
    /// </summary>
    /// <returns></returns>
    public static bool IsIndependentModulesApplication(this byte[] application) => default;
}

public static class ModelExtensions {
    public static T ConnectedAbove<T>(this T node, IFlowNode referenceNode, int separation = 1773, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ConnectedToTheLeftOf<T>(this T node, IFlowNode referenceNode, int separation = 1773, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ConnectedToTheRightOf<T>(this T node, IFlowNode referenceNode, int separation = 1773, bool snapToGrid = false) where T: IFlowNode => default;
    public static T SnapToGrid<T>(this T node) where T: IFlowNode => default;
    public static T Into<T>(this T node, IConnector connector, bool snapToGrid = false) where T: IFlowNode => default;
    public static OutSystems.Model.Logic.IAssignment CreateAssignment(this OutSystems.Model.Logic.Nodes.IAssignNode assign, OutSystems.Model.Expressions.ExpressionDefinition variable, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    /// <summary>
    /// Helper method for calling IExpresion.SubstituteAndSimplify. It receives a (varying list) of substitutions
    /// which is converted to a dictionary and then calls IExpresion.SubstituteAndSimplify.
    /// </summary>
    /// <param name="expr">The expression where substitutions will be applied</param>
    /// <param name="substitutions">A list of substitutions</param>
    public static void SubstituteAndSimplify(this OutSystems.Model.Expressions.IExpression expr, ValueTuple<IObjectSignature, object>[] substitutions) {}
    public static T GetSelfOrAncestorOfType<T>(this IModelObject obj) where T: IModelObject => default;
    public static T GetAncestorOfType<T>(this IModelObject obj) where T: IModelObject => default;
    /// <summary>
    /// Helper method to set the value of a particular mapping in the type conversion without
    /// having to look it up in the Mappings list.
    /// </summary>
    /// <param name="targetAttribute">The target attribute to set</param>
    /// <param name="value">The value to be used</param>
    public static void SetAttributeMapping(this OutSystems.Model.Expressions.ITypeConversionExpression typeConv, OutSystems.Model.Types.IAttributeSignature targetAttribute, OutSystems.Model.Expressions.ExpressionDefinition value) {}
    /// <summary>
    /// True if the signature represents signature objects, i.e. objects that represent elements defined
    /// in a different module.
    /// </summary>
    public static bool IsSignatureInterface(this Type type) => default;
    /// <summary>
    /// True if the object is a signature object, i.e. it is the (local) representation of an object
    /// defined in a different module.
    /// </summary>
    public static bool IsSignatureObject(this IModelObject obj) => default;
    public static bool IsConcrete(this Type type) => default;
    public static IEnumerable<IModelObject> GetAllDescendantsOfType(this IModelObject obj, Type type) => default;
    public static IEnumerable<T> GetAllDescendantsOfType<T>(this IModelObject obj) where T: IModelObject => default;
    public static IEnumerable<IModelObject> GetDirectChildrenOfType(this IModelObject obj, Type type) => default;
    public static IEnumerable<T> GetDirectChildrenOfType<T>(this IModelObject obj) where T: IModelObject => default;
    public static IModelObjectIdentifier<T> GetObjectId<T>(this T obj) where T: IModelObject => default;
    public static IModelObjectAddress GetObjectAddress(this IModelObject obj) => default;
    /// <summary>
    /// Exposes enclose in
    /// </summary>
    public static T EncloseIn<T>(this IEnumerable<OutSystems.Model.UI.IWidget> widgets) where T: OutSystems.Model.UI.IWidget => default;
    public static T EncloseIn<T>(this OutSystems.Model.UI.IWidget widget) where T: OutSystems.Model.UI.IWidget => default;
    public static OutSystems.Model.UI.IWidget EncloseIn(this IEnumerable<OutSystems.Model.UI.IWidget> widgets, Type containerType) => default;
    public static OutSystems.Model.UI.IWidget EncloseIn(this OutSystems.Model.UI.IWidget widget, Type containerType) => default;
    public static IEnumerable<OutSystems.Model.UI.IWidget> TryGetContiguousWidgets(this IEnumerable<OutSystems.Model.UI.IWidget> widgets) => default;
    public static IEnumerable<OutSystems.Model.ModelPlugins.IRequiredModelPlugin> GetAllowedModelPlugins(this IObjectSignature context) => default;
    public static void RefreshDependenciesUsingSignatureBinary(this IESpace consumer, IESpace producer, bool updateSpecificVersion = false) {}
    public static OutSystems.Model.Signatures.SignatureInformation GetSignatureInformation(this IESpace producer) => default;
    public static T Named<T>(this IEnumerable<T> items, string name) where T: IModelObject => default;
    public static T Labelled<T>(this IEnumerable<T> items, string label) where T: IModelObject => default;
    public static T At<T>(this IEnumerable<T> items, int horizontalPosition, int verticalPosition) where T: IFlowNode => default;
    public static OutSystems.Model.Expressions.IRecordLiteralField GetField(this OutSystems.Model.Expressions.IRecordLiteralExpression recordLiteral, OutSystems.Model.Types.IAttributeSignature attribute) => default;
    public static OutSystems.Model.Expressions.IRecordLiteralField GetField(this OutSystems.Model.Expressions.IRecordLiteralExpression recordLiteral, string fieldName) => default;
    public static OutSystems.Model.Expressions.IRecordLiteralField SetField(this OutSystems.Model.Expressions.IRecordLiteralExpression expression, OutSystems.Model.Types.IAttributeSignature attribute, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IRecordLiteralField SetField(this OutSystems.Model.Expressions.IRecordLiteralExpression expression, string fieldName, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.BusinessProcesses.IActivityDataItem obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.BusinessProcesses.IActivityDataItem obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.BusinessProcesses.Nodes.IAutomaticActivityNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.BusinessProcesses.Nodes.IAutomaticActivityNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetScreenArgumentValue(this OutSystems.Model.BusinessProcesses.Nodes.IHumanActivityNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.BusinessProcesses.Nodes.IHumanActivityNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Data.ILifeCycleDatabaseEvent obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Data.ILifeCycleDatabaseEvent obj, IInputParameterSignature parameter) => default;
    public static IAIArgument SetArgumentValue(this OutSystems.Model.Logic.IActionHandler obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.IActionHandler obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.ICallAgentNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.ICallAgentNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.IDestinationNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.IDestinationNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.IExecuteClientActionNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.IExecuteClientActionNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.IExecuteServerActionNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.IExecuteServerActionNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.IJavaScriptNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.IJavaScriptNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.IRefreshDataNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.IRefreshDataNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.ISendEmailNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.ISendEmailNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.ISQLNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.ISQLNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.ITriggerGlobalNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.ITriggerGlobalNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Logic.Nodes.ITriggerNode obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Logic.Nodes.ITriggerNode obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Processes.IGlobalEventHandler obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Processes.IGlobalEventHandler obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.Processes.ITimer obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.Processes.ITimer obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.UI.IEventHandler obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.UI.IEventHandler obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IBuiltinEvent obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IBuiltinEvent obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IEvent obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IEvent obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.UI.Mobile.Events.IUILifeCycleEvent obj, IInputParameterSignature parameter) => default;
    public static IArgument SetArgumentValue(this OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget obj, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget obj, IInputParameterSignature parameter) => default;
    public static void SetAttributeValue(this OutSystems.Model.Data.IRecord record, OutSystems.Model.Data.IEntityAttribute attribute, OutSystems.Model.Expressions.ExpressionDefinition value) {}
    public static Func<IKey, System.Threading.Tasks.Task<OutSystems.Model.Signatures.SignatureInformation>> ToAsync(this Func<IKey, OutSystems.Model.Signatures.SignatureInformation> syncFunc) => default;
    public static IArgument SetArgumentValue(this IEnumerable<IArgument> arguments, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static IAIArgument SetArgumentValue(this IEnumerable<IAIArgument> arguments, IInputParameterSignature parameter, OutSystems.Model.Expressions.ExpressionDefinition value) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this IEnumerable<IArgument> arguments, IInputParameterSignature parameter) => default;
    public static OutSystems.Model.Expressions.IExpression GetArgumentValue(this IEnumerable<IAIArgument> arguments, IInputParameterSignature parameter) => default;
    public static OutSystems.Model.UI.Mobile.Widgets.IPlaceholderContentWidget GetPlaceholderContent(this OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget webBlockInstance, string placeholderName) => default;
    public static T Below<T>(this T node, IFlowNode referenceNode, bool snapToGrid = false) where T: IFlowNode => default;
    public static T Below<T>(this T node, IFlowNode referenceNode, int separation, bool snapToGrid = false) where T: IFlowNode => default;
    public static T Above<T>(this T node, IFlowNode referenceNode, bool snapToGrid = false) where T: IFlowNode => default;
    public static T Above<T>(this T node, IFlowNode referenceNode, int separation, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ToTheRightOf<T>(this T node, IFlowNode referenceNode, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ToTheRightOf<T>(this T node, IFlowNode referenceNode, int separation, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ToTheLeftOf<T>(this T node, IFlowNode referenceNode, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ToTheLeftOf<T>(this T node, IFlowNode referenceNode, int separation, bool snapToGrid = false) where T: IFlowNode => default;
    public static T ConnectedBelow<T>(this T node, IFlowNode referenceNode, int separation = 1773, bool snapToGrid = false) where T: IFlowNode => default;
}

public class ModelFeatureToggle {
    public Nullable<bool> Studio { get; set; }
    public Nullable<bool> Server { get; set; }
}

public enum ModuleOperations {
    None,
    Load,
    Clone,
    All,
}

public class PluginService : IPluginService {
    public void StandaloneInitialize() {}
}

public enum RuntimeMajor {
    NET6,
    NET7,
    NET8,
}

public class Services {
    public IModelServices ModelServices { get; }
}

public enum Spacing {
    NotDefined,
    Smaller,
    Normal,
    Larger,
}

public class StyleProperties {
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
    public string HeaderColor { get; set; }
    public string BackgroundColor { get; set; }
    public double HeaderSize { get; set; }
    public Spacing Spacing { get; set; }
    public FontFamily Font { get; set; }
    public double FontSize { get; set; }
    public string PrimaryHover { get; set; }
    public string PrimarySelected { get; set; }
    public bool IsDarkPrimary { get; set; }
    public bool IsDarkSecondary { get; set; }
    public bool IsDarkBackground { get; set; }
    public bool IsDarkHeader { get; set; }
    public bool IsDarkLoginBackground { get; set; }
    public string FontColor { get; set; }
    public Border BorderRadius { get; set; }
    public bool HasShadow { get; set; }
    public string LoginBackgroundColor { get; set; }
    public string LoginBackgroundGradient { get; set; }
    public string LoginBackgroundImagePath { get; set; }
    public bool HasNativeLookAndFeel { get; set; }
    public bool CanUseFontAsLocalResource { get; set; }
    public string FontFamilyRegularResource { get; set; }
    public string FontFamilyBoldResource { get; set; }
}

public static class StylePropertiesHelper {
    public static string GetDisplayValue(this FontFamily font) => default;
    public static string GetGenericFamily(this FontFamily font) => default;
    public static string GetFontURL(this FontFamily font) => default;
    public static string GetFontWeight(this FontFamily font, bool isRegular) => default;
    public static string GetFontUnicodeRange(this FontFamily font) => default;
}

public class SystemObjectsKeys {
}

public enum ValidationMessageType {
    Info,
    Warning,
    Error,
}

