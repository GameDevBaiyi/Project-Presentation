using System;

using LowLevelSystems.LocalizationSystems;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
[Serializable]
public class BuildingConfig
{
    [SerializeField]
    private BuildingEnum _buildingEnum;
    public BuildingEnum BuildingEnumPy => this._buildingEnum;
    public void SetBuildingEnum(BuildingEnum buildingEnum)
    {
        this._buildingEnum = buildingEnum;
    }

    [SerializeField]
    private TextId _nameId;
    public TextId NameIdPy => this._nameId;
    public void SetNameId(TextId nameId)
    {
        this._nameId = nameId;
    }
}
}