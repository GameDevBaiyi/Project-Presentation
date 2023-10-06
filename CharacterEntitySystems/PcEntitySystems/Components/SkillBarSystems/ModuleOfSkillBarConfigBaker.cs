#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;

using Common.Extensions;

using DevTools.DevConfigData;
using DevTools.MapProcessors;
using DevTools.ProgrammerTools;

using LowLevelSystems.Common;

using UnityEditor;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public static class ModuleOfSkillBarConfigBaker
{
    public static void RecordModuleOfSkillBarConfig()
    {
        DevConfigSO devConfigSO = DevUtilities.GetDevConfigSO();
        List<ModuleOfSkillBarConfig> moduleOfSkillBarConfigs = new List<ModuleOfSkillBarConfig>();
        foreach (GameObject prefab in devConfigSO.SkillBarModulePrefabs)
        {
            ModuleOfSkillBarConfig moduleOfSkillBarConfig = new ModuleOfSkillBarConfig();

            //ModuleOfSkillBarEnum _moduleOfSkillBarEnum
            ModuleOfSkillBarEnum moduleOfSkillBarEnum = prefab.name.ToEnum<ModuleOfSkillBarEnum>();
            moduleOfSkillBarConfig.SetModuleOfSkillBarEnum(moduleOfSkillBarEnum);

            //NodeOfModuleFlags[][] _nodeFlagsGrid
            Tilemap[] allCompressedTilemaps = MapRecordUtilities.GetAllCompressedTilemaps(prefab);
            if (allCompressedTilemaps.Length != 1)
            {
                Debug.LogError("记录 技能栏模组 时, 应该有且只有一个 Tilemap. ");
                return;
            }

            Tilemap tilemap = allCompressedTilemaps[0];
            MapRecordUtilities.CalculateTilemapSize(tilemap,out int mapWidth,out int mapHeight);

            NodeOfModuleFlags[][] nodeFlagsGrid = new NodeOfModuleFlags[mapWidth][];
            for (int i = 0; i < mapWidth; i++)
            {
                nodeFlagsGrid[i] = new NodeOfModuleFlags[mapHeight];
            }

            //先记录 IsWalkable.
            bool[][] walkableGrid = new bool[mapWidth][];
            for (int i = 0; i < mapWidth; i++)
            {
                walkableGrid[i] = new bool[mapHeight];
            }
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    walkableGrid[i][j] = tilemap.GetTile(new Vector3Int(i,j,0)) != null;
                }
            }
            //写入记录的 IsWalkable.
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (walkableGrid[i][j])
                    {
                        nodeFlagsGrid[i][j] = nodeFlagsGrid[i][j].AddFlags(NodeOfModuleFlags.IsWalkable);
                    }
                }
            }

            //记录 IsSugarPoint.
            List<Vector3Int> allSugarPoints = MapRecordUtilities.GetTileNameAndCoordList(tilemap)
                                                                .Where(t => t.String is MapDataRecordWindow.SpawnTileName or MapDataRecordWindow.InteractionTileName)
                                                                .Select(t => t.Vector3IntFd)
                                                                .ToList();
            foreach (Vector3Int sugarPoint in allSugarPoints)
            {
                nodeFlagsGrid[sugarPoint.x][sugarPoint.y] = nodeFlagsGrid[sugarPoint.x][sugarPoint.y].AddFlags(NodeOfModuleFlags.IsSugarPoint);
            }

            moduleOfSkillBarConfig.SetNodeFlagsList(nodeFlagsGrid);

            //List<Vector3Int> _starPoses
            List<Vector3Int> starPoses = MapRecordUtilities.GetTileNameAndCoordList(tilemap)
                                                           .Where(t => t.String == MapDataRecordWindow.InteractionTileName)
                                                           .Select(t => t.Vector3IntFd)
                                                           .ToList();
            moduleOfSkillBarConfig.SetStarPoses(starPoses);

            moduleOfSkillBarConfigs.Add(moduleOfSkillBarConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.SkillConfigHubPy.SetModuleOfSkillBarConfigs(moduleOfSkillBarConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"记录技能栏模组 {nameof(ModuleOfSkillBarConfig)} 成功.");
    }
}
}
#endif