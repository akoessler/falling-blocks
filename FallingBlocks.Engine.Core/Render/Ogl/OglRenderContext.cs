using System;
using System.Drawing;
using FallingBlocks.Engine.Core.Core;
using FallingBlocks.Engine.Core.Core.Primitive;
using FallingBlocks.Engine.Core.Core.Resource;
using FallingBlocks.Engine.Core.Ogl;
using FallingBlocks.Engine.Core.Util;

namespace FallingBlocks.Engine.Core.Render.Ogl
{
    /// <summary>
    /// Render context for openGl.
    /// </summary>
    public class OglRenderContext : IRenderContext
    {
        private AOpenGL gl;
        private uint lastTextureIndex = 999999;

        /// <summary>
        /// creates a render context based on openGl context.
        /// </summary>
        /// <param name="gl"></param>
        public OglRenderContext(AOpenGL gl)
        {
            this.gl = gl;
        }

        /// <inheritdoc/>
        public void PrepareImageResource(ImageResource resource)
        {
            if (resource.Tag == null)
            {
                // Create new open texture:
                resource.Tag = new Texture2D(this.gl, resource.Data);
            }
        }

        public void PushMatrix()
        {
            this.gl.PushMatrix();
        }

        public void PopMatrix()
        {
            this.gl.PopMatrix();
        }

        public bool Transform(PointF center, float scale, float rotation, PointF translate)
        {
            if (!FloatHelper.FloatEquals(rotation, 0.0f) ||
                !FloatHelper.FloatEquals(scale, 1.0f) ||
                !FloatHelper.FloatEquals(translate.X, 0.0f) ||
                !FloatHelper.FloatEquals(translate.Y, 0.0f))
            {
                this.PushMatrix();
                gl.Translate(center.X + translate.X, center.Y + translate.Y, 0);
                gl.Rotate(rotation, 0, 0);
                gl.Scale(scale, scale, 1.0f);
                gl.Translate(-center.X, -center.Y, 0);
                return true;
            }

            return false;
        }


        /// <inheritdoc/>
        public void Render(Image2D sprite2D)
        {
            //bool clear = false;
            float width = sprite2D.Width;
            float height = sprite2D.Height;
            float x = sprite2D.Position.X;
            float y = sprite2D.Position.Y;

            gl.Enable(AOpenGL.GL_TEXTURE_2D); // Ensure TEXTURE_2D is enabled!

            var texture2d = (Texture2D) sprite2D.Image.Tag;

            // A small optimisation if we draw the same image very often, e.g. Particles,
            // Remark: this could lead to a display problem if a new texture is loaded, as the loaded texture is always current as well.
            if (texture2d.TextureIndx != this.lastTextureIndex)
            {
                // Select the texture:
                texture2d.MakeCurrent(this.gl);
                this.lastTextureIndex = texture2d.TextureIndx;
            }

            // Do transformations if adjusted.
            bool clear = this.Transform(sprite2D.Position, sprite2D.Scale, sprite2D.Rotation, new PointF(0f, 0f));


            float halfwidth = width / 2.0f;
            float halfheight = height / 2.0f;
            // Draw the image with 4 vertices:
            gl.Begin(AOpenGL.GL_QUADS); // Each set of 4 vertices form a quad

            Color color = sprite2D.Color.GetValueOrDefault(Color.White);
            gl.Color(color.R / 256f, color.G / 256f, color.B / 256f, sprite2D.Opacity); // White, meaning no color?

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(x - halfwidth, y - halfheight); // x, y

            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(x + halfwidth, y - halfheight);

            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(x + halfwidth, y + halfheight);

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(x - halfwidth, y + halfheight);

            gl.End();
            if (clear)
            {
                this.PopMatrix();
            }
        }

        /// <inheritdoc/>
        public void Render(Rectangle2D rectangle)
        {
            var color = rectangle.Color.GetValueOrDefault(Color.Black);
            var x = rectangle.Position.X;
            var y = rectangle.Position.Y;
            var w = rectangle.Size.Width;
            var h = rectangle.Size.Height;
            var x2 = x + w;
            var y2 = y + h;

            gl.Disable(AOpenGL
                .GL_TEXTURE_2D); // need to disable TEXTURE_2D for whatever reason to have the correct color!

            // Do transformations if adjusted.
            bool clear = this.Transform(rectangle.Position, rectangle.Scale, rectangle.Rotation, new PointF(0f, 0f));

            gl.Begin(AOpenGL.GL_POLYGON);
            gl.Color(color.R / 255f, color.G / 255f, color.B / 255f, rectangle.Opacity);
            gl.Vertex(x, y);
            gl.Vertex(x2, y);
            gl.Vertex(x2, y2);
            gl.Vertex(x, y2);
            gl.End();

            gl.Enable(AOpenGL.GL_TEXTURE_2D); // Enable TEXTURE_2D again

            if (clear)
            {
                this.PopMatrix();
            }
        }

