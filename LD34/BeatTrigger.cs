using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LD34
{
    public class BeatTrigger : IUpdater
    {
        /// <summary>
        /// Tolerate small delays
        /// </summary>
        private readonly int _tolerance = 50;

        private readonly int _frequencyStep;
        private readonly int _initialFrequencyMs;
        private TimeSpan _lastTriggeredTime;

        private int _currentFrequencyMs;

        public BeatTrigger(int initialFrequencyMs, int frequencyStep = 50)
        {
            _lastTriggeredTime = TimeSpan.Zero;
            _initialFrequencyMs = initialFrequencyMs;
            _currentFrequencyMs = initialFrequencyMs;
            _frequencyStep = frequencyStep;
        }

        public void Initialise()
        {
        }

        public void Update(GameTime gameTime, IEnumerable<GestureEvent> completedGestures)
        {
            foreach (var keyEvent in completedGestures.GetEvents<KeyboardEvent>())
            {
                switch (keyEvent.Key)
                {
                    case Keys.Up:
                        _currentFrequencyMs -= _frequencyStep; // Increase frequency on up
                        break;
                    case Keys.Down:
                        _currentFrequencyMs += _frequencyStep;
                        break;
                    case Keys.F5:
                        _currentFrequencyMs = _initialFrequencyMs;
                        break;
                }
            }
        }

        public bool Triggering(GameTime gameTime)
        {
            if (gameTime.TotalGameTime == _lastTriggeredTime)
            {
                return true;
            }

            var delta = gameTime.TotalGameTime - _lastTriggeredTime;

            if (delta.TotalMilliseconds > _currentFrequencyMs + _tolerance)
            {
                _lastTriggeredTime = gameTime.TotalGameTime;
                return true;
            }
            
            return false;
        }
    }
}