using System.Collections.Generic;

namespace LD34
{
    public interface IGesturer : IGameComponent
    {
        IEnumerable<GestureEvent> DetectGestures();
    }
}