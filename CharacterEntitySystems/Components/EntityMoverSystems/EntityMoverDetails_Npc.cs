using System.Collections.Generic;

using Common.Utilities;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MechanicsAndFormulas;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems
{
public abstract class EntityMoverDetails_Npc : Details
{
    public static void WhenJourneyStartReached(NpcEntity npcEntity,EntityMover entityMover)
    {
        //每一段路的开始, 计算并设置角色的方向.
        Vector3Int currentCoord = entityMover.CurrentCoordPy;
        Vector3Int nextCoord = entityMover.CoordPathPy[entityMover.WaypointIndexPy + 1];
        npcEntity.NpcPy.CoordSystemPy.SetDirectionIndex(OffsetUtilities.CalculateDirectionIndex(currentCoord,nextCoord));
    }

    public static void WhenJourneyEndReached(NpcEntity npcEntity,EntityMover entityMover)
    {
        //当一段路到达目标点, 设置坐标.
        npcEntity.NpcPy.CoordSystemPy.SetCurrentCoord(entityMover.CurrentCoordPy);
    }

    public static void WhenBeginToMove(NpcEntity npcEntity,EntityMover entityMover)
    {
        Npc npc = npcEntity.NpcPy;
        // 功能: 每当角色的移动状态发生改变时, 动画也要改变.
        bool isInBattle = _battleManager.IsInBattlePy;
        npcEntity.CharacterAnimationSystemPy.DoRunAnime();

        if (!isInBattle) return;

        //开始移动时, 隐藏方向显示.
        npcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(false);
        //战斗中, 会消耗 Ap.
        //角色可以在该场景移动, 肯定有 Prefab.
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
        if (currentScene is not IHasScenePrefab hasScenePrefab)
        {
            Debug.LogError($"当前 Scene 没有 Prefab. SceneId: {currentScene.InstanceIdPy}");
            return;
        }
        TerrainStaticCell[][] terrainStaticGrid = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig().TerrainStaticGridPy;
        Dictionary<TileEnum,TileConfig> tileEnum_tileConfigRow = CommonDesignSO.TerrainConfigPy.TileEnum_TileConfigPy;
        float costAp = MovementMechanics.CalculateCostAp(npc.PropertySystemPy[PropertyEnum.Speed],entityMover.CoordPathPy,terrainStaticGrid,tileEnum_tileConfigRow);
        npc.PropertySystemPy.ChangeAp(-costAp);
        MechanicsOfAutoRemoveBuff.RecordHasConsumedSomeAp(npcEntity.BuffPoolPy,costAp);
    }

    public static void WhenStoppedMove(NpcEntity npcEntity,EntityMover entityMover)
    {
        // 功能: 每当角色的移动状态发生改变时, 动画也要改变.
        bool isInBattle = _battleManager.IsInBattlePy;
        npcEntity.CharacterAnimationSystemPy.DoIdleAnime();

        if (!isInBattle) return;

        //移动结束后时, 显示方向显示.
        npcEntity.CharacterPanelControllerPy.RefreshDirection();
        npcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(true);
    }
}
}