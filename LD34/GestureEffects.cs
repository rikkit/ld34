using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class GestureEffects : IRenderer, IUpdater
    {
        private TextureInfo[] _textures;
        private List<GestureType> _blockedGestureTypes;

        private SpriteFont _font;
        private Vector2 _keyInfoPosition;
        private Keys? _keyPressed;

        public void Initialise()
        {
            _textures = new TextureInfo[1];
            _blockedGestureTypes = new List<GestureType>();
        }

        public void Update(GameTime gameTime, IEnumerable<GestureEvent> completedGestures)
        {
            var gestures = completedGestures.Where(g => !_blockedGestureTypes.Contains(g.Type)).ToList();
            
            var keyGesture = gestures.GetEvents<KeyboardEvent>().FirstOrDefault();
            _keyPressed = keyGesture?.Key;
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, Rectangle windowBounds)
        {
            const int texW = 60;
            const int texH = 60;

            var texture = new Texture2D(graphicsDevice, texW, texH);
            var texSize = texW * texH * Game1.BYTES_PER_PIXEL;
            var bytes = new byte[texSize];
            for (var i = 0; i < texSize; i++)
            {
                bytes[i] = 255;
            }
            texture.SetData(0, new Rectangle(0, 0, 60, 60), bytes, 0, texSize);
            _textures[0] = new TextureInfo(texture, windowBounds);

            _font = contentManager.Load<SpriteFont>("DPComic");
            _keyInfoPosition = new Vector2(windowBounds.Left + 10, windowBounds.Bottom - 50);
        }

        public void Render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            if (_keyPressed.HasValue)
            {
                spriteBatch.DrawString(_font, _keyPressed.Value.ToString(), _keyInfoPosition, Color.White);
            }

            spriteBatch.End();
        }
    }
}