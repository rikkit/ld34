using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

    public class LevelManager : IRenderer, IUpdater
    {
        internal enum CellState
        {
            Inactive = 0,
            One = 1
        }

        private const int GRID_WIDTH = 50;
        private const int GRID_HEIGHT = 80;

        private readonly Color _bg1 = new Color(30, 30, 30);
        private readonly Color _bg2 = new Color(50, 50, 50);
        private readonly BeatTrigger _beatTrigger;

        private bool _bg12;

        private GraphicsDevice _graphicsDevice;
        private CellState[,] _grid;
        private Rectangle _windowBounds;
        private int _pixelsPerX;
        private int _pixelsPerY;
        private Texture2D[] _textures;

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
            _windowBounds = windowBounds;
            _pixelsPerX = _windowBounds.Width/GRID_WIDTH;
            _pixelsPerY = _windowBounds.Height/GRID_HEIGHT;
            
            _textures = new[]
            {
                Texture2DEx.NewSolid(graphicsDevice, _pixelsPerX, _pixelsPerY, _bg1),
                Texture2DEx.NewSolid(graphicsDevice, _pixelsPerX, _pixelsPerY, _bg1)
            };

            _grid = new CellState[GRID_WIDTH, GRID_HEIGHT];
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    _grid[x, y] = 0;
                }
            }
        }

        public void Render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw background
            if (_beatTrigger.Triggering(gameTime))
            {
                _bg12 = !_bg12;
            }
            var newBgColor = _bg12 ? _bg1 : _bg2;
            _graphicsDevice.Clear(newBgColor);
            
            // Draw pixel overlay
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    var mode = (x + y) % 2 == 0;
                    var newState = mode ? CellState.One : CellState.Inactive;
                    _grid[x, y] = newState;
                    
                    var colour = mode || _bg12 ? Color.SlateBlue : Color.OrangeRed;
                    
                    var tex = _textures[(int)newState];
                    spriteBatch.Draw(tex, new Rectangle(_pixelsPerX * x, _pixelsPerY * y, _pixelsPerX, _pixelsPerY), colour);
                }
            }
            spriteBatch.End();
        }
    }
}