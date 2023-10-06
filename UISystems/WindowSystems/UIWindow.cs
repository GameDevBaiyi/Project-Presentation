using FairyGUI;

using Sirenix.OdinInspector;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.UISystems.WindowSystems
{
public class UiWindow
{
    [Title("Data")]
    [ShowInInspector]
    protected GComponent _selfGCom;
    public GComponent SelfGComPy => this._selfGCom;
    public void SetSelfGCom(GComponent selfGCom)
    {
        this._selfGCom = selfGCom;
    }

    [ShowInInspector]
    protected BaseUiWindow _parentWindow;
    public BaseUiWindow ParentWindowPy => this._parentWindow;
    public void SetParentWindow(BaseUiWindow parentWindow)
    {
        this._parentWindow = parentWindow;
    }

    [ShowInInspector]
    protected bool _hasInitialized;
    public bool HasInitializedPy => this._hasInitialized;
    public void SetHasInitialize(bool hasInitialized)
    {
        this._hasInitialized = hasInitialized;
    }

    /// <summary>
    /// 只有第一次打开时会调用.
    /// </summary>
    public virtual void OnInitialize()
    {
    }

    /// <summary>
    /// 每次打开时调用.
    /// </summary>
    public virtual void OnOpen()
    {
    }

    /// <summary>
    /// 每次关闭时调用.
    /// </summary>
    public virtual void OnClose()
    {
    }
}

public abstract class UIWindow<T> : UiWindow where T : GComponent
{
    private T _view;
    public T ViewPy => this._view ??= this._selfGCom as T;
}
}