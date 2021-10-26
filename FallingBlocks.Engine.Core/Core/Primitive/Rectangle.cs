using System.Drawing;

namespace FallingBlocks.Engine.Core.Core.Primitive
{
    public class Rectangle2D : ARenderObject
    {
        public SizeF Size { get; set; }

        /// <summary>
        /// ctor.
        /// </summary>
        public Rectangle2D()
        {
        }

        /// <inheritdoc/>
        protected override void RenderInternal(IRenderContext context)
        {
            context.Render(this);
        }
    }
}