using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class KeyboardGesturer : IGesturer
    {
        private List<GestureEvent> _gesturesEvent;
        private KeyboardState _oldState;
        private Keys[] _acceptedKeys;

        public void Initialise()
        {
            _gesturesEvent = new List<GestureEvent>();

            _acceptedKeys = new[]
            {
                Keys.Up,
                Keys.Left,
                Keys.Escape,
                Keys.Space,
                Keys.Right,
                Keys.Down,
                Keys.Enter
            };
        }

        /// <summary>
        /// TODO just detect if key is down and throttle
        /// </summary>
        public IEnumerable<GestureEvent> DetectGestures()
        {
            var newState = Keyboard.GetState();

            _gesturesEvent.Clear();

            foreach (var key in _acceptedKeys)
            {
                if (newState.IsKeyDown(key) && _oldState.IsKeyUp(key))
                {
                    _gesturesEvent.Add(new KeyboardEvent(this, key));
                }
            }

            _oldState = newState;

            return _gesturesEvent;
        }
    }
}