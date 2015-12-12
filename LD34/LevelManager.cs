using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    public class LevelManager : IRenderer, IUpdater
    {
        private Color _bg1 = new Color(30, 30, 30);
        private Color _bg2 = new Color(50, 50, 50);

        private bool _bg12;

        private BeatTrigger _beatTrigger;
        private GraphicsDevice _graphicsDevice;

        public LevelManager(BeatTrigger beatBeatTrigger)
        {
            _beatTrigger = beatBeatTrigger;
        }

        public void Initialise()
        {
        }

        public void Update(GameTime gameTime, IEnumerable<GestureEvent> completedGestures)
        {
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, Rectangle windowBounds)
        {
            _graphicsDevice = graphicsDevice;
        }

        public void Render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (_beatTrigger.Triggering(gameTime))
            {
                _bg12 = !_bg12;
            }

            var newBgColor = _bg12 ? _bg1 : _bg2;
            _graphicsDevice.Clear(newBgColor);
        }
    }
}