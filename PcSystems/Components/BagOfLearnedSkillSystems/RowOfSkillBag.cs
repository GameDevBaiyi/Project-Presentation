using System;
using System.Collections.Generic;
using System.Linq;

using Common.Extensions;

using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
/// <summary>
/// 功能: 技能背包一行的概念, 两种技能背包的行是通用的.
/// </summary>
[Serializable]
public class RowOfSkillBag
{
    [Title("Data")]
    private readonly CharacterId _characterId;

    [ShowInInspector]
    private readonly int _countOfMaxCells;

    [ShowInInspector]
    private readonly int _rowIndex;
    public int RowIndexPy => this._rowIndex;

    [ShowInInspector]
    private readonly List<CellOfSkillBag> _cellsOfSkillBag;
    public List<CellOfSkillBag> CellsOfSkillBagPy => this._cellsOfSkillBag;

    public RowOfSkillBag(CharacterId characterId,int countOfMaxCells,int rowIndex)
    {
        this._characterId = characterId;
        this._countOfMaxCells = countOfMaxCells;
        this._rowIndex = rowIndex;
        this._cellsOfSkillBag = new List<CellOfSkillBag>(countOfMaxCells);
    }

    [Title("Methods")]
    [ShowInInspector]
    public int CurrentCountOfCellsPy => this._cellsOfSkillBag.Count;
    /// <summary>
    /// 剩余可添加的格子个数.
    /// </summary>
    [ShowInInspector]
    public int CountOfLeftCellsPy => this._countOfMaxCells - this.CurrentCountOfCellsPy;
    /// <summary>
    /// 功能: 所有空白格的 Column Indexes
    /// </summary>
    [ShowInInspector]
    public List<int> ColumnIndexesOfAllVacantCellsPy => this._cellsOfSkillBag.Where(t => !t.HasSkillSugarStringPy).Select(t => t.ColumnIndexPy).ToList();
    /// <summary>
    /// 功能: 将连续的空白格分组记录. 
    /// </summary>
    [ShowInInspector]
    public List<Vector2Int> GroupOfContinuousVacantCellsPy
    {
        get
        {
            List<Vector2Int> continuousVacantCellCache = new List<Vector2Int>(3);
            List<int> vacantCellIndexes = this.ColumnIndexesOfAllVacantCellsPy;
            int vacantCellIndexesCount = vacantCellIndexes.Count;

            int startIndex = 0;
            int count = 0;

            for (int i = 0; i < vacantCellIndexesCount; i++)
            {
                //起始点.
                if (i == 0)
                {
                    startIndex = vacantCellIndexes[i];
                    count = 1;
                }
                else
                {
                    //比较这一点是否和上一点连续.
                    if (vacantCellIndexes[i] - vacantCellIndexes[i - 1] == 1)
                    {
                        count++;
                    }
                    //如果不连续.
                    else
                    {
                        //记录当前的 start index 和 count.
                        continuousVacantCellCache.Add(new Vector2Int(startIndex,count));

                        //标记新的 start index 和 count.
                        startIndex = vacantCellIndexes[i];
                        count = 1;
                    }
                }
            }

            if (count != 0)
            {
                continuousVacantCellCache.Add(new Vector2Int(startIndex,count));
            }

            return continuousVacantCellCache;
        }
    }

    /// <summary>
    /// 添加格子, 传入参数大于剩余数量时不会报错, 但保证不会超出最大数量. 
    /// </summary>
    public void AddCells(int addend)
    {
        //功能: 格子不能减少.
        if (addend < 0)
        {
            Debug.LogError($"技能背包行 {typeof(RowOfSkillBag)} 中的格子不能减少.");
            return;
        }

        //功能: 增加数为 0, 不做改变.
        if (addend == 0) return;

        //功能: 传入的参数不应该大于最大剩余数量.
        if (addend > this.CountOfLeftCellsPy)
        {
            addend = this.CountOfLeftCellsPy;
        }

        for (int i = 0; i < addend; i++)
        {
            int columnIndex = this._cellsOfSkillBag.Count;
            CellOfSkillBag cellOfSkillBag = CellOfSkillBagFactory.GenerateCellOfSkillBag(this._rowIndex,columnIndex,this._characterId.InstanceId);
            this._cellsOfSkillBag.Add(cellOfSkillBag);
        }
    }

    /// <summary>
    /// 功能: 将 SugarString 标记到 Row 中. 即给对应的 Cell 添加 SugarStringId 和 SugarConfigId.
    /// </summary>
    public void AddSugarString(int startIndex,SkillSugarString sugarString)
    {
        int currentCountOfCells = this.CurrentCountOfCellsPy;

        for (int i = 0; i < sugarString.CountOfSugarsPy; i++)
        {
            int index = startIndex + i;
            if (index >= currentCountOfCells)
            {
                Debug.LogError($"往 Row 中添加 SugarString 时, 发现超出了 当前格子数 . "
                             + $"当前添加到的 Index: {index}, 当前 CountOfCells: {currentCountOfCells}, 添加的 SugarStringId: {sugarString.InstanceIdPy}");
                continue;
            }

            CellOfSkillBag cellOfSkillBag = this._cellsOfSkillBag[index];
            cellOfSkillBag.SetSugarStringId(sugarString.InstanceIdPy);
            cellOfSkillBag.SetMainSkillIdAndQualityEnum(sugarString.MainSkillIdAndQualityEnumListPy[i]);
        }
    }

