using System;
using System.Collections.Generic;

namespace FallingBlocks.Engine.Core.Core
{
    /// <summary>
    /// Encapsulates a list of render objects.
    /// </summary>
    public class RenderObjectCollection : ARenderObject
    {
        private List<ARenderObject> renderObjects = new List<ARenderObject>();

        /// <summary>
        /// Some behaviors require to remove objects from the render tree, this is not safe
        /// if we are enumerating them within the update cycle, so we need a safe point for doing such a modification.
        /// </summary>
        public Action AfterUpdate { get; set; }

        /// <summary>
        /// ctor. for an empty render object collection.
        /// </summary>
        public RenderObjectCollection()
        {
        }

        /// <summary>
        /// ctor. for an render object collection filled with the given objects.
        /// </summary>
        public RenderObjectCollection(params ARenderObject[] renderObjects)
        {
            this.renderObjects.AddRange(renderObjects);
        }

        /// <summary>
        /// We call the after update stuff after all children are properly updated.
        /// </summary>
        public override void Update(long timestamp)
        {
            base.Update(timestamp);
            if (this.AfterUpdate != null)
            {
                this.AfterUpdate();
                this.AfterUpdate = null;
            }
        }

        protected override void RenderInternal(IRenderContext context)
        {
            // Do nothing.
        }
    }
}