using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.InputSystems;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.MechanicsAndFormulas;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Sirenix.OdinInspector;

using UnityEngine;

using InputAction = UnityEngine.InputSystem.InputAction;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public class MovingInBattleState : InputState
{
    public static readonly TextId CantMoveToTextId = new TextId(1000016);      // 无法到达
    private static readonly TextId _hasNoEnoughApTextId = new TextId(1000008); // 气络不足

    public static void HandleInput(InputAction.CallbackContext _)
    {
        if (_inputManager.IsOnUIPy) return;
        Pc currentPcInControl = HeronTeam.CurrentPcInControlPy;
        PcEntity currentPcEntity = currentPcInControl.CharacterIdPy.PcEntityPy;
        EntityMover currentEntityMover = currentPcEntity.EntityMoverPy;
        //此种移动时不能再选择路径, 不接受输入.
        if (currentEntityMover.IsMovingPy) return;
        //镜头切换时, 不接受输入.
        if (_cameraManager.IsTransitioningPy) return;
        if (currentPcEntity.CharacterAnimationSystemPy.IsAttackingPy) return;

        Vector3Int currentCoord = currentPcEntity.SelfTransformPy.position.ToCoord();
        Vector3Int currentMouseCoord = _inputManager.MouseCoordPy;

        WaitCameraAndMoveAsync();

        async UniTask WaitCameraAndMoveAsync()
        {
            float speed = currentPcInControl.PropertySystemPy[PropertyEnum.Speed];
            //角色可以在该场景移动, 肯定有 Prefab.
            Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
            if (currentScene is not IHasScenePrefab hasScenePrefab)
            {
                Debug.LogError($"当前 Scene 没有 Prefab. SceneId: {currentScene.InstanceIdPy}");
                return;
            }
            TerrainStaticCell[][] terrainStaticGrid = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig().TerrainStaticGridPy;
            Dictionary<TileEnum,TileConfig> tileEnum_tileConfigRow = CommonDesignSO.TerrainConfigPy.TileEnum_TileConfigPy;

            //功能: 计算路径并更新 Cache, 但不移动.
            if (!_pathfindingManager.TryFindPath(currentCoord,currentMouseCoord,currentEntityMover.CoordPathPy,currentPcInControl.CampRelationsPy))
            {
                UiManager.PromptOnMousePosPy.Show($"{CantMoveToTextId.TextPy}");
                return;
            }

            currentEntityMover.ResetPathCache();
            //功能: 战斗时, 如果点击了某一点, 计算了路径, 会在 Ap UI 上显示消耗多少.
            float neededAp = MovementMechanics.CalculateNeededAp(speed,currentEntityMover.CoordPathPy,terrainStaticGrid,tileEnum_tileConfigRow);
            if (currentPcInControl.PropertySystemPy.CurrentApPy < neededAp)
            {
                UiManager.PromptOnMousePosPy.Show($"{_hasNoEnoughApTextId.TextPy}");
                return;
            }

            await _cameraManager.ChangeTargetAsync(currentPcEntity.SelfTransformPy);
            currentEntityMover.SetTargetCoord(currentMouseCoord);
        }
    }

    private static float _timer;
    private static async UniTask ShowInterestCostWithDelayAsync()
    {
        Scene currentScene = SceneHub.CurrentSceneIdPy.ScenePy;
        if (currentScene is not IHasScenePrefab hasScenePrefab)
        {
            Debug.LogError($"当前 Scene 没有 Prefab. SceneId: {currentScene.InstanceIdPy}");
            return;
        }
        TerrainStaticCell[][] terrainStaticGrid = hasScenePrefab.ScenePrefabEnumPy.ScenePrefabConfig().TerrainStaticGridPy;
        Dictionary<TileEnum,TileConfig> tileEnum_tileConfigRow = CommonDesignSO.TerrainConfigPy.TileEnum_TileConfigPy;

        while (true)
        {
            await UniTask.NextFrame();
            if (!DetailsOfBattleMap.MovingInBattleInput.enabled) break;
            Pc currentPcInControl = HeronTeam.CurrentPcInControlPy;
            PcEntity currentPcEntity = currentPcInControl.CharacterIdPy.PcEntityPy;
            EntityMover currentEntityMover = currentPcEntity.EntityMoverPy;
            float speed = currentPcInControl.PropertySystemPy[PropertyEnum.Speed];

            //如果正在移动, 那么就 continue.
            if (currentPcEntity.EntityMoverPy.IsMovingPy)
            {
                HideCircle();
                _timer = 0f;
                continue;
            }

            if (_inputManager.IsOnUIPy)
            {
                HideCircle();
                _timer = 0f;
                continue;
            }

            Vector3Int currentCoord = currentPcEntity.SelfTransformPy.position.ToCoord();
            if (!_pathfindingManager.TryFindPath(currentCoord,_inputManager.MouseCoordPy,currentEntityMover.CoordPathPy,currentPcInControl.CampRelationsPy))
            {
                HideCircle();
                _timer = 0f;
                continue;
            }

            currentEntityMover.ResetPathCache();
            float neededAp = MovementMechanics.CalculateNeededAp(speed,currentEntityMover.CoordPathPy,terrainStaticGrid,tileEnum_tileConfigRow);
            bool hasEnoughAp = currentPcInControl.PropertySystemPy.CurrentApPy >= neededAp;
            int costAp = MovementMechanics.CalculateCostAp(speed,currentEntityMover.CoordPathPy,terrainStaticGrid,tileEnum_tileConfigRow);
            ApCostCalculated?.Invoke(costAp / 100);
            UiManager.PathDrawerPy.ShowCircle(costAp / 100,hasEnoughAp,_inputManager.MouseCoordPy.ToWorldPos());

            //如果 前后坐标不一致, 就隐藏路径. 并清空 Timer 记录. 并 continue.
            if (_inputManager.PreviousMouseCoordPy != _inputManager.MouseCoordPy)
            {
                UiManager.PathDrawerPy.HideLine();
                _timer = 0f;
                continue;
            }

            //如果一致, 增加 Timer. 
            _timer += Time.deltaTime;
            //Timer 检测 且 寻路 检测再通过. 显示圈圈和消耗.
            if (_timer < SettingsSo.DelayToShowPathCost) continue;
            UiManager.PathDrawerPy.ShowLine(currentEntityMover.WorldPathPy);
            _timer = 0f;
        }
    }
    private static void HideCircle()
    {
        UiManager.PathDrawerPy.HideCircle();
        CircleHided?.Invoke();
    }

    public MovingInBattleState(InputFSM.InputStateEnum stateEnum) : base(stateEnum)
    {
    }

    [Title("Events")]
    public static event Action<int> ApCostCalculated;
    public static event Action CircleHided;
    public static event Action<bool> StateChanged;

    [Title("Methods")]
    public override bool CanEnter()
    {
        //非战斗状态, 不能进入该状态.
        if (!_battleManager.IsInBattlePy)
        {
            Debug.LogError("不在战斗中, 依然在尝试进入 战斗移动控制状态.");
            return false;
        }
        //恢复 Ap 阶段, 不能进入该状态.
        if (_battleManager.APRecoveryControllerPy.IsRecoveringApPy) return false;
        //不是该玩家的回合, 不能进入该状态.
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        if (_battleManager.RoundControllerPy.ActingCharacterEntityPy.InstanceIdPy != currentPc.InstanceIdPy) return false;
        //如果角色被控制了, 不能进入该状态.
        if (MechanicsOfControlEffect.HasControlTypeOfCantMove(currentPc.CharacterIdPy.CharacterEntityPy.BuffPoolPy)) return false;

        return true;
    }
    public override void OnEnter()
    {
        //显示可移动范围.
        DetailsOfBattleMap.MovingInBattleInput.Enable();
        ShowInterestCostWithDelayAsync();
        MovingInBattleState.StateChanged?.Invoke(true);
    }
    public override bool CanExit()
    {
        //如果是在镜头移动的阶段, 也不能退出该状态.
        if (_cameraManager.IsTransitioningPy) return false;
        //如果当前控制的角色正在移动, 那么不能退出该状态.
        if (HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy.EntityMoverPy.IsMovingPy) return false;

        return true;
    }
    public override void OnExit()
    {
        DetailsOfBattleMap.MovingInBattleInput.Disable();

        //隐藏相关 UI.
        UiManager.PathDrawerPy.HidePath();
        PcEntity pcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;
        pcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(false);

        MovingInBattleState.StateChanged?.Invoke(false);
    }
}
}