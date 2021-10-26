using System.Drawing;
using System.Drawing.Imaging;
using FallingBlocks.Engine.Core.Ogl;

namespace FallingBlocks.Engine.Core.Render.Ogl
{
    /// <summary>
    /// Creates an openGl texture from a given image.
    /// </summary>
    public class Texture2D
    {
        /// <summary>
        /// OpenGL texture index.
        /// </summary>
        public uint TextureIndx { get; private set; }

        /// <summary>
        /// ctor.
        /// </summary>
        public Texture2D(AOpenGL gl, Bitmap textureImage)
        {
            // Create a texture index.
            this.TextureIndx = createTextureIndex(gl);

            //  Bind the texture.
            gl.BindTexture(AOpenGL.GL_TEXTURE_2D, this.TextureIndx);

            //  Tell OpenGL where the texture data is.
            gl.TexImage2D(AOpenGL.GL_TEXTURE_2D, 0, 4, textureImage.Width, textureImage.Height, 0,
                AOpenGL.GL_BGRA, AOpenGL.GL_UNSIGNED_BYTE,
                textureImage.LockBits(new Rectangle(0, 0, textureImage.Width, textureImage.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb).Scan0);

            //  Specify linear filtering.
            gl.TexParameter(AOpenGL.GL_TEXTURE_2D, AOpenGL.GL_TEXTURE_MIN_FILTER, AOpenGL.GL_LINEAR);
            gl.TexParameter(AOpenGL.GL_TEXTURE_2D, AOpenGL.GL_TEXTURE_MAG_FILTER, AOpenGL.GL_LINEAR);
        }

        /// <summary>
        /// Makes the texture the current one.
        /// </summary>
        public void MakeCurrent(AOpenGL gl)
        {
            gl.BindTexture(AOpenGL.GL_TEXTURE_2D, this.TextureIndx);
        }

        private uint createTextureIndex(AOpenGL gl)
        {
            uint[] textures = new uint[1];
            //  Get one texture id, and stick it into the textures array.
            gl.GenTextures(1, textures);
            return textures[0];
        }
    }
}