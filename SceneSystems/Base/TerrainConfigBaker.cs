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

using UnityEditor;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
public static class TerrainConfigBaker
{
    private const string _tbTileRes = "tbTileRes";

    public static void GenerateTileEnum()
    {
        string path = Path.Combine(Application.dataPath,"Scripts/Scripts/LowLevelSystems/SceneSystems/Base/TileEnum.cs");
        JsonData tbTileRes = JsonUtilities.GetJsonData(_tbTileRes);
        List<string> tileEnumStrings = tbTileRes["TileID"].Keys.ToList();
        CodeGenerateUtilities.GenerateEnumFile(path,$"{typeof(TileEnum).Namespace}",$"{nameof(TileEnum)}",tileEnumStrings,Enum.GetNames(typeof(TileEnum)));
        AssetDatabase.ImportAsset(@"Assets/Scripts/Scripts/LowLevelSystems/SceneSystems/Base/TileEnum.cs");
    }

    public static Dictionary<string,bool> GetGroundTileName_Walkable()
    {
        JsonData jsonData = JsonUtilities.GetJsonData(_tbTileRes);
        Dictionary<string,bool> groundTileName_walkable = new Dictionary<string,bool>();
        foreach (string key in jsonData["TileID"].Keys)
        {
            groundTileName_walkable[key] = jsonData["TileID"][key]["IsMove"].ToInt() == 1;
        }
        return groundTileName_walkable;
    }

    public static void BakeTbTileResToTerrainConfig()
    {
        JsonData jsonData = JsonUtilities.GetJsonData(_tbTileRes);
        List<TileConfig> tileConfigRows = new List<TileConfig>();
        foreach (string key in jsonData["TileID"].Keys)
        {
            //每次遍历 Key 都是一个 TileConfigRow.
            TileConfig tileConfig = new TileConfig();
            TileEnum tileEnum = key.ToEnum<TileEnum>();
            int apCostMultiplier = jsonData["TileID"][key]["MoveCost"].ToInt();

            tileConfig.SetTileEnum(tileEnum);
            tileConfig.SetAPCostMultiplier(apCostMultiplier);

            tileConfigRows.Add(tileConfig);
        }

        CommonDesignSO commonDesignSO = DevUtilities.GetCommonConfigSO();
        commonDesignSO.TerrainConfigPy.SetTileConfigRows(tileConfigRows);
        EditorUtility.SetDirty(commonDesignSO);
        AssetDatabase.SaveAssetIfDirty(commonDesignSO);

        Debug.Log($"TerrainConfig 录制成功. ");
    }
}
}
#endif