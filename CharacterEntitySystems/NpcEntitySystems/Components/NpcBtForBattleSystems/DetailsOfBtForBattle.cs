using System.Collections.Generic;

using Common.BehaviourTree;
using Common.Extensions;
using Common.Utilities;

using JetBrains.Annotations;

using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MechanicsAndFormulas;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using UnityEngine;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
public abstract partial class DetailsOfBtForBattle : Details
{
    [CanBeNull]
    public static CharacterEntity CalculateSkillTargetInBattle(NpcEntity npcEntity,SkillSugarConfig skillSugarConfig)
    {
        SkillMainConfig skillMainConfig = skillSugarConfig.SkillMainIdPy.SkillMainConfigPy;
        NpcTargetConfig npcTargetConfig = skillMainConfig.NpcTargetConfigPy;
        SkillTargetTypeEnum skillTargetTypeEnum = npcTargetConfig.SkillTargetTypeEnumPy;
        switch (skillTargetTypeEnum)
        {
        case SkillTargetTypeEnum.NearestEnemy:
            return GetNearestEnemy(npcEntity);

        case SkillTargetTypeEnum.LowestHpAlly:
            return GetLowestHpAlly(npcEntity);

        case SkillTargetTypeEnum.NoBuffAlly:
            return GetNoBuffAlly(npcEntity,npcTargetConfig);

        case SkillTargetTypeEnum.NoBuffEnemy:
            return GetNoBuffEnemy(npcEntity,npcTargetConfig);

        default:
            Debug.LogError($" CalculateSkillTargetInBattle 含有未预设的 switch.");
            return null;
        }
    }

    public static BaseNode.StatusEnum MoveToTargetCoord(NpcEntity npcEntity)
    {
        Npc npc = npcEntity.NpcPy;
        BtForBattle btForBattle = npcEntity.BtForBattlePy;

        //如果 IsMoving, 返回 Running.  
        EntityMover entityMover = npcEntity.EntityMoverPy;
        if (entityMover.IsMovingPy) return BaseNode.StatusEnum.Running;

        //如果 到达了 目标点, 返回 Success.
        Vector3Int npcCoord = npc.CoordSystemPy.CurrentCoordPy;
        if (npcCoord == btForBattle.TargetCoordPy) return BaseNode.StatusEnum.Success;

        //如果寻路失败, 返回 Failure. 
        if (!_pathfindingManager.TryFindPath(npcCoord,btForBattle.TargetCoordPy,entityMover.CoordPathPy,npc.CampRelationsPy)) return BaseNode.StatusEnum.Failure;

        // 如果成功: 计算 Ap.  如果 Ap 够, 开始移动, 返回 Running.
        PropertySystem npcPropertySystem = npc.PropertySystemPy;
        float speed = npcPropertySystem[PropertyEnum.Speed];
        //角色可以在该场景移动, 肯定有 Prefab.
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
        if (currentScene is not IHasScenePrefab hasScenePrefab)
        {
            Debug.LogError($"当前 Scene 没有 Prefab. SceneId: {currentScene.InstanceIdPy}");
            return BaseNode.StatusEnum.Failure;
        }
        TerrainStaticCell[][] terrainStaticGrid = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig().TerrainStaticGridPy;
        Dictionary<TileEnum,TileConfig> tileEnum_tileConfigRow = CommonDesignSO.TerrainConfigPy.TileEnum_TileConfigPy;
        float neededAp = MovementMechanics.CalculateNeededAp(speed,entityMover.CoordPathPy,terrainStaticGrid,tileEnum_tileConfigRow);
        if (npcPropertySystem.CurrentApPy >= neededAp)
        {
            entityMover.SetTargetCoord(btForBattle.TargetCoordPy);
            return BaseNode.StatusEnum.Running;
        }

        //如果 Ap 不够, 结合地形和 Ap 计算最远能移动到的点. 
        float apCost = 0f;
        Vector3Int targetCoord = Vector3Int.zero;
        int pathIndex = 1;
        while (true)
        {
            Vector3Int currentWaypoint = entityMover.CoordPathPy[pathIndex];
            apCost += (100f / speed) * tileEnum_tileConfigRow[terrainStaticGrid[currentWaypoint.x][currentWaypoint.y].TileEnumPy].APCostMultiplierPy;
            if (npcPropertySystem.CurrentApPy >= apCost
             && _pathfindingManager.TryFindPath(npcCoord,currentWaypoint,null,npc.CampRelationsPy))
            {
                targetCoord = currentWaypoint;
                pathIndex++;
            }
            else
            {
                break;
            }
        }

        //  如果该点就是 Npc 现在的点, 代表一步都不能走, 返回 Failure. 
        if (targetCoord == Vector3Int.zero) return BaseNode.StatusEnum.Failure;

        entityMover.SetTargetCoord(targetCoord);
        return BaseNode.StatusEnum.Running;
    }

    public static BaseNode.StatusEnum Attack(NpcEntity npcEntity)
    {
        Npc npc = npcEntity.NpcPy;
        BtForBattle btForBattle = npcEntity.BtForBattlePy;

        // 攻击动画 时, 返回 Running. 
        if (npcEntity.CharacterAnimationSystemPy.IsAttackingPy) return BaseNode.StatusEnum.Running;

        //AP 不够了, Failure
        SkillSugarConfig skillSugarConfig = btForBattle.CurrentSkillSugarConfigPy;
        if (npc.PropertySystemPy.CurrentApPy < skillSugarConfig.CostApPy) return BaseNode.StatusEnum.Failure;

        List<List<Vector3Int>> effectRangeList = new List<List<Vector3Int>>();
        foreach (SkillSugarConfig.SkillEffectConfigForOneRound skillEffectConfigForOneRound in skillSugarConfig.SkillEffectConfigPy)
        {
            List<Vector3Int> rangeInCubeCoord = skillEffectConfigForOneRound.EffectRangeEnumPy.RangeInCubeCoord();
            List<Vector3Int> realEffectRange
                = OffsetUtilities.Convert0DirectionRelativeCubesToOffset(rangeInCubeCoord,btForBattle.SkillEffectCenterCoordPy,btForBattle.SkillEffectDirectionPy);
            effectRangeList.Add(realEffectRange);
        }

        // //调整 Npc 朝向.
        float angle = Vector2.SignedAngle(Vector2.right,btForBattle.SkillEffectCenterCoordPy.ToWorldPos() - npc.CoordSystemPy.CurrentCoordPy.ToWorldPos());
        if (angle < 0f) angle += 360f;
        int direction = GridUtilities.AngleRangeOfDirections.FindIndex(t => angle.IsInRange(t.x,t.y,ExclusiveFlags.None));
        if (direction == -1)
        {
            Debug.LogError($"未找到该 angle 的区间: {angle}");
            direction = 0;
        }
        npcEntity.ChangeDirection(direction);

        //应用技能效果.
        DetailsOfSkillEffects.ApplySkillEffectsAndDoAnime(npcEntity,btForBattle.SkillEffectCenterCoordPy,effectRangeList,skillSugarConfig,btForBattle.SkillEffectDirectionPy);
        btForBattle.IncreaseCurrentNormalActionIndex();

        return BaseNode.StatusEnum.Success;
    }
}
}