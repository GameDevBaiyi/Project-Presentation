using System;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.CurrencyExchangeSystems
{
[Serializable]
public class CurrencyExchange : Interaction
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.CurrencyExchange;

    public CurrencyExchange(CharacterId characterId) : base(characterId)
    {
    }
}
}