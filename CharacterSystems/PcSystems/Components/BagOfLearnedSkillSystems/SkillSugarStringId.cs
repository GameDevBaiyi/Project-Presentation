using System;

using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
[Serializable]
public struct SkillSugarStringId
{
    public int InstanceId;
    public CharacterId CharacterId;

    [ShowInInspector]
    public SkillSugarString SkillSugarStringPy => SkillSugarStringIdDetails.GetSkillSugarString(this);
}
}