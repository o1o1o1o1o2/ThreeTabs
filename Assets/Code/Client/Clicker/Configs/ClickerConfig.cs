using UnityEngine;

namespace Client.Clicker.Configs
{
    [CreateAssetMenu(menuName = "Three Tabs/Clicker Config", fileName = "ClickerConfig")]
    internal sealed class ClickerConfig : ScriptableObject, IClickerConfig
    {
        [SerializeField, Min(1)] private int _currencyPerClick = 1;
        [SerializeField, Min(1)] private int _energyCostPerClick = 1;
        [SerializeField, Min(1)] private int _maxEnergy = 1000;
        [SerializeField, Min(0)] private int _energyRestoreAmount = 10;
        [SerializeField, Min(0.1f)] private float _autoCollectSeconds = 3.0f;
        [SerializeField, Min(0.1f)] private float _energyRestoreSeconds = 10.0f;

        public int CurrencyPerClick => _currencyPerClick;
        public int EnergyCostPerClick => _energyCostPerClick;
        public int MaxEnergy => _maxEnergy;
        public int EnergyRestoreAmount => _energyRestoreAmount;
        public float AutoCollectSeconds => _autoCollectSeconds;
        public float EnergyRestoreSeconds => _energyRestoreSeconds;
    }
}