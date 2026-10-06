// SOAP telemetry (metrics and tracing interfaces for SOAP calls)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.SOAP.SOAPTelemetry;

public enum ImportWsdlResult {
    Success,
    SuccessWithWarnings,
    Failure,
    Invalid,
}

public enum SoapVersion {
    None,
    SOAP_1_1,
    SOAP_1_2,
    Both,
}

public enum WsdlVersion {
    None,
    WSDL_1_1,
    WSDL_2_0,
}

