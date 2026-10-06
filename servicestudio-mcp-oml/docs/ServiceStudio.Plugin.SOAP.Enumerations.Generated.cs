// Enumerations for SOAP consumption configuration (binding styles, encoding, faults)
// Enumeration types used by SOAP web service plugin
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.SOAP.Enumerations;

/// Authentication methods for SOAP web service consumption
public enum AuthenticationType {
    None,
    BasicAuthentication,
}

/// Callback handler types for SOAP service operations
public enum CallbackType {
    Unknown,
    OnRequest,
}

/// Behavior for handling default parameter values in SOAP requests
public enum DefaultValueBehavior {
    Send,
    DontSend,
    SendNil,
}

/// Placement locations for input parameters in SOAP messages
public enum InputPlacement {
    BodyPlacement,
    HeaderPlacement,
}

/// Windows integrated authentication configuration
public enum IntegratedAuthentication {
    Send,
    DontSend,
}

/// XML element nil value support configuration
public enum Nillable {
    Yes,
    No,
}

/// Placement locations for output parameters in SOAP responses
public enum OutputPlacement {
    BodyPlacement,
    HeaderPlacement,
}

/// XML schema wildcard processing instructions
public enum ProcessContents {
    Skip,
    Lax,
    Strict,
}

/// XML schema wildcard element types
public enum WildcardType {
    Any,
}

