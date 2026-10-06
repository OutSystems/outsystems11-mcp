// Enumerations supporting native runtime (NR) widget configuration and behavior
// Enumeration types used by NR Widgets plugin
using System;
using System.Collections.Generic;

namespace ServiceStudio.Plugin.NRWidgets.Enumerations;

/// File type acceptance filters for file input widgets
public enum Accept {
    Image,
    Video,
    Any,
}

/// Text alignment options for widget content
public enum Align {
    Default,
    Left,
    Center,
    Right,
}

/// Display modes for dropdown widgets
public enum DropdownMode {
    Text,
    Custom,
}

/// Icon sizing options relative to font size
public enum IconSize {
    FontSize,
    Twotimes,
    Threetimes,
    Fourtimes,
}

/// Input field types for form widgets
public enum InputType {
    Text,
    Password,
    Number,
    Time,
    Date,
    Datetime,
    Phone,
    Email,
    Search,
}

/// Widget rendering modes
public enum Mode {
    Default,
    Custom,
}

/// Resource types for widget content
public enum Type {
    Static,
    External,
    Binary,
}

