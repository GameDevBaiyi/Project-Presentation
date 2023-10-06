#if UNITY_EDITOR

using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems;
using LowLevelSystems.Common;
using LowLevelSystems.QualitySystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.CitySystems.Base
{
public static class CityLevelConfigBaker
{
    public const string TbCityLevel = "tbCityLevel";

    public static void BakeTbCityLevelToCityLevelConfig()
    {
        List<CityLevelConfig> cityLevelConfigs = new List<CityLevelConfig>();

        JsonData tbCityLevel = JsonUtilities.GetJsonData(TbCityLevel);

        foreach (string cityLevelString in tbCityLevel["CityLevel"].Keys)
        {
            CityLevelConfig cityLevelConfig = new CityLevelConfig();

            //int _cityLevel
            int cityLevel = int.Parse(cityLevelString);
            cityLevelConfig.SetCityLevel(cityLevel);

            //StoreLevel _storeLevel
            StoreLevel storeLevel = new StoreLevel();
            if (tbCityLevel.TryGetNestedJson(out JsonData shopLevelJson,"CityLevel",cityLevelString,"ShopLevel"))
            {
                storeLevel.Level = shopLevelJson.ToInt();
            }
            cityLevelConfig.SetStoreLevel(storeLevel);

            //int _qualityEnumOfStolenItem
            int qualityEnumOfStolenItem = default(int);
            if (tbCityLevel.TryGetNestedJson(out JsonData qualityEnumOfStolenItemJson,"CityLevel",cityLevelString,"StealQuality"))
            {
                qualityEnumOfStolenItem = qualityEnumOfStolenItemJson.ToInt();
            }
            cityLevelConfig.SetQualityEnumOfStolenItem((QualityEnum)qualityEnumOfStolenItem);
            
            //Vector2Int _rangeOfBountyTasks
            Vector2Int rangeOfBountyTasks = new Vector2Int();
            if (tbCityLevel.TryGetNestedJson(out JsonData rangeOfBountyTasksJson,"CityLevel",cityLevelString,"RMRange"))
            {
                string[] rangeMissionBounty = rangeOfBountyTasksJson.ToString().Split("_");
                rangeOfBountyTasks.x = int.Parse(rangeMissionBounty[0]);
                rangeOfBountyTasks.y = int.Parse(rangeMissionBounty[1]);
            }
            cityLevelConfig.SetRangeOfBountyTasks(rangeOfBountyTasks);
            
            //List<int> _weightsOfBountyTaskLevel
            List<int> weightsOfBountyTaskLevel = new List<int>();
            if (tbCityLevel.TryGetNestedJson(out JsonData weightsOfBountyTaskLevelJson,"CityLevel",cityLevelString,"RMPro"))
            {
                weightsOfBountyTaskLevel = weightsOfBountyTaskLevelJson.ToList<int>();
            }
            cityLevelConfig.SetWeightsOfBountyTaskLevel(weightsOfBountyTaskLevel);
            

            cityLevelConfigs.Add(cityLevelConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.SceneConfigHubPy.SetCityLevelConfigs(cityLevelConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"{TbCityLevel} => {nameof(CityLevelConfig)} 录制成功.");
    }
}
}
#endif