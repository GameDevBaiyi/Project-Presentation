using System;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.RestSystems
{
[Serializable]
public class RestConfig : InteractionConfig
{
    public override InteractionEnum InteractionEnumPy => InteractionEnum.Rest;
}
}