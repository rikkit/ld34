using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public static class InputEx
    {
        public static Vector2 GetPosition(this MouseState state)
        {
            return new Vector2(state.X, state.Y);
        }

        public static IEnumerable<T> GetEvents<T>(this IEnumerable<GestureEvent> events) where T : GestureEvent
        {
            return events.OfType<T>().Where(e => e.Active);
        }
    }
}