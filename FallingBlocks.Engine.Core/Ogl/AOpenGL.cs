using System;

namespace FallingBlocks.Engine.Core.Ogl
{
    /// <summary>
    /// Subset of OpenGL used by this engine
    ///
    /// Remark:
    ///     Analysis showed that event OpenGL constants must have the same value over all platform (hopefully, this is true)
    /// </summary>
    public abstract class AOpenGL
    {
        public const uint GL_TEXTURE_2D = 0x0DE1;
        public const uint GL_BGRA = 0x80E1;

        public const uint GL_UNSIGNED_BYTE = 0x1401;

        //   TextureParameterName
        public const uint GL_TEXTURE_MAG_FILTER = 0x2800;
        public const uint GL_TEXTURE_MIN_FILTER = 0x2801;

        //   TextureMagFilter
        public const uint GL_NEAREST = 0x2600;
        public const uint GL_LINEAR = 0x2601;

        /// <summary>
        /// Treats each group of four vertices as an independent quadrilateral. Vertices 4n - 3, 4n - 2, 4n - 1, and 4n define quadrilateral n. N/4 quadrilaterals are drawn.
        /// </summary>
        public const uint GL_QUADS = 0x0007;

        /// <summary>
        /// Draws a single, convex polygon. Vertices 1 through N define this polygon.
        /// </summary>
        public const uint GL_POLYGON = 0x0009;

        public const uint GL_BLEND = 0x0BE2;
        public const uint GL_LINE_SMOOTH = 0x0B20;
        public const uint GL_SMOOTH = 0x1D01;
        public const uint GL_POINT_SMOOTH = 0x0B10;

        public const uint GL_SRC_ALPHA = 0x0302;
        public const uint GL_ONE_MINUS_SRC_ALPHA = 0x0303;

        public const uint GL_COLOR_BUFFER_BIT = 0x00004000;
        public const uint GL_DEPTH_BUFFER_BIT = 0x00000100;

        public const uint GL_VIEWPORT = 0x0BA2;

        public const uint GL_PROJECTION = 0x1701;

        public abstract void LoadIdentity();
        public abstract void Ortho(double left, double right, double bottom, double top, double zNear, double zFar);

        public abstract void Color(double red, double green, double blue, double alpha);
        public abstract void ClearColor(float red, float green, float blue, float alpha);
        public abstract void Clear(uint mask);
        public abstract void Enable(uint cap);
        public abstract void Disable(uint cap);
        public abstract void BlendFunc(uint sourceFactor, uint destinationFactor);

        public abstract void GetInteger(uint pname, int[] parameters);
        public abstract void MatrixMode(uint mode);

        public abstract void Begin(uint mode);
        public abstract void End();
        public abstract void TexCoord(float s, float t);
        public abstract void Vertex(float x, float y);
        public abstract void Flush();

        public abstract void BindTexture(uint target, uint texture);


        public abstract void PushMatrix();
        public abstract void PopMatrix();

        public abstract void Translate(double x, double y, double z);
        public abstract void Rotate(float angle, float x, float y);
        public abstract void Scale(double x, double y, double z);

        public abstract void GenTextures(int n, uint[] textures);

        public abstract void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border,
            uint format, uint type, byte[] pixels);

        public abstract void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border,
            uint format, uint type, IntPtr pixels);
        //public abstract void TexSubImage2D(uint target, int level, int xoffset, int yoffset, int width, int height, uint format, uint type, int[] pixels);

        public abstract void TexParameter(uint target, uint pname, float param);
        public abstract void TexParameter(uint target, uint pname, int param);

        public abstract void Text(BitmapText bitmapText, double x, double y, string text);
        public abstract void CalcTextSize(BitmapText bitmapText, string text, out float width, out float height);

        public enum BitmapText
        {
            GLUT_BITMAP_8_BY_13,
            GLUT_BITMAP_9_BY_15,
            GLUT_BITMAP_TIMES_ROMAN_10,
            GLUT_BITMAP_TIMES_ROMAN_24,
            GLUT_BITMAP_HELVETICA_10,
            GLUT_BITMAP_HELVETICA_12,
            GLUT_BITMAP_HELVETICA_18,
        }
    }
}