using System;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems
{
[Serializable]
public abstract class InteractionConfig
{
    public abstract InteractionEnum InteractionEnumPy { get; }
}
}