using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

    public static class CellStateExtensions
    {
        public static bool Collides(this CellState state) => state == CellState.Active || state == CellState.Player;
    }

    public enum CellState
    {
        Empty = 0,
        Pending = 1,
        Active = 2,
        Player = 3
    }

    public class GameEntity
    {
        public CellState[,] Template { get; private set; }
        public Point Start { get; private set; }

        private GameEntity() {}
        
        public static GameEntity Glider(Point position)
        {
            return new GameEntity
            {
                Start = position,
                Template = new CellState[3, 3]
                {
                    {
                        CellState.Active,
                        CellState.Empty,
                        CellState.Active
                    },
                    {
                        CellState.Empty,
                        CellState.Active,
                        CellState.Active
                    },
                    {
                        CellState.Empty,
                        CellState.Active,
                        CellState.Empty
                    }
                }
            };
        }

        public static GameEntity Dot(Point position)
        {
            return new GameEntity
            {
                Start = position,
                Template = new CellState[1, 1]
                {
                    {
                        CellState.Pending
                    }
                }
            };
        }
    }

    public class LevelManager : IRenderer, IUpdater
    {
        private const int GRID_WIDTH = 100;
        private const int GRID_HEIGHT = 80;
        private const int SPAWN_HEIGHT = 30;
        private const int CURRENT_BOARD_INDEX = 4;

        private readonly Color _bg1 = new Color(30, 30, 30);
        private readonly Color _bg2 = new Color(50, 50, 50);
        private readonly BeatTrigger _beatTrigger;
        private readonly Random _random;
        private readonly Queue<CellState[,]> _boards;

        private bool _bg12;

        private GraphicsDevice _graphicsDevice;
        private Rectangle _windowBounds;
        private int _pixelsPerX;
        private int _pixelsPerY;
        private Texture2D[] _textures;
        
        private CellState GetCellState(CellState[,] grid, int x, int y)
        {
            if (y == 1)
            {
                return CellState.Active;
            }

            if (x < 0 || y < 0 || x >= GRID_WIDTH || y >= GRID_HEIGHT)
            {
                return CellState.Empty;
            }

            return grid[x, y];
        }

        private Color CurrentBackground => _bg12 ? _bg1 : _bg2;

        public LevelManager(BeatTrigger beatBeatTrigger)
        {
            _random = new Random();
            _beatTrigger = beatBeatTrigger;

            const int boardCount = CURRENT_BOARD_INDEX + 1;
            _boards = new Queue<CellState[,]>(boardCount);
        }

        public void Initialise()
        {
            ResetGrid();
        }
        

        private void ResetGrid()
        {
            _boards.Clear();

            for (int i = 0; i <= CURRENT_BOARD_INDEX; i++)
            {
                // Seed the grid
                var grid = new CellState[GRID_WIDTH, GRID_HEIGHT];
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    for (int y = 0; y < GRID_HEIGHT; y++)
                    {
                        grid[x, y] = _random.NextDouble() > 0.8
                            ? CellState.Pending
                            : CellState.Empty;
                    }
                }

                _boards.Enqueue(grid);
            }
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
        }

        public void Update(GameTime gameTime, IEnumerable<GestureEvent> completedGestures)
        {
            if (completedGestures.GetEvents<KeyboardEvent>().Any(k => k.Key == Keys.F5))
            {
                ResetGrid();
                return;
            }

            long iteration;
            if (!_beatTrigger.Triggering(gameTime, out iteration))
            {
                return;
            }
            
            // 3 gliders and 1 dot every ten iterations
            var gliders = Enumerable.Range(0,3)
                .Select(i => new Point(_random.Next(0, GRID_WIDTH), _random.Next(0, SPAWN_HEIGHT)))
                .OrderBy(p => p.X)
                .Select(p => GameEntity.Glider(p));
            var dots = Enumerable.Range(0, 1)
                .Select(i => new Point(_random.Next(0, GRID_WIDTH), _random.Next(0, SPAWN_HEIGHT)))
                .OrderBy(p => p.X)
                .Select(p => GameEntity.Dot(p));
            var shapes = iteration%10 == 0 ? gliders.Concat(dots) : Enumerable.Empty<GameEntity>();
            var shapesArr = shapes.ToArray();

            GameEntity currentSpawning = null;
            var currentState = new CellState[9];
            var newGrid = new CellState[GRID_WIDTH, GRID_HEIGHT];
            var currentGrid = _boards.ElementAt(CURRENT_BOARD_INDEX);
            for (int x = 0; x < GRID_WIDTH; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    currentState[0] = GetCellState(currentGrid, x, y); // Self

                    if (currentSpawning == null)
                    {
                        currentSpawning = shapesArr.FirstOrDefault(shape => shape.Start.X == x && shape.Start.Y == y);
                    }

                    CellState? newState = null;
                    if (currentSpawning != null)
                    {
                        var p = currentSpawning.Start;

                        var oX = x - p.X;
                        var oY = y - p.Y;
                        
                        var boundX = currentSpawning.Template.GetUpperBound(0);
                        var boundY = currentSpawning.Template.GetUpperBound(1);

                        if (oY >= 0 && oX >= 0 && oY <= boundY && oX <= boundX)
                        {
                            newState = currentSpawning.Template[oX, oY];

                            if (oY == boundY && oX == boundX)
                            {
                                currentSpawning = null;
                            }
                        }
                    }
                    
                    if (currentState[0] == CellState.Pending)
                    {
                        newState = CellState.Active;
                    }
                    else if (!newState.HasValue)
                    {
                        currentState[1] = GetCellState(currentGrid, x, y - 1); // North
                        currentState[2] = GetCellState(currentGrid, x + 1, y - 1); // NE
                        currentState[3] = GetCellState(currentGrid, x + 1, y); // East
                        currentState[4] = GetCellState(currentGrid, x + 1, y + 1); // SE
                        currentState[5] = GetCellState(currentGrid, x, y + 1); // South
                        currentState[6] = GetCellState(currentGrid, x - 1, y + 1); // SW
                        currentState[7] = GetCellState(currentGrid, x - 1, y); // West
                        currentState[8] = GetCellState(currentGrid, x - 1, y - 1); // NW

                        var neighbourCount = currentState.Skip(1).Count(s => s.Collides());

                        // 2. Any live cell with two or three live neighbours lives on to the next generation.
                        if (currentState[0].Collides() && neighbourCount >= 2 && neighbourCount <= 3)
                        {
                            newState = currentState[0];
                        }
                        else if (!currentState[0].Collides() && neighbourCount == 3)
                        {
                            // 4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                            newState = CellState.Active;
                        }
                        else
                        {
                            // 1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                            // 3. Any live cell with more than three live neighbours dies, as if by over-population.
                            newState = CellState.Empty;
                        }
                    }
                    
                    if (newState.Value.Collides() && _boards.All(b => b[x, y].Collides()))
                    {
                        newState = CellState.Empty;
                    }

                    newGrid[x, y] = newState.Value;
                }
            }
            
            _boards.Enqueue(newGrid);
            _boards.Dequeue();
        }

        public void Render(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw background
            //if (_beatTrigger.Triggering(gameTime))
            //{
            //    _bg12 = !_bg12;
            //}
            var newBgColor = _bg12 ? _bg1 : _bg2;
            _graphicsDevice.Clear(newBgColor);

            var currentBoard = _boards.ElementAt(CURRENT_BOARD_INDEX);
            // Draw pixel overlay
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    Color c;
                    switch (currentBoard[x,y])
                    {
                        case CellState.Active:
                            c = Color.Crimson;
                            break;
                        case CellState.Pending:
                            c = Color.OrangeRed;
                            break;
                        case CellState.Player:
                            c = Color.CornflowerBlue;
                            break;
                        default:
                            continue;
                    }

                    var tex = _textures[0];
                    spriteBatch.Draw(tex, new Rectangle(_pixelsPerX * x, _pixelsPerY * y, _pixelsPerX, _pixelsPerY), c);
                }
            }
            spriteBatch.End();
        }
    }
}