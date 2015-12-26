using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    public static class Texture2DEx
    {
        public static Texture2D NewSolid(GraphicsDevice graphicsDevice, int width, int height, Color colour)
        {
            var texture = new Texture2D(graphicsDevice, width, height);
            var texSize = width * height * Game1.BYTES_PER_PIXEL;
            var bytes = new byte[texSize];
            for (var i = 0; i < texSize; i++)
            { 
                bytes[i] = 255;
            }
            texture.SetData(0, new Rectangle(0, 0, width, height), bytes, 0, texSize);

            return texture;
        }
    }
}