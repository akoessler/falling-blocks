using System;

namespace FallingBlocks.Engine.Core.Core.Resource
{
    /// <summary>
    /// Baseclass for resources.
    /// </summary>
    public abstract class AResource
    {
        /// <summary>
        /// Allows the different rendereres to store their render specific stuff, during the prepare step if needed.
        /// </summary>
        public Object Tag { get; set; }

        /// <summary>
        /// So the system can check if a resource was already prepared for rendering.
        /// </summary>
        public bool IsPrepared { get; protected set; }

        /// <summary>
        /// Is called by the subsystem to prepare a resource if not already prepared.
        /// </summary>
        /// <param name="context"></param>
        public abstract void Prepare(IRenderContext context);
    }
}