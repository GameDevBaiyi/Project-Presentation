using System.Collections.Generic;

using Common.Utilities;

using Cysharp.Threading.Tasks;

using LowLevelSystems.BattleSystems.Base;
using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.Common;
using LowLevelSystems.InteractableSystems;
using LowLevelSystems.PathfindingSystems;

using Sirenix.OdinInspector;

using UnityEngine;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems
{
public class EntityMover
{
    [Title("References")]
    [ShowInInspector]
    private readonly CharacterEntity _characterEntity;
    public CharacterEntity CharacterEntityPy => this._characterEntity;
    private readonly Transform _entityTransform;

    private static PathfindingManager _pathfindingManager;

    [Title("Data")]
    [ShowInInspector]
    private Vector3Int _targetCoord;
    [ShowInInspector]
    private IInteractable _interactable;

    public void SetTargetCoord(Vector3Int targetCoord,IInteractable interactable = null)
    {
        this._targetCoord = targetCoord;
        this._interactable = interactable;
        if (this._isMoving) return;
        Vector3Int currentCoord = this._characterEntity.CharacterPy.CoordSystemPy.CurrentCoordPy;
        if (this._targetCoord == currentCoord) return;
        BattleConfig.CampRelations campRelations = BattleManager.InstancePy.IsInBattlePy ? this._characterEntity.CharacterPy.CampRelationsPy : null;
        if (!_pathfindingManager.TryFindPath(currentCoord,targetCoord,this._coordPath,campRelations)) return;
        this.ResetPathCache();
        this.ToMoveAsync();
    }

    [ShowInInspector]
    private bool _isMoving;
    public bool IsMovingPy => this._isMoving;
    //功能: 到达下一点时停止.
    [ShowInInspector]
    private bool _isToStopAtNextWaypoint;
    public void StopAtNextWaypoint()
    {
        if (!this._isMoving) return;
        this._isToStopAtNextWaypoint = true;
    }
    //功能: 移速.
    [ShowInInspector]
    private float _moveSpeed;

    //一条路径的 Cache.
    [ShowInInspector]
    private readonly List<Vector3Int> _coordPath = new List<Vector3Int>(20);
    public List<Vector3Int> CoordPathPy => this._coordPath;
    [ShowInInspector]
    private readonly List<Vector3> _worldPath = new List<Vector3>(20);
    public List<Vector3> WorldPathPy => this._worldPath;
    [ShowInInspector]
    private int _waypointCount;
    public int WaypointCountPy => this._waypointCount;
    [ShowInInspector]
    private int _waypointIndex;
    public int WaypointIndexPy => this._waypointIndex;

    public EntityMover(CharacterEntity characterEntity,float initialSpeed,Transform entityTransform)
    {
        this._characterEntity = characterEntity;
        this._entityTransform = entityTransform;

        _pathfindingManager ??= PathfindingManager.InstancePy;

        this._moveSpeed = initialSpeed;
    }

    [Title("Methods")]
    public Vector3Int CurrentCoordPy => this._coordPath[this._waypointIndex];
    public Vector3Int PreviousCoordPy => this._coordPath[this._waypointIndex - 1];
    public Vector3Int NextCoordPy => this._coordPath[this._waypointIndex + 1];
    public int DirectionToNextWaypointPy => OffsetUtilities.CalculateDirectionIndex(this.CurrentCoordPy,this.NextCoordPy);
    public Vector3Int DestinationCoordPy => this._coordPath[^1];
    public Vector3Int CoordStoppedAtPy => !this._isMoving ? this._characterEntity.CharacterPy.CoordSystemPy.CurrentCoordPy : this.CurrentCoordPy;

    //停止移动的方法.
    /// <summary>
    /// 功能: 停在下一点的格子中心.
    /// </summary>
    public async UniTask StopAtNextWaypointAndIdleAsync()
    {
        this._isToStopAtNextWaypoint = true;
        await UniTask.WaitUntil(() => !this.IsMovingPy);
    }
    /// <summary>
    /// 功能: 格式化.
    /// </summary>
    public async UniTask FormatAsync()
    {
        //功能: 先停止移动.
        await this.StopAtNextWaypointAndIdleAsync();

        //功能: 重置需要重置的数据.
        this._isToStopAtNextWaypoint = false;
        this._worldPath.Clear();
        this._waypointCount = 0;
        this._waypointIndex = 0;
    }

    //开始移动的方法.
    public void ResetPathCache()
    {
        this._worldPath.Clear();
        foreach (Vector3Int coordWayPoint in this._coordPath)
        {
            this._worldPath.Add(coordWayPoint.ToWorldPos());
        }

        this._waypointCount = Mathf.Clamp(this._coordPath.Count,0,int.MaxValue);
        this._waypointIndex = 0;
    }

    private async UniTask ToMoveAsync()
    {
        if (this._isMoving)
        {
            Debug.LogError("上一次的移动并未结束即开始了新的移动. ");
            return;
        }

        this._isMoving = true;

        // 开始移动前. 
        CalculateAndChangeSpineDirection();
        EntityMoverDetails.WhenBeginToMove(this);

        while (true)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) break;
#endif

            Vector3 position = this._entityTransform.position;
            Vector3 target = this._worldPath[this._waypointIndex];
            Vector3 directionVector = target - position;
            float maxDistanceDelta = this._moveSpeed * Time.deltaTime;
            bool hasDistanceToNextWaypoint = Vector3.Magnitude(directionVector) > maxDistanceDelta;

            //功能: 检测是否移动到了一个路径点.
            if (!hasDistanceToNextWaypoint)
            {
                //功能: 路径点的对应行为分为三种情况: 起始点, 终点, 中途点.
                //功能: 移动到的是起始点.
                if (this._waypointIndex == 0)
                {
                    EntityMoverDetails.WhenJourneyStartReached(this);
                }
                //功能: 移动到的是终点.
                else if (this._waypointIndex == this._waypointCount - 1)
                {
                    EntityMoverDetails.WhenJourneyEndReached(this);
                    // 如果走到中途, 路径终点发生改变. 那么重新寻路.
                    if (this._targetCoord != this.DestinationCoordPy)
                    {
                        BattleConfig.CampRelations campRelations = BattleManager.InstancePy.IsInBattlePy ? this._characterEntity.CharacterPy.CampRelationsPy : null;
                        if (!_pathfindingManager.TryFindPath(this._characterEntity.CharacterPy.CoordSystemPy.CurrentCoordPy,this._targetCoord,this._coordPath,campRelations)) break;
                        this.ResetPathCache();
                        CalculateAndChangeSpineDirection();
                    }
                    else
                    {
                        EntityMoverDetails.WhenDestinationReached(this);
                        // 如果到达的是终点, 那么直接移动其到中心. 
                        this._entityTransform.position =this.DestinationCoordPy.ToWorldPos();
                        this._interactable?.InteractWithCurrentPcEntity();
                        break;
                    }
                }
                //功能: 移动到的是中途点的路径点.
                else
                {
                    EntityMoverDetails.WhenJourneyStartReached(this);
                    EntityMoverDetails.WhenJourneyEndReached(this);
                    // 如果走到中途, 路径终点发生改变. 那么重新寻路.
                    if (this._targetCoord != this.DestinationCoordPy)
                    {
                        BattleConfig.CampRelations campRelations = BattleManager.InstancePy.IsInBattlePy ? this._characterEntity.CharacterPy.CampRelationsPy : null;
                        if (!_pathfindingManager.TryFindPath(this._characterEntity.CharacterPy.CoordSystemPy.CurrentCoordPy,this._targetCoord,this._coordPath,campRelations)) break;
                        this.ResetPathCache();
                        CalculateAndChangeSpineDirection();
                    }
                }

                //功能: 移动到一个路径点就停下来.
                if (this._isToStopAtNextWaypoint) break;

                //功能: 去下一个路径点.
                this._waypointIndex++;
            }

            // 保证移动足够的距离. 
            if (!hasDistanceToNextWaypoint) continue;
            this._entityTransform.position = position + maxDistanceDelta * directionVector.normalized;

            //功能: Async. 
            await UniTask.NextFrame();
        }

        //功能: 重置, 调用停止移动时的事件.
        this._isToStopAtNextWaypoint = false;
        this._isMoving = false;
        EntityMoverDetails.WhenStoppedMove(this);

        void CalculateAndChangeSpineDirection()
        {
            // 调整 spine 左右方向.
            float startX = this._worldPath[0].x;
            float destinationX = this._worldPath[^1].x;
            if (Mathf.Abs(startX - destinationX) > Mathf.Epsilon)
            {
                this._characterEntity.CharacterAnimationSystemPy.SkeletonAnimationPy.skeleton.ScaleX = destinationX < startX ? -1f : 1f;
            }
        }
    }
}
}