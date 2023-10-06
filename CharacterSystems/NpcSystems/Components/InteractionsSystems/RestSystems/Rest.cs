using System;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems
{
[Serializable]
public class Rest : Interaction
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Rest;

    public Rest(CharacterId characterId) : base(characterId)
    {
    }
}
}