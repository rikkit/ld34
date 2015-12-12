namespace LD34
{
    public abstract class GestureEvent
    {
        /// <summary>
        /// The type of event
        /// </summary>
        public GestureType Type { get; private set; }

        /// <summary>
        /// The class that sent this event
        /// </summary>
        public IGesturer Provider { get; private set; }

        /// <summary>
        /// Whether this event should be considered
        /// </summary>
        public bool Active { get; private set; }

        protected GestureEvent(IGesturer provider, GestureType type)
        {
            Provider = provider;
            Type = type;
            Active = true;
        }

        public void Deactivate()
        {
            Active = false;
        }
    }
}