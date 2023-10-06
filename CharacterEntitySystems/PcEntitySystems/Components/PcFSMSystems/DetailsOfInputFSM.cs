using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterSystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems
{
public abstract class DetailsOfInputFSM : Details
{
    public static void ChangeInputStateByCurrentContext()
    {
        InputFSM inputFSM = PcEntity.InputFSMPy;

        // 战斗中. 
        if (_battleManager.IsInBattlePy)
        {
            bool isSuccessful = inputFSM.TransitionTo(InputFSM.InputStateEnum.Normal);

            //Debug.
            if (!isSuccessful)
            {
                Debug.LogError($"未能成功进入该输入状态:  {InputFSM.InputStateEnum.Normal}");
            }

            return;
        }
        bool isSuccessful1 = inputFSM.TransitionTo(InputFSM.InputStateEnum.MovingInCity);

        //Debug.
        if (!isSuccessful1)
        {
            Debug.LogError($"未能成功进入该输入状态:  {InputFSM.InputStateEnum.MovingInCity}");
        }
    }

    public static async UniTask TransitionToNormalAsync()
    {
        if (HeronTeam.CurrentCharacterEnumInControlPy == CharacterEnum.None) return;
        if (HeronTeam.CurrentPcInControlPy.CharacterIdPy.TryGetPcEntity(out PcEntity pcEntity))
        {
            await pcEntity.EntityMoverPy.StopAtNextWaypointAndIdleAsync();
        }

        if (PcEntity.InputFSMPy != null
         && !PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.Normal))
        {
            Debug.LogError($"进入 Normal Input 状态失败. ");
        }
    }
}
}