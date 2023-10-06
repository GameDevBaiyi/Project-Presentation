using System;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcBtForBattleSystems.SpecialActionConditions
{
public enum SpecialActionConditionEnum
{
    None,
    HasLowHpAlly,
    HasLowHpEnemy,
    HasAllyWithoutBuff,
    HasEnemyWithoutBuff,
    HasMultipleEnemies,
}

[Serializable]
public abstract class SpecialActionCondition
{
    public abstract SpecialActionConditionEnum SpecialActionConditionEnumPy { get; }
}
}