using System;
using System.Collections.Generic;
using System.Linq;

using Common.Extensions;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;

using Sirenix.OdinInspector;

using UnityEngine;

using Random = UnityEngine.Random;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
[Serializable]
public class CityConfig
{
    [Serializable]
    public class BattleConfigLibrary
    {
        [SerializeField]
        private CampEnum _campEnum;
        public CampEnum CampEnumPy => this._campEnum;
        public void SetCampEnum(CampEnum campEnum)
        {
            this._campEnum = campEnum;
        }

        [SerializeField]
        private BattleConfig.BattleTypeEnum _battleTypeEnum;
        public BattleConfig.BattleTypeEnum BattleTypeEnumPy => this._battleTypeEnum;
        public void SetBattleTypeEnum(BattleConfig.BattleTypeEnum battleTypeEnum)
        {
            this._battleTypeEnum = battleTypeEnum;
        }

        [SerializeField]
        private List<BattleConfigId> _battleConfigIds = new List<BattleConfigId>();
        public List<BattleConfigId> BattleConfigIdsPy => this._battleConfigIds;
        public void SetBattleConfigIds(List<BattleConfigId> battleConfigIds)
        {
            this._battleConfigIds = battleConfigIds;
        }
    }

    [Serializable]
    public class ScenePrefabLibrary
    {
        [SerializeField]
        private BattleConfig.BattleTypeEnum _battleTypeEnum;
        public BattleConfig.BattleTypeEnum BattleTypeEnumPy => this._battleTypeEnum;
        public void SetBattleTypeEnum(BattleConfig.BattleTypeEnum battleTypeEnum)
        {
            this._battleTypeEnum = battleTypeEnum;
        }

        [SerializeField]
        private List<ScenePrefabEnum> _scenePrefabEnums = new List<ScenePrefabEnum>();
        public List<ScenePrefabEnum> ScenePrefabEnumsPy => this._scenePrefabEnums;
        public void SetScenePrefabEnums(List<ScenePrefabEnum> scenePrefabEnums)
        {
            this._scenePrefabEnums = scenePrefabEnums;
        }
    }

    [SerializeField]
    private CityEnum _cityEnum;
    public CityEnum CityEnumPy => this._cityEnum;
    public void SetCityEnum(CityEnum cityEnum)
    {
        this._cityEnum = cityEnum;
    }

    [SerializeField]
    private ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;
    public void SetScenePrefabEnum(ScenePrefabEnum scenePrefabEnum)
    {
        this._scenePrefabEnum = scenePrefabEnum;
    }

    [SerializeField]
    private string _cityName;
    public string CityNamePy => this._cityName;
    public void SetCityName(string cityName)
    {
        this._cityName = cityName;
    }

    [SerializeField]
    private TextId _cityIntroduction;
    public TextId CityIntroductionPy => this._cityIntroduction;
    public void SetCityIntroduction(TextId cityIntroduction)
    {
        this._cityIntroduction = cityIntroduction;
    }

    [SerializeField]
    private float _dayTimeStart;
    public float DayTimeStartPy => this._dayTimeStart;
    public void SetDayTimeStart(float dayTimeStart)
    {
        this._dayTimeStart = dayTimeStart;
    }

    [SerializeField]
    private float _nightTimeStart;
    public float NightTimeStartPy => this._nightTimeStart;
    public void SetNightTimeStart(float nightTimeStart)
    {
        this._nightTimeStart = nightTimeStart;
    }

    [SerializeField]
    private List<BattleConfigId> _battleListToUnlockLv2;
    public List<BattleConfigId> BattleListToUnlockLv2Py => this._battleListToUnlockLv2;
    public void SetBattleListToUnlockLv2(List<BattleConfigId> battleListToUnlockLv2)
    {
        this._battleListToUnlockLv2 = battleListToUnlockLv2;
    }

    [SerializeField]
    private CampEnum _initialCampEnum;
    public CampEnum InitialCampEnumPy => this._initialCampEnum;
    public void SetInitialCampEnum(CampEnum initialCampEnum)
    {
        this._initialCampEnum = initialCampEnum;
    }

