using Microsoft.Xna.Framework;

namespace LD34
{
    public class MouseEvent : GestureEvent
    {
        public Vector2 Origin { get; private set; }
        public Vector2 Target { get; private set; }

        public MouseEvent(IGesturer provider, GestureType type, Vector2 origin, Vector2 target)
            : base(provider, type)
        {
            Origin = origin;
            Target = target;
        }
    }
}