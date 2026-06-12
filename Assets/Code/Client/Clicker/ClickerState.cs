using System;
using Client.Clicker.Configs;

namespace Client.Clicker
{
    internal sealed class ClickerState
    {
        public event Action Changed;

        public ClickerState(IClickerConfig config)
        {
            Energy = config.MaxEnergy;
            MaxEnergy = config.MaxEnergy;
        }

        public int Currency { get; private set; }
        public int Energy { get; private set; }
        public int MaxEnergy { get; }

        public bool TryCollect(IClickerConfig config)
        {
            if (Energy < config.EnergyCostPerClick)
                return false;

            Energy -= config.EnergyCostPerClick;
            Currency += config.CurrencyPerClick;
            Changed?.Invoke();
            return true;
        }

        public void RestoreEnergy(IClickerConfig config)
        {
            var restored = Math.Min(MaxEnergy, Energy + config.EnergyRestoreAmount);
            if (restored == Energy)
                return;

            Energy = restored;
            Changed?.Invoke();
        }
    }
}