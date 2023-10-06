using System;

using Cysharp.Threading.Tasks;

using FairyGUI;

using LowLevelSystems.Common;

namespace LowLevelSystems.UISystems.WindowSystems
{
public abstract class UiWindowFactory : Details
{
    public static async UniTask<T> GenerateUiWindowAsync<T>(string packageName,BaseUiWindow parentWindow) where T : UiWindow,new()
    {
        T uiWindow = new T();

        //GComponent _selfGCom
        Type type = typeof(T);
        GComponent selfGCom = await GComFactory.GenerateGComForUiWindowAsync(packageName,type.Name);
        uiWindow.SetSelfGCom(selfGCom);
        parentWindow.SelfGComPy.AddChild(selfGCom);

        //BaseUiWindow _parentWindow
        uiWindow.SetParentWindow(parentWindow);

        UiManager.UIWindowHubPy.RecordInstance(uiWindow);

        return uiWindow;
    }
}
}