using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    public struct TextureInfo
    {
        public Texture2D Texture;
        public Rectangle Canvas;

        public TextureInfo(Texture2D texture, Rectangle canvas)
        {
            Texture = texture;
            Canvas = canvas;
        }
    }
}