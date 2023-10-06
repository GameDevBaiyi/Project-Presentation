using System.Collections.Generic;
using System.IO;

using Cysharp.Threading.Tasks;

using FairyGUI;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.UISystems
{
public class UiPackageManager
{
    [Title("Data")]
    private readonly Dictionary<string,UIPackage> _name_uiPackages = new Dictionary<string,UIPackage>(20);

    /// <summary>
    /// 机制1: 初始时, 加载所有图片资源包. GCom 包只加载一个.
    /// </summary>
    [Title("Methods")]
    public async UniTask InitializeAsync()
    {
        //先加载所有的 图片资源包.
        await this.AddPackageAsync(UiConst.TexturesPackage);
        await this.AddPackageAsync(UiConst.ResIcon);
        await this.AddPackageAsync(UiConst.ResCartoon);
        await this.AddPackageAsync(UiConst.ResMap);
        await this.AddPackageAsync(UiConst.ResCityMap);
        await this.AddPackageAsync(UiConst.ResTeaching);
        await this.AddPackageAsync(UiConst.CharacterPackage);
        //GCom 包只加载一个 UiCommon 即可.
        await this.AddPackageAsync(UiConst.UICommonPackage);
    }

    // 添加 UiPackage 的机制. 
    public async UniTask AddPackageAsync(string packageName)
    {
        //已添加就跳过.
        if (this._name_uiPackages.ContainsKey(packageName)) return;

        //加载 定义包.
        string descriptionABPath = UiPackageUtilities.GetDescriptionABPath(packageName);
        //Debug. 定义包一定有, 图片包可能没有.
        if (!File.Exists(descriptionABPath))
        {
            Debug.LogError($"该 Package: {packageName} 未找到定义文件包. 路径: {descriptionABPath}. ");
            return;
        }
        AssetBundle descriptionAB = await AssetBundle.LoadFromFileAsync(descriptionABPath);

        //加载图片包.
        string resourcesABPath = UiPackageUtilities.GetResourcesABPath(packageName);
        AssetBundle resourcesAB = File.Exists(resourcesABPath) ? await AssetBundle.LoadFromFileAsync(resourcesABPath) : null;

        //添加 UiPackage 并记录.
        UIPackage uiPackage = UIPackage.AddPackage(descriptionAB,resourcesAB);
        //Debug. 
        if (uiPackage == null)
        {
            Debug.LogError($"该 Package: {packageName} 对应的 UIPackage 加载失败: 定义包路径: {descriptionABPath} ,资源包路径: {resourcesABPath}");
            return;
        }
        this._name_uiPackages[packageName] = uiPackage;
    }
}
}