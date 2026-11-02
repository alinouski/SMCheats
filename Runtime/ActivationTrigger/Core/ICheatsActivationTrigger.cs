using System;

namespace SMCheats
{
    public interface ICheatsActivationTrigger
    {
        event Action OnActivateTrigger;

        void SetActiveModule(bool state);
    }
}