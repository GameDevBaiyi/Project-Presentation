using System;
using System.Collections.Generic;

using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
[Serializable]
public class SceneConfigHub
{
    [SerializeField]
    private List<ScenePrefabConfig> _scenePrefabConfigs;
    private Dictionary<ScenePrefabEnum,ScenePrefabConfig> _scenePrefabEnum_scenePrefabConfig;
    public Dictionary<ScenePrefabEnum,ScenePrefabConfig> ScenePrefabEnum_ScenePrefabConfigPy => this._scenePrefabEnum_scenePrefabConfig;
    public void SetScenePrefabConfigs(List<ScenePrefabConfig> scenePrefabConfigs)
    {
        this._scenePrefabConfigs = scenePrefabConfigs;
    }

    [SerializeField]
    private List<CityConfig> _cityConfigs;
    private Dictionary<CityEnum,CityConfig> _cityEnum_cityConfig;
    public Dictionary<CityEnum,CityConfig> CityEnum_CityConfigPy => this._cityEnum_cityConfig;
    public void SetCityConfigs(List<CityConfig> cityConfigs)
    {
        this._cityConfigs = cityConfigs;
    }

    [SerializeField]
    private List<CityLevelConfig> _cityLevelConfigs;
    private Dictionary<int,CityLevelConfig> _cityLevel_cityLevelConfig;
    public Dictionary<int,CityLevelConfig> CityLevel_CityLevelConfigPy => this._cityLevel_cityLevelConfig;
    public void SetCityLevelConfigs(List<CityLevelConfig> cityLevelConfigs)
    {
        this._cityLevelConfigs = cityLevelConfigs;
    }

    [SerializeField]
    private List<BuildingConfig> _buildingConfigs;
    private Dictionary<BuildingEnum,BuildingConfig> _buildingEnum_buildingConfig;
    public Dictionary<BuildingEnum,BuildingConfig> BuildingEnum_BuildingConfigPy => this._buildingEnum_buildingConfig;
    public void SetBuildingConfigs(List<BuildingConfig> buildingConfigs)
    {
        this._buildingConfigs = buildingConfigs;
    }

    public void Initialize()
    {
        foreach (ScenePrefabConfig scenePrefabConfig in this._scenePrefabConfigs)
        {
            scenePrefabConfig.Initialize();
        }

        this._scenePrefabEnum_scenePrefabConfig = new Dictionary<ScenePrefabEnum,ScenePrefabConfig>(this._scenePrefabConfigs.Count);
        foreach (ScenePrefabConfig scenePrefabConfig in this._scenePrefabConfigs)
        {
            this._scenePrefabEnum_scenePrefabConfig[scenePrefabConfig.ScenePrefabEnumPy] = scenePrefabConfig;
        }

        this._cityEnum_cityConfig = new Dictionary<CityEnum,CityConfig>(this._cityConfigs.Count);
        foreach (CityConfig cityConfig in this._cityConfigs)
        {
            cityConfig.Initialize();
        }
        foreach (CityConfig cityConfig in this._cityConfigs)
        {
            this._cityEnum_cityConfig[cityConfig.CityEnumPy] = cityConfig;
        }

        this._cityLevel_cityLevelConfig = new Dictionary<int,CityLevelConfig>(this._cityLevelConfigs.Count);
        foreach (CityLevelConfig cityLevelConfig in this._cityLevelConfigs)
        {
            this._cityLevel_cityLevelConfig[cityLevelConfig.CityLevelPy] = cityLevelConfig;
        }

        this._buildingEnum_buildingConfig = new Dictionary<BuildingEnum,BuildingConfig>(this._buildingConfigs.Count);
        foreach (BuildingConfig buildingConfig in this._buildingConfigs)
        {
            this._buildingEnum_buildingConfig[buildingConfig.BuildingEnumPy] = buildingConfig;
        }
    }
}
}