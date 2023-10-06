using System;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.ForgingSystems
{
[Serializable]
public class Forging : Interaction
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Forging;

    [ShowInInspector]
    private Smelting _smelting;
    public Smelting SmeltingPy => this._smelting;
    public void SetSmelting(Smelting smelting)
    {
        this._smelting = smelting;
    }

    [ShowInInspector]
    private Hammering _hammering;
    public Hammering HammeringPy => this._hammering;
    public void SetHammering(Hammering hammering)
    {
        this._hammering = hammering;
    }
    public Forging(CharacterId characterId) : base(characterId)
    {
    }
}
}