using System;
using System.Collections.Generic;

using Common.Template;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.HeronTeamSystems.Base;
using LowLevelSystems.MissionSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.CitySystems.Components.DungeonSystems
{
[Serializable]
public class DungeonHub : InstanceHub<Dungeon>
{
    [ShowInInspector]
    private readonly CityEnum _cityEnum;

    public DungeonHub(CityEnum cityEnum)
    {
        this._instanceId_instance = new Dictionary<int,Dungeon>(20);
        this._cityEnum = cityEnum;
    }

    [Title("Methods")]
    [ShowInInspector]
    public IEnumerable<Dungeon> AllDungeonsPy
    {
        get
        {
            HeronTeam.InstancePy.MissionHubPy.CloseExpiredTasks();
            return this._instanceId_instance.Values;
        }
    }

    public Dungeon CreateDungeon(BattleConfigId battleConfigId,MissionId correspondingMissionId,int npcLvAddend)
    {
        Dungeon dungeon = new Dungeon(this.GetNextInstanceId(),battleConfigId,correspondingMissionId,npcLvAddend);
        this.RecordInstance(dungeon);

        return dungeon;
    }
}
}