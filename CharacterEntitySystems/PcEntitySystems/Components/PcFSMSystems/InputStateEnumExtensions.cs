using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public static class InputStateEnumExtensions
{
    public static InputState InputState(this InputFSM.InputStateEnum inputStateEnum)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return null;
#endif
        if (inputStateEnum == InputFSM.InputStateEnum.None) return null;

        if (!PcEntity.InputFSMPy.InputStateEnum_InputStatePy.TryGetValue(inputStateEnum,out InputState inputState))
        {
            Debug.LogError($"未找到 {nameof(InputFSM.InputStateEnum)} : {inputStateEnum} 对应的 {nameof(PcFSMSystems.InputState)}");
        }

        return inputState;
    }
}
}