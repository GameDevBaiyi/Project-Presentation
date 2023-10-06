#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Common.Extensions;
using Common.Utilities;

using DevTools.ProgrammerTools;

using LitJson;

using LowLevelSystems.Common;
using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
public static class BuildingConfigBaker
{
    public static void GenerateBuildingEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/SceneSystems/BuildingSystems/BuildingEnum.cs");
        JsonData tbBuilding = JsonUtilities.GetJsonData(CityConfigBaker.TbBuild);
        List<string> buildingEnumStrings = tbBuilding["BuildID"].Keys.ToList();
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(BuildingEnum).Namespace}",$"{nameof(BuildingEnum)}",buildingEnumStrings,Enum.GetNames(typeof(BuildingEnum)));
        AssetDatabase.ImportAsset(@"Assets/Scripts/Scripts/LowLevelSystems/SceneSystems/BuildingSystems/BuildingEnum.cs");
    }

    public static void BakeTbBuildToBuildingConfigs()
    {
        List<BuildingConfig> buildingConfigs = new List<BuildingConfig>();
        
        JsonData tbBuild = JsonUtilities.GetJsonData(CityConfigBaker.TbBuild);

        foreach (string buildEnumString in tbBuild["BuildID"].Keys)
        {
            BuildingConfig buildingConfig = new BuildingConfig();
            JsonData buildJson = tbBuild["BuildID"][buildEnumString];
            
            //BuildingEnum _buildingEnum
            BuildingEnum buildingEnum = buildEnumString.ToEnum<BuildingEnum>();
            buildingConfig.SetBuildingEnum(buildingEnum);
            
            //TextId _nameId
            if (buildJson.TryGetNestedJson(out JsonData buildNameJsonData,"Name"))
            {
                TextId textId = new TextId(buildNameJsonData.ToInt());
                buildingConfig.SetNameId(textId);
            }

            buildingConfigs.Add(buildingConfig);
        }
        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.SceneConfigHubPy.SetBuildingConfigs(buildingConfigs);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);
        Debug.Log("BuildingConfig录制成功");
    }

}
}
#endif