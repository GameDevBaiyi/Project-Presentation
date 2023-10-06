using System;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components
{
/// <summary>
/// Ai 使用技能时的目标类型
/// </summary>
public enum SkillTargetTypeEnum
{
    None,
    /// <summary>
    /// 最近的敌方.
    /// </summary>
    NearestEnemy,
    /// <summary>
    /// 指定范围内血量最低的己方和友方.
    /// </summary>
    LowestHpAlly,
    /// <summary>
    /// 指定范围内, 没有该技能的第一个 Buff 的己方和友方.
    /// </summary>
    NoBuffAlly,
    /// <summary>
    /// 指定范围内, 没有该技能的第一个 Buff 的敌方.
    /// </summary>
    NoBuffEnemy,
}

[Serializable]
public abstract class NpcTargetConfig
{
    public abstract SkillTargetTypeEnum SkillTargetTypeEnumPy { get; }
}
}