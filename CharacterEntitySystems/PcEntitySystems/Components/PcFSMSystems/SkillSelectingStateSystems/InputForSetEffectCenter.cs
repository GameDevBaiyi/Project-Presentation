using Common.InputFrame;

using Cysharp.Threading.Tasks;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
/// <summary>
/// 鼠标左键, 设置 技能释放状态 的技能生效中心,有了中心才能计算具体的 生效范围.
/// 适用于 以鼠标为中心的 可旋转. 用于设置 Center. 设置后, 会打开确定生效范围的 Input.
/// </summary>
public class InputForSettingEffectCenter : Details
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

    public InputForSettingEffectCenter()
    {
        this._input = new KeyDownAction(KeyCode.Mouse0,this.HandleInput);
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

        //如果点在 UI 上, 不管.
        if (_inputManager.IsOnUIPy) return;

        //如果点在了技能范围外面, 无效.
        Vector3Int mouseGridCoord = _inputManager.MouseCoordPy;
        if (!selectingSkillState.SkillUsingRangeSetPy.Contains(mouseGridCoord)) return;

        //设置技能中心 和 默认为 0 的 direction. 计算一次 生效范围.
        selectingSkillState.SetEffectCenter(mouseGridCoord);
        selectingSkillState.SetEffectDirectionIndex(0);
        SelectingSkillStateDetails.RecalculateEffectRange();

        //有了中心后, 关闭本身的 Input.
        this.SetIsReceivingInput(false);

        //避免误触, 下一帧再计算其他的 Input.
        this.ReceiveInputOnNextFrameAsync(selectingSkillState);
    }

    private async UniTask ReceiveInputOnNextFrameAsync(SelectingSkillState selectingSkillState)
    {
        await UniTask.NextFrame();
        //开启一个 Input, 使得 生效范围可旋转.
        selectingSkillState.InputForCalculateRotatableEffectRangePy.SetIsReceivingInput(true);

        //由于有了默认范围, 那么一定可以开启一个 Input, 用于 释放技能 或者 开启下一轮选择.
        selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(true);
    }
}
}