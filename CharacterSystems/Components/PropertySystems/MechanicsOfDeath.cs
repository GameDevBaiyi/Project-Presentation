using Cysharp.Threading.Tasks;

using LowLevelSystems.BattleSystems.Base;
using LowLevelSystems.BattleSystems.Components.RoundControllerSystems;
using LowLevelSystems.BattleSystems.Conditions;
using LowLevelSystems.CharacterEntitySystems;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems;
using LowLevelSystems.Common;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.Components.PropertySystems
{
public abstract class MechanicsOfDeath : Details
{
    /// <summary>
    /// 标记为死亡的时机: 当 Hp 被改变时, 如果 Hp 小于等于 0, 即标记为死亡.
    /// 这个时机做什么事? : 如果是 Npc. 停止所有 AI. 停止移动.
    /// 如果是 Pc 且是 当前控制的 Pc, 停止移动. 并且调整输入状态. 
    /// </summary>
    public static async UniTask MarkAsDeadAsync(PropertySystem propertySystem)
    {
        propertySystem.SetIsAlive(false);

        if (!propertySystem.CharacterIdPy.TryGetCharacterEntity(out CharacterEntity characterEntity)) return;
        switch (characterEntity)
        {
            case NpcEntity npcEntity:
                await npcEntity.NpcAIForLivingPy.StopAIAndFormatAsync();
                await npcEntity.EntityMoverPy.FormatAsync();
                break;

            case PcEntity pcEntity:
                if (pcEntity.CharacterPy.CharacterEnumPy != HeronTeam.CurrentCharacterEnumInControlPy) break;
                await pcEntity.EntityMoverPy.FormatAsync();
                PcEntity.InputFSMPy.TransitionTo(InputFSM.InputStateEnum.Normal);
                //Debug.
                if (PcEntity.InputFSMPy.CurrentStateEnumPy != InputFSM.InputStateEnum.Normal)
                {
                    Debug.LogError($"当前控制的角色死亡后, 输入状态未能成功进入 Normal.");
                }
                break;

            default:
                Debug.LogError($"不应出现: {characterEntity.GetType().Name}.");
                break;
        }

        MechanicsOfConditions.UpdateConditionsRelatedToDeath();
    }

    /// <summary>
    /// 影响机制1. 死亡状态对于 PcFsm 输入状态的影响. 不能退出普通状态.
    /// </summary>
    public static bool CheckHasDeadForInputFsm(PropertySystem currentPcsPropertySystem)
    {
        return !currentPcsPropertySystem.IsAlivePy;
    }

    // 死亡动画的机制.
    /// <summary>
    /// 死亡动画的播放时机: 一定是在受击的红光闪烁后, 如果角色是死亡状态, 就做死亡动画. 死亡动画后可能会执行切换回合的操作.
    /// </summary>
    public static async UniTask DoDieAnimeAfterFlashingRedLightAsync(CharacterEntity characterEntity)
    {
        Character character = characterEntity.CharacterPy;
        if (character.PropertySystemPy.IsAlivePy) return;
        await characterEntity.CharacterAnimationSystemPy.DoDieAnimeAsync();
        if (_battleManager.IsInBattlePy
         && !_battleManager.HasConditionsMetPy
         && _battleManager.RoundControllerPy.ActingCharacterEntityPy.InstanceIdPy == character.InstanceIdPy)
        {
            RoundControllerDetails.SwitchRoundAsync();
        }
        await characterEntity.HideAsync();
        BattleDetails.CheckAndSettleBattleAsync();
    }
}
}