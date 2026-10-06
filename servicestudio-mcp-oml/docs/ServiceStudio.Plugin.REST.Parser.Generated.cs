// REST definition parsing and analysis utilities (spec ingestion and normalization)
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.REST.Parser;

public interface IDefinition {
    bool IsVersionValid();
}

public enum ParserType {
    Unknown,
    Text,
    Integer,
    LongInteger,
    Decimal,
    Boolean,
    DateTime,
    Date,
    Time,
    BinaryData,
    Object,
    Array,
}

