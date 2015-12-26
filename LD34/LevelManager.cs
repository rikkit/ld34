using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Math;

namespace LD34
{
    public class LevelManager : IRenderer, IUpdater
    {
        private const int GRID_WIDTH = 50;
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
        private int _virtualBoardWidth;
        private int _virtualBoardHeight;
        private GameEntity[] _newShapes;
        private const int MAX_NEW_SHAPES = 10;

        private Color CurrentBackground => _bg12 ? _bg1 : _bg2;

        public LevelManager(BeatTrigger beatBeatTrigger)
        {
            _random = new Random();
            _beatTrigger = beatBeatTrigger;

            _virtualBoardWidth = (int)Ceiling(Sqrt(Pow(GRID_WIDTH, 2) * 2));
            _virtualBoardHeight = 400; // TODO lol this works for the values w = 50, h = 80, gave up trying to work this out. also the name doesn't mean what it says

            const int boardCount = CURRENT_BOARD_INDEX + 1;
            _boards = new Queue<CellState[,]>(boardCount);

            _newShapes = new GameEntity[10];
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
                var grid = new CellState[GRID_WIDTH, _virtualBoardHeight];
                for (int x = 0; x < GRID_WIDTH; x++)
                {
                    for (int y = 0; y < _virtualBoardHeight; y++)
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

        private void LoadGameEntities(long iteration, IList<GestureEvent> completedGestures, GameEntity[] newShapes)
        {
            // 3 gliders and 1 dot every ten iterations
            var gliders = Enumerable.Range(0, 5)
                .Select(i => new Point(_random.Next(0, GRID_WIDTH), _random.Next(0, SPAWN_HEIGHT)))
                .OrderBy(p => p.X)
                .Select(GameEntity.Glider);
            var dots = Enumerable.Range(0, 5)
                .Select(i => new Point(_random.Next(0, GRID_WIDTH), _random.Next(0, SPAWN_HEIGHT)))
                .OrderBy(p => p.X)
                .Select(GameEntity.Dot);
            var shapes = iteration % 10 == 0 ? gliders.Concat(dots) : Enumerable.Empty<GameEntity>();

            var enumerator = shapes.GetEnumerator();
            for (int i = 0; i < MAX_NEW_SHAPES; i++)
            {
                newShapes[i] = enumerator.MoveNext() ? enumerator.Current : null;
            }
        }

        public void Update(GameTime gameTime, IList<GestureEvent> completedGestures)
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
            
            LoadGameEntities(iteration, completedGestures, _newShapes);
            
            GameEntity currentSpawning = null;
            var currentState = new CellState[9];
            var newGrid = new CellState[GRID_WIDTH, _virtualBoardHeight];
            var currentGrid = _boards.ElementAt(CURRENT_BOARD_INDEX);
            for (int virtualX = 0; virtualX <  GRID_WIDTH * 2; virtualX++) // TODO not sure how many iterations this should actually do
            {
                for (int virtualY = 0; virtualY < GRID_HEIGHT; virtualY++)
                {
                    currentState[0] = GetCellState(currentGrid, virtualX, virtualY); // Self
                    
                    if (currentSpawning == null)
                    {
                        currentSpawning = _newShapes
                            .Where(shape => shape != null)
                            .FirstOrDefault(shape => shape.Start.X == virtualX && shape.Start.Y == virtualY);
                    }

                    CellState? newState = null;
                    if (currentSpawning != null)
                    {
                        var p = currentSpawning.Start;

                        var oX = virtualX - p.X;
                        var oY = virtualY - p.Y;
                        
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
                        currentState[1] = GetCellState(currentGrid, virtualX, virtualY - 1); // North
                        currentState[2] = GetCellState(currentGrid, virtualX + 1, virtualY - 1); // NE
                        currentState[3] = GetCellState(currentGrid, virtualX + 1, virtualY); // East
                        currentState[4] = GetCellState(currentGrid, virtualX + 1, virtualY + 1); // SE
                        currentState[5] = GetCellState(currentGrid, virtualX, virtualY + 1); // South
                        currentState[6] = GetCellState(currentGrid, virtualX - 1, virtualY + 1); // SW
                        currentState[7] = GetCellState(currentGrid, virtualX - 1, virtualY); // West
                        currentState[8] = GetCellState(currentGrid, virtualX - 1, virtualY - 1); // NW

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
                    
                    if (newState.Value.Collides() && _boards.All(b => GetCellState(b, virtualX, virtualY).Collides()))
                    {
                        newState = CellState.Empty;
                    }
                    
                    var actualX = virtualX % GRID_WIDTH;
                    var band = (virtualX / GRID_WIDTH);
                    var actualY = virtualY + (band * GRID_WIDTH);

                    newGrid[actualX, actualY] = newState.Value;
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

            // Draw pixel overlay
            var rotMat = Matrix.CreateRotationZ(MathHelper.ToRadians(45));
            var rotRect = new Vector2[4];
            var currentBoard = _boards.ElementAt(CURRENT_BOARD_INDEX);

            var boardOffset = new Vector2(0, 0);
            var yOffset = Sqrt(Pow(GRID_WIDTH, 2)/2) - 10;

            for (int x = 0; x < GRID_HEIGHT + yOffset; x++)
            {
                for (int y = 0; y < GRID_HEIGHT; y++)
                {
                    Color c;
                    switch (GetCellState(currentBoard, x, y))
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
                        case CellState.Bound:
                            c = Color.CornflowerBlue;
                            break;
                        default:
                            continue;
                    }
                    
                    var targetO = new Vector2(_pixelsPerX * x, _pixelsPerY * y);
                    var rotTranslation = new Vector2(GRID_WIDTH * _pixelsPerX, 0);
                    var targetT = targetO - rotTranslation;
                    var targetR = Vector2.Transform(targetT, rotMat);
                    var targetN = targetR + rotTranslation + boardOffset;
                    
                    var tex = _textures[0];
                    spriteBatch.Draw(tex, targetN, new Rectangle(0,0, _pixelsPerX, _pixelsPerY), c, MathHelper.ToRadians(45), new Vector2(0, 0), Vector2.One, SpriteEffects.None, 1);
                }
            }
            spriteBatch.End();
        }

        private CellState GetCellState(CellState[,] grid, int virtualX, int virtualY)
        {
            if (virtualX < 0 || virtualY < 0)
            {
                return CellState.Bound;
            }

            // Clip the top side
            if (virtualX + virtualY < GRID_WIDTH)
            {
                return CellState.Active;
            }

            // Clip the left side
            if (virtualY - virtualX == GRID_WIDTH)
            {
                return CellState.Bound;
            }

            // Clip the right side
            if (virtualX - virtualY == GRID_WIDTH + 1)
            {
                return CellState.Bound;
            }

            // clip the bottom
            if (virtualX + virtualY >= _virtualBoardHeight)
            {
                return CellState.Bound;
            }
            
            var band = virtualX / GRID_WIDTH;
            var actualX = virtualX % GRID_WIDTH;
            var actualY = band * GRID_WIDTH + virtualY;

            return grid[actualX, actualY];
        }
    }
}