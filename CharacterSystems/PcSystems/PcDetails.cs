using System.Collections.Generic;

using CityHudPackage;

using Common.Extensions;

using Cysharp.Threading.Tasks;

using HighLevelManagers.InputsAndEntityManagers;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems;
using LowLevelSystems.MissionSystems.Conditions;
using LowLevelSystems.MissionSystems.PlotGuideSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems
{
public abstract class PcDetails : Details
{
    public static void GeneratePcsForNewGame(NewGameConfig newGameGuideConfig)
    {
        //先将所有 Pc 生成到休息空间中.
        foreach (PcConfig pcConfig in CommonDesignSO.CharacterConfigHubPy.PCEnum_ConfigPy.Values)
        {
            PcFactory.GeneratePc(pcConfig);
        }

        //将初始角色入队, 并调整 Scene 和 Coord
        foreach (NewGameConfig.CharacterAndCoord characterConfigRow in newGameGuideConfig.CharacterAndCoordsPy)
        {
            Pc pc = characterConfigRow.CharacterEnumPy.Pc();

            pc.SetHasJoined(true);
            pc.CoordSystemPy.SetCurrentCoord(characterConfigRow.CharacterCoordPy);
            SceneDetails.MoveCharacterTo(pc,SceneHub.CurrentSceneIdPy.ScenePy);
        }
    }

    /// <summary>
    /// 由移动触发的 去到某场景. 
    /// </summary>
    public static async UniTask MoveToAsync(TerrainStaticFlags terrainStaticFlags,Pc pc,
                                            Scene targetScene)
    {
        SceneId lastSceneId = pc.SceneIdPy;

        void ProcessData()
        {
            Dictionary<int,Vector3Int> sceneId_entranceCoord = pc.SceneId_EntranceCoordPy;
            //功能: 记录进来时的入口.
            SceneId sceneId = pc.SceneIdPy;
            sceneId_entranceCoord[sceneId.Id] = pc.CoordSystemPy.CurrentCoordPy;

            //构造一个 Method 计算出去的 Coord.
            Vector3Int CalculateExitCoord()
            {
                // 先计算出 该场景的所有出口.
                List<Vector3Int> exits;
                if (targetScene is City)
                {
                    //Debug. 理论上只有此情况: 目标是 City, 那么自己当前是在 Room 中.
                    if (SceneHub.CurrentSceneIdPy.ScenePy is not Room room)
                    {
                        Debug.LogError($"当前的 Scene 不是 Room, 但出口导向是 City.");
                        return Vector3Int.zero;
                    }

                    exits = room.BuildingIdPy.BuildingPy.BuildingInstanceConfigPy.InteractionsPy;
                }
                else
                {
                    //Debug. 
                    if (targetScene is not Room room)
                    {
                        Debug.LogError($"走向的 Scene 即不是 City 也不是 Room.");
                        return Vector3Int.zero;
                    }

                    //目标是 Room, 那么可能是需要 出口点 也可能是 楼梯点.
                    exits = terrainStaticFlags.HasFlag(TerrainStaticFlags.IsSpawnPoint)
                                ? room.ScenePrefabEnumPy.ScenePrefabConfig().EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.Upstairs]
                                : room.ScenePrefabEnumPy.ScenePrefabConfig().EditorTileEnum_CoordsPy[ScenePrefabConfig.EditorTileEnum.SpawnPoint];
                }

                //如果之前记录了, 那么直接使用即可.
                if (sceneId_entranceCoord.TryGetValue(targetScene.InstanceIdPy,out Vector3Int value))
                {
                    return exits.Contains(value) ? value : exits.GetRandomItem();
                }
                //功能: 如果该场景第一次进来, 那么要分成该场景是 City 还是 Room.
                else
                {
                    return exits.GetRandomItem();
                }
            }

            Vector3Int exitCoord = CalculateExitCoord();
            pc.CoordSystemPy.SetCurrentCoord(exitCoord);
            SceneDetails.MoveCharacterTo(pc,targetScene);
            SceneHub.SetCurrentSceneId(targetScene.InstanceIdPy);
        }

        await EntityManagerDetails.RefreshAllEntitiesAsync(ProcessData);

        //角色的场景移动会触发一些节点. 
        DetailsOfMissionConditions.CheckHasPcExitedRoom(lastSceneId,pc);
        DetailsOfMissionConditions.CheckHasPcEnteredRoom(pc);

        if (PlotGuideManager.IsGuidingPlotPy) return;
        if (SceneHub.CurrentSceneIdPy.ScenePy.SceneTypeEnumPy == SceneTypeEnum.City)
        {
            await CityHud.OpenCityHudWithoutAnimeAsync();
        }
        else
        {
            await CityHud.OpenCityHudWithAnimeAsync();
        }
    }
}
}