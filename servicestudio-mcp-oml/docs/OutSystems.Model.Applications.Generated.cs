// Application module abstraction: base interfaces for versioning, lifecycle, and module persistence
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Applications;

public interface IModule : OutSystems.Model.IModelObject, IDisposable {
    Version Version { get; }
    string Name { get; set; }
    /// <summary>
    /// Text that documents the element.
    /// Useful for documentation purpose. The maximum size of this property is 2000 characters.
    /// </summary>
    string Description { get; set; }
    string Digest { get; }
    /// <summary>
    /// The digest of the whole content of an IModule at the time of the last save. Changing the module will not change this value.
    /// </summary>
    Guid DigestBeforeUpgrades { get; }
    string ActivationCode { get; }
    /// <summary>
    /// Saves the module to disk.
    /// </summary>
    /// <param name="path">Path to the target file.</param>
    void Save(string path);
    /// <summary>
    /// Saves the module in memory.
    /// </summary>
    /// <returns>The module's binary</returns>
    byte[] Save();
}

