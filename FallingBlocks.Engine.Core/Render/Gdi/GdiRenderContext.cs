using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Core.Resource;
using FallingBlocks.Engine.Core.Util;

namespace FallingBlocks.Engine.Core.Render.Gdi
{
    /// <summary>
    /// Render context for rendering on a graphics.
    /// </summary>
    public class GdiRenderContext : IRenderContext
    {
        private readonly Graphics graphics;
        private readonly Stack<GraphicsState> graphicsStates = new Stack<GraphicsState>();

        /// <summary>
        /// Creates an render context from a GDI graphics object.
        /// </summary>
        /// <param name="gr"></param>
        public GdiRenderContext(Graphics gr)
        {
            this.graphics = gr;
            var bounds = this.graphics.VisibleClipBounds;
            this.graphics.TranslateTransform(bounds.Width / 2, bounds.Height / 2);

            this.graphics.CompositingQuality = CompositingQuality.HighSpeed;
            this.graphics.PixelOffsetMode = PixelOffsetMode.None;
            this.graphics.SmoothingMode = SmoothingMode.None;
            this.graphics.InterpolationMode = InterpolationMode.Default;

            this.PushMatrix();
        }

        /// <inheritdoc/>
        public void PrepareImageResource(ImageResource resource)
        {
            var bitmap = resource.Data;
            resource.Tag = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format32bppPArgb);
        }

        /// <inheritdoc/>
        public void Render(Image2D sprite2D)
        {
            var needRestore = this.Transform(sprite2D.Position, sprite2D.Scale, sprite2D.Rotation, new PointF(0f, 0f));

            var bitmap = sprite2D.Image.Tag as Bitmap ?? sprite2D.Image.Data;
            int width = sprite2D.Width;
            int height = sprite2D.Height;
            int x = (int) sprite2D.Position.X;
            int y = (int) sprite2D.Position.Y;
            int halfwidth = width / 2;
            int halfheight = height / 2;
            var destRectangle = new Rectangle(x - halfwidth, y - halfheight, width, height);

            ColorMatrix cm = new ColorMatrix();
            cm.Matrix33 = sprite2D.Opacity;
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            this.graphics.DrawImage(bitmap, destRectangle, 0, 0, width, height, GraphicsUnit.Pixel, ia);

            if (needRestore) this.PopMatrix();
        }

        /// <inheritdoc/>
        public void Render(Rectangle2D rectangle)
        {
            var needRestore =
                this.Transform(rectangle.Position, rectangle.Scale, rectangle.Rotation, new PointF(0f, 0f));

            var brush = new SolidBrush(rectangle.Color.GetValueOrDefault(Color.Black));
            this.graphics.FillRectangle(brush, rectangle.Position.X, rectangle.Position.Y, rectangle.Size.Width,
                rectangle.Size.Height);

            if (needRestore) this.PopMatrix();
        }

        /// <inheritdoc/>
        public void Render(Text2D text)
        {
            var needRestore = this.Transform(text.Position, text.Scale, text.Rotation, new PointF(0f, 0f));
            var font = this.GetFont(text.Font);

            var size = this.graphics.MeasureString("X", font);
            var x = text.Position.X;
            var y = text.Position.Y;
            y -= size.Height / 2;

            var brush = new SolidBrush(text.Color.GetValueOrDefault(Color.Black));
            this.graphics.DrawString(text.Text, font, brush, x, y);

            if (needRestore) this.PopMatrix();
        }

        /// <inheritdoc/>
        public void Init(ASceneGraph sg)
        {
            this.graphics.Clear(sg.Background.GetValueOrDefault(Color.White));
        }

        /// <inheritdoc/>
        public void BeginRender()
        {
        }

        /// <inheritdoc/>
        public void EndRender()
        {
        }

        public void PushMatrix()
        {
            this.graphicsStates.Push(this.graphics.Save());
        }

        public void PopMatrix()
        {
            this.graphics.Restore(this.graphicsStates.Pop());
        }

        public bool Transform(PointF center, float scale, float rotation, PointF translate)
        {
            if (!FloatHelper.FloatEquals(rotation, 0.0f) ||
                !FloatHelper.FloatEquals(scale, 1.0f) ||
                !FloatHelper.FloatEquals(translate.X, 0.0f) ||
                !FloatHelper.FloatEquals(translate.Y, 0.0f))
            {
                this.PushMatrix();
                this.graphics.TranslateTransform(center.X + translate.X, center.Y + translate.Y, 0);
                this.graphics.RotateTransform(rotation);
                this.graphics.ScaleTransform(scale, scale);
                this.graphics.TranslateTransform(center.X + translate.X, center.Y + translate.Y, 0);
                return true;
            }

            return false;
        }

        public RectangleF GetScreenBounds()
        {
            return this.graphics.VisibleClipBounds;
        }

        public void CalcTextSize(Text2D.TextFont textFont, string text, out float width, out float height)
        {
            var graphicsfont = this.GetFont(textFont);
            var size = this.graphics.MeasureString(text, graphicsfont);
            width = size.Width;
            height = size.Height;
        }

        private Font GetFont(Text2D.TextFont textFont)
        {
            Font font;
            switch (textFont)
            {
                case Text2D.TextFont.FixedSize_8:
                    font = new Font("Consolas", 8);
                    break;
                case Text2D.TextFont.FixedSize_9:
                    font = new Font("Consolas", 9);
                    break;
                case Text2D.TextFont.TimesRoman_10:
                    font = new Font("Times New Roman", 10);
                    break;
                case Text2D.TextFont.TimesRoman_24:
                    font = new Font("Times New Roman", 24);
                    break;
                case Text2D.TextFont.Helvetica_10:
                    font = new Font("Helvetica", 10);
                    break;
                case Text2D.TextFont.Helvetica_12:
                    font = new Font("Helvetica", 12);
                    break;
                case Text2D.TextFont.Helvetica_18:
                    font = new Font("Helvetica", 16);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return font;
        }
    }
}