using System;

using LowLevelSystems.SkillSystems.Base;

using Sirenix.OdinInspector;

namespace LowLevelSystems.CharacterSystems.PcSystems.Components.BagOfLearnedSkillSystems
{
/// <summary>
/// 功能: 技能背包格子, 两种技能背包的格子是通用的.
/// </summary>
[Serializable]
public class CellOfSkillBag
{
    [Title("Data")]
    [ShowInInspector]
    private int _rowIndex;
    public int RowIndexPy => this._rowIndex;
    public void SetRowIndex(int rowIndex)
    {
        this._rowIndex = rowIndex;
    }
    [ShowInInspector]
    private int _columnIndex;
    public int ColumnIndexPy => this._columnIndex;
    public void SetColumnIndex(int columnIndex)
    {
        this._columnIndex = columnIndex;
    }
    [ShowInInspector]
    private SkillSugarStringId _sugarStringId;
    public SkillSugarStringId SugarStringIdPy => this._sugarStringId;
    public void SetSugarStringId(SkillSugarStringId sugarStringId)
    {
        this._sugarStringId = sugarStringId;
    }
    public void SetSugarStringId(int sugarStringId)
    {
        this._sugarStringId.InstanceId = sugarStringId;
    }

    [ShowInInspector]
    private SkillMainIdAndQualityEnum _skillMainIdAndQualityEnum;
    public SkillMainIdAndQualityEnum SkillMainIdAndQualityEnumPy => this._skillMainIdAndQualityEnum;
    public void SetMainSkillIdAndQualityEnum(SkillMainIdAndQualityEnum skillMainIdAndQualityEnum)
    {
        this._skillMainIdAndQualityEnum = skillMainIdAndQualityEnum;
    }

    [Title("Methods")]
    [ShowInInspector]
    public bool HasSkillSugarStringPy => this._sugarStringId.InstanceId != 0;
}
}