using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.HeronTeamSystems.Components.ManualSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
public static class CharacterEnumExtensions
{
    public static CharacterConfig CharacterConfig(this CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (characterEnum == CharacterEnum.None) return null;

        if (!Details.CommonDesignSO.CharacterConfigHubPy.CharacterEnum_ConfigPy.TryGetValue(characterEnum,out CharacterConfig characterConfig))
        {
            Debug.LogError($"未找到: {characterEnum} 的 {typeof(CharacterConfig)}.");
            return null;
        }
        return characterConfig;
    }

    public static PcConfig PcConfig(this CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (characterEnum == CharacterEnum.None) return null;

        if (!Details.CommonDesignSO.CharacterConfigHubPy.PCEnum_ConfigPy.TryGetValue(characterEnum,out PcConfig characterConfig))
        {
            Debug.LogError($"未找到: {characterEnum} 的 {typeof(PcConfig)}.");
            return null;
        }
        return characterConfig;
    }

    public static AIConfig AIConfig(this CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (characterEnum == CharacterEnum.None) return null;

        CharacterConfigHub characterConfigHub = Details.CommonDesignSO.CharacterConfigHubPy;
        if (!characterConfigHub.CharacterEnum_AIConfigPy.TryGetValue(characterEnum,out AIConfig aiConfig))
        {
            aiConfig = characterConfigHub.CommonAIConfigPy;
        }

        return aiConfig;
    }

    public static Pc Pc(this CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (characterEnum == CharacterEnum.None) return null;

        if (!Details.CharacterHub.PCEnum_CharacterIdPy.TryGetValue(characterEnum,out CharacterId characterId))
        {
            Debug.LogError($"未记录 Pc: {characterEnum} 对应的 CharacterId.");
            return null;
        }

        return characterId.PcPy;
    }

    public static ManualCard ManualCard(this CharacterEnum characterEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (characterEnum == CharacterEnum.None) return null;

        if (!Details.HeronTeam.ManualPy.MonsterManualPy.CharacterEnum_ManualCardPy.TryGetValue(characterEnum,out ManualCard manualCard))
        {
            Debug.LogError($"未记录 ManualCard: {characterEnum} .");
        }

        return manualCard;
    }
}
}