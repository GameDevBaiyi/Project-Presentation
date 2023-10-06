using Common.BehaviourTree;
using Common.Extensions;
using Common.Utilities;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.MissionSystems.PlotGuideSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
public abstract class NpcAIForLivingDetails : Details
{
    /// <summary>
    /// Process 的期望: 如果没有闲逛, 就开启闲逛, 如果闲逛过了, 就返回 Success 并且设置为 未闲逛. 
    /// </summary>
    public static BaseNode.StatusEnum HangOut(NpcAIForLiving npcAIForLiving)
    {
        //IsStillOnLivingPy 使得 Npc 不会到处闲逛.
        if (npcAIForLiving.NpcEntityPy.NpcPy.IsStillOnLivingPy) return BaseNode.StatusEnum.Success;
        if (PlotGuideManager.IsGuidingPlotPy) return BaseNode.StatusEnum.Success;

        NpcEntity npcEntity = npcAIForLiving.NpcEntityPy;
        //如果正在移动, 代表正在闲逛.
        if (npcEntity.EntityMoverPy.IsMovingPy) return BaseNode.StatusEnum.Running;

        //如果这一轮未闲逛, 设置一个闲逛路线.
        if (!npcAIForLiving.HasHungOutPy)
        {
            //找到 Npc 的所在场景, 根据场景设置路线.
            Npc npc = npcEntity.NpcPy;
            // ReSharper disable once PossibleNullReferenceException
            Scene currentScene = npc.SceneIdPy.ScenePy;

            //Debug. 切换场景时, 不用做太多事.
            if (currentScene.InstanceIdPy != SceneHub.CurrentSceneIdPy.Id)
            {
                npcAIForLiving.SetHasHungOut(false);
                return BaseNode.StatusEnum.Failure;
            }

            Vector3Int currentCoord = npc.CoordSystemPy.CurrentCoordPy;
            //功能: Npc 在城镇中移动.
            if (currentScene.SceneTypeEnumPy == SceneTypeEnum.City)
            {
                Vector3Int respawnCoord = npc.RespawnCoordInCityPy;
                //功能: 随机移动 _movementRange 距离内.
                int movementRange = Random.Range(1,NpcEntity.MovementRange + 1);

                //构造一个可移动的格子选择. 功能: 可行走, 且距离 起始点不能超过一个范围.
                bool MovableCoordSelector(Vector3Int offsetCoord)
                {
                    return _pathfindingManager.CheckIfInRangeAndWalkable(offsetCoord,npc.CampRelationsPy,true)
                        && (OffsetUtilities.CalculateSteps(offsetCoord,respawnCoord) <= NpcEntity.MovementRadius);
                }

                OffsetUtilities.GetRing(currentCoord,movementRange,npcAIForLiving.Ring,MovableCoordSelector);
                //功能: 没有可移动的点就返回.
                if (npcAIForLiving.Ring.Count == 0)
                {
                    npcAIForLiving.SetHasHungOut(false);
                    return BaseNode.StatusEnum.Failure;
                }
                //功能: 随机选择一个目标点进行路径计算和移动.
                Vector3Int randomDestination = npcAIForLiving.Ring.GetRandomItem();
                //功能: 无路径就返回.
                if (!_pathfindingManager.TryFindPath(currentCoord,randomDestination))
                {
                    npcAIForLiving.SetHasHungOut(false);
                    return BaseNode.StatusEnum.Failure;
                }
                npcEntity.EntityMoverPy.SetTargetCoord(randomDestination);
            }
            //功能: Npc 在房间中移动.
            else if (currentScene.SceneTypeEnumPy == SceneTypeEnum.Room)
            {
                Room currentRoom = (Room)currentScene;
                //从所有可移动的坐标处预选.
                Vector3Int targetCoord = currentRoom.ScenePrefabEnumPy.ScenePrefabConfig().AllWalkableCoordsPy.GetRandomItem();
                //功能: 无路径就返回.
                if (!_pathfindingManager.TryFindPath(currentCoord,targetCoord))
                {
                    npcAIForLiving.SetHasHungOut(false);
                    return BaseNode.StatusEnum.Failure;
                }
                //功能: 移动.
                npcEntity.EntityMoverPy.SetTargetCoord(targetCoord);
            }

            //设置为已经闲逛.
            npcAIForLiving.SetHasHungOut(true);
            return BaseNode.StatusEnum.Running;
        }

        //即没有移动, 也已经闲逛了, 那就返回 Success.
        npcAIForLiving.SetHasHungOut(false);
        return BaseNode.StatusEnum.Success;
    }

    public static BaseNode.StatusEnum Idle(NpcAIForLiving npcAIForLiving)
    {
        if (!npcAIForLiving.HasBeenIdlePy)
        {
            npcAIForLiving.SetIdleTimer(0f);
            npcAIForLiving.SetIdleTimeSpan(Details.SettingsSo.RangeOfNpcIdleTime.GetRandomNumber());
            npcAIForLiving.SetHasBeenIdle(true);
        }

        npcAIForLiving.SetIdleTimer(npcAIForLiving.IdleTimerPy + Time.deltaTime);

        if (npcAIForLiving.IdleTimerPy >= npcAIForLiving.IdleTimeSpanPy)
        {
            npcAIForLiving.SetHasBeenIdle(false);
            return BaseNode.StatusEnum.Success;
        }
        else
        {
            return BaseNode.StatusEnum.Running;
        }
    }
}
}