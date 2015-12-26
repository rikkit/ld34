using Microsoft.Xna.Framework;

namespace LD34
{
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
}