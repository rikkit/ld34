using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class MouseGesturer : IGesturer
    {
        private MouseState _oldState;
        private Vector2 _oldPosition;
        private GestureEvent[] _gesturesEvent;

        public void Initialise()
        {
            _gesturesEvent = new GestureEvent[3];
        }

        public IEnumerable<GestureEvent> DetectGestures()
        {
            var newState = Mouse.GetState();

            _gesturesEvent[0] = GetClickEvent(newState);
            _gesturesEvent[1] = GetScrollEvent(newState);

            var position = new Vector2(newState.X, newState.Y);
            _gesturesEvent[2] = new MouseEvent(this, GestureType.Move, _oldPosition, position);

            _oldState = newState;
            _oldPosition = position;

            return _gesturesEvent;
        }

        private GestureEvent GetScrollEvent(MouseState newState)
        {
            var scrollValue = newState.ScrollWheelValue;

            if (_oldState.ScrollWheelValue == scrollValue)
            {
                return null;
            }

            var gesture = new MouseEvent(this, GestureType.Scroll, new Vector2(0, _oldState.ScrollWheelValue), new Vector2(0, scrollValue));

            return gesture;
        }

        private GestureEvent GetClickEvent(MouseState newState)
        {
            if (newState.LeftButton == ButtonState.Pressed && _oldState.LeftButton == ButtonState.Released)
            {
                var mousePosition = newState.Position.ToVector2();
                return new MouseEvent(this, GestureType.Click, mousePosition, mousePosition);
            }

            return null;
        }
    }
}