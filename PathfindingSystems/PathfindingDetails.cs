using JetBrains.Annotations;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.Common;
using LowLevelSystems.SceneSystems.Base;

using UnityEngine;

namespace LowLevelSystems.PathfindingSystems
{
public abstract class PathfindingDetails : Details
{
    public static bool IsWalkable(PathfindingManager.AStarNode aStarNode,[CanBeNull] BattleConfig.CampRelations selfRelations = null,bool isTerminal = false)
    {
        //功能: 如果格子属性就是不可行走, 那么不可行走.
        if (!aStarNode.StaticCell.TerrainStaticFlagsPy.HasFlag(TerrainStaticFlags.IsWalkable)) return false;
        if (aStarNode.TerrainDynamicFlags.HasFlag(TerrainDynamicFlags.IsObstacle))
        {
            //Debug.
            if (!_battleManager.IsInBattlePy)
            {
                Debug.LogError($"目前的设计, 只有战斗会出现 Obstacle 格. 但现在非战斗仍然检测到了. 是否未清理?");
            }
            return false;
        }

        int countOfColliders = Physics2D.OverlapPoint(_grid.GetCellCenterWorld(aStarNode.Coord),PhysicsUtilities.FilterForCharacter,_pathfindingManager.Collider2DsCachePy);
        if (countOfColliders != 0)
        {
            // 如果是终点有人了, 不可行走. 
            if (isTerminal) return false;

            Collider2D firstCollider2D = _pathfindingManager.Collider2DsCachePy[0];
            //Debug.
            if (!firstCollider2D.TryGetComponent(out CharacterEntity characterEntity))
            {
                Debug.LogError($"该 Collider: {firstCollider2D.name} 未找到 CharacterEntity.");
                return false;
            }

            if (selfRelations != null)
            {
                // 敌对或者中立阵营, 就无法穿过.
                if (!selfRelations.IsSameOrFriendlyWith(characterEntity.CharacterPy.CampRelationsPy.CampIdPy)) return false;
            }
        }

        return true;
    }

    public static void ResetPathfinding()
    {
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
        if (currentScene is not IHasScenePrefab hasScenePrefab)
        {
            Debug.LogError($"当前场景没有 Prefab. 无法寻路. ");
            return;
        }
        ScenePrefabConfig scenePrefabConfig = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig();
        int mapWidth = scenePrefabConfig.MapWidthPy;
        int mapHeight = scenePrefabConfig.MapHeightPy;
        TerrainStaticCell[][] terrainStaticGrid = scenePrefabConfig.TerrainStaticGridPy;

        _pathfindingManager.ResetPathfinding(mapWidth,mapHeight,terrainStaticGrid,_battleManager.IsInBattlePy ? _battleManager.SpecialCoordsPy.ObstacleCoordsPy : null);
    }
}
}