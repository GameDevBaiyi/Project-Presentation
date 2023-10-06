using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
[Serializable]
public class BagOfLearnedSkill
{
    [Title("Config")]
    [ShowInInspector]
    public const int MaxNumberOfCellsPerRow = 10;
    [ShowInInspector]
    public const int MaxNumberOfAllCells = 40;
    [ShowInInspector]
    public static int MaxNumberOfRowsPy => (MaxNumberOfAllCells + MaxNumberOfCellsPerRow - 1) / MaxNumberOfCellsPerRow;

    [Title("Data")]
    [ShowInInspector]
    private readonly CharacterId _characterId;

    [ShowInInspector]
    private readonly SkillSugarStringHub _skillSugarStringHub = new SkillSugarStringHub();
    public SkillSugarStringHub SkillSugarStringHubPy => this._skillSugarStringHub;

    [ShowInInspector]
    private readonly List<RowOfSkillBag> _rowsOfSkillBag = new List<RowOfSkillBag>(MaxNumberOfRowsPy);
    public List<RowOfSkillBag> RowsOfSkillBagPy => this._rowsOfSkillBag;

    public BagOfLearnedSkill(CharacterId characterId)
    {
        this._characterId = characterId;
    }

    [Title("References")]
    private BagOfEquippedSkill _bagOfEquippedSkillCache;
    public BagOfEquippedSkill BagOfEquippedSkillCachePy => this._bagOfEquippedSkillCache ??= this._characterId.PcPy.BagOfEquippedSkillPy;

    [Title("Methods")]
    [ShowInInspector]
    public int CurrentCountOfCellsPy => this._rowsOfSkillBag.Sum(skillBagRow => skillBagRow?.CurrentCountOfCellsPy ?? 0);
    [ShowInInspector]
    public int CountOfLeftCellsPy => MaxNumberOfAllCells - this.CurrentCountOfCellsPy;
    public CellOfSkillBag this[int rowIndex,int columnIndex] => this._rowsOfSkillBag[rowIndex].CellsOfSkillBagPy[columnIndex];

    /// <summary>
    /// 功能: 添加一行背包格. 外部目前只需要添加格子.
    /// </summary>
    private void AddRow()
    {
        //功能: 如果当前的背包行数已经到达了最大值, 不能增加.
        int countOfRows = this._rowsOfSkillBag.Count;
        if (countOfRows >= MaxNumberOfRowsPy) return;
        //功能: 该 Row 的格子最大数量, 是 剩余格子数量 或者 配置的最大每行数量.
        int countOfMaxCellsForRow = Mathf.Min(this.CountOfLeftCellsPy,MaxNumberOfCellsPerRow);
        RowOfSkillBag rowOfSkillBag = new RowOfSkillBag(this._characterId,countOfMaxCellsForRow,countOfRows);
        this._rowsOfSkillBag.Add(rowOfSkillBag);
    }

    /// <summary>
    /// 功能: 添加格子.
    /// </summary>
    public void AddCells(int cellCount)
    {
        //功能: 添加值不能为负数.
        if (cellCount < 0)
        {
            Debug.LogError($"技能背包的添加值为: {cellCount}, 不能为负数.");
            return;
        }

        //功能: 如果是 0, 不做修改.
        if (cellCount == 0) return;

        //功能: 如果当前的规定格子数已经满了, 不能再增加.
        if (this.CurrentCountOfCellsPy >= MaxNumberOfAllCells) return;

        //功能: 如果当前背包中一行也没有, 添加一行.
        if (this._rowsOfSkillBag.Count == 0)
        {
            this.AddRow();
        }

        //功能: 可以添加的格子数量 是 剩余格子数 或 传入的参数.
        int cellCountToAdd = Mathf.Min(this.CountOfLeftCellsPy,cellCount);

        while (true)
        {
            RowOfSkillBag lastRowOfSkillBag = this._rowsOfSkillBag[^1];
            //当前的最后一行剩余数.
            int leftCountAtLastRow = lastRowOfSkillBag.CountOfLeftCellsPy;

            //功能: 如果最后一行的剩余可增加格子数不够.
            if (cellCountToAdd > leftCountAtLastRow)
            {
                //功能: 加满最后一行.
                lastRowOfSkillBag.AddCells(leftCountAtLastRow);
                //功能: 减少剩余需要增加的 Cells.
                cellCountToAdd -= leftCountAtLastRow;
                //功能: 添加新的一行.
                this.AddRow();
            }
            //功能: 如果最后一行的剩余格子数足够.
            else
            {
                lastRowOfSkillBag.AddCells(cellCountToAdd);
                break;
            }
        }
    }

