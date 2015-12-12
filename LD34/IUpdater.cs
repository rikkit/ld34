using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LD34
{
    /// <summary>
    /// For want of a better name. Something that should update every loop iteration
    /// </summary>
    public interface IUpdater : IGameComponent
    {
        void Update(GameTime gameTime, IEnumerable<GestureEvent> completedGestures);
    }
}