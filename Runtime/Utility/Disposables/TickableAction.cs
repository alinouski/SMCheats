using System;

namespace SMCheats
{
    public class TickableAction : ICheatsUpdateTickable, IDisposable
    {
        private readonly Action _action;

        public TickableAction(Action action)
        {
            _action = action;
            CheatsTicker.Add(this);
        }
        
        public void Dispose()
        {
            CheatsTicker.Remove(this);
        }

        public void Tick()
        {
            _action?.Invoke();
        }
    }
}
