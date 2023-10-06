using Common.Extensions;

using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.InputSystems;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.InputSystem;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public class RotatingPcState : InputState
{
    public static void HandleInput(InputAction.CallbackContext _)
    {
        //计算方向.
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        PcEntity pcEntity = currentPc.CharacterIdPy.PcEntityPy;
        float angle = Vector2.SignedAngle(Vector2.right,_inputManager.MouseWorldPosWithZ0Py - pcEntity.SelfTransformPy.position);
        if (angle < 0f) angle += 360f;
        int direction = GridUtilities.AngleRangeOfDirections.FindIndex(t => angle.IsInRange(t.x,t.y,ExclusiveFlags.None));
        if (direction == -1)
        {
            Debug.LogError($"未找到该 angle 的区间: {angle}");
            direction = 0;
        }
        pcEntity.ChangeDirection(direction);
    }

    public static void InputForConfirmDirection(InputAction.CallbackContext _)
    {
        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.MovingInBattle);
    }

    public RotatingPcState(InputFSM.InputStateEnum inputStateEnum) : base(inputStateEnum)
    {
    }

    [Title("Methods")]
    public override bool CanEnter()
    {
        return true;
    }
    public override void OnEnter()
    {
        PcEntity pcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;
        //显示方向 Ui. 
        pcEntity.CharacterPanelControllerPy.RefreshDirection();
        pcEntity.CharacterPanelControllerPy.ChangeDirectionUIVisible(true);
        pcEntity.CharacterPanelControllerPy.PlayTransition();
        //打开控制. 
        DetailsOfBattleMap.RotatingPcInput.Enable();
        DetailsOfBattleMap.ConfirmDirectionInput.Enable();
    }
    public override bool CanExit()
    {
        return true;
    }
    public override void OnExit()
    {
        PcEntity pcEntity = HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy;
        pcEntity.CharacterPanelControllerPy.RefreshCloseTransition();
        //关闭控制.
        DetailsOfBattleMap.RotatingPcInput.Disable();
        DetailsOfBattleMap.ConfirmDirectionInput.Disable();
    }
}
}