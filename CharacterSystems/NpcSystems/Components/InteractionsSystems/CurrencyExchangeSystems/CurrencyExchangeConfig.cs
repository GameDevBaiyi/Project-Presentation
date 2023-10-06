using System;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.CurrencyExchangeSystems
{
[Serializable]
public class CurrencyExchangeConfig : InteractionConfig
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.CurrencyExchange;
}
}