    /// <summary>
    /// 功能: 学习到了新技能.
    /// </summary>
    public void LearnNewSugarString(SkillSugarString sugarString)
    {
        int countOfSugars = sugarString.CountOfSugarsPy;

        //计算这个背包目前是否有足够的空格子装得下, 哪一行装的下, 起始的列坐标.
        bool hasEnoughVacantCells = false;
        RowOfSkillBag rowToHold = null;
        int startIndexToHold = 0;
        foreach (RowOfSkillBag skillBagRow in this._rowsOfSkillBag)
        {
            List<Vector2Int> vacantCells = skillBagRow.GroupOfContinuousVacantCellsPy;
            foreach (Vector2Int vacantCell in vacantCells)
            {
                if (vacantCell.y >= countOfSugars)
                {
                    hasEnoughVacantCells = true;
                    rowToHold = skillBagRow;
                    startIndexToHold = vacantCell.x;
                    break;
                }
            }
            if (hasEnoughVacantCells) break;
        }

        //没有足够的空格子就返回.
        if (!hasEnoughVacantCells) return;

        //先用 InstanceHub 设置 Id 并记录该 SugarString, .
        int instanceId = this._skillSugarStringHub.GetNextInstanceId();
        sugarString.SetInstanceId(instanceId);
        this._skillSugarStringHub.RecordInstance(sugarString);

        //功能: 放置到背包的合适位置.
        rowToHold.AddSugarString(startIndexToHold,sugarString);
    }

    /// <summary>
    /// 功能: 检测指定位置的空白格数量是否足够容纳指定的 SugarString.
    /// </summary>
    public bool CheckIfCanAddSugarStringToRow(SkillSugarString sugarString,int rowIndex,int columnIndex)
    {
        return this._rowsOfSkillBag[rowIndex].GetContinuousCells(columnIndex).y >= sugarString.CountOfSugarsPy;
    }

    /// <summary>
    /// 功能: 将一串 Sugar 按自身的指定位置加入到指定的 Row 的指定位置, 该方法会自动寻找合适的对齐.
    /// </summary>
    public void AddSugarStringToRow(SkillSugarString sugarString,int specifiedSugarIndex,int rowIndex,
                                    int columnIndex)
    {
        int sugarStringCount = sugarString.CountOfSugarsPy;
        RowOfSkillBag bag = this._rowsOfSkillBag[rowIndex];
        //功能: 拿到指定的 Row 此时的空格子, 看是否能填充.
        Vector2Int vacantCellsB = bag.GetContinuousCells(columnIndex);

        //功能: 如果 Row B 的该位置周围的连续空白格个数不够, 返回.
        if (vacantCellsB.y < sugarStringCount)
        {
            Debug.LogError($"指定的 Skill Bag Row 的空白格不足. vacantCellsCount: {vacantCellsB.y}, sugarStringCount {sugarStringCount}");
            return;
        }

        //功能: 检测以该点严格对应, 是否能填充进去. 如果 Row 的空白格, 距离其指定的 ColumnIndex 的左右距离均足够, 可以直接填充进去.
        int rowALeftDistance = specifiedSugarIndex;
        int rowARightDistance = sugarStringCount - specifiedSugarIndex;
        int rowBLeftDistance = columnIndex - vacantCellsB.x;
        int rowBRightDistance = vacantCellsB.x + vacantCellsB.y - columnIndex;

        //功能: 直接以该点严格对应, 填充进去.
        if (rowBLeftDistance >= rowALeftDistance
         && rowBRightDistance >= rowARightDistance)
        {
            //功能: 填充到以该点严格对应.
            bag.AddSugarString(columnIndex - rowALeftDistance,sugarString);
        }
        //功能: 对齐指定点时无法填充, 那么此时需要计算左对齐还是右对齐.
        else
        {
            //功能: 如果左边距离大一些, 那么表示希望偏右, 右对齐.
            if (rowBLeftDistance >= rowBRightDistance)
            {
                //功能: 填充到右对齐.
                bag.AddSugarString(vacantCellsB.x + vacantCellsB.y - sugarStringCount,sugarString);
            }
            //功能: 左对齐.
            else
            {
                //功能: 填充到左对齐.
                bag.AddSugarString(vacantCellsB.x,sugarString);
            }
        }
    }

    public void RemoveAllSugarStrings()
    {
        foreach (int skillSugarStringId in this._skillSugarStringHub.InstanceId_InstancePy.Keys.ToList())
        {
            this._skillSugarStringHub.RemoveInstance(skillSugarStringId);
        }

        foreach (RowOfSkillBag rowOfSkillBag in this._rowsOfSkillBag)
        {
            foreach (CellOfSkillBag cellOfSkillBag in rowOfSkillBag.CellsOfSkillBagPy)
            {
                cellOfSkillBag.SetSugarStringId(0);
                cellOfSkillBag.SetMainSkillIdAndQualityEnum(new SkillMainIdAndQualityEnum());
            }
        }
    }

    public void RemoveSugarString(int sugarStringId)
    {
        // 从 技能的已装备背包 的 RowOfSkillBag 中 卸下. 
        foreach (RowOfSkillBag rowOfSkillBag in this.BagOfEquippedSkillCachePy.RowsPy)
        {
            if (!rowOfSkillBag.ContainsSugarString(sugarStringId,out int columnIndex)) continue;
            this.BagOfEquippedSkillCachePy.UnEquipSugarString(rowOfSkillBag.RowIndexPy,columnIndex);
            break;
        }

        // 从 技能的已学习背包 的 RowOfSkillBag 中 移除.
        foreach (RowOfSkillBag rowOfSkillBag in this._rowsOfSkillBag)
        {
            if (!rowOfSkillBag.ContainsSugarString(sugarStringId,out int columnIndex)) continue;
            rowOfSkillBag.RemoveSugarString(columnIndex,out _);
            break;
        }
        
        // 从 SkillSugarStringHub 中 移除.
        this._skillSugarStringHub.RemoveInstance(sugarStringId);
    }
}
}