using Cysharp.Threading.Tasks;

using FairyGUI;

using LowLevelSystems.Common;

namespace LowLevelSystems.UISystems
{
public abstract class GComFactory : Details
{
    // 机制: 所有的 GComponent 都通过以下去生成.  

    /// <summary>
    /// 直接 New 出 GCom. 只有 Base UiWindow 需要此方式. 
    /// </summary>
    public static GComponent GenerateGComForBaseWindow(UiLayerEnum uiLayerEnum)
    {
        GComponent gCom = new GComponent { name = uiLayerEnum.ToString(),gameObjectName = uiLayerEnum.ToString(),fairyBatching = true,sortingOrder = (int)uiLayerEnum * 100, };
        GRoot gRoot = GRoot.inst;
        gCom.SetSize(gRoot.width,gRoot.height);
        gCom.AddRelation(gRoot,RelationType.Size);
        gRoot.AddChild(gCom);

        return gCom;
    }

    /// <summary>
    /// 为 Non Base UiWindow 生成 GCom. 
    /// </summary>
    public static async UniTask<GComponent> GenerateGComForUiWindowAsync(string packageName,string resourceName)
    {
        await UiManager.UIPackageManagerPy.AddPackageAsync(packageName);
        GObject gObject = UIPackage.CreateObject(packageName,resourceName);
        if (gObject == null) return null;
        gObject.visible = false;
        GComponent gCom = gObject as GComponent;
        GRoot gRoot = GRoot.inst;
        // ReSharper disable once PossibleNullReferenceException
        gCom.SetSize(gRoot.width,gRoot.height);
        gCom.AddRelation(gRoot,RelationType.Size);

        return gCom;
    }

    /// <summary>
    /// 生成非 Window 类型 的 GCom.
    /// </summary>
    public static async UniTask<GComponent> GenerateGComForNonWindow(string packageName,string resourceName)
    {
        await UiManager.UIPackageManagerPy.AddPackageAsync(packageName);
        GObject gObject = UIPackage.CreateObject(packageName,resourceName);
        if (gObject == null) return null;
        GComponent gCom = gObject as GComponent;
        return gCom;
    }
}
}