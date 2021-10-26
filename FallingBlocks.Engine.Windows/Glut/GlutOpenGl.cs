using System;
using FallingBlocks.Engine.Core.Ogl;
using Tao.OpenGl;
using TaoGlut = Tao.FreeGlut.Glut;

namespace FallingBlocks.Engine.Windows.Glut
{
    class GlutOpenGl : AOpenGL
    {
        public override void LoadIdentity()
        {
            Gl.glLoadIdentity();
        }

        public override void Ortho(double left, double right, double bottom, double top, double zNear, double zFar)
        {
            Gl.glOrtho(left, right, bottom, top, zNear, zFar);
        }

        public override void Color(double red, double green, double blue, double alpha)
        {
            Gl.glColor4d(red, green, blue, alpha);
        }

        public override void ClearColor(float red, float green, float blue, float alpha)
        {
            Gl.glClearColor(red, green, blue, alpha);
        }

        public override void Clear(uint mask)
        {
            Gl.glClear((int)mask);
        }

        public override void Enable(uint cap)
        {
            Gl.glEnable((int)cap);
        }

        public override void Disable(uint cap)
        {
            Gl.glDisable((int)cap);
        }

        public override void BlendFunc(uint sourceFactor, uint destinationFactor)
        {
            Gl.glBlendFunc((int)sourceFactor, (int)destinationFactor);
        }

        public override void GetInteger(uint pname, int[] parameters)
        {
            Gl.glGetIntegerv((int)pname, parameters);
        }

        public override void MatrixMode(uint mode)
        {
            Gl.glMatrixMode((int)mode);
        }

        public override void Begin(uint mode)
        {
            Gl.glBegin((int)mode);
        }

        public override void End()
        {
            Gl.glEnd();
        }

        public override void TexCoord(float s, float t)
        {
            Gl.glTexCoord2f(s, t);
        }

        public override void Vertex(float x, float y)
        {
            Gl.glVertex2d(x, y);
        }

        public override void Flush()
        {
            Gl.glFlush();
        }

        public override void BindTexture(uint target, uint texture)
        {
            Gl.glBindTexture((int)target, (int)texture);
        }

        public override void PushMatrix()
        {
            Gl.glPushMatrix();
        }

        public override void PopMatrix()
        {
            Gl.glPopMatrix();
        }

        public override void Translate(double x, double y, double z)
        {
            Gl.glTranslatef((float)x, (float)y, (float)z);
        }

        public override void Rotate(float angle, float x, float y)
        {
            Gl.glRotated(angle, x, y, 1.0);
        }

        public override void Scale(double x, double y, double z)
        {
            Gl.glScalef((float)x, (float)y, (float)z);
        }

        public override void GenTextures(int n, uint[] textures)
        {
            int[] tex = new int[textures.Length];
            Gl.glGenTextures(n, tex);
            for (int i = 0; i < tex.Length; ++i)
            {
                textures[i] = (uint)tex[i];
            }
        }

        public override void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, byte[] pixels)
        {
            throw new NotImplementedException();
        }

        public override void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels)
        {
            Gl.glTexImage2D((int)target, level, (int)internalformat, width, height, border, (int)format, (int)type, pixels);
        }

        public override void TexParameter(uint target, uint pname, float param)
        {
            Gl.glTexParameterf((int)target, (int)pname, param);
        }

        public override void TexParameter(uint target, uint pname, int param)
        {
            Gl.glTexParameteri((int)target, (int)pname, param);
        }

        public override void Text(BitmapText bitmapText, double x, double y, string text)
        {
            var bitmap = this.GetBitmapPtr(bitmapText);
            Gl.glRasterPos2d(x, y);
            TaoGlut.glutBitmapString(bitmap, text);
        }

        public override void CalcTextSize(BitmapText bitmapText, string text, out float width, out float height)
        {
            var bitmap = this.GetBitmapPtr(bitmapText);
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var charheight = TaoGlut.glutBitmapHeight(bitmap);
            height = charheight * lines.Length;

            var maxwidth = 0;
            foreach (var line in lines)
            {
                int lineWidth = 0;
                foreach (var ch in line)
                {
                    lineWidth += TaoGlut.glutBitmapWidth(bitmap, ch);
                }
                if (lineWidth > maxwidth) maxwidth = lineWidth;
            }

            width = maxwidth;
        }

        private IntPtr GetBitmapPtr(BitmapText bitmapText)
        {
            IntPtr bitmap;
            switch (bitmapText)
            {
                case BitmapText.GLUT_BITMAP_8_BY_13: bitmap = TaoGlut.GLUT_BITMAP_8_BY_13; break;
                case BitmapText.GLUT_BITMAP_9_BY_15: bitmap = TaoGlut.GLUT_BITMAP_9_BY_15; break;
                case BitmapText.GLUT_BITMAP_TIMES_ROMAN_10: bitmap = TaoGlut.GLUT_BITMAP_TIMES_ROMAN_10; break;
                case BitmapText.GLUT_BITMAP_TIMES_ROMAN_24: bitmap = TaoGlut.GLUT_BITMAP_TIMES_ROMAN_24; break;
                case BitmapText.GLUT_BITMAP_HELVETICA_10: bitmap = TaoGlut.GLUT_BITMAP_HELVETICA_10; break;
                case BitmapText.GLUT_BITMAP_HELVETICA_12: bitmap = TaoGlut.GLUT_BITMAP_HELVETICA_12; break;
                case BitmapText.GLUT_BITMAP_HELVETICA_18: bitmap = TaoGlut.GLUT_BITMAP_HELVETICA_18; break;
                default:
                    throw new ArgumentOutOfRangeException("bitmapText", bitmapText, null);
            }
            return bitmap;
        }
    }
}