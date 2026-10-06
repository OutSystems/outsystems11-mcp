// Mobile UI plugin extension interfaces (custom widgets, handlers, and platform bridges)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.ModelPlugins.UI.Mobile;

public interface ITheme : OutSystems.Model.ModelPlugins.IShareableModelPluginObject<ITheme, OutSystems.Model.UI.Mobile.IMobileThemeSignature> {
    string Name { get; set; }
    string Description { get; set; }
    int Columns { get; set; }
    int ColumnWidth { get; set; }
    OutSystems.Model.Enumerations.GridType GridType { get; set; }
    int GutterWidth { get; set; }
    int GutterWidthPercentage { get; set; }
    string InvisibleStyleSheet { get; }
    string StyleSheet { get; set; }
    Nullable<int> MaxWidth { get; set; }
    Nullable<int> MinWidth { get; set; }
    int TotalWidth { get; }
}

public class ThemeAttribute : Attribute {
    public string Description { get; set; }
    public OutSystems.Model.Enumerations.GridType GridType { get; set; }
    public int Columns { get; set; }
    public int GutterWidth { get; set; }
    public int ColumnWidth { get; set; }
    public bool HasMinWidth { get; set; }
    public int MinWidth { get; set; }
    public bool HasMaxWidth { get; set; }
    public int MaxWidth { get; set; }
}

