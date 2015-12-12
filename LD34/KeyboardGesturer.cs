using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class KeyboardGesturer : IGesturer
    {
        private List<GestureEvent> _gesturesEvent;
        private KeyboardState _oldState;

        public void Initialise()
        {
            _gesturesEvent = new List<GestureEvent>();
        }

        /// <summary>
        /// TODO just detect if key is down and throttle
        /// </summary>
        public IEnumerable<GestureEvent> DetectGestures()
        {
            var newState = Keyboard.GetState();

            _gesturesEvent.Clear();

            if (newState.IsKeyDown(Keys.Up) && _oldState.IsKeyUp(Keys.Up))
            {
                _gesturesEvent.Add(new KeyboardEvent(this, Keys.Up));
            }

            _oldState = newState;

            return _gesturesEvent;
        }
    }
}