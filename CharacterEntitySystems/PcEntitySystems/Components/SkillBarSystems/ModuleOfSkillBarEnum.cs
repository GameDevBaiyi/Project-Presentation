using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public enum ModuleOfSkillBarEnum
{
    None,
    Line_R0101_1,
    Line_R0101_2,
    Line_R0101_3,
    Line_R0102_1,
    Line_R0102_2,
    Line_R0102_3,
    Line_R0103_1,
    Line_R0103_2,
    Line_R0103_3,
    Line_R0104_1,
    Line_R0105_1,
    Line_R0105_2,
    Line_R0105_3,
    Line_R0106_1,
    Line_R0106_2,
    Line_R0106_3,
    Line_R0107_1,
    Line_R0107_2,
    Line_R0107_3,
    
}

public static class ModuleOfSkillBarEnumExtensions
{
    public static ModuleOfSkillBarConfig ModuleOfSkillBarConfig(this ModuleOfSkillBarEnum moduleOfSkillBarEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (moduleOfSkillBarEnum == ModuleOfSkillBarEnum.None) return null;

        if (!Details.CommonDesignSO.SkillConfigHubPy.ModuleEnum_ModuleConfigPy.TryGetValue(moduleOfSkillBarEnum,out ModuleOfSkillBarConfig moduleOfSkillBarConfig))
        {
            Debug.LogError($"未找到 {nameof(ModuleOfSkillBarEnum)}: {moduleOfSkillBarEnum} 对应的 {nameof(SkillBarSystems.ModuleOfSkillBarConfig)}.");
        }
        return moduleOfSkillBarConfig;
    }
}
}