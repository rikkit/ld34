namespace LD34
{
    public abstract class GestureEvent
    {
        public GestureType Type { get; private set; }

        public IGesturer Provider { get; private set; }

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