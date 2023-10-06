using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public class InputFSM
{
    public enum InputStateEnum
    {
        None,
        Normal,
        MovingInCity,
        MovingInBattle,
        SelectingSkill,
        RotatingPc,
    }

    [Title("Data")]
    [ShowInInspector]
    private Dictionary<InputStateEnum,InputState> _inputStateEnum_inputState;
    public Dictionary<InputStateEnum,InputState> InputStateEnum_InputStatePy => this._inputStateEnum_inputState;

    [ShowInInspector]
    private InputStateEnum _currentStateEnum;
    public InputStateEnum CurrentStateEnumPy => this._currentStateEnum;

    public InputFSM()
    {
        this._inputStateEnum_inputState = new Dictionary<InputStateEnum,InputState>(10);

        NormalState normalState = new NormalState(InputStateEnum.Normal);
        this._inputStateEnum_inputState[normalState.InputStateEnumPy] = normalState;

        MovingInCityState movingInCityState = new MovingInCityState(InputStateEnum.MovingInCity);
        this._inputStateEnum_inputState[movingInCityState.InputStateEnumPy] = movingInCityState;

        MovingInBattleState movingInBattleState = new MovingInBattleState(InputStateEnum.MovingInBattle);
        this._inputStateEnum_inputState[movingInBattleState.InputStateEnumPy] = movingInBattleState;

        SelectingSkillState selectingSkillState = new SelectingSkillState(InputStateEnum.SelectingSkill);
        this._inputStateEnum_inputState[selectingSkillState.InputStateEnumPy] = selectingSkillState;

        RotatingPcState rotatingPcState = new RotatingPcState(InputStateEnum.RotatingPc);
        this._inputStateEnum_inputState[rotatingPcState.InputStateEnumPy] = rotatingPcState;

        this._currentStateEnum = InputStateEnum.Normal;
        normalState.SetIsInState(true);
    }

    [Title("Methods")]
    [ShowInInspector]
    public InputState CurrentInputStatePy
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return null;
#endif
            if (this._currentStateEnum == InputStateEnum.None) return null;

            if (!this._inputStateEnum_inputState.TryGetValue(this._currentStateEnum,out InputState inputState))
            {
                Debug.LogError($"未找到 {nameof(InputStateEnum)} : {this._currentStateEnum} 对应的 {nameof(InputState)}");
            }

            return inputState;
        }
    }

    public bool TransitionTo(InputStateEnum inputStateEnum,Action doBeforeEnterDg = null)
    {
        InputState previousState = this.CurrentInputStatePy;
        if (!previousState.CanExit()) return false;
        previousState.OnExit();
        previousState.SetIsInState(false);

        doBeforeEnterDg?.Invoke();

        InputState targetState = this._inputStateEnum_inputState[inputStateEnum];
        if (!targetState.CanEnter())
        {
            this._currentStateEnum = InputStateEnum.Normal;
            this.CurrentInputStatePy.SetIsInState(true);
            this.CurrentInputStatePy.OnEnter();
            return false;
        }

        this._currentStateEnum = inputStateEnum;
        this.CurrentInputStatePy.SetIsInState(true);
        this.CurrentInputStatePy.OnEnter();
        return true;
    }
}
}