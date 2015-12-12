using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LD34
{
    public class FrameRateCounter : IRenderer
    {
        private SpriteFont _font;
        private Vector2 _counterPosition;
        private double _time;
        private TimeSpan _lastFrameTime;

        private const float COUNTER_SMOOTHING_RATIO = 0.1f;

        public void Initialise()
        {
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice, Rectangle windowBounds)
        {
            _font = contentManager.Load<SpriteFont>("DPComic");
            _counterPosition = new Vector2(windowBounds.Right - 50, windowBounds.Top + 10);
        }

        public void Render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            var currentTime = gameTime.TotalGameTime;
            var timeSinceLastFrame = currentTime - _lastFrameTime;

            _time = _time * (1.0 - COUNTER_SMOOTHING_RATIO) + timeSinceLastFrame.TotalSeconds * COUNTER_SMOOTHING_RATIO;

            var fps = 1 / _time;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            spriteBatch.DrawString(_font, Math.Round(fps, 0).ToString(), _counterPosition, Color.White);
            spriteBatch.End();

            _lastFrameTime = currentTime;
        }
    }
}