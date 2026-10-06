// Text chunking strategies for AI and search: smart, fixed-size, sentence-based, and recursive splitting methods
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Data.ChunkingMethods;

/// <summary>
/// Base interface for all chunking methods used in AI semantic search.
/// </summary>
public interface IChunkingMethod {
    /// <summary>
    /// Gets the type of chunking method being used.
    /// </summary>
    OutSystems.Model.Enumerations.ChunkingMethod MethodKind { get; }
}

public interface IFixedSizeChunkingMethod : IChunkingMethod {
    int ChunkSize { get; set; }
    int ChunkOverlap { get; set; }
}

public interface INoneChunkingMethod : IChunkingMethod {
}

public interface IRecursiveChunkingMethod : IFixedSizeChunkingMethod {
    string Separators { get; set; }
}

public interface ISentenceBasedChunkingMethod : IFixedSizeChunkingMethod {
    int SentencesPerChunk { get; set; }
}

public interface ISmartChunkingMethod : IChunkingMethod {
}

