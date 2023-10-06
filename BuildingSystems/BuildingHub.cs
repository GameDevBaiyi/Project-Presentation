using System;
using System.Collections.Generic;

using Common.Template;

using JetBrains.Annotations;

using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
[Serializable]
public class BuildingHub : InstanceHub<Building>
{
    /// <summary>
    /// 一个建筑阶段, 由多个建筑组成. 
    /// </summary>
    [Serializable]
    public class BuildingStage
    {
        [ShowInInspector]
        private readonly List<int> _buildingIds;
        public List<int> BuildingIdsPy => this._buildingIds;

        public BuildingStage(List<int> buildingIds)
        {
            this._buildingIds = buildingIds;
        }
    }

    /// <summary>
    /// 一个建筑状态, 由多个建筑阶段组成.
    /// </summary>
    [Serializable]
    public class BuildingState
    {
        [ShowInInspector]
        [CanBeNull]
        private readonly List<BuildingStage> _buildingStages;
        [CanBeNull]
        public List<BuildingStage> BuildingStagesPy => this._buildingStages;

        public BuildingState(List<BuildingStage> buildingStages)
        {
            this._buildingStages = buildingStages;
        }
    }

    [Title("Data")]
    [ShowInInspector]
    private readonly CityEnum _cityEnum;

    [ShowInInspector]
    private readonly List<int> _commonBuildingIds;
    [ShowInInspector]
    private readonly List<BuildingState> _buildingStates;

    [ShowInInspector]
    private readonly List<int> _currentStagePointers;

    [ShowInInspector]
    private readonly Dictionary<Vector3Int,int> _buildingCoord_buildingId = new Dictionary<Vector3Int,int>(50);

    public BuildingHub(CityEnum cityEnum,List<int> commonBuildingIds,List<BuildingState> buildingStates,
                       List<int> currentStagePointers)
    {
        this._instanceId_instance = new Dictionary<int,Building>(50);

        this._cityEnum = cityEnum;
        this._commonBuildingIds = commonBuildingIds;
        this._buildingStates = buildingStates;
        this._currentStagePointers = currentStagePointers;
    }

    [Title("Methods")]
    [NonSerialized]
    private static readonly List<Building> _allCurrentBuildingsCache = new List<Building>(50);
    /// <summary>
    /// 有 Cache, 获得数据后需立即使用. 
    /// </summary>
    public List<Building> AllCurrentBuildingsPy
    {
        get
        {
            // Use the _allCurrentBuildingsCache to store the result.
            _allCurrentBuildingsCache.Clear();
            foreach (int commonBuildingId in this._commonBuildingIds)
            {
                this.TryGetInstance(commonBuildingId,out Building building);
                if (building == null) continue;
                _allCurrentBuildingsCache.Add(building);
            }
            for (int i = 0; i < this._currentStagePointers.Count; i++)
            {
                int currentStagePointer = this._currentStagePointers[i];
                List<BuildingStage> buildingStages = this._buildingStates[i].BuildingStagesPy;
                if (buildingStages == null) continue;
                foreach (int buildingId in buildingStages[currentStagePointer].BuildingIdsPy)
                {
                    this.TryGetInstance(buildingId,out Building building);
                    if (building == null) continue;
                    _allCurrentBuildingsCache.Add(building);
                }
            }
            return _allCurrentBuildingsCache;
        }
    }

    [NonSerialized]
    private static readonly List<Building> _phasedBuildingsCache = new List<Building>(50);
    /// <summary>
    /// 有 Cache, 获得数据后需立即使用. 
    /// </summary>
    public List<Building> GetPhasedBuildingsByStateId(int stateId)
    {
        // Use the _phasedBuildingsCache to store the result.
        _phasedBuildingsCache.Clear();
        int currentStagePointer = this._currentStagePointers[stateId];

        if (stateId < 0
         || stateId >= this._buildingStates.Count)
        {
            Debug.LogError($"未找到 {stateId} 状态的 {nameof(Building)}");
            return _phasedBuildingsCache;
        }

        BuildingState buildingState = this._buildingStates[stateId];
        if (buildingState.BuildingStagesPy == null) return _phasedBuildingsCache;

        if (currentStagePointer < 0
         || currentStagePointer >= buildingState.BuildingStagesPy.Count)
        {
            Debug.LogError($"未找到 {stateId} 和 {currentStagePointer} 状态的 {nameof(Building)}");
            return _phasedBuildingsCache;
        }
        foreach (int buildingId in buildingState.BuildingStagesPy[currentStagePointer].BuildingIdsPy)
        {
            this.TryGetInstance(buildingId,out Building building);
            if (building == null) continue;
            _phasedBuildingsCache.Add(building);
        }

        return _phasedBuildingsCache;
    }

    public Building GetBuildingByBuildingCoord(Vector3Int buildingCoord)
    {
        if (!this._buildingCoord_buildingId.TryGetValue(buildingCoord,out int buildingId))
        {
            Debug.LogError($"未找到 {buildingCoord} 位置的 {nameof(Building)}");
            return null;
        }

        this.TryGetInstance(buildingId,out Building building);
        return building;
    }

    [NonSerialized]
    private static readonly List<Building> _allPhasedBuildingsCache = new List<Building>(50);
    /// <summary>
    /// 有 Cache, 获得数据后需立即使用. 
    /// </summary>
    public List<Building> AllPhasedBuildingsPy
    {
        get
        {
            // Use the _allPhasedBuildingsCache to store the result.
            _allPhasedBuildingsCache.Clear();
            for (int stateId = 0; stateId < this._currentStagePointers.Count; stateId++)
            {
                _allPhasedBuildingsCache.AddRange(this.GetPhasedBuildingsByStateId(stateId));
            }
            return _allPhasedBuildingsCache;
        }
    }

    public void UpdateBuildingCoords()
    {
        this._buildingCoord_buildingId.Clear();
        foreach (Building building in this.AllCurrentBuildingsPy)
        {
            Vector3Int coord = building.BuildingInstanceConfigPy.CoordPy;
            if (this._buildingCoord_buildingId.ContainsKey(coord))
            {
                Debug.LogError($"更新建筑坐标时, 发现同一建筑阶段有 重复坐标 {coord}. 当前 City 为 {this._cityEnum}");
            }
            this._buildingCoord_buildingId.Add(coord,building.InstanceIdPy);
        }
    }

    public void SetBuildingStage(int stateId,int stageId)
    {
        if (stateId < 0
         || stateId >= this._currentStagePointers.Count)
        {
            Debug.LogError($"未找到该 City: {this._cityEnum} {stateId} 状态 pointer");
            return;
        }

        this._currentStagePointers[stateId] = stageId;
    }
}
}