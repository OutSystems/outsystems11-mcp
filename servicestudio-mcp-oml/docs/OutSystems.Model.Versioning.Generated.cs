// Versioning constructs (model digests, differences, upgrade tracking)
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Versioning;

/// <summary>
/// Strategy used when committing a transaction
/// </summary>
public enum CommitStrategy {
    ReexecuteOnConflict,
    FullyOptimistic,
}

public interface IVersionedObject {
    /// <summary>
    /// A value that changes once in each transaction where the object is modified.
    /// 
    /// This allows for consumers of the object to detect in a lightweight way if the object may be different
    /// compared to the last time it was accessed.
    /// 
    /// Note that the value will increase on transactions that change the object, and also when undoing/redoing;
    /// undo returns to a previous model state, with the exception and that includes increasing the generation
    /// to an higher value.
    /// </summary>
    long Generation { get; }
    /// <summary>
    /// True if the object has been deleted.
    /// </summary>
    bool WasDeleted { get; }
    /// <summary>
    /// Deletes the object, which has the side effect of removing it from its parent's collections.
    /// </summary>
    void Delete();
}