        /// <inheritdoc/>
        public void Render(Text2D text)
        {
            var bitmapText = this.GetBitmapText(text.Font);

            gl.Disable(AOpenGL
                .GL_TEXTURE_2D); // need to disable TEXTURE_2D for whatever reason to have the correct color!

            var color = text.Color.GetValueOrDefault(Color.Black);
            var x = text.Position.X;
            var y = text.Position.Y;

            // Do transformations if adjusted.
            bool clear = this.Transform(text.Position, text.Scale, text.Rotation, new PointF(0f, 0f));

            gl.Begin(AOpenGL.GL_POINT_SMOOTH);
            gl.Color(color.R / 255f, color.G / 255f, color.B / 255f, text.Opacity);
            gl.Text(bitmapText, x, y, text.Text);
            gl.End();

            gl.Enable(AOpenGL.GL_TEXTURE_2D); // Enable TEXTURE_2D again

            if (clear)
            {
                this.PopMatrix();
            }
        }

        public void Init(ASceneGraph sg)
        {
            //  A bit of extra initialisation here, we have to enable textures.
            gl.Enable(AOpenGL.GL_TEXTURE_2D);
            gl.Enable(AOpenGL.GL_BLEND);
            gl.Enable(AOpenGL.GL_LINE_SMOOTH);
            gl.Enable(AOpenGL.GL_POINT_SMOOTH);
            gl.Enable(AOpenGL.GL_SMOOTH);

            if (sg.Background != null)
            {
                var col = sg.Background.Value;
                gl.ClearColor(col.R / 255f, col.G / 255f, col.B / 255f, col.A / 255f);
            }

            gl.BlendFunc(AOpenGL.GL_SRC_ALPHA, AOpenGL.GL_ONE_MINUS_SRC_ALPHA);
        }

        public void BeginRender()
        {
            enabled2d();
            gl.Clear(AOpenGL.GL_COLOR_BUFFER_BIT |
                     AOpenGL.GL_DEPTH_BUFFER_BIT); // Clear The Screen And The Depth Buffer
        }

        public void EndRender()
        {
            this.gl.Flush();
        }

        public RectangleF GetScreenBounds()
        {
            int[] viewPort = new int[4];
            gl.GetInteger(AOpenGL.GL_VIEWPORT, viewPort);
            float width = viewPort[2];
            float height = viewPort[3];

            return new RectangleF(-width / 2, -height / 2, width, height);
        }

        public void CalcTextSize(Text2D.TextFont textFont, string text, out float width,
            out float height)
        {
            var bitmapText = this.GetBitmapText(textFont);
            gl.CalcTextSize(bitmapText, text, out width, out height);
        }

        private void enabled2d()
        {
            int[] viewPort = new int[4];
            gl.GetInteger(AOpenGL.GL_VIEWPORT, viewPort);

            gl.MatrixMode(AOpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // Set up the orthographic projection

            gl.Ortho(viewPort[0], viewPort[0] + viewPort[2],
                viewPort[1] + viewPort[3], viewPort[1], -1, 1);

            // We want the 0,0 in the center of our window.
            float width = viewPort[2];
            float height = viewPort[3];
            gl.Translate(width / 2, height / 2, 0);

            // We want a good readable
            //double ratio = (double)viewPort[2] / (double)viewPort[3];
            //gl.Ortho(0, 200 * ratio, 200, 0, -1, 1);
        }

        private AOpenGL.BitmapText GetBitmapText(Text2D.TextFont textFont)
        {
            AOpenGL.BitmapText bitmapText;
            switch (textFont)
            {
                case Text2D.TextFont.FixedSize_8:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_8_BY_13;
                    break;
                case Text2D.TextFont.FixedSize_9:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_9_BY_15;
                    break;
                case Text2D.TextFont.TimesRoman_10:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_TIMES_ROMAN_10;
                    break;
                case Text2D.TextFont.TimesRoman_24:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_TIMES_ROMAN_24;
                    break;
                case Text2D.TextFont.Helvetica_10:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_HELVETICA_10;
                    break;
                case Text2D.TextFont.Helvetica_12:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_HELVETICA_12;
                    break;
                case Text2D.TextFont.Helvetica_18:
                    bitmapText = AOpenGL.BitmapText.GLUT_BITMAP_HELVETICA_18;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return bitmapText;
        }
    }
}