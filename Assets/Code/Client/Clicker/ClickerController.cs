using System;
using Client.Clicker.Configs;

namespace Client.Clicker
{
    internal sealed class ClickerController
    {
        private readonly IClickerConfig _config;
        private readonly ClickerState _state;

        public ClickerController(IClickerConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _state = new ClickerState(config);
        }

        public event Action Changed
        {
            add => _state.Changed += value;
            remove => _state.Changed -= value;
        }

        public int Currency => _state.Currency;
        public int Energy => _state.Energy;
        public int MaxEnergy => _state.MaxEnergy;
        public float AutoCollectSeconds => _config.AutoCollectSeconds;
        public float EnergyRestoreSeconds => _config.EnergyRestoreSeconds;

        public bool TryCollect() => _state.TryCollect(_config);
        public void RestoreEnergy() => _state.RestoreEnergy(_config);
    }
}