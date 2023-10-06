using LowLevelSystems.SkillSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
/// <summary>
/// 技能栏的格子.
/// </summary>
public class CellOfSkillBar
{
    [Title("Data")]
    [ShowInInspector]
    private PcEntity _pcEntity;
    public PcEntity PcEntityPy => this._pcEntity;
    public void SetPcEntity(PcEntity pcEntity)
    {
        this._pcEntity = pcEntity;
    }

    [ShowInInspector]
    private Vector3Int _coord;
    public Vector3Int CoordPy => this._coord;

    [ShowInInspector]
    private SkillSugar _skillSugar;
    public SkillSugar SkillSugarPy => this._skillSugar;
    public void SetSkillSugar(SkillSugar skillSugar)
    {
        this._skillSugar = skillSugar;
    }

    public CellOfSkillBar(PcEntity pcEntity,Vector3Int coord)
    {
        this._pcEntity = pcEntity;
        this._coord = coord;
    }
}
}