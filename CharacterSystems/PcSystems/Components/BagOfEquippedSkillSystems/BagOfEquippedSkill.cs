using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfEquippedSkillSystems
{
[Serializable]
public class BagOfEquippedSkill
{
    [Title("Data")]
    [ShowInInspector]
    private readonly CharacterId _characterId;
    public CharacterId CharacterIdPy => this._characterId;

    [ShowInInspector]
    private readonly List<RowOfSkillBag> _rows;
    public List<RowOfSkillBag> RowsPy => this._rows;

    public BagOfEquippedSkill(CharacterId characterId,int maxNumberOfRows)
    {
        this._characterId = characterId;
        this._rows = new List<RowOfSkillBag>(maxNumberOfRows);
    }

    [Title("Methods")]
    [ShowInInspector]
    public List<int> MaxNumberOfCellsPerRowPy => BagOfEquippedSkillDetails.GetMaxNumberOfCellsPerRow(this);
    [ShowInInspector]
    public List<int> InitialCountOfCellsPerRowPy => BagOfEquippedSkillDetails.GetInitialCountOfCellsPerRow(this);
    [ShowInInspector]
    public int MaxNumberOfCellsPy => this.MaxNumberOfCellsPerRowPy.Sum();
    [ShowInInspector]
    public int MaxNumberOfRowsPy => this.MaxNumberOfCellsPerRowPy.Count;
    [ShowInInspector]
    public int CurrentCountOfCellsPy
    {
        get
        {
            int count = 0;

            foreach (RowOfSkillBag skillBagRow in this._rows)
            {
                count += skillBagRow.CurrentCountOfCellsPy;
            }

            return count;
        }
    }
    
    [ShowInInspector]
    public int LeftCountOfCellsPy => this.MaxNumberOfCellsPy - this.CurrentCountOfCellsPy;

    public CellOfSkillBag this[int rowIndex,int columnIndex] => this._rows[rowIndex].CellsOfSkillBagPy[columnIndex];

    /// <summary>
    /// 功能: 解锁一行背包格.
    /// </summary>
    public void UnlockSkillBagRow()
    {
        //功能: 如果当前的背包行数已经到达了最大值, 不能增加.
        int rowCount = this._rows.Count;
        if (rowCount >= this.MaxNumberOfRowsPy) return;
        //功能: 先添加一行.
        RowOfSkillBag nextRowOfSkillBag = new RowOfSkillBag(this._characterId,this.MaxNumberOfCellsPerRowPy[rowCount],rowCount);
        this._rows.Add(nextRowOfSkillBag);
        //功能: 并且立刻添加初次需要解锁的 Cells.
        this.AddBagCell(rowCount,this.InitialCountOfCellsPerRowPy[rowCount]);
    }

    /// <summary>
    /// 功能: 添加格子.
    /// </summary>
    public void AddBagCell(int rowIndex,int cellCount)
    {
        if (rowIndex >= this._rows.Count)
        {
            Debug.LogError($"技能背包(已装备) 第 {rowIndex} 行未解锁, 不能直接添加格子.");
            return;
        }
        this._rows[rowIndex].AddCells(cellCount);
    }

    /// <summary>
    /// 功能: 装备新技能.
    /// </summary>
    public void EquipNewSugarString(SkillSugarString sugarString,int rowIndex)
    {
        RowOfSkillBag bag = this._rows[rowIndex];
        //功能: 检测该 Row 中是否已经有了该 SugarString. 如果有了, 返回.
        int sugarStringID = sugarString.InstanceIdPy;
        if (bag.ContainsSugarString(sugarStringID)) return;
        //功能: 检测该 Row 中是否有足够的位置. 如果没有, 无事发生.
        if (!bag.CanHold(sugarString.CountOfSugarsPy)) return;

        //功能: 将该 SugarString 填充到左对齐.
        int startIndex = 0;
        foreach (CellOfSkillBag bagCell in bag.CellsOfSkillBagPy)
        {
            int cellSugarStringId = bagCell.SugarStringIdPy.InstanceId;

            if (cellSugarStringId == 0)
            {
                startIndex = bagCell.ColumnIndexPy;
                break;
            }
        }
        bag.AddSugarString(startIndex,sugarString);

        //功能: 检测别的 Row 是否有该技能, 如果有, 清除. 
        foreach (RowOfSkillBag skillBagRow in this._rows)
        {
            if (skillBagRow.RowIndexPy == rowIndex) continue;

            if (skillBagRow.ContainsSugarString(sugarStringID,out int columnIndex))
            {
                skillBagRow.RemoveSugarString(columnIndex,out _);
                //功能: 清除后, 自动排序.
                skillBagRow.AlignLeft();
                break;
            }
        }
    }

    /// <summary>
    /// 功能: 卸载掉装备着的技能串. 
    /// </summary>
    public void UnEquipSugarString(int rowIndex,int columnIndex)
    {
        if (!this[rowIndex,columnIndex].HasSkillSugarStringPy) return;

        RowOfSkillBag rowOfSkillBag = this._rows[rowIndex];
        rowOfSkillBag.RemoveSugarString(columnIndex,out _);
        rowOfSkillBag.AlignLeft();
    }

    public void RemoveAllSugarString()
    {
        foreach (RowOfSkillBag rowOfSkillBag in this._rows)
        {
            foreach (CellOfSkillBag cellOfSkillBag in rowOfSkillBag.CellsOfSkillBagPy)
            {
                cellOfSkillBag.SetSugarStringId(0);
                cellOfSkillBag.SetMainSkillIdAndQualityEnum(new SkillMainIdAndQualityEnum());
            }
        }
    }
}
}