    /// <summary>
    /// 功能: 移除指定点的 Sugar String 的记录, 指定的点不需要是起始点.
    /// </summary>
    public void RemoveSugarString(int columnIndex,out Vector2Int continuousCells)
    {
        continuousCells = Vector2Int.zero;
        int currentCountOfCells = this.CurrentCountOfCellsPy;

        if (!columnIndex.IsInRange(0,currentCountOfCells,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"从 Row 中移除 SugarString 时, 传入的参数为: columnIndex{columnIndex}, 超出了格子范围: 当前格子个数: {this.CurrentCountOfCellsPy}.");
            return;
        }

        CellOfSkillBag targetCell = this._cellsOfSkillBag[columnIndex];
        //功能: 如果该背包格子没有装东西, 不需要清理.
        if (!targetCell.HasSkillSugarStringPy) return;

        //功能: 拿到记录了的背包格子, 清理.
        continuousCells = this.GetContinuousCells(columnIndex);
        int startIndex = continuousCells.x;
        int count = continuousCells.y;

        for (int i = 0; i < count; i++)
        {
            int index = startIndex + i;
            CellOfSkillBag cellOfSkillBag = this._cellsOfSkillBag[index];
            cellOfSkillBag.SetSugarStringId(0);
            cellOfSkillBag.SetMainSkillIdAndQualityEnum(new SkillMainIdAndQualityEnum());
        }
    }

    /// <summary>
    /// 功能: 使左对齐, 空白格放后面去.
    /// </summary>
    public void AlignLeft()
    {
        //记录装有 String (即 非 0) 的格子数据.
        var cellDataList = this._cellsOfSkillBag.Where(t => t.HasSkillSugarStringPy)
                               .Select(t => new { t.SugarStringIdPy,MainSkillIdAndQualityEnumPy = t.SkillMainIdAndQualityEnumPy, })
                               .ToList();

        int currentCountOfCells = this.CurrentCountOfCellsPy;
        for (int i = 0; i < currentCountOfCells; i++)
        {
            int countOfCellDataList = cellDataList.Count;
            bool isOutOfRangeForDataList = i >= countOfCellDataList;
            // 将记录的数据从左到右填进去, 超出部分的都设置为 0.
            CellOfSkillBag cell = this._cellsOfSkillBag[i];
            cell.SetSugarStringId(isOutOfRangeForDataList ? 0 : cellDataList[i].SugarStringIdPy.InstanceId);
            cell.SetMainSkillIdAndQualityEnum(isOutOfRangeForDataList ? new SkillMainIdAndQualityEnum() : cellDataList[i].MainSkillIdAndQualityEnumPy);
        }
    }

    /// <summary>
    /// 功能: 获得这一点周围同类型的 Cells. X 表示起始的 ColumnIndex, Y 表示这一段的数量.
    /// </summary>
    public Vector2Int GetContinuousCells(int columnIndex)
    {
        int currentCellCount = this.CurrentCountOfCellsPy;
        if (!columnIndex.IsInRange(0,currentCellCount,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"寻找连续的 BagRow 中的一串 SugarString 的 Cells, 传入的参数为: " + $"columnIndex{columnIndex}, 超出了格子范围: 格子个数: {this.CurrentCountOfCellsPy}.");
            return Vector2Int.zero;
        }

        int sugarStringId = this._cellsOfSkillBag[columnIndex].SugarStringIdPy.InstanceId;
        //功能: 向前找到起点.
        int startIndex = columnIndex;
        while (true)
        {
            if (startIndex == 0
             || this._cellsOfSkillBag[startIndex - 1].SugarStringIdPy.InstanceId != sugarStringId)
            {
                break;
            }

            startIndex--;
        }

        //功能: 往后找到终点.
        int endIndex = columnIndex;
        while (true)
        {
            if (endIndex == currentCellCount - 1
             || this._cellsOfSkillBag[endIndex + 1].SugarStringIdPy.InstanceId != sugarStringId)
            {
                break;
            }

            endIndex++;
        }
        //注意: endIndex 额外需要加一, 才能计算正确的 Count.
        endIndex++;

        return new Vector2Int(startIndex,endIndex - startIndex);
    }

    /// <summary>
    /// 功能: 检测是否包含某个 SugarString.
    /// </summary>
    public bool ContainsSugarString(int sugarStringId)
    {
        return this._cellsOfSkillBag.Any(cell => cell.SugarStringIdPy.InstanceId == sugarStringId);
    }
    public bool ContainsSugarString(int sugarStringId,out int startColumnIndex)
    {
        startColumnIndex = 0;
        foreach (CellOfSkillBag cell in this._cellsOfSkillBag)
        {
            if (cell.SugarStringIdPy.InstanceId != sugarStringId) continue;
            startColumnIndex = cell.ColumnIndexPy;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 功能: 检测是否能容纳某长度的 SugarString.
    /// </summary>
    public bool CanHold(int lengthOfSugarString)
    {
        List<Vector2Int> continuousVacantCells = this.GroupOfContinuousVacantCellsPy;
        return continuousVacantCells.Any(continuousVacantCell => continuousVacantCell.y >= lengthOfSugarString);
    }
}
}