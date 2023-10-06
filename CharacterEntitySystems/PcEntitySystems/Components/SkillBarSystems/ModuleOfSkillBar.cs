using System;
using System.Collections.Generic;
using System.Linq;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillDrawPileSystems;
using LowLevelSystems.SkillSystems.Base;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public class ModuleOfSkillBar
{
    private struct DistanceOfStarPosAndVacantCell : IComparable<DistanceOfStarPosAndVacantCell>
    {
        public int Distance;
        public Vector3Int StarPos;
        public Vector3Int VacantCellCoord;

        public int CompareTo(DistanceOfStarPosAndVacantCell other)
        {
            return this.Distance.CompareTo(other.Distance);
        }
    }

    [Title("Data")]
    [ShowInInspector]
    private PcEntity _pcEntity;
    public PcEntity PcEntityPy => this._pcEntity;
    public void SetPcEntity(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
    }

    [ShowInInspector]
    private ModuleOfSkillBarEnum _moduleOfSkillBarEnum;
    public ModuleOfSkillBarEnum ModuleOfSkillBarEnumPy => this._moduleOfSkillBarEnum;

    [ShowInInspector]
    private Dictionary<Vector3Int,CellOfSkillBar> _coord_cell;
    public Dictionary<Vector3Int,CellOfSkillBar> Coord_CellPy => this._coord_cell;

    private readonly PathfindingForSkillBarModule _pathfindingForSkillBarModule = new PathfindingForSkillBarModule();

    [Title("Cache")]
    private readonly List<Vector3Int> _coordToHide = new List<Vector3Int>(10);
    private readonly Dictionary<SkillSugar,Vector3Int> _skillSugar_previousCoord = new Dictionary<SkillSugar,Vector3Int>(20);
    private readonly Dictionary<SkillSugar,Vector3Int> _skillSugar_currentCoord = new Dictionary<SkillSugar,Vector3Int>(20);
    private readonly Dictionary<Vector3Int,List<Vector3Int>> _coord_path = new Dictionary<Vector3Int,List<Vector3Int>>(20);
    public ModuleOfSkillBar(PcEntity pcEntity,ModuleOfSkillBarEnum moduleOfSkillBarEnum)
    {
        this._pcEntity = pcEntity;
        this._moduleOfSkillBarEnum = moduleOfSkillBarEnum;
        ModuleOfSkillBarConfig moduleOfSkillBarConfig = this._moduleOfSkillBarEnum.ModuleOfSkillBarConfig();
        this._coord_cell
            = new Dictionary<Vector3Int,CellOfSkillBar>(moduleOfSkillBarConfig.NodeFlagsGridPy.Sum(row => row.Count(cell => cell.HasFlag(NodeOfModuleFlags.IsSugarPoint))));
        for (int i = 0; i < moduleOfSkillBarConfig.MapWidthPy; i++)
        {
            for (int j = 0; j < moduleOfSkillBarConfig.MapHeightPy; j++)
            {
                if (!moduleOfSkillBarConfig.NodeFlagsGridPy[i][j].HasFlag(NodeOfModuleFlags.IsSugarPoint)) continue;

                Vector3Int coord = new Vector3Int(i,j,0);
                this._coord_cell[coord] = new CellOfSkillBar(pcEntity,coord);
            }
        }

        this._pathfindingForSkillBarModule.ResetPathfinding(moduleOfSkillBarConfig.NodeFlagsGridPy);
    }

    [Title("Events")]
    public static event Action<ModuleOfSkillBar,List<Vector3Int>,Dictionary<Vector3Int,List<Vector3Int>>> OnFillSugars;

    [Title("Methods")]
    public List<SkillSugar> RemoveSkillSugars(List<Vector3Int> coords)
    {
        List<SkillSugar> skillSugarsToRemove = new List<SkillSugar>(coords.Count);
        foreach (Vector3Int coord in coords)
        {
            //Debug.
            if (!this._coord_cell.TryGetValue(coord,out CellOfSkillBar cellOfSkillBar))
            {
                Debug.LogError($"从技能栏模组中移除技能时, 传入的坐标并没有对应的格子. {coord}");
                continue;
            }

            skillSugarsToRemove.Add(cellOfSkillBar.SkillSugarPy);
            cellOfSkillBar.SetSkillSugar(null);
        }

        return skillSugarsToRemove;
    }

    public void FillSugars(out Dictionary<Vector3Int,List<Vector3Int>> coord_path)
    {
        ModuleOfSkillBarConfig moduleOfSkillBarConfig = this._moduleOfSkillBarEnum.ModuleOfSkillBarConfig();
        SkillDrawPile skillDrawPile = this._pcEntity.SkillDrawPilePy;

        //记录 UI 数据.
        this._coordToHide.Clear();
        foreach (CellOfSkillBar cellOfSkillBar in this._coord_cell.Values.Where(t => t.SkillSugarPy == null))
        {
            this._coordToHide.Add(cellOfSkillBar.CoordPy);
        }

        this._skillSugar_previousCoord.Clear();
        foreach (CellOfSkillBar cellOfSkillBar in this._coord_cell.Values.Where(t => t.SkillSugarPy != null))
        {
            this._skillSugar_previousCoord[cellOfSkillBar.SkillSugarPy] = cellOfSkillBar.CoordPy;
        }

        foreach (List<Vector3Int> list in this._coord_path.Values)
        {
            list.Clear();
        }
        coord_path = this._coord_path;

        while (true)
        {
            //通过星位坐标找到星位的对应格子, 如果是空的, 就先给所有空白星位填充一个.
            foreach (Vector3Int starPos in moduleOfSkillBarConfig.StarPosesPy)
            {
                //Debug.
                if (!this._coord_cell.TryGetValue(starPos,out CellOfSkillBar cellOfSkillBar))
                {
                    Debug.LogError($"该 {nameof(ModuleOfSkillBarEnum)} : {this._moduleOfSkillBarEnum} 的此星位: {starPos} 没有格子. ");
                    continue;
                }

                if (cellOfSkillBar.SkillSugarPy == null)
                {
                    SkillSugar skillSugar = skillDrawPile.RemoveFromPredicatedSugars();
                    cellOfSkillBar.SetSkillSugar(skillSugar);
                    MechanicsOfPassiveSkillSugar.OnAddPassiveSkillSugarToModule(skillSugar,this._pcEntity);
                }
            }

            //计算星位和所有空白格的距离, 并且排序. 然后选出一个距离最小的组合. 
            List<DistanceOfStarPosAndVacantCell> distanceOfStarPosAndVacantCells = new List<DistanceOfStarPosAndVacantCell>();
            foreach (Vector3Int starPos in moduleOfSkillBarConfig.StarPosesPy)
            {
                foreach (CellOfSkillBar cellOfSkillBar in this._coord_cell.Values)
                {
                    if (cellOfSkillBar.SkillSugarPy != null) continue;
                    DistanceOfStarPosAndVacantCell distanceOfStarPosAndVacantCell = new DistanceOfStarPosAndVacantCell();
                    distanceOfStarPosAndVacantCell.Distance = Mathf.Abs(starPos.x - cellOfSkillBar.CoordPy.x) + Mathf.Abs(starPos.y - cellOfSkillBar.CoordPy.y);
                    distanceOfStarPosAndVacantCell.StarPos = starPos;
                    distanceOfStarPosAndVacantCell.VacantCellCoord = cellOfSkillBar.CoordPy;
                    distanceOfStarPosAndVacantCells.Add(distanceOfStarPosAndVacantCell);
                }
            }
            distanceOfStarPosAndVacantCells.Sort();
            //如果没有空白格了, 就break. 
            if (distanceOfStarPosAndVacantCells.Count == 0) break;
            //Debug.
            DistanceOfStarPosAndVacantCell firstDistanceData = distanceOfStarPosAndVacantCells[0];
            if (firstDistanceData.Distance <= 0)
            {
                Debug.LogError($"不应该出现距离 <=0 的, 此时的 数据为 : {firstDistanceData.Distance}, {firstDistanceData.StarPos}, {firstDistanceData.VacantCellCoord}");
                break;
            }

            List<Vector3Int> path = new List<Vector3Int>();
            this._pathfindingForSkillBarModule.TryFindPath(firstDistanceData.StarPos,firstDistanceData.VacantCellCoord,path);
            List<SkillSugar> allSkillSugars = path.Select(t => this._coord_cell[t].SkillSugarPy).Where(t => t != null).ToList();
            foreach (Vector3Int coord in path)
            {
                this._coord_cell[coord].SetSkillSugar(null);
            }
            for (int i = 0; i < allSkillSugars.Count; i++)
            {
                this._coord_cell[path[i]].SetSkillSugar(allSkillSugars[i]);
            }
        }

        this._skillSugar_currentCoord.Clear();
        foreach (CellOfSkillBar cellOfSkillBar in this._coord_cell.Values.Where(t => t.SkillSugarPy != null))
        {
            this._skillSugar_currentCoord[cellOfSkillBar.SkillSugarPy] = cellOfSkillBar.CoordPy;
        }
        foreach (KeyValuePair<SkillSugar,Vector3Int> pair in this._skillSugar_previousCoord)
        {
            if (!this._skillSugar_currentCoord.TryGetValue(pair.Key,out Vector3Int currentCoord)) continue;
            Vector3Int previousCoord = pair.Value;
            if (currentCoord == previousCoord) continue;

            if (!this._coord_path.TryGetValue(previousCoord,out List<Vector3Int> path))
            {
                path = new List<Vector3Int>(Mathf.Abs(currentCoord.x - previousCoord.x) + Mathf.Abs(currentCoord.y - previousCoord.y));
                this._coord_path[previousCoord] = path;
            }
            this._pathfindingForSkillBarModule.TryFindPath(currentCoord,previousCoord,path);
        }

        ModuleOfSkillBar.OnFillSugars?.Invoke(this,this._coordToHide,this._coord_path);
    }
}
}