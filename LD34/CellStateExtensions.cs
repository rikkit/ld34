namespace LD34
{
    public static class CellStateExtensions
    {
        public static bool Collides(this CellState state) => state == CellState.Active || state == CellState.Player;
    }
}