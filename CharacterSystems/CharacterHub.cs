using System;
using System.Collections.Generic;
using System.Linq;

using Common.Template;

using LowLevelSystems.CharacterSystems.PcSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems
{
[Serializable]
public class CharacterHub : SingletonHub<CharacterHub,Character>
{
    //预设
    private const int _characterCapacity = 200;

    [Title("Data")]
    [ShowInInspector]
    private Dictionary<CharacterEnum,CharacterId> _pcEnum_characterId;
    public Dictionary<CharacterEnum,CharacterId> PCEnum_CharacterIdPy => this._pcEnum_characterId;

    [Title("Methods")]
    [ShowInInspector]
    public IEnumerable<Pc> AllPcsPy => this._pcEnum_characterId.Select(t => t.Value.PcPy);

    public override void RecordInstance(Character instance)
    {
        base.RecordInstance(instance);
        if (instance is Pc pc)
        {
            if (this._pcEnum_characterId.ContainsKey(pc.CharacterEnumPy))
            {
                Debug.LogError($"记录 {pc.CharacterEnumPy} 时发现已经存在.");
                return;
            }

            this._pcEnum_characterId[pc.CharacterEnumPy] = new CharacterId(pc.InstanceIdPy);
        }
    }

    public void Initialize()
    {
        this._instanceId_instance = new Dictionary<int,Character>(_characterCapacity);
        this._pcEnum_characterId = new Dictionary<CharacterEnum,CharacterId>(20);
    }
}
}