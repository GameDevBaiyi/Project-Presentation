using System;
using System.Collections.Generic;

using Common.Template;

using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
[Serializable]
public class Building : IInstance
{
    [Title("Data")]
    [ShowInInspector]
    private readonly int _instanceId;
    public int InstanceIdPy => this._instanceId;

    [ShowInInspector]
    private readonly CityEnum _cityEnum;
    public CityEnum CityEnumPy => this._cityEnum;

    [ShowInInspector]
    private readonly (bool IsCommon,int IdInCommonList,int StateId,int StageId,int IdInStage) _buildingInstanceConfigKey;
    public (bool IsCommon,int IdInCommonList,int StateId,int StageId,int IdInStage) BuildingInstanceConfigKeyPy => this._buildingInstanceConfigKey;

    [ShowInInspector]
    [NonSerialized]
    private BuildingInstanceConfig _buildingInstanceConfig;
    public BuildingInstanceConfig BuildingInstanceConfigPy
    {
        get
        {
            if (this._buildingInstanceConfig != null) return this._buildingInstanceConfig;
            this._buildingInstanceConfig = this._cityEnum.CityConfig().GetBuildingInstanceConfigBy(this._buildingInstanceConfigKey);
            return this._buildingInstanceConfig;
        }
    }

    // 设计: 一层一个房间, 没有地下室. 从 0 开始.
    [ShowInInspector]
    private readonly List<SceneId> _roomSceneIds;
    public List<SceneId> RoomSceneIdsPy => this._roomSceneIds;
    public Building(int instanceId,CityEnum cityEnum,(bool IsCommon,int IdInCommonList,int StateId,int StageId,int IdInStage) buildingInstanceConfigKey,
                    List<SceneId> roomSceneIds)
    {
        this._instanceId = instanceId;
        this._cityEnum = cityEnum;
        this._buildingInstanceConfigKey = buildingInstanceConfigKey;
        this._roomSceneIds = roomSceneIds;
    }

    [Title("Methods")]
    [ShowInInspector]
    public int FloorCountPy => this._roomSceneIds.Count;
    [ShowInInspector]
    public string BuildingNamePy => this.BuildingInstanceConfigPy.NameIdPy.Id == 0
                                        ? this.BuildingInstanceConfigPy.BuildingEnumPy.GetBuildingName()
                                        : this.BuildingInstanceConfigPy.NameIdPy.TextPy;
}
}