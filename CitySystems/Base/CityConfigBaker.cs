#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Common.Extensions;
using Common.Utilities;

using DevTools;
using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEditor;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
public static class CityConfigBaker
{
    public const string TbBuild = "tbBuild";
    public const string TbCity = "tbCity";

    public static void GenerateCityEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/SceneSystems/CitySystems/Base/CityEnum.cs");
        JsonData tbCity = JsonUtilities.GetJsonData(TbCity);
        List<string> cityEnumStrings = tbCity["CityID"].Keys.ToList();
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(CityEnum).Namespace}",$"{nameof(CityEnum)}",cityEnumStrings,Enum.GetNames(typeof(CityEnum)));
        AssetDatabase.ImportAsset(@"Assets/Scripts/Scripts/LowLevelSystems/SceneSystems/CitySystems/Base/CityEnum.cs");
    }

    public static void BakeTbCityAndEditorToCityConfig()
    {
        List<CityConfig> cityConfigs = new List<CityConfig>();
        JsonData tbCity = JsonUtilities.GetJsonData(TbCity);
        foreach (string cityEnumString in tbCity["CityID"].Keys)
        {
            CityConfig cityConfig = new CityConfig();
            JsonData cityConfigJson = tbCity["CityID"][cityEnumString];

            //CityEnum _cityEnum
            CityEnum cityEnum = cityEnumString.ToEnum<CityEnum>();
            cityConfig.SetCityEnum(cityEnum);

            //ScenePrefabEnum _scenePrefabEnum
            ScenePrefabEnum scenePrefabEnum = cityEnumString.ToEnumNoError<ScenePrefabEnum>();
            cityConfig.SetScenePrefabEnum(scenePrefabEnum);

            //string _cityName
            string cityName = default(string);
            if (cityConfigJson.TryGetNestedJson(out JsonData cityNameJson,"Name"))
            {
                cityName = cityNameJson.ToString();
            }
            cityConfig.SetCityName(cityName);

            //TextId _cityIntroduction
            if (cityConfigJson.TryGetNestedJson(out JsonData cityIntroductionJson,"Introduction"))
            {
                TextId textId = new TextId(cityIntroductionJson.ToInt());
                cityConfig.SetCityIntroduction(textId);
            }

            //float _dayTimeStart
            float dayTimeStart = default(float);
            if (cityConfigJson.TryGetNestedJson(out JsonData dayTimeStartJson,"DayTime"))
            {
                dayTimeStart = dayTimeStartJson.ToFloat();
            }
            cityConfig.SetDayTimeStart(dayTimeStart);

            //float _nightTimeStart
            float nightTimeStart = default(float);
            if (cityConfigJson.TryGetNestedJson(out JsonData nightTimeStartJson,"NightTime"))
            {
                nightTimeStart = nightTimeStartJson.ToFloat();
            }
            cityConfig.SetNightTimeStart(nightTimeStart);

            //List<BattleConfigId> _battleListToUnlockLv2
            List<BattleConfigId> battleListToUnlockLv2 = new List<BattleConfigId>();
            if (cityConfigJson.TryGetNestedJson(out JsonData battleListToUnlockLv2Json,"CityDun"))
            {
                battleListToUnlockLv2 = battleListToUnlockLv2Json.ToList<int>().Select(t => new BattleConfigId(t)).ToList();
            }
            cityConfig.SetBattleListToUnlockLv2(battleListToUnlockLv2);

            //CampEnum _initialCampEnum
            CampEnum initialCampEnum = default(CampEnum);
            if (cityConfigJson.TryGetNestedJson(out JsonData initialCampEnumJson,"CountryID"))
            {
                initialCampEnum = initialCampEnumJson.ToInt() == 0 ? CampEnum.Sun : CampEnum.Moon;
            }
            cityConfig.SetInitialCampEnum(initialCampEnum);

            //根据该 City 找到其对应的 EditorPrefab. 
            GameObject editorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Assets/Prefabs/SceneEditorPrefabs/Cities/{cityEnumString}_Editor.prefab");
            if (editorPrefab == null) continue;
            Transform gridTransform = editorPrefab.transform.Find("Grid");
            if (gridTransform == null) continue;
            Grid grid = gridTransform.GetComponent<Grid>();
            Transform entitiesTransform = gridTransform.Find("Entities");
            Tilemap entityTilemap = entitiesTransform.GetComponent<Tilemap>();
            if (entitiesTransform == null) continue;
            Transform commonBuildingsTransform = entitiesTransform.Find("CommonBuildings");
            if (commonBuildingsTransform == null) continue;
            Transform statefulBuildingsTransform = entitiesTransform.Find("StatefulBuildings");
            if (statefulBuildingsTransform == null) continue;

            List<BuildingMb> commonBuildingMbs = commonBuildingsTransform.GetComponentsInChildren<BuildingMb>().ToList();
            List<BuildingInstanceConfig> commonBuildingInstanceConfigs = ScenePrefabConfigBaker.RecordBuildingInstanceConfigs(grid,entityTilemap,commonBuildingMbs,cityEnumString);
            cityConfig.SetBuildingInstanceConfigs(commonBuildingInstanceConfigs);

            int buildingStateCount = statefulBuildingsTransform.childCount;
            List<CityConfig.BuildingStateConfig> buildingStateConfigs = new List<CityConfig.BuildingStateConfig>(buildingStateCount);
            cityConfig.SetBuildingStateConfigs(buildingStateConfigs);
            for (int i = 0; i < buildingStateCount; i++)
            {
                CityConfig.BuildingStateConfig buildingStateConfig = new CityConfig.BuildingStateConfig();
                Transform singleStateTransform = statefulBuildingsTransform.GetChild(i);
                int stageCount = singleStateTransform.childCount;
                List<CityConfig.BuildingStageConfig> buildingStageConfigs = new List<CityConfig.BuildingStageConfig>(stageCount);
                buildingStateConfig.SetList(buildingStageConfigs);
                buildingStateConfigs.Add(buildingStateConfig);

                for (int stageId = 0; stageId < stageCount; stageId++)
                {
                    CityConfig.BuildingStageConfig buildingStageConfig = new CityConfig.BuildingStageConfig();
                    Transform singleStageTransform = singleStateTransform.GetChild(stageId);
                    List<BuildingMb> buildingMbs = singleStageTransform.GetComponentsInChildren<BuildingMb>().ToList();
                    List<BuildingInstanceConfig> buildingInstanceConfigs = ScenePrefabConfigBaker.RecordBuildingInstanceConfigs(grid,entityTilemap,buildingMbs,cityEnumString);
                    buildingStageConfig.SetList(buildingInstanceConfigs);
                    buildingStageConfigs.Add(buildingStageConfig);
                }
            }

            //Vector3Int _hotelCoord
            BuildingInstanceConfig hotelConfig = commonBuildingInstanceConfigs.FirstOrDefault(t => t.BuildingEnumPy == BuildingEnum.Hotel);
            if (hotelConfig == null)
            {
                Debug.LogError($"该 City: {cityEnumString} 没有旅店. ");
            }
            Vector3Int hotelCoord = hotelConfig?.CoordPy ?? default(Vector3Int);
            cityConfig.SetHotelCoord(hotelCoord);

            //List<InitialCharacterData> _initialCharacters.
            List<InitialCharacterData> initialCharacters = new List<InitialCharacterData>();
            cityConfig.SetInitialCharacters(initialCharacters);
            Transform charactersParentTransform = editorPrefab.transform.Find("Characters");
            if (charactersParentTransform != null)
            {
                int characterCount = charactersParentTransform.transform.childCount;
                for (int i = 0; i < characterCount; i++)
                {
                    InitialCharacterData initialCharacterData = new InitialCharacterData();
                    Transform characterMbTransform = charactersParentTransform.transform.GetChild(i);
                    if (!characterMbTransform.TryGetComponent(out InitialCharacterDataMb characterDataMb))
                    {
                        Debug.LogError($"{editorPrefab} 的 {charactersParentTransform} 下的角色找不到 {nameof(InitialCharacterDataMb)}");
                        continue;
                    }

                    initialCharacterData.SetInstanceId(characterDataMb.InstanceId);
                    initialCharacterData.SetCoord(grid.WorldToCell(characterDataMb.transform.position));
                    initialCharacterData.SetDirection(characterDataMb.IsToLeft ? 1 : 5);
                    initialCharacterData.SetIsStill(characterDataMb.IsStill);
                    initialCharacters.Add(initialCharacterData);
                }
            }

            //List<BattleConfigLibrary> _battleConfigLibraries
            List<CityConfig.BattleConfigLibrary> battleConfigLibraries = new List<CityConfig.BattleConfigLibrary>();
            if (cityConfigJson.TryGetNestedJson(out JsonData _,"RanDun"))
            {
                foreach (string ranDunString in cityConfigJson["RanDun"]["0"].Keys)
                {
                    JsonData ranDunJson = cityConfigJson["RanDun"]["0"];
                    if (ranDunJson.TryGetNestedJson(out JsonData battleIdsString,ranDunString))
                    {
                        CityConfig.BattleConfigLibrary battleConfigLibrary = new CityConfig.BattleConfigLibrary();
                        string[] ranDunProperty = ranDunString.Split("_");
                        //CampEnum _campEnum
                        CampEnum campEnum = (CampEnum)int.Parse(ranDunProperty[1]);
                        battleConfigLibrary.SetCampEnum(campEnum);
                        //BattleConfig.BattleTypeEnum _battleTypeEnum
                        BattleConfig.BattleTypeEnum battleTypeEnum = ranDunProperty[2].ToEnum<BattleConfig.BattleTypeEnum>();
                        battleConfigLibrary.SetBattleTypeEnum(battleTypeEnum);
                        //List<BattleConfigId> _battleConfigIds
                        List<BattleConfigId> battleConfigIds = new List<BattleConfigId>();
                        List<int> ids = battleIdsString.ToList<int>();
                        foreach (int id in ids)
                        {
                            BattleConfigId battleConfigId = new BattleConfigId(id);
                            battleConfigIds.Add(battleConfigId);
                        }
                        battleConfigLibrary.SetBattleConfigIds(battleConfigIds);
                        //加入List
                        battleConfigLibraries.Add(battleConfigLibrary);
                    }
                }
            }
            cityConfig.SetBattleConfigLibraries(battleConfigLibraries);
            
            //List<ScenePrefabLibrary> _scenePrefabLibraries
            List<CityConfig.ScenePrefabLibrary> scenePrefabLibraries = new List<CityConfig.ScenePrefabLibrary>();
            if (cityConfigJson.TryGetNestedJson(out JsonData _,"RanMap"))
            {
                foreach (string ranMapString in cityConfigJson["RanMap"]["0"].Keys)
                {
                    JsonData ranMapJson = cityConfigJson["RanMap"]["0"];
                    if (ranMapJson.TryGetNestedJson(out JsonData scenePrefabEnumsString,ranMapString))
                    {
                        CityConfig.ScenePrefabLibrary scenePrefabLibrary = new CityConfig.ScenePrefabLibrary();
                        string[] ranMapProperty = ranMapString.Split("_");
                        //BattleConfig.BattleTypeEnum _battleTypeEnum
                        BattleConfig.BattleTypeEnum battleTypeEnum = ranMapProperty[1].ToEnum<BattleConfig.BattleTypeEnum>();
                        scenePrefabLibrary.SetBattleTypeEnum(battleTypeEnum);
                        //List<ScenePrefabEnum> _scenePrefabEnums
                        List<ScenePrefabEnum> scenePrefabEnums = new List<ScenePrefabEnum>();
                        scenePrefabEnums = scenePrefabEnumsString.ToList<ScenePrefabEnum>();
                        scenePrefabLibrary.SetScenePrefabEnums(scenePrefabEnums);
                        //加入List
                        scenePrefabLibraries.Add(scenePrefabLibrary);
                    }
                }
            }
            cityConfig.SetScenePrefabLibraries(scenePrefabLibraries);

            cityConfigs.Add(cityConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.SceneConfigHubPy.SetCityConfigs(cityConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        DebugCharacterIds(cityConfigs);
        Debug.Log($"{nameof(CityConfig)} 录制成功. ");
    }

    private static void DebugCharacterIds(List<CityConfig> cityConfigs)
    {
        HashSet<int> instanceIds = new HashSet<int>();
        HashSet<int> recordInstanceIds = new HashSet<int>();
        //第一次遍历记录所有的InstanceId
        foreach (CityConfig cityConfig in cityConfigs)
        {
            foreach (InitialCharacterData initialCharacterData in cityConfig.InitialCharactersPy)
            {
                if (!instanceIds.Contains(initialCharacterData.InstanceIdPy))
                {
                    instanceIds.Add(initialCharacterData.InstanceIdPy);
                }
                else
                {
                    recordInstanceIds.Add(initialCharacterData.InstanceIdPy);
                }
            }
            foreach (BuildingInstanceConfig buildingInstanceConfig in cityConfig.CommonBuildingInstanceConfigsPy)
            {
                foreach (RoomInstanceConfig roomInstanceConfig in buildingInstanceConfig.RoomConfigsPy)
                {
                    foreach (InitialCharacterData initialCharacterData in roomInstanceConfig.InitialCharactersPy)
                    {
                        if (!instanceIds.Contains(initialCharacterData.InstanceIdPy))
                        {
                            instanceIds.Add(initialCharacterData.InstanceIdPy);
                        }
                        else
                        {
                            recordInstanceIds.Add(initialCharacterData.InstanceIdPy);
                        }
                    }
                }
            }
        }

        //第二次遍历找出有问题数据的错误地方
        foreach (CityConfig cityConfig in cityConfigs)
        {
            foreach (InitialCharacterData initialCharacterData in cityConfig.InitialCharactersPy)
            {
                if (recordInstanceIds.Contains(initialCharacterData.InstanceIdPy))
                {
                    Debug.LogError($"InstanceId:{initialCharacterData.InstanceIdPy}在{cityConfig.CityNamePy}出现");
                }
            }
            foreach (BuildingInstanceConfig buildingInstanceConfig in cityConfig.CommonBuildingInstanceConfigsPy)
            {
                foreach (RoomInstanceConfig roomInstanceConfig in buildingInstanceConfig.RoomConfigsPy)
                {
                    foreach (InitialCharacterData initialCharacterData in roomInstanceConfig.InitialCharactersPy)
                    {
                        if (recordInstanceIds.Contains(initialCharacterData.InstanceIdPy))
                        {
                            Debug.LogError($"InstanceId:{initialCharacterData.InstanceIdPy}在{cityConfig.CityNamePy}的{buildingInstanceConfig.BuildingEnumPy}建筑的{roomInstanceConfig.ScenePrefabEnumPy}号房出现");
                        }
                    }
                }
            }
        }
    }

    public static void BakeTbCityAndBuildingConfig()
    {
        BakeTbCityAndEditorToCityConfig();
        BuildingConfigBaker.BakeTbBuildToBuildingConfigs();
    }
}
}
#endif