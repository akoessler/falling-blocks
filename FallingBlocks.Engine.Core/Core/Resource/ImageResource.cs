using System;
using System.Drawing;

namespace FallingBlocks.Engine.Core.Core.Resource
{
    /// <summary>
    /// An image resource of the render engine.
    /// </summary>
    public class ImageResource : AResource
    {
        /// <summary>
        /// Data of the image.
        /// </summary>
        public Bitmap Data { get; private set; }

        /// <summary>
        /// ctor.
        /// </summary>
        public ImageResource(String fileName)
        {
            this.Data = new Bitmap(fileName);
        }

        /// <summary>
        /// ctor.
        /// </summary>
        public ImageResource(Bitmap bitmap)
        {
            this.Data = bitmap;
        }

        /// <inheritdoc/>
        public override void Prepare(IRenderContext context)
        {
            if (!this.IsPrepared)
            {
                context.PrepareImageResource(this);
                this.IsPrepared = true;
            }
        }
    }
}