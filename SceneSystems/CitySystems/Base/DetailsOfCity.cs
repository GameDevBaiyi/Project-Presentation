using System.Collections.Generic;
using System.Linq;

using CityHudPackage;

using Common.Extensions;

using Cysharp.Threading.Tasks;

using HighLevelManagers.InputsAndEntityManagers;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems.Conditions;
using LowLevelSystems.MissionSystems.PlotGuideSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
public abstract class DetailsOfCity : Details
{
    public static async UniTask MoveToAnotherCity(City targetCity)
    {
        //Debug. 
        if (!SceneHub.CurrentCityEnumPy.City().CanLeavePy)
        {
            Debug.LogError($"不能离开 该城镇, 不应有 Ui 能交互到. ");
            return;
        }
        //就在当前 City, 什么也不发生.
        if (targetCity.CityEnumPy == SceneHub.CurrentCityEnumPy) return;

        void ProcessData()
        {
            //处理 CityEnum
            SceneHub.SetCurrentCityEnum(targetCity.CityEnumPy);

            //所有 队伍中的 Pc 移动到该 City.
            List<Pc> pcsInTeam = HeronTeam.PcsInTeamPy.ToList();
            foreach (Pc pc in pcsInTeam)
            {
                SceneDetails.MoveCharacterTo(pc,targetCity);
            }
            //给这些 Pc 一些随机的不重复的坐标.
            List<Vector3Int> randomCoords = targetCity.ScenePrefabEnumPy.ScenePrefabConfig()
                                                      .EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.SpawnPoint]
                                                      .TakeRandomItems(pcsInTeam.Count)
                                                      .ToList();
            for (int i = 0; i < pcsInTeam.Count; i++)
            {
                pcsInTeam[i].CoordSystemPy.SetCurrentCoord(randomCoords[i]);
            }

            SceneHub.SetCurrentSceneId(targetCity.InstanceIdPy);
        }

        await EntityManagerDetails.RefreshAllEntitiesAsync(ProcessData);
        DetailsOfMissionConditions.CheckEnterCityFromWorldMap();
        if (!PlotGuideManager.IsGuidingPlotPy)
        {
            await CityHud.OpenCityHudWithAnimeAsync();
        }
    }

    public static void ConnectScenesInCity(City city)
    {
        city.EntranceCoord_SceneIdPy.Clear();
        foreach (Building building in city.BuildingHubPy.AllCurrentBuildingsPy)
        {
            // 先记录 City 到 Building 第一层.
            //尝试拿到 Building 的第一层.
            if (building.FloorCountPy == 0) continue;
            SceneId firstRoomId = building.RoomSceneIdsPy[0];
            foreach (Vector3Int interaction in building.BuildingInstanceConfigPy.InteractionsPy)
            {
                city.EntranceCoord_SceneIdPy[interaction] = firstRoomId;
            }

            // 再记录 Room 的
            foreach (SceneId roomSceneId in building.RoomSceneIdsPy)
            {
                Room room = roomSceneId.RoomPy;
                room.EntranceCoord_SceneIdPy.Clear();
                ScenePrefabConfig roomConfig = room.ScenePrefabEnumPy.ScenePrefabConfig();
                // 如果是第一个房间, 那么它和城镇是相连的.
                int floorIndex = room.FloorIndexPy;
                if (floorIndex == 0)
                {
                    //添加房间刷新点为去往城镇的出口.
                    foreach (Vector3Int respawnCoord in roomConfig.EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.SpawnPoint])
                    {
                        room.EntranceCoord_SceneIdPy[respawnCoord] = city.SceneIdPy;
                    }
                }
                //如果不是第一个房间, 那么连接的是到上一个房间. 
                else
                {
                    //Cache 上一个房间.
                    Room previousRoom = building.RoomSceneIdsPy[floorIndex - 1].RoomPy;
                    //添加房间刷新点为去往上一个房间的出口.
                    foreach (Vector3Int respawnCoord in roomConfig.EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.SpawnPoint])
                    {
                        room.EntranceCoord_SceneIdPy[respawnCoord] = previousRoom.SceneIdPy;
                    }
                }

                //再连接楼梯位置.
                //如果不是最后一个房间.
                bool isFinalRoom = room.IsFinalRoomPy;
                if (!isFinalRoom)
                {
                    //Cache 下一个房间.
                    Room nextRoom = building.RoomSceneIdsPy[floorIndex + 1].RoomPy;
                    //Cache 下一个房间的 SceneId
                    //添加房间响应格为去往下一个房间的入口.
                    foreach (Vector3Int upstairsCoord in roomConfig.EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.Upstairs])
                    {
                        room.EntranceCoord_SceneIdPy[upstairsCoord] = nextRoom.SceneIdPy;
                    }
                }
            }
        }
    }

    private static readonly List<Pc> _pcsInPreviousBuildingsCache = new List<Pc>(10);
    public static async UniTask SwitchBuildingStageAsync(City city,int stateId,int stageId)
    {
        // 找到当前阶段的所有建筑里的 Pc. 将他们移动到 旅店. 
        _pcsInPreviousBuildingsCache.Clear();
        List<Building> phasedBuildingsByStateId = city.BuildingHubPy.GetPhasedBuildingsByStateId(stateId);
        foreach (Building phasedBuilding in phasedBuildingsByStateId)
        {
            foreach (Character character in phasedBuilding.RoomSceneIdsPy.SelectMany(roomSceneId => roomSceneId.RoomPy.CharactersPy))
            {
                if (character is not Pc pc) continue;
                _pcsInPreviousBuildingsCache.Add(pc);
            }
        }
        Building hotel = city.HotelPy;
        foreach (Pc pc in _pcsInPreviousBuildingsCache)
        {
            Room randomRoom = hotel.RoomSceneIdsPy.GetRandomItem().RoomPy;
            Vector3Int coord = randomRoom.ScenePrefabConfigPy.AllWalkableCoordsPy.GetRandomItem();
            if (pc.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity))
            {
                await pcEntity.HideAsync();
            }
            pc.CoordSystemPy.SetCurrentCoord(coord);
            SceneDetails.MoveCharacterTo(pc,randomRoom);
        }

        // 在 BuildingHub 里设置阶段.
        city.BuildingHubPy.SetBuildingStage(stateId,stageId);

        // 连接场景.
        ConnectScenesInCity(city);

        // 更新建筑坐标.
        city.BuildingHubPy.UpdateBuildingCoords();

        if (city.CityEnumPy == SceneHub.CurrentCityEnumPy)
        {
            await SceneDetails.ChangeCurrentSceneAndRefreshEntitiesAndOpenCityHudAsync(HeronTeam.CurrentPcInControlPy.SceneIdPy.Id,true);
        }
    }
}
}