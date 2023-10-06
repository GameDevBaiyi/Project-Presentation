using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.UISystems.WindowSystems
{
public class UiWindowHub
{
    [Title("Data")]
    [ShowInInspector]
    private readonly Dictionary<UiLayerEnum,BaseUiWindow> _uiLayerEnum_baseWindow;
    public Dictionary<UiLayerEnum,BaseUiWindow> UiLayerEnum_BaseWindowPy => this._uiLayerEnum_baseWindow;

    [ShowInInspector]
    private readonly Dictionary<Type,UiWindow> _type_uiWindow;
    public Dictionary<Type,UiWindow> Type_UIWindowPy => this._type_uiWindow;

    public UiWindowHub()
    {
        UiLayerEnum[] uiLayerEnums = (UiLayerEnum[])Enum.GetValues(typeof(UiLayerEnum));
        this._uiLayerEnum_baseWindow = new Dictionary<UiLayerEnum,BaseUiWindow>(uiLayerEnums.Length - 1);
        this._type_uiWindow = new Dictionary<Type,UiWindow>(50);

        //生成 BaseUiWindow.
        foreach (UiLayerEnum uiLayerEnum in uiLayerEnums)
        {
            if (uiLayerEnum == UiLayerEnum.None) continue;
            BaseUiWindow baseUiWindow = new BaseUiWindow(uiLayerEnum);
            this._uiLayerEnum_baseWindow[uiLayerEnum] = baseUiWindow;
        }
    }

    [Title("Methods")]
    public void RecordInstance(UiWindow instance)
    {
        Type type = instance.GetType();
        if (this._type_uiWindow.ContainsKey(type))
        {
            Debug.LogError($"该 UiWindow: {type.Name} 已经存在, 不应该再次生成, 重复记录.");
        }
        else
        {
            this._type_uiWindow[type] = instance;
        }
    }

    public async UniTask<T> GetWindow<T>(string packageName,UiLayerEnum uiLayerEnum) where T : UiWindow,new()
    {
        //如果该类型窗口已经有了.
        if (this._type_uiWindow.TryGetValue(typeof(T),out UiWindow uiWindow))
        {
            //Debug.
            if (uiWindow.ParentWindowPy.UiLayerEnumPy != uiLayerEnum)
            {
                Debug.LogError($"UiWindow 的设计是初次确定 Layer 后无法更改. 传入的 Layer 为: {uiLayerEnum}. 但先前是: {uiWindow.ParentWindowPy.UiLayerEnumPy}");
            }
            return uiWindow as T;
        }

        return await UiWindowFactory.GenerateUiWindowAsync<T>(packageName,uiLayerEnum.BaseUiWindow());
    }

    /// <summary>
    /// 显示 Window 的机制: 关闭同层级的当前显示的 Window. 然后再打开记录此 Window.
    /// </summary>
    public void Show(UiWindow uiWindow)
    {
        BaseUiWindow parentWindow = uiWindow.ParentWindowPy;
        //Debug.
        if (parentWindow == null)
        {
            Debug.LogError($"可以显示的 UiWindow 应该必定有 ParentWindow. 出问题的 UiWindow 叫做: {uiWindow.SelfGComPy.gameObjectName}. ");
            return;
        }

        //如果当前有同层级的显示着的 Window. 就先关闭.
        if (parentWindow.OpeningChildWindowPy != null)
        {
            this.Hide(parentWindow.OpeningChildWindowPy);
        }

        uiWindow.SelfGComPy.visible = true;
        parentWindow.SetOpeningChildWindow(uiWindow);

        if (!uiWindow.HasInitializedPy)
        {
            uiWindow.OnInitialize();
            uiWindow.SetHasInitialize(true);
        }
        uiWindow.OnOpen();
    }

    /// <summary>
    /// 关闭 Window 的机制: 隐藏自身 GCom 并且 ParentWindow 记录对应行为. 
    /// </summary>
    public void Hide(UiWindow uiWindow)
    {
        uiWindow.SelfGComPy.visible = false;
        BaseUiWindow parentWindow = uiWindow.ParentWindowPy;
        //Debug.
        if (parentWindow == null)
        {
            Debug.LogError($"可以显示的 UiWindow 应该必定有 ParentWindow. 出问题的 UiWindow 叫做: {uiWindow.SelfGComPy.gameObjectName}. ");
            return;
        }
        parentWindow.SetOpeningChildWindow(null);

        uiWindow.OnClose();
    }

    public void CloseAllButLoadingsAndPromptWindows()
    {
        foreach (BaseUiWindow baseUiWindow in this._uiLayerEnum_baseWindow.Values)
        {
            if (baseUiWindow.UiLayerEnumPy is UiLayerEnum.Prompt or UiLayerEnum.Loading) continue;
            if (baseUiWindow.OpeningChildWindowPy == null) continue;
            this.Hide(baseUiWindow.OpeningChildWindowPy);
        }
    }
}
}