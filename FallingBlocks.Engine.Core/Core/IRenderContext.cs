using System.Drawing;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Core.Resource;

namespace FallingBlocks.Engine.Core.Core
{
    /// <summary>
    /// Render context.
    /// </summary>
    public interface IRenderContext
    {
        /// <summary>
        /// Initialize the context.
        /// </summary>
        void Init(ASceneGraph sg);

        /// <summary>
        /// Is called to prepare a resource.
        /// </summary>
        /// <param name="resource"></param>
        void PrepareImageResource(ImageResource resource);

        /// <summary>
        /// Actions before rendering.
        /// </summary>
        void BeginRender();

        /// <summary>
        /// Actions after rendering.
        /// </summary>
        void EndRender();

        /// <summary>
        /// Store the current transformation matrix.
        /// </summary>
        void PopMatrix();

        /// <summary>
        /// Restore the current transformation matrix.
        /// </summary>
        void PushMatrix();

        /// <summary>
        /// Apply a generic transformation.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        /// <param name="translate"></param>
        /// <returns></returns>
        bool Transform(PointF center, float scale, float rotation, PointF translate);

        /// <summary>
        /// Renders a given 2D sprite
        /// </summary>
        void Render(Image2D sprite2D);

        /// <summary>
        /// Renders a given 2D rectangle
        /// </summary>
        void Render(Rectangle2D rectangle);

        /// <summary>
        /// Renders a given 2D text
        /// </summary>
        void Render(Text2D rectangle);

        /// <summary>
        /// Returns the screen bounds, i.e. the minimun/maximum screen coordinates
        /// </summary>
        /// <returns></returns>
        RectangleF GetScreenBounds();

        void CalcTextSize(Text2D.TextFont textFont, string text, out float width, out float height);
    }
}