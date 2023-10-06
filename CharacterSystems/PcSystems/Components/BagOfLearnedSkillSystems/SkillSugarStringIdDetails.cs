using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
public abstract class SkillSugarStringIdDetails : Details
{
    public static SkillSugarString GetSkillSugarString(SkillSugarStringId skillSugarStringId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = skillSugarStringId.InstanceId;
        if (instanceId == 0) return null;

        skillSugarStringId.CharacterId.PcPy.BagOfLearnedSkillPy.SkillSugarStringHubPy.TryGetInstance(instanceId,out SkillSugarString skillSugarString);
        return skillSugarString;
    }
}
}