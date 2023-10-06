using Common.Extensions;

using LowLevelSystems.Common;
using LowLevelSystems.SkillSystems.SkillSugarStringSystems;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
public abstract class BagOfLearnedSkillDetails : Details
{
    /// <summary>
    /// 功能: 交换两个背包格中的内容, 不一定是交换 SugarString, 也有可能一个是空白.
    /// </summary>
    public static void ExchangeContentOfTwoCells(BagOfLearnedSkill bagOfLearnedSkill,int rowIndexOfCellA,int columnIndexOfCellA,
                                                 int rowIndexOfCellB,int columnIndexOfCellB)
    {
        // Debug.
        int countOfRows = bagOfLearnedSkill.RowsOfSkillBagPy.Count;
        if (!rowIndexOfCellA.IsInRange(0,countOfRows,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"交换两个 技能背包(已学习) 的背包格子的技能时, 传入的参数 cellARowIndex: {rowIndexOfCellA} 超过了当前背包行数: {countOfRows}.");
            return;
        }
        if (!rowIndexOfCellB.IsInRange(0,countOfRows,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"交换两个 技能背包(已学习) 的背包格子的技能时, 传入的参数 cellBRowIndex: {rowIndexOfCellB} 超过了当前背包行数: {countOfRows}.");
            return;
        }
        RowOfSkillBag rowA = bagOfLearnedSkill.RowsOfSkillBagPy[rowIndexOfCellA];
        RowOfSkillBag rowB = bagOfLearnedSkill.RowsOfSkillBagPy[rowIndexOfCellB];
        int cellCountOfRowA = rowA.CurrentCountOfCellsPy;
        int cellCountOfRowB = rowB.CurrentCountOfCellsPy;
        if (!columnIndexOfCellA.IsInRange(0,cellCountOfRowA,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"交换两个 技能背包(已学习) 的背包格子的技能时, 传入的参数 cellAColumnIndex: {columnIndexOfCellA} 超过了当前 Row 的格子数: {cellCountOfRowA}.");
            return;
        }
        if (!columnIndexOfCellB.IsInRange(0,cellCountOfRowB,ExclusiveFlags.MaxExclusive))
        {
            Debug.LogError($"交换两个 技能背包(已学习) 的背包格子的技能时, 传入的参数 cellBColumnIndex: {columnIndexOfCellB} 超过了当前 Row 的格子数 {cellCountOfRowB}.");
            return;
        }

        //1. A 无, 返回.
        CellOfSkillBag cellA = rowA.CellsOfSkillBagPy[columnIndexOfCellA];
        Vector2Int sugarStringCellsA;
        if (!cellA.HasSkillSugarStringPy) return;

        //2. A 有, B 无.
        SkillSugarString sugarStringA = cellA.SugarStringIdPy.SkillSugarStringPy;
        CellOfSkillBag cellB = rowB.CellsOfSkillBagPy[columnIndexOfCellB];
        if (!cellB.HasSkillSugarStringPy)
        {
            //功能: 清理 A 之前的位置.
            rowA.RemoveSugarString(columnIndexOfCellA,out sugarStringCellsA);
            bool canHoldA = bagOfLearnedSkill.CheckIfCanAddSugarStringToRow(sugarStringA,rowIndexOfCellB,columnIndexOfCellB);
            if (!canHoldA)
            {
                rowA.AddSugarString(sugarStringCellsA.x,sugarStringA);
                return;
            }
            bagOfLearnedSkill.AddSugarStringToRow(sugarStringA,columnIndexOfCellA - sugarStringCellsA.x,rowIndexOfCellB,columnIndexOfCellB);
            return;
        }

        //3. A 有, B 有. 但 A B 相同. 那么实际上和 2 差不多.
        SkillSugarString sugarStringB = cellB.SugarStringIdPy.SkillSugarStringPy;
        if (sugarStringA.InstanceIdPy == sugarStringB.InstanceIdPy)
        {
            //功能: 清理 A 之前的位置.
            rowA.RemoveSugarString(columnIndexOfCellA,out sugarStringCellsA);
            bool canHoldA = bagOfLearnedSkill.CheckIfCanAddSugarStringToRow(sugarStringA,rowIndexOfCellB,columnIndexOfCellB);
            if (!canHoldA)
            {
                rowA.AddSugarString(sugarStringCellsA.x,sugarStringA);
                return;
            }
            bagOfLearnedSkill.AddSugarStringToRow(sugarStringA,columnIndexOfCellA - sugarStringCellsA.x,rowIndexOfCellB,columnIndexOfCellB);
            return;
        }

        //4. A 有, B 有. 且 A B 不同. 但在同一行. 那么只有数量完全相同才能互相容纳. 
        Vector2Int sugarStringCellsB;
        if (rowIndexOfCellA == rowIndexOfCellB)
        {
            if (sugarStringA.CountOfSugarsPy != sugarStringB.CountOfSugarsPy) return;
            rowA.RemoveSugarString(columnIndexOfCellA,out sugarStringCellsA);
            rowB.RemoveSugarString(columnIndexOfCellB,out sugarStringCellsB);
            rowA.AddSugarString(sugarStringCellsB.x,sugarStringB);
            rowB.AddSugarString(sugarStringCellsA.x,sugarStringA);
            return;
        }

        //5. A 有, B 有. 且 A B 不同. 且不在同一行.
        rowA.RemoveSugarString(columnIndexOfCellA,out sugarStringCellsA);
        rowB.RemoveSugarString(columnIndexOfCellB,out sugarStringCellsB);
        bool canExchange = bagOfLearnedSkill.CheckIfCanAddSugarStringToRow(sugarStringA,rowIndexOfCellB,columnIndexOfCellB)
                        && bagOfLearnedSkill.CheckIfCanAddSugarStringToRow(sugarStringB,rowIndexOfCellA,columnIndexOfCellA);
        //功能: 如果可以互相容纳, 添加对方.
        if (canExchange)
        {
            bagOfLearnedSkill.AddSugarStringToRow(sugarStringA,columnIndexOfCellA - sugarStringCellsA.x,rowIndexOfCellB,columnIndexOfCellB);
            bagOfLearnedSkill.AddSugarStringToRow(sugarStringB,columnIndexOfCellB - sugarStringCellsB.x,rowIndexOfCellA,columnIndexOfCellA);
        }
        //功能: 如果不能, 恢复之前删除的记录.
        else
        {
            rowA.AddSugarString(sugarStringCellsA.x,sugarStringA);
            rowB.AddSugarString(sugarStringCellsB.x,sugarStringB);
        }
    }
}
}