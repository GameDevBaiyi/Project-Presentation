#if UNITY_EDITOR

using System.Collections.Generic;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.Common;
using LowLevelSystems.QualitySystems;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.CharacterSystems.NpcSystems.Components.InteractionsSystems.TradeSystems
{
public abstract class StoreLevelConfigBaker
{
    private const string TbShopQuality = "tbShopQuality";

    public static void BakeTbShopQualityToStoreLevelConfig()
    {
        List<StoreLevelConfig> storeLevelConfigs = new List<StoreLevelConfig>();

        JsonData tbShopQuality = JsonUtilities.GetJsonData(TbShopQuality);

        foreach (string level in tbShopQuality["ShopLevel"].Keys)
        {
            StoreLevelConfig storeLevelConfig = new StoreLevelConfig();

            //int _storeLevel;
            int storeLevel = 0;
            storeLevel = int.Parse(level);

            //List<QualityEnum> _qualityEnums   List<int> _weights
            List<QualityEnum> qualityEnums = new List<QualityEnum>();
            List<int> weights = new List<int>();
            foreach (string quality in tbShopQuality["ShopLevel"][level].Keys)
            {
                QualityEnum qualityEnum = quality.ToEnum<QualityEnum>();
                qualityEnums.Add(qualityEnum);

                if (tbShopQuality.TryGetNestedJson(out JsonData weightJson,"ShopLevel",level,quality))
                {
                    int weight = weightJson.ToInt();
                    weights.Add(weight);
                }
            }

            storeLevelConfig.SetStoreLevel(storeLevel);
            storeLevelConfig.SetQualityEnums(qualityEnums);
            storeLevelConfig.SetWeights(weights);

            storeLevelConfigs.Add(storeLevelConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.InteractionConfigHubPy.SetStoreLevelConfigs(storeLevelConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"{TbShopQuality} => {nameof(StoreLevelConfig)} 录制成功.");
    }
}
}
#endif