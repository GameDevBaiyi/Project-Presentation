#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Common.DataTypes;
using Common.Extensions;
using Common.Utilities;

using DevTools;
using DevTools.DevConfigData;
using DevTools.MapProcessors;
using DevTools.ProgrammerTools;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.MissionSystems.EventTriggerSystems;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace LowLevelSystems.SceneSystems.Base
{
public static class ScenePrefabConfigBaker
{
    public static void GenerateScenePrefabAndEntityTilemapEnum()
    {
        GenerateScenePrefabEnum();
        GenerateEntityTilemapEnum();
    }

    private static void GenerateScenePrefabEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/SceneSystems/Base/ScenePrefabEnum.cs");
        List<string> scenePrefabEnumStrings = DevUtilities
                                             .GetPrefabsByPath(DevUtilities.CityRuntimePrefabsPath,DevUtilities.DungeonRuntimePrefabsPath,DevUtilities.RoomRuntimePrefabsPath)
                                             .Select(t => t.name)
                                             .ToList();
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(ScenePrefabEnum).Namespace}",$"{nameof(ScenePrefabEnum)}",scenePrefabEnumStrings,
                                               Enum.GetNames(typeof(ScenePrefabEnum)));
        AssetDatabase.ImportAsset(@"Assets/Scripts/Scripts/LowLevelSystems/SceneSystems/Base/ScenePrefabEnum.cs");
    }

    private static void GenerateEntityTilemapEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/SceneSystems/Base/EntityTilemapEnum.cs");
        GameObject firstCityPrefab = DevUtilities.GetPrefabsByPath(DevUtilities.CityRuntimePrefabsPath).First();
        IEnumerable<string> strings = MapRecordUtilities.GetTilemapsBySortingLayer(firstCityPrefab,MapDataRecordWindow.EntitiesLayerName).Select(t => t.gameObject.name);
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(EntityTilemapEnum).Namespace}",$"{nameof(EntityTilemapEnum)}",strings.ToList(),
                                               Enum.GetNames(typeof(EntityTilemapEnum)));
        AssetDatabase.ImportAsset(@"Assets/Scripts/Scripts/LowLevelSystems/SceneSystems/Base/ScenePrefabEnum.cs");
    }

    public static void RecordScenePrefabConfigAndAddress()
    {
        List<ScenePrefabConfig> scenePrefabConfigs = new List<ScenePrefabConfig>();
        List<GameObject> allSceneEditorPrefabs = DevUtilities.GetPrefabsByPath(DevUtilities.CityEditorPrefabsPath,DevUtilities.DungeonEditorPrefabsPath,
                                                                               DevUtilities.RoomEditorPrefabsPath);
        foreach (GameObject editorPrefab in allSceneEditorPrefabs)
        {
            ScenePrefabConfig scenePrefabConfig = new ScenePrefabConfig();

            //ScenePrefabEnum _scenePrefabEnum
            string sceneRuntimeName = editorPrefab.name.Replace("_Editor","");
            ScenePrefabEnum scenePrefabEnum = sceneRuntimeName.ToEnum<ScenePrefabEnum>();
            scenePrefabConfig.SetScenePrefabEnum(scenePrefabEnum);

            //string _sceneEntityAddress
            string sceneEntityAddress = sceneRuntimeName;
            scenePrefabConfig.SetSceneEntityAddress(sceneEntityAddress);

            //int _mapWidth nt _mapHeight
            Tilemap[] allTilemaps = MapRecordUtilities.GetAllCompressedTilemaps(editorPrefab);
            MapRecordUtilities.CalculateTilemapMaxSize(allTilemaps,out int mapWidth,out int mapHeight);
            scenePrefabConfig.SetMapWidth(mapWidth);
            scenePrefabConfig.SetMapHeight(mapHeight);

            //TerrainStaticCell[] _terrainStaticCellsList
            TerrainStaticCell[] terrainStaticCellsList = RecordTerrainStaticCellList(editorPrefab);
            scenePrefabConfig.SetTerrainStaticCellsList(terrainStaticCellsList);

            //List<EditorTileEnumAndCoords> _editorTileEnumAndCoordsList
            List<ScenePrefabConfig.EditorTileEnumAndCoords> editorTileEnumAndCoordsList = RecordEditorTileEnumAndCoordsList(editorPrefab);
            scenePrefabConfig.SetEditorTileEnumAndCoordsList(editorTileEnumAndCoordsList);

            //List<BackAndEntitiesCoords> _backAndEntitiesCoordsList
            List<ScenePrefabConfig.BackAndEntitiesCoords> backAndEntitiesCoordsList = RecordBackAndEntitiesCoordsList(editorPrefab);
            scenePrefabConfig.SetBackAndEntitiesCoordsList(backAndEntitiesCoordsList);

            //11. List<Vector3Int> _allWalkableCoords.
            List<Vector3Int> allWalkableCoords = terrainStaticCellsList.Where(t => t.TerrainStaticFlagsPy.HasFlag(TerrainStaticFlags.IsWalkable)).Select(t => t.CoordPy).ToList();
            scenePrefabConfig.SetAllWalkableCoords(allWalkableCoords);

            //List<BattleTypeAndSpecialCoordsList> _battleTypeAndSpecialCoordsLists
            RecordRandomBattleEditor(editorPrefab,scenePrefabConfig);

            scenePrefabConfigs.Add(scenePrefabConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.SceneConfigHubPy.SetScenePrefabConfigs(scenePrefabConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Address();
        EventTriggerAreaConfigBaker.BakeEventTriggerAreaData();

        Debug.Log($"ScenePrefabConfig 录制成功. ");
    }

    private static TerrainStaticCell[] RecordTerrainStaticCellList(GameObject scenePrefab)
    {
        //声明一个 TerrainStaticCell[].
        Tilemap[] allTilemaps = scenePrefab.GetComponentsInChildren<Tilemap>(true);
        MapRecordUtilities.CalculateTilemapMaxSize(allTilemaps,out int mapWidth,out int mapHeight);
        int countOfCells = mapWidth * mapHeight;
        TerrainStaticCell[] terrainStaticCellsList = new TerrainStaticCell[countOfCells];
        for (int i = 0; i < countOfCells; i++)
        {
            terrainStaticCellsList[i] = new TerrainStaticCell();
        }
        //①. Vector3Int _coord. 
        //开始填数据.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                terrainStaticCellsList[x * mapHeight + y].SetCoord(new Vector3Int(x,y,0));
            }
        }

        //②. TerrainStaticFlags.IsWalkable. 
        //初始全部标记为不可行走.
        bool[][] isWalkableGrid = new bool[mapWidth][];
        for (int x = 0; x < mapWidth; x++)
        {
            isWalkableGrid[x] = new bool[mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                isWalkableGrid[x][y] = false;
            }
        }
        //拿到所有 Ground 层. 并按照视觉(即 SortingOrder) 进行降序排列. 第一个看到的 Tile (最上面的 Tile) 确认该点是否可行走.
        Tilemap[] allGroundTilemaps = MapRecordUtilities.GetTilemapsBySortingLayer(allTilemaps,MapDataRecordWindow.GroundLayerName);
        List<Tilemap> sortedGroundTilemaps = allGroundTilemaps.OrderByDescending(tilemap => tilemap.GetComponent<TilemapRenderer>().sortingOrder).ToList();
        Dictionary<string,bool> tileName_walkable = TerrainConfigBaker.GetGroundTileName_Walkable();
        Dictionary<string,List<Vector3Int>> debugDictionary = new Dictionary<string,List<Vector3Int>>();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //Cache 当前坐标.
                Vector3Int currentCoord = new Vector3Int(x,y,0);
                foreach (Tilemap groundTilemap in sortedGroundTilemaps)
                {
                    TileBase currentTile = groundTilemap.GetTile(currentCoord);
                    if (currentTile == null) continue;
                    //有 Tile, 则根据该 Tile 定.
                    if (tileName_walkable.TryGetValue(currentTile.name,out bool walkable))
                    {
                        isWalkableGrid[x][y] = walkable;
                    }
                    else
                    {
                        if (debugDictionary.TryGetValue(currentTile.name,out List<Vector3Int> coords))
                        {
                            coords.Add(currentCoord);
                        }
                        else
                        {
                            debugDictionary[currentTile.name] = new List<Vector3Int>() { currentCoord };
                        }
                    }
                    break;
                }
            }
        }
        foreach (KeyValuePair<string,List<Vector3Int>> pair in debugDictionary)
        {
            Debug.Log($"{scenePrefab} 上还有未配置的 Tile, 不知道其是否可行走.  {pair.Key}, 坐标: {string.Join(" + ",pair.Value)}.");
        }

        //大地板也在 Ground 层, 如果降序检测到了一个大地板, 需要使得 大地板的多个格子 可行走.
        Dictionary<string,DevConfigSO.BigGroundTileInfo> tileName_bigGroundConfigRow = DevConfigSO.BigGroundTileName_Info;
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //Cache 当前坐标.
                Vector3Int currentCoord = new Vector3Int(x,y,0);
                foreach (Tilemap groundTilemap in sortedGroundTilemaps)
                {
                    TileBase currentTile = groundTilemap.GetTile(currentCoord);
                    if (currentTile == null) continue;

                    //额外检测, 如果该 Tile 是大地板, 还要记录其周围格.
                    if (tileName_bigGroundConfigRow.TryGetValue(currentTile.name,out DevConfigSO.BigGroundTileInfo bigGroundDevConfigRow))
                    {
                        List<Vector3Int> offsetGroundCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,bigGroundDevConfigRow.GroundCubes);
                        foreach (Vector3Int offsetGroundCoord in offsetGroundCoords)
                        {
                            isWalkableGrid[offsetGroundCoord.x][offsetGroundCoord.y] = true;
                        }
                    }
                }
            }
        }

        //遍历 Entity 层, 建筑和障碍物的占地格 不可行走.
        Tilemap[] entitiesTilemaps = MapRecordUtilities.GetTilemapsBySortingLayer(allTilemaps,MapDataRecordWindow.EntitiesLayerName);
        if (entitiesTilemaps.Length <= 0)
        {
            Debug.LogError($"检测到 {scenePrefab} 没有 Entities 层, 可能丢失数据.");
        }
        //建筑 Tile Name 对应 建筑配置.
        Dictionary<string,DevConfigSO.BuildingTileInfo> buildingTileName_configRow = DevConfigSO.BuildingTileName_Info;
        //障碍物 Tile Name 对应 障碍物配置.
        Dictionary<string,DevConfigSO.ObstacleTileInfo> obstacleTileName_configRow = DevConfigSO.ObstacleTileName_Info;
        Dictionary<string,List<Vector3Int>> debugDictionary1 = new Dictionary<string,List<Vector3Int>>();
        foreach (Tilemap entitiesTilemap in entitiesTilemaps)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3Int currentCoord = new Vector3Int(x,y,0);
                    TileBase currentTile = entitiesTilemap.GetTile(currentCoord);
                    if (currentTile == null) continue;

                    //Cache TileName 和 SpriteName
                    string tileName = currentTile.name;
                    if (buildingTileName_configRow.TryGetValue(tileName,out DevConfigSO.BuildingTileInfo buildingDevConfigRow))
                    {
                        //找到 建筑占地格.
                        List<Vector3Int> occupyingCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,buildingDevConfigRow.OccupyingCubes);
                        foreach (Vector3Int occupyingCoord in occupyingCoords)
                        {
                            try
                            {
                                isWalkableGrid[occupyingCoord.x][occupyingCoord.y] = false;
                            }
                            catch
                            {
                                //可以不用 Debug. Debug.LogError($"录制 {scenePrefab} 时出错, 出错位置为 {tileName} 的占地格: {occupyingCoord}");
                            }
                        }
                        continue;
                    }
                    else if (obstacleTileName_configRow.TryGetValue(tileName,out DevConfigSO.ObstacleTileInfo obstacleDevConfigRow))
                    {
                        //找到 障碍物 占地格.
                        List<Vector3Int> occupyingCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,obstacleDevConfigRow.OccupyingCubes);
                        foreach (Vector3Int occupyingCoord in occupyingCoords)
                        {
                            try
                            {
                                isWalkableGrid[occupyingCoord.x][occupyingCoord.y] = false;
                            }
                            catch
                            {
                                //可以不用 Debug. Debug.LogError($"录制 {scenePrefab} 时出错, 出错位置为 {tileName} 的占地格: {occupyingCoord}");
                            }
                        }
                        continue;
                    }

                    // 记录 Debug.
                    if (debugDictionary1.TryGetValue(currentTile.name,out List<Vector3Int> coords))
                    {
                        coords.Add(currentCoord);
                    }
                    else
                    {
                        debugDictionary1[currentTile.name] = new List<Vector3Int>() { currentCoord };
                    }
                }
            }
        }

        //③. TerrainStaticFlags.IsBehindEntity. 再记录是否为后背格.
        //先储存信息, 初始全部标记为非背后格.
        bool[][] isBehindEntityGrid = new bool[mapWidth][];
        for (int x = 0; x < mapWidth; x++)
        {
            isBehindEntityGrid[x] = new bool[mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                isBehindEntityGrid[x][y] = false;
            }
        }
        //遍历 entitiesTilemap, 记录背后格, 如果背后格超出了地图外, 不用记录,反正过不去.
        foreach (Tilemap entitiesTilemap in entitiesTilemaps)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3Int currentCoord = new Vector3Int(x,y,0);
                    TileBase currentTile = entitiesTilemap.GetTile(currentCoord);
                    if (currentTile == null) continue;

                    //Cache TileName 和 SpriteName
                    string tileName = currentTile.name;
                    if (buildingTileName_configRow.TryGetValue(tileName,out DevConfigSO.BuildingTileInfo buildingsDevConfigRow))
                    {
                        //找到 建筑背后格.
                        List<Vector3Int> backCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,buildingsDevConfigRow.BackCubes);
                        foreach (Vector3Int backCoord in backCoords)
                        {
                            try
                            {
                                isBehindEntityGrid[backCoord.x][backCoord.y] = true;
                            }
                            catch
                            {
                                //DoNothing.
                            }
                        }
                        continue;
                    }
                    else if (obstacleTileName_configRow.TryGetValue(tileName,out DevConfigSO.ObstacleTileInfo obstacleDevConfigRow))
                    {
                        //找到 障碍物 背后格.
                        List<Vector3Int> backCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,obstacleDevConfigRow.BackCubes);
                        foreach (Vector3Int backCoord in backCoords)
                        {
                            try
                            {
                                isBehindEntityGrid[backCoord.x][backCoord.y] = true;
                            }
                            catch
                            {
                                //DoNothing.
                            }
                        }
                        continue;
                    }
                    // 记录 Debug.
                    if (debugDictionary1.TryGetValue(currentTile.name,out List<Vector3Int> coords))
                    {
                        coords.Add(currentCoord);
                    }
                    else
                    {
                        debugDictionary1[currentTile.name] = new List<Vector3Int>() { currentCoord };
                    }
                }
            }
        }

        foreach (KeyValuePair<string,List<Vector3Int>> pair in debugDictionary1)
        {
            Debug.LogError($"{scenePrefab}  含有未记录的 Entities : {pair.Key}, 坐标: {string.Join(" + ",pair.Value)}");
        }

        //④. TerrainStaticFlags.IsSpawnPoint. 
        //初始都不是 生成点.
        bool[][] isSpawnPointGrid = new bool[mapWidth][];
        for (int x = 0; x < mapWidth; x++)
        {
            isSpawnPointGrid[x] = new bool[mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                isSpawnPointGrid[x][y] = false;
            }
        }
        //拿到 一个 SortingOrder == 2 的 Editor 层. 这一层是用来记录 生成点, 楼梯, 敌人生成点 等等东西的.
        Tilemap[] allEditorTilemaps = MapRecordUtilities.GetTilemapsBySortingLayer(allTilemaps,MapDataRecordWindow.EditorLayerName);
        Tilemap editorTilemap2 = allEditorTilemaps.FirstOrDefault(t => t.GetComponent<TilemapRenderer>().sortingOrder == 2);
        if (editorTilemap2 == null)
        {
            Debug.LogError($"记录 {scenePrefab} 的刷新点 {MapDataRecordWindow.SpawnTileName} 时, 未找到 EditorEnter 层.");
        }
        List<StringAndVector3Int> tileNameAndCoordList = MapRecordUtilities.GetTileNameAndCoordList(editorTilemap2);
        //拿到其中的 生成点 位置.
        List<Vector3Int> spawnCoords = tileNameAndCoordList.Where(t => t.String == MapDataRecordWindow.SpawnTileName).Select(t => t.Vector3IntFd).ToList();
        foreach (Vector3Int spawnCoord in spawnCoords)
        {
            isSpawnPointGrid[spawnCoord.x][spawnCoord.y] = true;
        }

        //⑤. TerrainStaticFlags.IsUpstairs. 
        //初始都不是 楼梯.
        bool[][] isUpstairsGrid = new bool[mapWidth][];
        for (int x = 0; x < mapWidth; x++)
        {
            isUpstairsGrid[x] = new bool[mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                isUpstairsGrid[x][y] = false;
            }
        }
        //拿到其中的 楼梯 位置.
        List<Vector3Int> upstairsCoords = tileNameAndCoordList.Where(t => t.String == MapDataRecordWindow.UpstairsTileName).Select(t => t.Vector3IntFd).ToList();
        foreach (Vector3Int upstairsCoord in upstairsCoords)
        {
            isUpstairsGrid[upstairsCoord.x][upstairsCoord.y] = true;
        }

        //⑥. TerrainStaticFlags.IsInteractionPoint. 
        //初始都不是 交互点.
        bool[][] isInteractionPointGrid = new bool[mapWidth][];
        for (int x = 0; x < mapWidth; x++)
        {
            isInteractionPointGrid[x] = new bool[mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                isInteractionPointGrid[x][y] = false;
            }
        }
        //遍历 entitiesTilemap, 从记录的建筑数据中找到入口, 记录为交互格.
        foreach (Tilemap entitiesTilemapOfOrder1 in entitiesTilemaps)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3Int currentCoord = new Vector3Int(x,y,0);
                    TileBase currentTile = entitiesTilemapOfOrder1.GetTile(currentCoord);
                    if (currentTile == null) continue;

                    //Cache TileName 和 SpriteName
                    string tileName = currentTile.name;
                    if (buildingTileName_configRow.TryGetValue(tileName,out DevConfigSO.BuildingTileInfo buildingDevConfigRow))
                    {
                        //找到 建筑入口.
                        List<Vector3Int> entranceCoords = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,buildingDevConfigRow.InteractionCubes);
                        foreach (Vector3Int entranceCoord in entranceCoords)
                        {
                            isInteractionPointGrid[entranceCoord.x][entranceCoord.y] = true;
                        }
                    }
                }
            }
        }

        // 记录完了 TerrainStaticFlags 需要的数据. 填入数据.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                TerrainStaticCell currentStaticCell = terrainStaticCellsList[x * mapHeight + y];
                if (isWalkableGrid[x][y])
                {
                    currentStaticCell.SetTerrainStaticFlags(currentStaticCell.TerrainStaticFlagsPy.AddFlags(TerrainStaticFlags.IsWalkable));
                }
                if (isBehindEntityGrid[x][y])
                {
                    currentStaticCell.SetTerrainStaticFlags(currentStaticCell.TerrainStaticFlagsPy.AddFlags(TerrainStaticFlags.IsBehindEntity));
                }
                if (isSpawnPointGrid[x][y])
                {
                    currentStaticCell.SetTerrainStaticFlags(currentStaticCell.TerrainStaticFlagsPy.AddFlags(TerrainStaticFlags.IsSpawnPoint));
                }
                if (isUpstairsGrid[x][y])
                {
                    currentStaticCell.SetTerrainStaticFlags(currentStaticCell.TerrainStaticFlagsPy.AddFlags(TerrainStaticFlags.IsUpstairs));
                }
                if (isInteractionPointGrid[x][y])
                {
                    currentStaticCell.SetTerrainStaticFlags(currentStaticCell.TerrainStaticFlagsPy.AddFlags(TerrainStaticFlags.IsInteractionPoint));
                }
            }
        }

        //⑦. TerrainStaticCell._tileEnum. 
        TileEnum[][] tileEnumGrid = RecordTileEnumGrid(mapWidth,mapHeight,allGroundTilemaps);
        // 记录完了 TerrainStaticCell._tileEnum 需要的数据. 填入数据.
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                TerrainStaticCell currentStaticCell = terrainStaticCellsList[x * mapHeight + y];
                currentStaticCell.SetTileEnum(tileEnumGrid[x][y]);
            }
        }

        return terrainStaticCellsList;
    }

    private static TileEnum[][] RecordTileEnumGrid(int mapWidth,int mapHeight,Tilemap[] groundTilemaps)
    {
        TileEnum[][] tileEnumGrid = new TileEnum[mapWidth][];
        for (int i = 0; i < mapWidth; i++)
        {
            tileEnumGrid[i] = new TileEnum[mapHeight];
        }

        //功能: 由于显示最上面的就是所需的 TileEnum, 将 GroundTilemaps 进行排序.
        groundTilemaps = groundTilemaps.OrderByDescending(t => t.GetComponent<TilemapRenderer>().sortOrder).ToArray();

        //开始遍历.
        HashSet<string> tileNameSet = new HashSet<string>();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                TileEnum tileEnum = TileEnum.None;
                foreach (Tilemap groundTilemap in groundTilemaps)
                {
                    TileBase currentTile = groundTilemap.GetTile(new Vector3Int(x,y,0));
                    //功能: 如果没有 Tile, 继续下一个. 
                    if (currentTile == null) continue;

                    //功能: 如果有 Tile, 说明能找到合适的 TileEnum, 找到后 Break.
                    tileEnum = currentTile.name.ToEnumNoError<TileEnum>();
                    if (tileEnum == TileEnum.None)
                    {
                        tileNameSet.Add(currentTile.name);
                    }
                    break;
                }

                tileEnumGrid[x][y] = tileEnum;
            }
        }
        if (tileNameSet.Count > 0)
        {
            Debug.LogError($"未配置该 Tile: {string.Join(" + ",tileNameSet)}");
        }

        //返回计算好的数据.
        return tileEnumGrid;
    }

    private static List<ScenePrefabConfig.EditorTileEnumAndCoords> RecordEditorTileEnumAndCoordsList(GameObject scenePrefab)
    {
        List<ScenePrefabConfig.EditorTileEnumAndCoords> editorTileEnumAndCoordsList;
        Tilemap[] allTilemaps = scenePrefab.GetComponentsInChildren<Tilemap>(true);

        Tilemap[] allEditorTilemaps = MapRecordUtilities.GetTilemapsBySortingLayer(allTilemaps,MapDataRecordWindow.EditorLayerName);
        Tilemap editorTilemap2 = allEditorTilemaps.FirstOrDefault(t => t.GetComponent<TilemapRenderer>().sortingOrder == 2);
        if (editorTilemap2 == null)
        {
            Debug.LogError($"未找到 EditorTilemap2 层.");
            return new List<ScenePrefabConfig.EditorTileEnumAndCoords>();
        }

        //构造一个 Selector, 方便 debug.
        ScenePrefabConfig.EditorTileEnumAndCoords Selector(KeyValuePair<string,List<StringAndVector3Int>> t)
        {
            ScenePrefabConfig.EditorTileEnum editorTileEnum = t.Key.ToEnum<ScenePrefabConfig.EditorTileEnum>();
            if (editorTileEnum == ScenePrefabConfig.EditorTileEnum.None)
            {
                foreach (StringAndVector3Int stringAndVector3Int in t.Value)
                {
                    Debug.LogError($"{scenePrefab} 的 该 Tilemap: {editorTilemap2.transform.name} 此位置 : {stringAndVector3Int.Vector3IntFd} 有未设置 Enum 的 Tile : {t.Key}.");
                }
            }
            return new ScenePrefabConfig.EditorTileEnumAndCoords(editorTileEnum,t.Value.Select(t1 => t1.Vector3IntFd).ToList());
        }
        editorTileEnumAndCoordsList = MapRecordUtilities.GetTileNameAndCoordList(editorTilemap2).GroupBy(t => t.String).Select(Selector).ToList();
        return editorTileEnumAndCoordsList;
    }

    private static List<ScenePrefabConfig.BackAndEntitiesCoords> RecordBackAndEntitiesCoordsList(GameObject scenePrefab)
    {
        //      List<BackAndEntitiesCoords> _backAndEntitiesCoordsList. 记录背后格对应的实物所在的格.
        List<ScenePrefabConfig.BackAndEntitiesCoords> backAndEntitiesCoordsList = new List<ScenePrefabConfig.BackAndEntitiesCoords>();

        //先拿到 Entities 层.
        Tilemap[] allTilemaps = scenePrefab.GetComponentsInChildren<Tilemap>(true);

        Tilemap[] entitiesTilemaps = MapRecordUtilities.GetTilemapsBySortingLayer(allTilemaps,MapDataRecordWindow.EntitiesLayerName);
        if (entitiesTilemaps.Length <= 0)
        {
            Debug.LogError($"检测到 {scenePrefab} 没有 Entities 层, 可能丢失数据.");
            return backAndEntitiesCoordsList;
        }

        //Cache 两个要用的字典.
        //建筑 Tile Name 对应 建筑配置.
        Dictionary<string,DevConfigSO.BuildingTileInfo> buildingTileName_configRow = DevConfigSO.BuildingTileName_Info;
        //障碍物 Tile Name 对应 障碍物配置.
        Dictionary<string,DevConfigSO.ObstacleTileInfo> obstacleTileName_configRow = DevConfigSO.ObstacleTileName_Info;

        //临时储存 背后格 对应的 Entities.
        Dictionary<Vector3Int,HashSet<Vector3Int>> backCoord_entitiesCoords = new Dictionary<Vector3Int,HashSet<Vector3Int>>();
        //临时储存 Entities 对应的 背后格.
        Dictionary<Vector3Int,List<Vector3Int>> entitiesCoord_backCoords = new Dictionary<Vector3Int,List<Vector3Int>>();
        MapRecordUtilities.CalculateTilemapMaxSize(allTilemaps,out int mapWidth,out int mapHeight);
        foreach (Tilemap entitiesTilemap in entitiesTilemaps)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Vector3Int currentCoord = new Vector3Int(x,y,0);
                    TileBase currentTileBase = entitiesTilemap.GetTile(currentCoord);
                    if (currentTileBase == null) continue;

                    //拿到 Tile Name .
                    string currentTileName = currentTileBase.name;
                    if (buildingTileName_configRow.TryGetValue(currentTileName,out DevConfigSO.BuildingTileInfo buildingsDevConfigRow))
                    {
                        if (buildingsDevConfigRow.BackCubes != null
                         && buildingsDevConfigRow.BackCubes.Count != 0)
                        {
                            try
                            {
                                entitiesCoord_backCoords[currentCoord] = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,buildingsDevConfigRow.BackCubes);
                            }
                            catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                        }
                        continue;
                    }
                    else if (obstacleTileName_configRow.TryGetValue(currentTileName,out DevConfigSO.ObstacleTileInfo obstacleDevConfigRow))
                    {
                        if (obstacleDevConfigRow.BackCubes != null
                         && obstacleDevConfigRow.BackCubes.Count != 0)
                        {
                            entitiesCoord_backCoords[currentCoord] = OffsetUtilities.RelativeCubesToAbsolutes(currentCoord,obstacleDevConfigRow.BackCubes);
                        }

                        continue;
                    }
                }
            }
        }

        //将一个 entities 坐标对应的 backCoords, 转化为 一个 backCoords 对应哪些 entities.
        //先初始化 keys.
        foreach (KeyValuePair<Vector3Int,List<Vector3Int>> entitiesCoordBackCoords in entitiesCoord_backCoords)
        {
            foreach (Vector3Int backCoord in entitiesCoordBackCoords.Value)
            {
                backCoord_entitiesCoords[backCoord] = new HashSet<Vector3Int>();
            }
        }
        //再填入 values.
        foreach (KeyValuePair<Vector3Int,List<Vector3Int>> entitiesCoordBackCoords in entitiesCoord_backCoords)
        {
            foreach (Vector3Int backCoord in entitiesCoordBackCoords.Value)
            {
                if (backCoord_entitiesCoords[backCoord] == null)
                {
                    backCoord_entitiesCoords[backCoord] = new HashSet<Vector3Int>() { entitiesCoordBackCoords.Key };
                }
                else
                {
                    backCoord_entitiesCoords[backCoord].Add(entitiesCoordBackCoords.Key);
                }
            }
        }
        foreach (KeyValuePair<Vector3Int,HashSet<Vector3Int>> keyValuePair in backCoord_entitiesCoords)
        {
            ScenePrefabConfig.BackAndEntitiesCoords backAndEntitiesCoords = new ScenePrefabConfig.BackAndEntitiesCoords();
            backAndEntitiesCoords.SetBackCoord(keyValuePair.Key);
            backAndEntitiesCoords.SetEntitiesCoords(keyValuePair.Value.ToList());
            backAndEntitiesCoordsList.Add(backAndEntitiesCoords);
        }

        return backAndEntitiesCoordsList;
    }

    public static List<BuildingInstanceConfig> RecordBuildingInstanceConfigs(Grid grid,Tilemap entityTilemap,List<BuildingMb> buildingMbs,
                                                                             string cityPrefabName)
    {
        List<BuildingInstanceConfig> buildingInstanceConfigs = new List<BuildingInstanceConfig>(buildingMbs.Count);
        foreach (BuildingMb buildingMb in buildingMbs)
        {
            BuildingInstanceConfig buildingInstanceConfig = new BuildingInstanceConfig();
            Vector3Int buildingCoord = grid.WorldToCell(buildingMb.transform.position);
            string tileName = buildingMb.TileBase == null ? entityTilemap.GetTile(buildingCoord)?.name : buildingMb.TileBase.name;
            if (tileName == null) continue;
            buildingMb.BuildingEnum = tileName.Split('_')[0].ToEnumNoError<BuildingEnum>();
            if (buildingMb.BuildingEnum == BuildingEnum.None) continue;
            buildingInstanceConfigs.Add(buildingInstanceConfig);

            buildingInstanceConfig.SetCoord(buildingCoord);

            if (buildingMb.TileBase != null)
            {
                //BaiyiTODO. 有一些图片是通用的. 需要放到公共包. 
                buildingInstanceConfig.SetTileAddress(buildingMb.TileBase.name);
                AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                AddressableAssetGroup addressableAssetGroup = settings.FindGroup(cityPrefabName);
                string assetGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(buildingMb.TileBase));
                AddressableAssetEntry addressableAssetEntry = settings.CreateOrMoveEntry(assetGuid,addressableAssetGroup);
                addressableAssetEntry.SetAddress(buildingMb.TileBase.name);
            }

            BuildingEnum buildingEnum = buildingMb.BuildingEnum;
            buildingInstanceConfig.SetBuildingEnum(buildingEnum);

            DevConfigSO.BuildingTileName_Info.TryGetValue(tileName,out DevConfigSO.BuildingTileInfo buildingTileInfo);
            if (buildingTileInfo == null) continue;
            List<Vector3Int> interactions = OffsetUtilities.RelativeCubesToAbsolutes(buildingCoord,buildingTileInfo.InteractionCubes).ToList();
            buildingInstanceConfig.SetInteractions(interactions);

            TextId nameId = new TextId(buildingMb.NameTextId);
            buildingInstanceConfig.SetNameId(nameId);

            List<RoomInstanceConfig> roomConfigs = new List<RoomInstanceConfig>();
            buildingInstanceConfig.SetRoomConfigs(roomConfigs);
            foreach (BuildingMb.RoomReferences roomReferences in buildingMb.RoomReferencesList)
            {
                RoomInstanceConfig roomInstanceConfig = new RoomInstanceConfig();
                if (roomReferences.ParentGo == null) continue;
                ScenePrefabEnum scenePrefabEnum = roomReferences.ParentGo.name.Replace("_Editor","").ToEnum<ScenePrefabEnum>();
                if (scenePrefabEnum == ScenePrefabEnum.None) continue;
                roomConfigs.Add(roomInstanceConfig);
                roomInstanceConfig.SetScenePrefabEnum(scenePrefabEnum);

                //List<InitialCharacterData> _initialCharacters.
                List<InitialCharacterData> initialCharacters = new List<InitialCharacterData>();
                roomInstanceConfig.SetInitialCharacters(initialCharacters);
                if (roomReferences.RoomToFind != null)
                {
                    int characterCount = roomReferences.RoomToFind.transform.childCount;
                    for (int i = 0; i < characterCount; i++)
                    {
                        InitialCharacterData initialCharacterData = new InitialCharacterData();
                        Transform characterMbTransform = roomReferences.RoomToFind.transform.GetChild(i);
                        if (!characterMbTransform.TryGetComponent(out InitialCharacterDataMb characterDataMb))
                        {
                            Debug.LogError($"{roomReferences.ParentGo} 的 {roomReferences.RoomToFind} 下的角色找不到 {nameof(InitialCharacterDataMb)}");
                            continue;
                        }

                        initialCharacterData.SetInstanceId(characterDataMb.InstanceId);
                        initialCharacterData.SetCoord(grid.WorldToCell(characterDataMb.transform.position));
                        initialCharacterData.SetDirection(characterDataMb.IsToLeft ? 1 : 5);
                        initialCharacterData.SetIsStill(characterDataMb.IsStill);
                        initialCharacters.Add(initialCharacterData);
                    }
                }
            }
        }
        return buildingInstanceConfigs;
    }

    private static void Address()
    {
        string[] guids = AssetDatabase.FindAssets("",new[] { DevUtilities.CityRuntimePrefabsPath,DevUtilities.DungeonRuntimePrefabsPath,DevUtilities.RoomRuntimePrefabsPath, });
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;

        // BaiyiTODO. 房间应该按照城镇分组.
        foreach (string assetGuid in guids)
        {
            string assetName = Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(assetGuid));
            AddressableAssetGroup addressableAssetGroup = settings.FindGroup(assetName) ?? settings.CreateGroup(assetName,false,false,false,settings.DefaultGroup.Schemas,
                                                                                                                settings.DefaultGroup.SchemaTypes.ToArray());
            AddressableAssetEntry addressableAssetEntry = settings.CreateOrMoveEntry(assetGuid,addressableAssetGroup);
            addressableAssetEntry.SetAddress(assetName);
        }

        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssetIfDirty(settings);

        Debug.Log($"Address SceneRuntimePrefabs 成功. ");
    }

    private static void RecordRandomBattleEditor(GameObject sceneEditorPrefab,ScenePrefabConfig scenePrefabConfig)
    {
        Dictionary<BattleConfig.BattleTypeEnum,List<(SpecialCoords SpecialCoords,Dictionary<int,List<(int,CharacterEnum,Vector3Int)>>)>> battleTypeEnum_editorData = new();

        Transform randomBattleEditorTransform = sceneEditorPrefab.transform.Find("RandomBattleEditor");
        if (randomBattleEditorTransform == null) return;
        int childCount = randomBattleEditorTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform groupSortedByBattleType = randomBattleEditorTransform.GetChild(i);
            BattleConfig.BattleTypeEnum battleTypeEnum = groupSortedByBattleType.gameObject.name.ToEnum<BattleConfig.BattleTypeEnum>();
            BattleConfigBaker.RecordBattleEditor(groupSortedByBattleType,sceneEditorPrefab,
                                                 out List<(string,(SpecialCoords SpecialCoords,Dictionary<int,List<(int,CharacterEnum,Vector3Int)>>))>
                                                         battleConfigId_editorDataOfSingle);
            // Add battleTypeEnum and battleConfigId_editorDataOfSingle.Item2 to battleTypeEnum_editorData.
            if (!battleTypeEnum_editorData.TryGetValue(battleTypeEnum,out var editorData))
            {
                editorData = new List<(SpecialCoords SpecialCoords,Dictionary<int,List<(int,CharacterEnum,Vector3Int)>>)>();
                battleTypeEnum_editorData[battleTypeEnum] = editorData;
            }
            editorData.AddRange(battleConfigId_editorDataOfSingle.Select(t => t.Item2));
        }
        ScenePrefabConfig.BattleTypeAndSpecialCoordsList Selector(
            KeyValuePair<BattleConfig.BattleTypeEnum,List<(SpecialCoords SpecialCoords,Dictionary<int,List<(int,CharacterEnum,Vector3Int)>>)>> t)
        {
            ScenePrefabConfig.BattleTypeAndSpecialCoordsList battleTypeAndSpecialCoordsList = new ScenePrefabConfig.BattleTypeAndSpecialCoordsList();
            battleTypeAndSpecialCoordsList.SetBattleTypeEnum(t.Key);
            List<SpecialCoords> specialCoordsList = new List<SpecialCoords>();
            battleTypeAndSpecialCoordsList.SetSpecialCoordsList(specialCoordsList);
            foreach (var valueTuple in t.Value)
            {
                SpecialCoords specialCoords = valueTuple.SpecialCoords;
                specialCoordsList.Add(specialCoords);
            }

            return battleTypeAndSpecialCoordsList;
        }
        List<ScenePrefabConfig.BattleTypeAndSpecialCoordsList> battleTypeAndSpecialCoordsLists = battleTypeEnum_editorData.Select(Selector).ToList();
        scenePrefabConfig.SetBattleTypeAndSpecialCoordsLists(battleTypeAndSpecialCoordsLists);
    }
}
}
#endif