    [SerializeField]
    private Vector3Int _hotelCoord;
    public Vector3Int HotelCoordPy => this._hotelCoord;
    public void SetHotelCoord(Vector3Int hotelCoord)
    {
        this._hotelCoord = hotelCoord;
    }

    [SerializeField]
    private List<BuildingInstanceConfig> _commonBuildingInstanceConfigs;
    public List<BuildingInstanceConfig> CommonBuildingInstanceConfigsPy => this._commonBuildingInstanceConfigs;
    public void SetBuildingInstanceConfigs(List<BuildingInstanceConfig> buildingInstanceConfigs)
    {
        this._commonBuildingInstanceConfigs = buildingInstanceConfigs;
    }

    /// <summary>
    /// 一个建筑阶段, 有多个建筑实例.
    /// </summary>
    [Serializable]
    public class BuildingStageConfig
    {
        [SerializeField]
        private List<BuildingInstanceConfig> _list = new List<BuildingInstanceConfig>();
        public List<BuildingInstanceConfig> ListPy => this._list;
        public void SetList(List<BuildingInstanceConfig> list)
        {
            this._list = list;
        }
    }

    /// <summary>
    /// 一个建筑状态, 有多个建筑阶段.
    /// </summary>
    [Serializable]
    public class BuildingStateConfig
    {
        [SerializeField]
        private List<BuildingStageConfig> _list = new List<BuildingStageConfig>();
        public List<BuildingStageConfig> ListPy => this._list;
        public void SetList(List<BuildingStageConfig> list)
        {
            this._list = list;
        }
    }

    [SerializeField]
    private List<BuildingStateConfig> _buildingStateConfigs = new List<BuildingStateConfig>();
    public List<BuildingStateConfig> BuildingStateConfigsPy => this._buildingStateConfigs;
    public void SetBuildingStateConfigs(List<BuildingStateConfig> buildingStateConfigs)
    {
        this._buildingStateConfigs = buildingStateConfigs;
    }

    [SerializeField]
    private List<InitialCharacterData> _initialCharacters;
    public List<InitialCharacterData> InitialCharactersPy => this._initialCharacters;
    public void SetInitialCharacters(List<InitialCharacterData> initialCharacters)
    {
        this._initialCharacters = initialCharacters;
    }

    [SerializeField]
    private List<BattleConfigLibrary> _battleConfigLibraries = new List<BattleConfigLibrary>();
    private Dictionary<CampEnum,Dictionary<BattleConfig.BattleTypeEnum,List<BattleConfigId>>> _campEnum_battleTypeEnum_battleConfigIds;
    public void SetBattleConfigLibraries(List<BattleConfigLibrary> battleConfigLibraries)
    {
        this._battleConfigLibraries = battleConfigLibraries;
    }
    public BattleConfigId GetRandomBattleConfigId(CampEnum campEnum,BattleConfig.BattleTypeEnum battleTypeEnum)
    {
        bool isToGetCommon = Random.Range(0,2) == 0;
        if (isToGetCommon)
        {
            campEnum = CampEnum.None;
        }

        if (!this._campEnum_battleTypeEnum_battleConfigIds.TryGetValue(campEnum,out Dictionary<BattleConfig.BattleTypeEnum,List<BattleConfigId>> battleTypeEnum_battleConfigIds))
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的 Camp: {campEnum} 的 BattleConfigIds, 但未找到. ");
            battleTypeEnum_battleConfigIds = this._campEnum_battleTypeEnum_battleConfigIds.First().Value;
        }

