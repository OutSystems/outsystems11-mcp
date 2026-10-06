// REST model type definitions (payload structures and serialization shapes)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.REST.ModelDefinition.Types;

public enum SimpleType {
    Text,
    Integer,
    Decimal,
    LongInteger,
    Boolean,
    DateTime,
    BinaryData,
    Date,
    Time,
}

