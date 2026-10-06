// Enumerations used by REST service publication plugin (exposure settings, auth, caching)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.RESTService.Enumerations;

public enum Authentication {
    None,
    Basic,
    Custom,
}

public enum AuthenticationCallbackType {
    None,
    Custom,
    Basic,
    OAuth2,
}

public enum CallbackType {
    Unknown,
    OnRequest,
    OnResponse,
}

public enum CrossSiteRequests {
    Allow,
    Restrict,
}

public enum DefaultValueBehavior {
    Send,
    DontSend,
}

public enum HTTPMethod {
    GET,
    PUT,
    POST,
    DELETE,
    PATCH,
}

public enum HttpSecurity {
    None,
    SSL,
}

public enum InternalAccess {
    No,
    Yes,
}

public enum Interoperability {
    No,
    Yes,
}

public enum IsSensitive {
    Sensitive,
    NotSensitive,
}

public enum ReceiveIn {
    URL,
    Header,
    Body,
}

public enum SendIn {
    Body,
    Header,
}

public enum ShowDocumentation {
    No,
    Yes,
}

