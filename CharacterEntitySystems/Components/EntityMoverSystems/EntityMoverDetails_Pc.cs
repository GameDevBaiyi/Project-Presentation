using System.Collections.Generic;

using Common.Utilities;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.InterestSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MechanicsAndFormulas;
using LowLevelSystems.MissionSystems.Conditions;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.CitySystems.Base;
using LowLevelSystems.SceneSystems.Entity;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Scripts.FGUIPartialScripts.UICommon;

using UICommon;

using UnityEngine;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems
{
public abstract class EntityMoverDetails_Pc : Details
{
    public static void WhenJourneyStartReached(PcEntity pcEntity,EntityMover entityMover)
    {
        Pc pc = pcEntity.PcPy;
        // 功能: 每当到达一个路径点, LineRenderer 减少一截. 
        UiManager.PathDrawerPy.ChangeLineWaypointCount(entityMover.WaypointCountPy - entityMover.WaypointIndexPy - 1);

        //功能: 计算角色的方向.
        Vector3Int currentCoord = entityMover.CurrentCoordPy;
        Vector3Int nextCoord = entityMover.CoordPathPy[entityMover.WaypointIndexPy + 1];
        pc.CoordSystemPy.SetDirectionIndex(OffsetUtilities.CalculateDirectionIndex(currentCoord,nextCoord));
    }
    public static void WhenJourneyEndReached(PcEntity pcEntity,EntityMover entityMover)
    {
        Pc pc = pcEntity.PcPy;

        Vector3Int currentCoord = entityMover.CurrentCoordPy;
        Vector3Int lastCoord = entityMover.PreviousCoordPy;
        //功能: 记录进 Abstract Data.
        pc.CoordSystemPy.SetCurrentCoord(currentCoord);

        //如果当前 Scene 无预制体, 下面的效果不用做.
        if (SceneHub.CurrentSceneIdPy.ScenePy is not IHasScenePrefab hasScenePrefab) return;

        // 功能: 移动时如果移动到建筑物背后, 要做一些效果.
        // Cache 两个坐标对应的 Entities Coords.
        SceneEntity currentSceneEntity = EntityManager.CurrentSceneEntityPy;
        ScenePrefabConfig currentScenePrefabConfig = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig();
        currentScenePrefabConfig.BackCoord_EntitiesCoordsPy.TryGetValue(lastCoord,out HashSet<Vector3Int> lastEntitiesCoords);
        currentScenePrefabConfig.BackCoord_EntitiesCoordsPy.TryGetValue(currentCoord,out HashSet<Vector3Int> currentEntitiesCoords);

        //清理之前的 Cache.
        pcEntity.EntitiesCoordsPy.Clear();

        //计算新进入的建筑.
        if (currentEntitiesCoords != null)
        {
            foreach (Vector3Int currentEntitiesTile in currentEntitiesCoords)
            {
                if (lastEntitiesCoords == null)
                {
                    pcEntity.EntitiesCoordsPy.Add(currentEntitiesTile);
                    continue;
                }

                if (!lastEntitiesCoords.Contains(currentEntitiesTile))
                {
                    pcEntity.EntitiesCoordsPy.Add(currentEntitiesTile);
                }
            }
        }

        //功能: 进入的新建筑减少 Alpha.
        foreach (Vector3Int entitiesCoord in pcEntity.EntitiesCoordsPy)
        {
            currentSceneEntity.ReduceEntityAlpha(entitiesCoord);
        }

        //清理之前的 Cache.
        pcEntity.EntitiesCoordsPy.Clear();

        //计算离开的建筑.
        if (lastEntitiesCoords != null)
        {
            foreach (Vector3Int lastEntitiesCoord in lastEntitiesCoords)
            {
                if (currentEntitiesCoords == null)
                {
                    pcEntity.EntitiesCoordsPy.Add(lastEntitiesCoord);
                    continue;
                }

                if (!currentEntitiesCoords.Contains(lastEntitiesCoord))
                {
                    pcEntity.EntitiesCoordsPy.Add(lastEntitiesCoord);
                }
            }
        }

        //功能: 离开的建筑恢复 Alpha.
        foreach (Vector3Int entitiesCoord in pcEntity.EntitiesCoordsPy)
        {
            currentSceneEntity.RestoreEntityAlpha(entitiesCoord);
        }

        //功能: 非战斗, 在城镇中移动, 消耗兴致值.
        if (!_battleManager.IsInBattlePy
         && SceneHub.CurrentSceneIdPy.ScenePy.SceneTypeEnumPy == SceneTypeEnum.City)
        {
            InterestSystem interestSystem = pc.InterestSystemPy;
            DetailsOfInterestSystem.ChangeLimitedValue(interestSystem,-SettingsSo.InterestCostPerMovement);
            //如果移动导致兴致值降到 0, 会有惩罚. 
            if (interestSystem.CurrentInterestValuePy <= 0f)
            {
                FaintPopup.OpenAsync(pc.CharacterEnumPy.CharacterConfig().CharacterNamePy,pc);
                return;
            }
        }

        DetailsOfMissionConditions.CheckHasPcsInEventTriggerArea(pcEntity);
    }
    public static void WhenDestinationReached(PcEntity pcEntity,EntityMover entityMover)
    {
        Vector3Int destinationCoord = entityMover.DestinationCoordPy;
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;

        if (currentScene is not IHasScenePrefab hasScenePrefab) return;

        //如果是 City 的刷新点, 打开世界地图.
        ScenePrefabConfig scenePrefabConfig = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig();
        TerrainStaticFlags terrainStaticFlags = scenePrefabConfig.TerrainStaticGridPy[destinationCoord.x][destinationCoord.y].TerrainStaticFlagsPy;
        if (currentScene is City currentCity
         && currentCity.CanLeavePy
         && terrainStaticFlags.HasFlag(TerrainStaticFlags.IsSpawnPoint))
        {
            WorldMapPopup.OpenAsync();
            return;
        }

        //功能: 如果到达的是入口点, 进入对应场景.
        if (currentScene is IHasEntranceToScene hasEntranceToScene
         && hasEntranceToScene.EntranceCoord_SceneIdPy.TryGetValue(destinationCoord,out SceneId targetSceneId))
        {
            PcDetails.MoveToAsync(terrainStaticFlags,pcEntity.PcPy,targetSceneId.ScenePy);
            return;
        }
    }
    public static void WhenBeginToMove(PcEntity pcEntity,EntityMover entityMover)
    {
        //功能: 如果在战斗中, 需要在开始时计算行动力, 然后消耗掉.
        bool isInBattle = _battleManager.IsInBattlePy;
        if (isInBattle)
        {
            //如果当前场景不是预设场景, 逻辑需要改变.
            if (SceneHub.CurrentSceneIdPy.ScenePy is not IHasScenePrefab hasScenePrefab) return;
            ScenePrefabConfig scenePrefabConfig = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig();

            //计算行动力.
            Pc pc = pcEntity.PcPy;
            int costAp = MovementMechanics.CalculateCostAp(pc.PropertySystemPy[PropertyEnum.Speed],entityMover.CoordPathPy,scenePrefabConfig.TerrainStaticGridPy,
                                                           CommonDesignSO.TerrainConfigPy.TileEnum_TileConfigPy);
            pc.PropertySystemPy.ChangeAp(-costAp);
            MechanicsOfAutoRemoveBuff.RecordHasConsumedSomeAp(pcEntity.BuffPoolPy,costAp);
            //开始移动时, 隐藏方向显示.
            pcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(false);

            //功能: 移动时取消部分操作和 UI.
            MovingInBattleState movingInBattleState = (MovingInBattleState)InputFSM.InputStateEnum.MovingInBattle.InputState();
            if (movingInBattleState.IsInStatePy) UiManager.CellUiShowerForMovableRangePy.Hide();
        }

        // 功能: 每当角色的移动状态发生改变时, 动画也要改变.
        pcEntity.CharacterAnimationSystemPy.DoRunAnime();
    }
    public static void WhenStoppedMove(PcEntity pcEntity,EntityMover entityMover)
    {
        UiManager.PathDrawerPy.HidePath();
        // 功能: 每当角色的移动状态发生改变时, 动画也要改变.
        pcEntity.CharacterAnimationSystemPy.DoIdleAnime();
        if (_battleManager.IsInBattlePy) PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.RotatingPc);

        DetailsOfMissionConditions.ProcessVisibleOfNpcEntitiesAsync();
    }
}
}