using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Common.Utilities;

using LowLevelSystems.CharacterSystems.Components.CoordSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.Common;
using LowLevelSystems.ItemSystems.EquipmentSystems.Base;
using LowLevelSystems.ItemSystems.EquipmentSystems.WeaponSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.Config;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
public abstract class SelectingSkillStateDetails : Details
{
    public static void EnterNextSelectingRound()
    {
        SelectingSkillState selectingSkillState = InputFSM.InputStateEnum.SelectingSkill.InputState() as SelectingSkillState;
        Pc currentPc = HeronTeam.CurrentPcInControlPy;

        //清理上一轮可能的 Cache.
        // ReSharper disable once PossibleNullReferenceException
        selectingSkillState.SkillUsingRangeSetPy.Clear();
        selectingSkillState.SetEffectCenter(null);
        selectingSkillState.SetEffectDirectionIndex(null);
        selectingSkillState.InputForSettingEffectCenterPy.SetIsReceivingInput(false);
        selectingSkillState.InputForCalculateRotatableEffectRangePy.SetIsReceivingInput(false);
        selectingSkillState.InputForCalculateNonRotatableEffectRangePy.SetIsReceivingInput(false);
        selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(false);

        UiManager.CellUiShowerForSkillUsingRangePy.Hide();
        UiManager.CellUiShowerForSkillEffectRangePy.Hide();

        //计算这是第几轮.
        selectingSkillState.SetSelectingRoundIndex(selectingSkillState.SelectingRoundIndexPy == null ? 0 : selectingSkillState.SelectingRoundIndexPy.Value + 1);

        //计算技能选择范围 UI 并 赋值实际技能范围.
        SelectingSkillStateDetails.RecalculateUsingRange();

        //技能范围显示后, 开始处理技能的 生效范围.
        SkillSugarConfig.SkillEffectConfigForOneRound skillEffectConfigForOneRound = selectingSkillState.SkillEffectConfigForOneRoundPy;
        switch (skillEffectConfigForOneRound.SkillEffectCenterEnumPy)
        {
            case SkillSugarConfig.SkillEffectCenterEnum.UseMouseCoord:
                //以鼠标为中心, 且可旋转.
                if (skillEffectConfigForOneRound.CanRotatePy)
                {
                    //打开输入让玩家自己输入中心.
                    selectingSkillState.InputForSettingEffectCenterPy.SetIsReceivingInput(true);
                }
                //以鼠标为中心, 但不可旋转.
                else
                {
                    //打开输入, 让玩家自己设置中心, 但是不可旋转.
                    selectingSkillState.InputForCalculateNonRotatableEffectRangePy.SetIsReceivingInput(true);
                }
                break;

            case SkillSugarConfig.SkillEffectCenterEnum.UsePcCoord:
                //以角色为中心, 且可旋转.
                // ReSharper disable once PossibleNullReferenceException
                CoordSystem currentPcCoordSystem = currentPc.CoordSystemPy;
                if (skillEffectConfigForOneRound.CanRotatePy)
                {
                    //设置中心.
                    selectingSkillState.SetEffectCenter(currentPcCoordSystem.CurrentCoordPy);
                    //设置一个初始的方向.
                    selectingSkillState.SetEffectDirectionIndex(CubeUtilities.CalculateDirection(OffsetUtilities.OffsetToCube(_inputManager.MouseCoordPy)
                                                                                               - OffsetUtilities.OffsetToCube(selectingSkillState.EffectCenterPy.Value)));
                    //显示一次范围.
                    SelectingSkillStateDetails.RecalculateEffectRange();
                    //打开输入, 允许玩家旋转.
                    selectingSkillState.InputForCalculateRotatableEffectRangePy.SetIsReceivingInput(true);
                    //此情况一定有范围, 可以接收 释放技能 Input.
                    selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(true);
                }
                //以角色为中心, 但不可旋转.
                else
                {
                    //设置中心.
                    selectingSkillState.SetEffectCenter(currentPcCoordSystem.CurrentCoordPy);
                    //设置一个初始的方向.
                    selectingSkillState.SetEffectDirectionIndex(currentPcCoordSystem.DirectionIndexPy);
                    //显示一次范围.
                    SelectingSkillStateDetails.RecalculateEffectRange();
                    //此情况一定有范围, 可以接收 释放技能 Input.
                    selectingSkillState.InputForBeginNextSelectingRoundOrUseSkillPy.SetIsReceivingInput(true);
                }
                break;

            default:
                Debug.LogError($"不应该出现: {skillEffectConfigForOneRound.SkillEffectCenterEnumPy}");
                break;
        }

        //此时可以 接收 Input: 用于取消技能.
        selectingSkillState.InputForCancelSkillPy.SetIsReceivingInput(true);
    }

