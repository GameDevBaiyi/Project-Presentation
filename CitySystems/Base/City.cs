using System;
using System.Collections.Generic;

using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Components;
using LowLevelSystems.SceneSystems.CitySystems.Components.DungeonSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
[Serializable]
public class City : Scene,IHasScenePrefab,IHasEntranceToScene,IHasSceneName
{
    // 有预设
    [ShowInInspector]
    private readonly ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;

    [ShowInInspector]
    private readonly CityEnum _cityEnum;
    public CityEnum CityEnumPy => this._cityEnum;

    [ShowInInspector]
    [NonSerialized]
    private CityConfig _cityConfig;
    public CityConfig CityConfigPy
    {
        get
        {
            if (this._cityConfig != null) return this._cityConfig;
            this._cityConfig = this._cityEnum.CityConfig();
            return this._cityConfig;
        }
    }

    [ShowInInspector]
    private readonly CityJurisdictionSystem _cityJurisdictionSystem;
    public CityJurisdictionSystem CityJurisdictionSystemPy => this._cityJurisdictionSystem;

    [ShowInInspector]
    private readonly BuildingHub _buildingHub;
    public BuildingHub BuildingHubPy => this._buildingHub;

    [ShowInInspector]
    private readonly CityExploreSystem _cityExploreSystem;
    public CityExploreSystem CityExploreSystemPy => this._cityExploreSystem;

    [ShowInInspector]
    private readonly CityLock _cityLock;
    public CityLock CityLockPy => this._cityLock;

    [ShowInInspector]
    private readonly BulletinBoard _bulletinBoard;
    public BulletinBoard BulletinBoardPy => this._bulletinBoard;

    [ShowInInspector]
    private readonly DungeonHub _dungeonHub;
    public DungeonHub DungeonHubPy => this._dungeonHub;

    // 地形上有去往其他 Scene 的点.
    [ShowInInspector]
    private readonly Dictionary<Vector3Int,SceneId> _entranceCoord_sceneId = new Dictionary<Vector3Int,SceneId>(50);
    public Dictionary<Vector3Int,SceneId> EntranceCoord_SceneIdPy => this._entranceCoord_sceneId;

    // 禁止出城的实现: 
    // 1. 禁止 Pc 移动到目的地的 交互.
    // 2. 禁止世界地图的 Ui.
    // 3. 禁止世界地图调用的方法并 Debug. 
    [ShowInInspector]
    private readonly List<int> _noLeaveMissionIds = new List<int>();
    public List<int> NoLeaveMissionIdsPy => this._noLeaveMissionIds;
    public bool CanLeavePy => this._noLeaveMissionIds.Count == 0;

    [ShowInInspector]
    private readonly List<(Vector2 Position,int IdInList)> _buildingUiData = new List<(Vector2 Position,int IdInList)>();
    public List<(Vector2 Position,int IdInList)> BuildingUiDataPy => this._buildingUiData;

    public City(int instanceId,ScenePrefabEnum scenePrefabEnum,CityEnum cityEnum,
                BuildingHub buildingHub,CityExploreSystem cityExploreSystem,CityJurisdictionSystem cityJurisdictionSystem,
                CityLock cityLock) : base(instanceId,SceneTypeEnum.City)
    {
        this._scenePrefabEnum = scenePrefabEnum;
        this._cityEnum = cityEnum;
        this._cityJurisdictionSystem = cityJurisdictionSystem;
        this._buildingHub = buildingHub;
        this._cityExploreSystem = cityExploreSystem;
        this._cityLock = cityLock;
        this._bulletinBoard = new BulletinBoard(cityEnum);
        this._dungeonHub = new DungeonHub(cityEnum);
    }

    [Title("Methods")]
    public Building HotelPy => this._buildingHub.GetBuildingByBuildingCoord(this.CityConfigPy.HotelCoordPy);
    public string SceneNamePy => this.CityConfigPy.CityNamePy;
}
}