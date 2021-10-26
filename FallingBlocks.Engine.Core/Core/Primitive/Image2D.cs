using FallingBlocks.Engine.Core.Core.Resource;

namespace FallingBlocks.Engine.Core.Core.Primitive
{
    public class Image2D : ARenderObject
    {
        /// <summary>
        /// Image resource used by the sprite.
        /// </summary>
        public ImageResource Image { get; set; }

        /// <summary>
        /// Width of the sprite.
        /// </summary>
        public int Width
        {
            get { return this.Image.Data.Width; }
        }

        /// <summary>
        /// Height of the sprite.
        /// </summary>
        public int Height
        {
            get { return this.Image.Data.Height; }
        }

        /// <summary>
        /// ctor.
        /// </summary>
        public Image2D(ImageResource image)
        {
            this.Image = image;
        }

        /// <inheritdoc/>
        protected override void RenderInternal(IRenderContext context)
        {
            context.Render(this);
        }
    }
}