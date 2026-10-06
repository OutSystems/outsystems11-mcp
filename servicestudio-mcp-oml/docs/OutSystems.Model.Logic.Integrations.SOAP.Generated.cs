// SOAP service consumption interfaces (proxies, methods, faults, and serialization helpers)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Logic.Integrations.SOAP;

public interface ISOAPClient : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    string Description { get; set; }
    string Endpoint { get; }
    bool IntegratedAuthentication { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    string OriginalDescription { get; }
    string OriginalName { get; }
    string Prefix { get; set; }
    string URL { get; }
    bool UsesOldStyleChoice { get; set; }
    bool UsesOldStyleEnums { get; set; }
    string WSDL { get; }
    IEnumerable<ISOAPClientAction> Actions { get; }
    IEnumerable<ISOAPClientExternalSchema> ExternalSchemas { get; }
    IEnumerable<ISOAPClientImport> Imports { get; }
    IEnumerable<OutSystems.Model.Data.IStructure> WebReferenceStructures { get; }
}

public interface ISOAPClientAction : OutSystems.Model.Logic.IActionSignature {
    Nullable<int> CacheInMinutes { get; set; }
    string CreatedBy { get; set; }
    string Description { get; set; }
    bool Function { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string OriginalDescription { get; }
    string OriginalName { get; }
    string OriginalWSDLName { get; }
    Nullable<int> TimeoutInSeconds { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    new OutSystems.Model.ISequence<OutSystems.Model.IOutputParameter> OutputParameters { get; }
}

public interface ISOAPClientExternalSchema : OutSystems.Model.IObject {
    string Content { get; }
    string URL { get; }
}

public interface ISOAPClientImport : ISOAPClientExternalSchema {
}

public interface ISOAPService : OutSystems.Model.IObject {
    string CreatedBy { get; set; }
    string Description { get; set; }
    OutSystems.Model.Enumerations.HTTPSecurityWithCertificates HTTPSecurity { get; set; }
    bool IntegratedAuthentication { get; set; }
    bool InternalAccessOnly { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    IEnumerable<ISOAPServiceAction> Actions { get; }
    ISOAPServiceAction CreateAction(string name = null, OutSystems.Model.IKey key = null);
}

public interface ISOAPServiceAction : OutSystems.Model.Logic.IAction {
}

