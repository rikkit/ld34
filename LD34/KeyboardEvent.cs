using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class KeyboardEvent : GestureEvent
    {
        public Keys Key { get; private set; }

        public KeyboardEvent(IGesturer provider, Keys key)
            : base(provider, GestureType.KeyPress)
        {
            Key = key;
        }
    }
}