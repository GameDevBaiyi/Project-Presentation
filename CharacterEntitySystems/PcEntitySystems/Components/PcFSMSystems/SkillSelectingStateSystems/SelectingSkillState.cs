using System;
using System.Collections.Generic;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems;
using LowLevelSystems.CharacterSystems.PcSystems;
using LowLevelSystems.CharacterSystems.PcSystems.Components.LuggageSystems;
using LowLevelSystems.SkillSystems.Config;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems.SkillSelectingStateSystems
{
public class SelectingSkillState : InputState
{
    [Title("技能栏相关引用")]
    [ShowInInspector]
    public ModuleOfSkillBar SelectedModuleOfSkillBar;
    [ShowInInspector]
    public CellOfSkillBar SelectedCellOfSkillBar;
    [ShowInInspector]
    public bool IsUsingSugarPy => this.SelectedCellOfSkillBar != null;
    public void SetSelectedModuleAndCell(ModuleOfSkillBar selectedModule,CellOfSkillBar selectedCell)
    {
        this.SelectedModuleOfSkillBar = selectedModule;
        this.SelectedCellOfSkillBar = selectedCell;
    }

    [Title("行囊道具相关引用")]
    [ShowInInspector]
    public CellOfLuggage SelectedCellOfLuggage;
    public bool IsUsingItemPy => this.SelectedCellOfLuggage != null;
    public void SetSelectedCellOfLuggage(CellOfLuggage selectedCellOfLuggage)
    {
        this.SelectedCellOfLuggage = selectedCellOfLuggage;
    }

    [Title("Data")]
    [ShowInInspector]
    private HashSet<Vector3Int> _skillUsingRangeSet;
    public HashSet<Vector3Int> SkillUsingRangeSetPy => this._skillUsingRangeSet;

    [Title("技能生效范围相关")]
    [ShowInInspector]
    private Vector3Int? _effectCenter;
    public Vector3Int? EffectCenterPy => this._effectCenter;
    public void SetEffectCenter(Vector3Int? effectCenter)
    {
        this._effectCenter = effectCenter;
    }

    [ShowInInspector]
    private int? _effectDirectionIndex;
    public int? EffectDirectionIndexPy => this._effectDirectionIndex;
    public void SetEffectDirectionIndex(int? effectDirectionIndex)
    {
        this._effectDirectionIndex = effectDirectionIndex;
    }

    [Title("技能选择的轮次 和 每轮选中 Data")]
    [ShowInInspector]
    private int? _selectingRoundIndex;
    public int? SelectingRoundIndexPy => this._selectingRoundIndex;
    public void SetSelectingRoundIndex(int? selectingRoundIndex)
    {
        this._selectingRoundIndex = selectingRoundIndex;
    }

    [ShowInInspector]
    private readonly List<List<Vector3Int>> _effectRangeList;
    public List<List<Vector3Int>> EffectRangeListPy => this._effectRangeList;

    [Title("输入相关的组件")]
    [ShowInInspector]
    private readonly InputForSettingEffectCenter _inputForSettingEffectCenter;
    public InputForSettingEffectCenter InputForSettingEffectCenterPy => this._inputForSettingEffectCenter;

    [ShowInInspector]
    private readonly InputForCalculateRotatableEffectRange _inputForCalculateRotatableEffectRange;
    public InputForCalculateRotatableEffectRange InputForCalculateRotatableEffectRangePy => this._inputForCalculateRotatableEffectRange;

    [ShowInInspector]
    private readonly InputForCalculateNonRotatableEffectRange _inputForCalculateNonRotatableEffectRange;
    public InputForCalculateNonRotatableEffectRange InputForCalculateNonRotatableEffectRangePy => this._inputForCalculateNonRotatableEffectRange;

    [ShowInInspector]
    private readonly InputForBeginNextSelectingRoundOrUseSkill _inputForBeginNextSelectingRoundOrUseSkill;
    public InputForBeginNextSelectingRoundOrUseSkill InputForBeginNextSelectingRoundOrUseSkillPy => this._inputForBeginNextSelectingRoundOrUseSkill;

    [ShowInInspector]
    private readonly InputForCancelSkill _inputForCancelSkill;
    public InputForCancelSkill InputForCancelSkillPy => this._inputForCancelSkill;

    public SelectingSkillState(InputFSM.InputStateEnum stateEnum) : base(stateEnum)
    {
        this._skillUsingRangeSet = new HashSet<Vector3Int>(10);

        this._effectRangeList = new List<List<Vector3Int>>(5);

        this._inputForSettingEffectCenter = new InputForSettingEffectCenter();
        this._inputForCalculateRotatableEffectRange = new InputForCalculateRotatableEffectRange();
        this._inputForCalculateNonRotatableEffectRange = new InputForCalculateNonRotatableEffectRange();
        this._inputForBeginNextSelectingRoundOrUseSkill = new InputForBeginNextSelectingRoundOrUseSkill();
        this._inputForCancelSkill = new InputForCancelSkill();
    }

    [Title("Events")]
    [ShowInInspector]
    public static event Action<int> OnCalculateApCost;
    [ShowInInspector]
    public static event Action<bool> OnStateChanged;

    [Title("Data")]
    public SkillSugarConfig SkillSugarConfigPy => this.IsUsingSugarPy
                                                      ? this.SelectedCellOfSkillBar.SkillSugarPy.SkillMainIdAndQualityEnumPy.SkillSugarConfigPy
                                                      : this.SelectedCellOfLuggage.ItemPileInLuggagePy.ItemHasSkillSugarConfigPy.SkillSugarConfigPy;
    public SkillSugarConfig.SkillEffectConfigForOneRound SkillEffectConfigForOneRoundPy => this.SkillSugarConfigPy.SkillEffectConfigPy[this._selectingRoundIndex.Value];

    public override bool CanEnter()
    {
        //如果必要的参数没设置, 无法进入该状态.
        if (this.SelectedCellOfSkillBar == null
         && this.SelectedCellOfLuggage == null)
        {
            Debug.LogError($"未设置 技能 和 道具, 但仍试图进入技能释放状态. ");
            return false;
        }
        //如果所需 Ap 不足. 无法进入该状态.
        Pc currentPc = HeronTeam.CurrentPcInControlPy;
        float currentAp = currentPc.PropertySystemPy.CurrentApPy;
        SkillSugarConfig skillSugarConfig = this.SkillSugarConfigPy;
        if (currentAp < skillSugarConfig.CostApPy) return false;
        //如果有技能糖, 且该技能糖被污染了, 无法进入该状态.
        if (this.SelectedCellOfSkillBar != null
         && MechanicsOfTaintingSugar.HasBeenTainted(this.SelectedCellOfSkillBar.SkillSugarPy)) return false;
        //如果在尝试使用道具道具被控制了, 无法进入该状态.
        BuffPool buffPool = currentPc.CharacterIdPy.CharacterEntityPy.BuffPoolPy;
        if (this.SelectedCellOfLuggage != null
         && MechanicsOfControlEffect.HasControlTypeOfCantUseItem(buffPool)) return false;
        //如果在尝试使用技能时被控制了, 无法进入该状态.
        if (this.SelectedCellOfSkillBar != null
         && MechanicsOfControlEffect.HasControlTypeOfCantUseSkill(buffPool)) return false;

        return true;
    }
    public override void OnEnter()
    {
        //如果点击是被动技能, 就使用该被动技能.
        if (this.SelectedCellOfSkillBar != null
         && !this.SelectedCellOfSkillBar.SkillSugarPy.SkillMainIdAndQualityEnumPy.SkillMainIdPy.SkillMainConfigPy.IsActivePy)
        {
            MechanicsOfPassiveSkillSugar.UsingPassiveSkillSugar(this.SelectedModuleOfSkillBar,this.SelectedCellOfSkillBar,HeronTeam.CurrentPcInControlPy.CharacterIdPy.PcEntityPy);
            return;
        }

        //进入下一轮的选择.
        SelectingSkillStateDetails.EnterNextSelectingRound();

        //功能: Ap 预消耗显示.
        SkillSugarConfig skillSugarConfig = this.SkillSugarConfigPy;
        int apCost = skillSugarConfig.CostApPy % 100 == 0 ? skillSugarConfig.CostApPy / 100 : skillSugarConfig.CostApPy / 100 + 1;
        SelectingSkillState.OnCalculateApCost?.Invoke(apCost);
        SelectingSkillState.OnStateChanged?.Invoke(true);
    }
    public override bool CanExit()
    {
        return true;
    }
    public override void OnExit()
    {
        //隐藏 UI.
        UiManager.CellUiShowerForSkillUsingRangePy.Hide();
        this.Format();

        SelectingSkillState.OnStateChanged?.Invoke(false);
    }

    private void Format()
    {
        this.SelectedModuleOfSkillBar = null;
        this.SelectedCellOfSkillBar = null;

        this.SelectedCellOfLuggage = null;

        this._skillUsingRangeSet.Clear();
        this._effectCenter = null;
        this._effectDirectionIndex = null;

        this._selectingRoundIndex = null;
        this._effectRangeList.Clear();

        this._inputForSettingEffectCenter.SetIsReceivingInput(false);
        this._inputForCalculateRotatableEffectRange.SetIsReceivingInput(false);
        this._inputForCalculateNonRotatableEffectRange.SetIsReceivingInput(false);
        this._inputForBeginNextSelectingRoundOrUseSkill.SetIsReceivingInput(false);
        this._inputForCancelSkill.SetIsReceivingInput(false);

        //无论使用了技能还是取消了, 都隐藏该 UI.
        UiManager.CellUiShowerForSkillUsingRangePy.Hide();
        UiManager.CellUiShowerForSkillEffectRangePy.Hide();

        this._isInState = false;
    }
}
}