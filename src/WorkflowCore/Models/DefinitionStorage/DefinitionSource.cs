using System;
using JetBrains.Annotations;

namespace WorkflowCore.Models.DefinitionStorage
{
    /// <summary>
    /// Base class for workflow definition source which can be serialized and stored
    /// </summary>
    [PublicAPI]
    public abstract class DefinitionSource
    {
        /// <summary>
        /// Identifier of workflow definition (scheme)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Version of workflow scheme
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Human-readable workflow description
        /// </summary>
        public string Description { get; set; }
    }
}
