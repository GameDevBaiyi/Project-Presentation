using System;
using System.Collections.Generic;
using System.Linq;

using Common.Template;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.CharacterSystems.PcSystems;

using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.SceneSystems.Base
{
public enum SceneTypeEnum
{
    None,
    BlankSpace,
    Room,
    City,
    Dungeon,
}

[Serializable]
public abstract class Scene : IInstance
{
    [Title("Data")]
    [ShowInInspector]
    protected readonly int _instanceId;
    public int InstanceIdPy => this._instanceId;

    [ShowInInspector]
    protected readonly SceneTypeEnum _sceneTypeEnum;
    public SceneTypeEnum SceneTypeEnumPy => this._sceneTypeEnum;

    [ShowInInspector]
    protected readonly HashSet<CharacterId> _characterIdSet = new HashSet<CharacterId>(10);

    protected Scene(int instanceId,SceneTypeEnum sceneTypeEnum)
    {
        this._instanceId = instanceId;
        this._sceneTypeEnum = sceneTypeEnum;
    }

    [Title("Methods")]
    public SceneId SceneIdPy => new SceneId(this._instanceId);
    [ShowInInspector]
    public IEnumerable<Character> CharactersPy => this._characterIdSet.Select(t => t.CharacterPy);
    [ShowInInspector]
    public IEnumerable<Pc> PcsPy => this._characterIdSet.Select(t => t.CharacterPy).Where(t => t.CharacterTypeEnumPy == Character.CharacterTypeEnum.Pc).Select(t => (Pc)t);
    [ShowInInspector]
    public IEnumerable<Npc> NpcsPy => this._characterIdSet.Select(t => t.CharacterPy).Where(t => t.CharacterTypeEnumPy == Character.CharacterTypeEnum.Npc).Select(t => (Npc)t);

    // Wrap 添加和移除, 方便追踪 usages.
    public void AddCharacterId(CharacterId characterId)
    {
        this._characterIdSet.Add(characterId);
    }
    public void RemoveCharacterId(CharacterId characterId)
    {
        this._characterIdSet.Remove(characterId);
    }
}
}