        if (!battleTypeEnum_battleConfigIds.TryGetValue(battleTypeEnum,out List<BattleConfigId> battleConfigIds))
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的 Camp: {campEnum} 的 BattleTypeEnum: {battleTypeEnum} 的 BattleConfigIds, 但未找到. ");
            battleConfigIds = battleTypeEnum_battleConfigIds.First().Value;
        }

        return battleConfigIds.GetRandomItem();
    }

    [SerializeField]
    private List<ScenePrefabLibrary> _scenePrefabLibraries = new List<ScenePrefabLibrary>();
    private Dictionary<BattleConfig.BattleTypeEnum,List<ScenePrefabEnum>> _battleTypeEnum_scenePrefabEnums;
    public void SetScenePrefabLibraries(List<ScenePrefabLibrary> scenePrefabLibraries)
    {
        this._scenePrefabLibraries = scenePrefabLibraries;
    }
    public ScenePrefabEnum GetRandomScenePrefabEnum(BattleConfig.BattleTypeEnum battleTypeEnum)
    {
        if (!this._battleTypeEnum_scenePrefabEnums.TryGetValue(battleTypeEnum,out List<ScenePrefabEnum> scenePrefabEnums))
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的 BattleTypeEnum: {battleTypeEnum} 的 ScenePrefabEnums, 但未找到. ");
            scenePrefabEnums = this._battleTypeEnum_scenePrefabEnums.First().Value;
        }

        return scenePrefabEnums.GetRandomItem();
    }

    public void Initialize()
    {
        this._campEnum_battleTypeEnum_battleConfigIds = new Dictionary<CampEnum,Dictionary<BattleConfig.BattleTypeEnum,List<BattleConfigId>>>();
        foreach (BattleConfigLibrary battleConfigLibrary in this._battleConfigLibraries)
        {
            if (!this._campEnum_battleTypeEnum_battleConfigIds.ContainsKey(battleConfigLibrary.CampEnumPy))
            {
                this._campEnum_battleTypeEnum_battleConfigIds.Add(battleConfigLibrary.CampEnumPy,new Dictionary<BattleConfig.BattleTypeEnum,List<BattleConfigId>>());
            }
            this._campEnum_battleTypeEnum_battleConfigIds[battleConfigLibrary.CampEnumPy].Add(battleConfigLibrary.BattleTypeEnumPy,battleConfigLibrary.BattleConfigIdsPy);
        }

        this._battleTypeEnum_scenePrefabEnums = new Dictionary<BattleConfig.BattleTypeEnum,List<ScenePrefabEnum>>();
        foreach (ScenePrefabLibrary scenePrefabLibrary in this._scenePrefabLibraries)
        {
            this._battleTypeEnum_scenePrefabEnums.Add(scenePrefabLibrary.BattleTypeEnumPy,scenePrefabLibrary.ScenePrefabEnumsPy);
        }
    }

    [Title("Methods")]
    public BuildingInstanceConfig GetBuildingInstanceConfigBy((bool IsCommon,int IdInCommonList,int StateId,int StageId,int IdInStage) key)
    {
        if (key.IsCommon)
        {
            //Debug
            if (key.IdInCommonList < 0
             || key.IdInCommonList >= this._commonBuildingInstanceConfigs.Count)
            {
                Debug.LogError($"尝试获取该 City: {this._cityEnum} 的通用建筑: {key.IdInCommonList}, 但未找到. ");
                return null;
            }

            return this._commonBuildingInstanceConfigs[key.IdInCommonList];
        }

        if (key.StateId < 0
         || key.StateId >= this._buildingStateConfigs.Count)
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的该状态的建筑: {key.StateId}, 但未找到. ");
            return null;
        }

        List<BuildingStageConfig> buildingStageConfigs = this._buildingStateConfigs[key.StateId].ListPy;
        if (key.StageId < 0
         || key.StageId >= buildingStageConfigs.Count)
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的该状态: {key.StateId} 该阶段: {key.StageId} 的建筑: , 但未找到. ");
            return null;
        }

        List<BuildingInstanceConfig> buildingInstanceConfigs = buildingStageConfigs[key.StageId].ListPy;
        if (key.IdInStage < 0
         || key.IdInStage >= buildingInstanceConfigs.Count)
        {
            Debug.LogError($"尝试获取该 City: {this._cityEnum} 的该状态: {key.StateId} 该阶段: {key.StageId} 的该 Id: {key.IdInStage} 的建筑: , 但未找到. ");
            return null;
        }

        return buildingInstanceConfigs[key.IdInStage];
    }
}
}