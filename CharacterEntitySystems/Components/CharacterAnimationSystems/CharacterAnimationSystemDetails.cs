using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.EquipmentSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;

using Spine;

using UnityEngine;

using Animation = Spine.Animation;

namespace LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems
{
    public abstract class CharacterAnimationSystemDetails : Details
    {
        public static Animation GetIdleAnimation(CharacterAnimationSystem characterAnimationSystem)
        {
            //1. 先看是否是战斗中, 确定前缀.
            CharacterEntity characterEntity = characterAnimationSystem.CharacterEntityPy;
            string idleAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.IdleInBattle : CharacterAnimationSystem.Idle;
            if (_battleManager.IsInBattlePy)
            {
                return characterEntity.SpineControllerPy.GetBattleAni(idleAnimeString);
            }
            else
            {
                Animation animation = characterAnimationSystem.SkeletonAnimationPy.AnimationState.Data.SkeletonData.FindAnimation(idleAnimeString);
                if (animation == null)
                {
                    Debug.LogError("找不到角色" + characterEntity.CharacterPy.CharacterEnumPy + "的idle动画");
                }
                return animation;
            }

            ////2. 如果是 Pc, 根据武器加一个后缀.
            //if (_battleManager.IsInBattlePy && characterEntity is PcEntity pcEntity)
            //{
            //    Weapon weapon = pcEntity.PcPy.EquipmentInventoryPy.WeaponPy;
            //    if (weapon != null)
            //    {
            //        EquipmentSubTypeConfig equipmentSubTypeConfig = weapon.ItemConfigIdAndQualityEnumPy.GetItemConfig<WeaponConfig>().EquipmentSubTypeEnumPy.EquipmentSubTypeConfig();
            //        string skillSubTypeEnumString = equipmentSubTypeConfig.SkillSubTypeEnumPy.ToString();
            //        idleAnimeString = idleAnimeString + "_" + skillSubTypeEnumString;
            //    }
            //}

            //SkeletonData skeletonData = characterAnimationSystem.SkeletonAnimationPy.AnimationState.Data.SkeletonData;
            //Animation idleAnimation = skeletonData.FindAnimation(idleAnimeString);
            //if (idleAnimation == null)
            //{
            //    Debug.LogError($"未找到该角色: {characterEntity.CharacterPy.CharacterEnumPy} 的该动画: {idleAnimeString}, 使用 {CharacterAnimationSystem.Idle} 替代.");
            //    idleAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.IdleInBattle : CharacterAnimationSystem.Idle;
            //    idleAnimation = skeletonData.FindAnimation(idleAnimeString);
            //}

            //if (idleAnimation == null)
            //{
            //    Debug.LogError($"{idleAnimeString} 也未找到.");
            //}

            //return idleAnimation;
        }

        public static Animation GetRunAnimation(CharacterAnimationSystem characterAnimationSystem)
        {
            CharacterEntity characterEntity = characterAnimationSystem.CharacterEntityPy;
            SkeletonData skeletonData = characterAnimationSystem.SkeletonAnimationPy.AnimationState.Data.SkeletonData;
            string runAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.RunInBattle : CharacterAnimationSystem.Run;
            if (_battleManager.IsInBattlePy)
            {
                return characterEntity.SpineControllerPy.GetBattleAni(runAnimeString);
            }
            else
            {
                if (characterEntity is NpcEntity npcEntity)
                {
                    runAnimeString = CharacterAnimationSystem.Walk;
                }
                Animation animation = characterAnimationSystem.SkeletonAnimationPy.AnimationState.Data.SkeletonData.FindAnimation(runAnimeString);
                if (animation == null)
                {
                    Debug.LogError("找不到角色" + characterEntity.CharacterPy.CharacterEnumPy + "的动画:" + runAnimeString);
                }
                return animation;
            }


            ////如果是 Pc. 
            //if (characterEntity is PcEntity pcEntity)
            //{
            //    //1. 先看是否是战斗中, 确定前缀.
            //    string runAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.RunInBattle : CharacterAnimationSystem.Run;
            //    //2. 如果是 Pc, 根据武器加一个后缀.
            //    Weapon weapon = pcEntity.PcPy.EquipmentInventoryPy.WeaponPy;
            //    if (_battleManager.IsInBattlePy
            //     && weapon != null)
            //    {
            //        EquipmentSubTypeConfig equipmentSubTypeConfig = weapon.ItemConfigIdAndQualityEnumPy.GetItemConfig<WeaponConfig>().EquipmentSubTypeEnumPy.EquipmentSubTypeConfig();
            //        string skillSubTypeEnumString = equipmentSubTypeConfig.SkillSubTypeEnumPy.ToString();
            //        runAnimeString = runAnimeString + "_" + skillSubTypeEnumString;
            //    }

            //    Animation runAnimation = skeletonData.FindAnimation(runAnimeString);
            //    if (runAnimation == null)
            //    {
            //        Debug.LogError($"未找到该角色: {characterEntity.CharacterPy.CharacterEnumPy} 的该动画: {runAnimeString}, 使用 {CharacterAnimationSystem.Run} 替代.");
            //        runAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.RunInBattle : CharacterAnimationSystem.Run;
            //        runAnimation = skeletonData.FindAnimation(runAnimeString);
            //    }
            //    if (runAnimation == null)
            //    {
            //        Debug.LogError($"该角色: {characterEntity.CharacterPy.CharacterEnumPy} 的该动画: {runAnimeString} 未找到.");
            //    }

            //    return runAnimation;
            //}
            ////如果是 Npc.
            //else
            //{
            //    string runAnimeString = _battleManager.IsInBattlePy ? CharacterAnimationSystem.RunInBattle : CharacterAnimationSystem.Walk;
            //    Animation runAnimation = skeletonData.FindAnimation(runAnimeString);
            //    if (runAnimation == null)
            //    {
            //        Debug.LogError($"该角色: {characterEntity.CharacterPy.CharacterEnumPy} 的该动画: {runAnimeString} 未找到.");
            //    }

            //    return runAnimation;
        }
    }
}
