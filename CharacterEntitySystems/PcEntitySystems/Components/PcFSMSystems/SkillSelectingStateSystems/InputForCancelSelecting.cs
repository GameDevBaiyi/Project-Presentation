using Common.InputFrame;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
public class InputForCancelSkill
{
    [Title("Data")]
    [ShowInInspector]
    private KeyDownAction _input;
    public KeyDownAction InputPy => this._input;
    public void SetInput(KeyDownAction input)
    {
        this._input = input;
    }

    [ShowInInspector]
    private bool _isReceivingInput;
    public bool IsReceivingInputPy => this._isReceivingInput;
    public void SetIsReceivingInput(bool isReceivingInput)
    {
        this._isReceivingInput = isReceivingInput;
    }

    public InputForCancelSkill()
    {
        this._input = new KeyDownAction(KeyCode.Mouse1,this.HandleInput);
    }

    [Title("Methods")]
    private void HandleInput()
    {
        if (!this._isReceivingInput) return;

        //做一个检测, 如果该行为, 不在技能选择状态, 就报错. 
        SelectingSkillState selectingSkillState = InputFSM.InputStateEnum.SelectingSkill.InputState() as SelectingSkillState;
        if (!selectingSkillState.IsInStatePy)
        {
            Debug.LogError($"未在技能输入状态.");
            return;
        }

        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.Normal);
        //再尝试自动进入移动, 简化操作. 
        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.MovingInBattle);
    }
}
}