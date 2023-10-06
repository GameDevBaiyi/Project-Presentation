using System;

using Common.Template;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.MissionSystems;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.CitySystems.Components.DungeonSystems
{
[Serializable]
public class Dungeon : IInstance
{
    [ShowInInspector]
    private readonly int _instanceId;
    public int InstanceIdPy => this._instanceId;

    [ShowInInspector]
    private readonly BattleConfigId _battleConfigId;
    public BattleConfigId BattleConfigIdPy => this._battleConfigId;

    [ShowInInspector]
    private readonly MissionId _correspondingMissionId;
    public MissionId CorrespondingMissionIdPy => this._correspondingMissionId;

    [ShowInInspector]
    private readonly int _npcLvAddend;
    public int NpcLvAddendPy => this._npcLvAddend;

    [ShowInInspector]
    private int _indexOnUi = -1;
    public int IndexOnUiPy => this._indexOnUi;
    public void SetIndexOnUi(int indexOnUi)
    {
        this._indexOnUi = indexOnUi;
    }

    public Dungeon(int instanceId,BattleConfigId battleConfigId,MissionId correspondingMissionId,
                   int npcLvAddend)
    {
        this._instanceId = instanceId;
        this._battleConfigId = battleConfigId;
        this._correspondingMissionId = correspondingMissionId;
        this._npcLvAddend = npcLvAddend;
    }
}
}