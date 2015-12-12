using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    public interface IRenderer : IGameComponent
    {
        void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, Rectangle windowBounds);

        void Render(SpriteBatch spriteBatch, GameTime gameTime);
    }
}