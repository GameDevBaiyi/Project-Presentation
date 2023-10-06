using LowLevelSystems.Common;

using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public abstract class InputState : Details
{
    [ShowInInspector]
    protected InputFSM.InputStateEnum _inputStateEnum;
    public InputFSM.InputStateEnum InputStateEnumPy => this._inputStateEnum;

    [ShowInInspector]
    protected bool _isInState;
    public bool IsInStatePy => this._isInState;
    public void SetIsInState(bool isInState)
    {
        this._isInState = isInState;
    }

    public abstract bool CanEnter();
    public abstract void OnEnter();

    public abstract bool CanExit();
    public abstract void OnExit();

    protected InputState(InputFSM.InputStateEnum inputStateEnum)
    {
        this._inputStateEnum = inputStateEnum;
    }
}
}