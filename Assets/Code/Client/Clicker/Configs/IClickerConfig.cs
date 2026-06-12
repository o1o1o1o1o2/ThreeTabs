namespace Client.Clicker.Configs
{
    internal interface IClickerConfig
    {
        int CurrencyPerClick { get; }
        int EnergyCostPerClick { get; }
        int MaxEnergy { get; }
        int EnergyRestoreAmount { get; }
        float AutoCollectSeconds { get; }
        float EnergyRestoreSeconds { get; }
    }
}