    /// <summary>
    /// 根据 设置好的 Center 和 Direction 计算 生效范围, 设置 并 显示 UI.
    /// </summary>
    [SuppressMessage("ReSharper","PossibleInvalidOperationException")]
    public static void RecalculateEffectRange()
    {
        SelectingSkillState selectingSkillState = InputFSM.InputStateEnum.SelectingSkill.InputState() as SelectingSkillState;

        // 每一轮技能技能选择都有对应的配置, 先确定当前轮次的对应配置.
        SkillSugarConfig.SkillEffectConfigForOneRound skillEffectConfigForOneRound = selectingSkillState.SkillEffectConfigForOneRoundPy;

        // 显示 UI.
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        List<Vector3Int> effectRangeInCube;
        if (skillEffectConfigForOneRound.IsUsingWeaponAsEffectRangePy)
        {
            Weapon weapon = currentPc.EquipmentInventoryPy.WeaponPy;
            if (weapon == null)
            {
                Debug.LogError("玩家未装备武器, 不能使用该技能.");
                effectRangeInCube = SkillRangeEnum.R1_1.RangeInCubeCoord();
            }
            else
            {
                effectRangeInCube = weapon.ItemConfigIdAndQualityEnumPy.GetItemConfig<WeaponConfig>().EquipmentSubTypeEnumPy.EquipmentSubTypeConfig().SkillRangeEnumPy.RangeInCubeCoord();
            }
        }
        else
        {
            effectRangeInCube = skillEffectConfigForOneRound.EffectRangeEnumPy.RangeInCubeCoord();
        }

        Vector3Int effectCenter = selectingSkillState.EffectCenterPy.Value;
        int directionIndex = selectingSkillState.EffectDirectionIndexPy.Value;
        List<Vector3Int> skillEffectRange = OffsetUtilities.Convert0DirectionRelativeCubesToOffset(effectRangeInCube,effectCenter,directionIndex);
        UiManager.CellUiShowerForSkillEffectRangePy.Show(skillEffectRange);

        //然后 实际生效范围 到 技能选择状态. 
        List<List<Vector3Int>> effectRangeList = selectingSkillState.EffectRangeListPy;
        int countOfEffectRangeList = effectRangeList.Count;
        int selectingRoundIndex = selectingSkillState.SelectingRoundIndexPy.Value;
        if (countOfEffectRangeList > selectingRoundIndex)
        {
            effectRangeList[selectingRoundIndex] = skillEffectRange;
        }
        else
        {
            effectRangeList.Add(skillEffectRange);
        }
    }

    /// <summary>
    /// 显示技能范围 UI, 并设置 实际的技能范围 给 技能选择状态.
    /// </summary>
    public static void RecalculateUsingRange()
    {
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        if (currentPc == null)
        {
            Debug.LogError($"显示技能范围时, 发现当前控制的 Pc 为 null, 不应该出现此种情况.");
            return;
        }
        SelectingSkillState selectingSkillState = InputFSM.InputStateEnum.SelectingSkill.InputState() as SelectingSkillState;

        //处理技能范围.
        SkillSugarConfig.SkillEffectConfigForOneRound effectConfigForOneRound = selectingSkillState.SkillEffectConfigForOneRoundPy;
        SkillRangeEnum usingRangeEnum = effectConfigForOneRound.UsingRangeEnumPy;

        List<Vector3Int> usingRangeInCube;
        if (effectConfigForOneRound.IsUsingWeaponAsUsingRangePy)
        {
            Weapon weapon = currentPc.EquipmentInventoryPy.WeaponPy;
            if (weapon == null)
            {
                Debug.LogError("玩家未装备武器, 不能使用该技能.");
                usingRangeInCube = SkillRangeEnum.R1_1.RangeInCubeCoord();
            }
            else
            {
                usingRangeInCube = weapon.ItemConfigIdAndQualityEnumPy.GetItemConfig<WeaponConfig>().EquipmentSubTypeEnumPy.EquipmentSubTypeConfig().SkillRangeEnumPy.RangeInCubeCoord();
            }
        }
        else
        {
            usingRangeInCube = usingRangeEnum.RangeInCubeCoord();
        }

        // 显示技能范围 UI.
        CoordSystem currentPcCoordSystem = currentPc.CoordSystemPy;
        List<Vector3Int> skillSelectingRange
            = OffsetUtilities.Convert0DirectionRelativeCubesToOffset(usingRangeInCube,currentPcCoordSystem.CurrentCoordPy,currentPcCoordSystem.DirectionIndexPy);
        UiManager.CellUiShowerForSkillUsingRangePy.Show(skillSelectingRange);

        // 计算实际范围 并 设置.
        foreach (Vector3Int vector3Int in skillSelectingRange)
        {
            selectingSkillState.SkillUsingRangeSetPy.Add(vector3Int);
        }
    }
}
}