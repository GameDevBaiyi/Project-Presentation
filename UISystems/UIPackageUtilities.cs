using System.Globalization;
using System.IO;
using System.Text;

using UnityEngine;

namespace LowLevelSystems.UISystems
{
public static class UiPackageUtilities
{
    private static StringBuilder _stringBuilder = new StringBuilder();
    public static string GetDescriptionABName(string packageName)
    {
        _stringBuilder.Clear();

        return _stringBuilder.Append(packageName).Append("DescriptionAB").Append(".bundle").ToString().ToLower(CultureInfo.InvariantCulture);
    }

    public static string GetResourcesABName(string packageName)
    {
        _stringBuilder.Clear();

        return _stringBuilder.Append(packageName).Append("ResourcesAB").Append(".bundle").ToString().ToLower(CultureInfo.InvariantCulture);
    }

    public static string GetABFolder()
    {
        return Path.Combine(Application.streamingAssetsPath,"FGUIAssetBundles");
    }

    public static string GetDescriptionABPath(string packageName)
    {
        return Path.Combine(GetABFolder(),GetDescriptionABName(packageName));
    }

    public static string GetResourcesABPath(string packageName)
    {
        return Path.Combine(GetABFolder(),GetResourcesABName(packageName));
    }
}
}