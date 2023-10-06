using System;

using JetBrains.Annotations;

using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
public struct CharacterId
{
    public int InstanceId;

    public CharacterId(int instanceId)
    {
        this.InstanceId = instanceId;
    }

    [ShowInInspector]
    public Character CharacterPy => CharacterIdDetails.GetCharacter(this);
    public Pc PcPy => CharacterIdDetails.GetPc(this);
    public Npc NpcPy => CharacterIdDetails.GetNpc(this);

    public CharacterEntity CharacterEntityPy => CharacterIdDetails.GetCharacterEntity(this);
    public PcEntity PcEntityPy => CharacterIdDetails.GetPcEntity(this);
    public NpcEntity NpcEntityPy => CharacterIdDetails.GetNpcEntity(this);

    public SpecialNpcConfig SpecialNpcConfigPy => CharacterIdDetails.GetSpecialNpcConfig(this);
    [CanBeNull]
    public SpecialNpcConfig SpecialNpcConfigWithoutErrorPy => CharacterIdDetails.GetSpecialNpcConfigWithoutError(this);

    public bool TryGetCharacterEntity(out CharacterEntity characterEntity)
    {
        return CharacterIdDetails.TryGetCharacterEntity(this,out characterEntity);
    }
    public bool TryGetNpcEntity(out NpcEntity npcEntity)
    {
        return CharacterIdDetails.TryGetNpcEntity(this,out npcEntity);
    }
    public bool TryGetPcEntity(out PcEntity pcEntity)
    {
        return CharacterIdDetails.TryGetPcEntity(this,out pcEntity);
    }
}
}