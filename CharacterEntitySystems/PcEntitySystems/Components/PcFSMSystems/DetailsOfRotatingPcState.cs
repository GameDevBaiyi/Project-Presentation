using LowLevelSystems.CharacterSystems.Components.PropertySystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public abstract class DetailsOfRotatingPcState : Details
{
    // 尝试主动进入该状态
    public static void CheckAndEnterRotatingPcState()
    {
        PropertySystem propertySystem = HeronTeam.CurrentPcInControlPy.PropertySystemPy;
        int apCostForPcRotation = SettingsSo.ApCostForPcRotation;
        if (propertySystem.CurrentApPy < apCostForPcRotation) return;
        if (PcEntity.InputFSMPy.CurrentStateEnumPy == InputFSM.InputStateEnum.RotatingPc) return;
        PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.RotatingPc);
        if (PcEntity.InputFSMPy.CurrentStateEnumPy == InputFSM.InputStateEnum.RotatingPc)
        {
            float apCost = Mathf.Min(propertySystem[PropertyEnum.MaxAp],apCostForPcRotation);
            propertySystem.ChangeAp(-apCost);
        }
    }
}
}