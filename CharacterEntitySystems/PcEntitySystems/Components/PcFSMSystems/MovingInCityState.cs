using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.InputSystems;
using LowLevelSystems.InteractableSystems;
using LowLevelSystems.SceneSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public class MovingInCityState : InputState
{
    [ShowInInspector]
    private static CharacterEnum _pcHasDestination;
    [ShowInInspector]
    private static Vector3Int _destination;
    [ShowInInspector]
    private static IInteractable _interactable;

    private static void SetDefaultDestination()
    {
        _pcHasDestination = CharacterEnum.None;
        _destination = Vector3Int.zero;
        _interactable = null;
    }
    private static void SetPcAndDestination(CharacterEnum pcHasDestination,Vector3Int destination,IInteractable interactable = null)
    {
        _pcHasDestination = pcHasDestination;
        _destination = destination;
        _interactable = interactable;
    }

    public static void HandleStarted(InputAction.CallbackContext _)
    {
        _canUpdatePath = true;
        UpdateDestination();
    }
    public static void HandleCanceled(InputAction.CallbackContext _)
    {
        _canUpdatePath = false;

        // Ui 提示. 
        if (_inputManager.IsOnUIPy) return;
        if (_inputManager.MousePosPy.TryFindInteractable(out IInteractable _)) return;
        if (_inputManager.MousePosPy.TryFindPc(out PcEntity _)) return;
        PcEntity currentPcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;
        Vector3Int currentCoord = currentPcEntity.SelfTransformPy.position.ToCoord();
        bool hasPath = _pathfindingManager.TryFindPath(currentCoord,_inputManager.MouseCoordPy);
        if (!hasPath) UiManager.PromptOnMousePosPy.Show(MovingInBattleState.CantMoveToTextId.TextPy);
    }

    private static bool _canUpdatePath;
    private static async UniTask UpdateDestination()
    {
        while (true)
        {
            await UniTask.NextFrame();
            if (!_canUpdatePath) break;
            if (_inputManager.IsOnUIPy)
            {
                SetDefaultDestination();
                continue;
            }
            PcEntity currentPcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;

            // 与 Interactable Objects 交互. 
            if (_inputManager.MousePosPy.TryFindInteractable(out IInteractable interactable))
            {
                if (interactable is NpcEntity npcEntity)
                {
                    npcEntity.NpcAIForLivingPy.StopAIAndFormatAsync();
                    npcEntity.EntityMoverPy.StopAtNextWaypoint();
                }
                Vector3Int targetCoord = interactable.CalculateTargetCoord();
                if (targetCoord != currentPcEntity.CharacterPy.CoordSystemPy.CurrentCoordPy)
                {
                    SetPcAndDestination(currentPcEntity.CharacterPy.CharacterEnumPy,targetCoord,interactable);
                }
                else
                {
                    interactable.InteractWithCurrentPcEntity();
                }

                continue;
            }

            // 普通的移动. 
            //保证目的地可以到达. 
            Vector3Int currentCoord = currentPcEntity.SelfTransformPy.position.ToCoord();
            bool hasPath = _pathfindingManager.TryFindPath(currentCoord,_inputManager.MouseCoordPy);
            if (!hasPath)
            {
                SetDefaultDestination();
                continue;
            }
            SetPcAndDestination(currentPcEntity.CharacterPy.CharacterEnumPy,_inputManager.MouseCoordPy);
        }
    }

    private static readonly List<Vector3Int> _coordPathCache = new List<Vector3Int>(20);
    private static async UniTask MovePcEntityToDestinationAsync()
    {
        while (true)
        {
            await UniTask.NextFrame();
            if (!DetailsOfCityMap.MovingInCityInput.enabled) break;
            CharacterEnum currentCharacterEnumInControl = HeronTeam.CurrentCharacterEnumInControlPy;
            if (currentCharacterEnumInControl == CharacterEnum.None) continue;
            if (currentCharacterEnumInControl != _pcHasDestination) continue;
            PcEntity currentPcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;
            EntityMover currentEntityMover = currentPcEntity.EntityMoverPy;

            // 如果未设置目标点, 停止 Pc 移动.
            if (_destination == Vector3Int.zero)
            {
                currentEntityMover.StopAtNextWaypoint();
                continue;
            }

            // 如果 Pc 未移动, 尝试寻路使其移动. 
            if (!currentEntityMover.IsMovingPy)
            {
                currentEntityMover.SetTargetCoord(_destination,_interactable);
                continue;
            }

            // 如果正在移动, 但目标点就是当前点, 那么 continue.
            if (currentEntityMover.DestinationCoordPy == _destination)
            {
                continue;
            }

            currentEntityMover.SetTargetCoord(_destination,_interactable);
        }
    }

    private static float _timer;
    private static async UniTask ShowInterestCostWithDelayAsync()
    {
        while (true)
        {
            await UniTask.NextFrame();
            if (!DetailsOfCityMap.MovingInCityInput.enabled) break;
            if (HeronTeam.CurrentCharacterEnumInControlPy == CharacterEnum.None) continue;
            Pc currentPcInControl = HeronTeam.CurrentPcInControlPy;
            PcEntity currentPcEntity = currentPcInControl.CharacterIdPy.PcEntityPy;

            //如果 点在 Ui 上, 或者正在移动, 或者 前后坐标不一致 那么就 continue.
            if (_inputManager.IsOnUIPy
             || currentPcEntity.EntityMoverPy.IsMovingPy
             || _inputManager.PreviousMouseCoordPy != _inputManager.MouseCoordPy)
            {
                UiManager.PathDrawerPy.HideCircle();
                _timer = 0f;
                continue;
            }

            //如果一致, 增加 Timer. 
            _timer += Time.deltaTime;
            //Timer 检测 且 寻路 检测再通过. 显示圈圈和消耗.
            if (_timer < SettingsSo.DelayToShowPathCost) continue;
            Vector3Int currentCoord = currentPcEntity.SelfTransformPy.position.ToCoord();
            if (!_pathfindingManager.TryFindPath(currentCoord,_inputManager.MouseCoordPy,_coordPathCache)) continue;
            int neededInterest = (_coordPathCache.Count - 1) * SettingsSo.InterestCostPerMovement;
            bool hasEnoughInterestValue = currentPcInControl.InterestSystemPy.HasEnoughInterestValue(neededInterest);
            UiManager.PathDrawerPy.ShowCircle(neededInterest,hasEnoughInterestValue,_inputManager.MouseCoordPy.ToWorldPos());
        }
    }

    public MovingInCityState(InputFSM.InputStateEnum stateEnum) : base(stateEnum)
    {
    }

    [Title("Methods")]
    public override bool CanEnter()
    {
        return true;
    }
    public override void OnEnter()
    {
        _destination = Vector3Int.zero;
        DetailsOfCityMap.BeginUpdateCursorInCity();
        DetailsOfCityMap.MovingInCityInput.Enable();
        MovePcEntityToDestinationAsync();
        if (SceneHub.CurrentSceneIdPy.ScenePy.SceneTypeEnumPy == SceneTypeEnum.City) ShowInterestCostWithDelayAsync();
    }
    public override bool CanExit()
    {
        if (HeronTeam.CurrentPcInControlPy.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity)
         && pcEntity.EntityMoverPy.IsMovingPy) return false;
        return true;
    }
    public override void OnExit()
    {
        _destination = Vector3Int.zero;
        DetailsOfCityMap.StopUpdateCursorInCity();
        DetailsOfCityMap.MovingInCityInput.Disable();
        _pcHasDestination = CharacterEnum.None;
    }
}
}