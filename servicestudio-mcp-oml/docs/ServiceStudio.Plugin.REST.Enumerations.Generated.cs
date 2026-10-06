// Enumerations used by REST consumption plugin (HTTP method config, authentication, serialization)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.REST.Enumerations;

public enum AuthenticationType {
    None,
    BasicAuthentication,
    OAuth2,
}

public enum CallbackType {
    Unknown,
    OnBeforeRequest,
    OnBeforeRequestAdvanced,
    OnAfterResponse,
    OnAfterResponseAdvanced,
}

public enum ClientAuthentication {
    SendAsBasicAuthHeader,
    SendInBody,
}

public enum DateFormat {
    ISO,
    UNIXTimestamp,
    WCF,
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

public enum InputPlacement {
    UrlPlacement,
    HeaderPlacement,
    BodyPlacement,
}

public enum Interoperability {
    No,
    Yes,
}

public enum IsSensitive {
    Sensitive,
    NotSensitive,
}

public enum OutputPlacement {
    BodyPlacement,
    HeaderPlacement,
}

public enum RequestFormat {
    None,
    JSON,
    Binary,
    PlainText,
    FormURLEncoded,
    MultipartFormat,
}

public enum ResponseFormat {
    None,
    JSON,
    Binary,
    PlainText,
    MultipartFormat,
}

