// REST API model definition (resource, data type, and schema description constructs)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.REST.ModelDefinition;

public enum InputLocation {
    Query,
    Path,
    Header,
    Body,
    FormData,
    Cookie,
}

