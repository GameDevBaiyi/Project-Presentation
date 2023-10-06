using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
public abstract class CharacterIdDetails : Details
{
    public static Character GetCharacter(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        CharacterHub.TryGetInstance(instanceId,out Character character);
        return character;
    }

    public static Pc GetPc(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        CharacterHub.TryGetInstance(instanceId,out Pc character);
        return character;
    }

    public static Npc GetNpc(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        CharacterHub.TryGetInstance(instanceId,out Npc character);
        return character;
    }

    public static CharacterEntity GetCharacterEntity(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        _characterEntityHub.TryGetInstance(instanceId,out CharacterEntity characterEntity);
        return characterEntity;
    }

    public static PcEntity GetPcEntity(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        _characterEntityHub.TryGetInstance(instanceId,out PcEntity characterEntity);
        return characterEntity;
    }

    public static NpcEntity GetNpcEntity(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        _characterEntityHub.TryGetInstance(instanceId,out NpcEntity characterEntity);
        return characterEntity;
    }

    public static SpecialNpcConfig GetSpecialNpcConfig(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;

        if (!CommonDesignSO.CharacterConfigHubPy.CharacterId_SpecialNpcConfigPy.TryGetValue(instanceId,out SpecialNpcConfig specialNpcConfig))
        {
            Debug.LogError($"未找到 SpecialNpcConfig: {instanceId}");
        }

        return specialNpcConfig;
    }
    public static SpecialNpcConfig GetSpecialNpcConfigWithoutError(CharacterId characterId)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif

        int instanceId = characterId.InstanceId;
        if (instanceId == 0) return null;
        CommonDesignSO.CharacterConfigHubPy.CharacterId_SpecialNpcConfigPy.TryGetValue(instanceId,out SpecialNpcConfig specialNpcConfig);
        return specialNpcConfig;
    }

    public static bool TryGetCharacterEntity(CharacterId characterId,out CharacterEntity characterEntity)
    {
        return _characterEntityHub.InstanceId_InstancePy.TryGetValue(characterId.InstanceId,out characterEntity);
    }

    public static bool TryGetNpcEntity(CharacterId characterId,out NpcEntity npcEntity)
    {
        npcEntity = null;
        if (!_characterEntityHub.InstanceId_InstancePy.TryGetValue(characterId.InstanceId,out CharacterEntity characterEntity)) return false;

        if (characterEntity is not NpcEntity npcEntityLc)
        {
            Debug.LogError($"找到了 {nameof(CharacterId)} : {characterId.InstanceId} 对应的 {nameof(CharacterEntity)}, 但其不是 {nameof(NpcEntity)}.");
            return false;
        }

        npcEntity = npcEntityLc;
        return true;
    }

    public static bool TryGetPcEntity(CharacterId characterId,out PcEntity pcEntity)
    {
        pcEntity = null;
        if (!_characterEntityHub.InstanceId_InstancePy.TryGetValue(characterId.InstanceId,out CharacterEntity characterEntity)) return false;

        if (characterEntity is not PcEntity pcEntityLc)
        {
            Debug.LogError($"找到了 {nameof(CharacterId)} : {characterId.InstanceId} 对应的 {nameof(CharacterEntity)}, 但其不是 {nameof(PcEntity)}.");
            return false;
        }

        pcEntity = pcEntityLc;
        return true;
    }
}
}