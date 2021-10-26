using System.Collections.Generic;

namespace FallingBlocks.Engine.Core.Core.Resource
{
    public class ResourceCollection : AResource
    {
        private List<AResource> resources = new List<AResource>();

        /// <summary>
        /// Adds a resource to the resource collection.
        /// Uses a generic argument so that the same instance can be returned, which is handy when using it.
        /// </summary>
        public T Add<T>(T resource) where T : AResource
        {
            this.resources.Add(resource);
            this.IsPrepared = false;
            return resource;
        }

        /// <summary>
        /// Removes a resource from the resource collection.
        /// </summary>
        public T Remove<T>(T resource) where T : AResource
        {
            this.resources.Remove(resource);
            return resource;
        }

        /// <summary>
        /// Prepares all not already prepared resources in the collection.
        /// </summary>
        /// <param name="context"></param>
        public override void Prepare(IRenderContext context)
        {
            foreach (var resource in this.resources)
            {
                if (!resource.IsPrepared)
                {
                    resource.Prepare(context);
                }
            }
        }
    }
}