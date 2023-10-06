using FairyGUI;

using Sirenix.OdinInspector;

namespace LowLevelSystems.UISystems.WindowSystems
{
public class BaseUiWindow
{
    [ShowInInspector]
    private readonly UiLayerEnum _uiLayerEnum;
    public UiLayerEnum UiLayerEnumPy => this._uiLayerEnum;

    [ShowInInspector]
    private readonly GComponent _selfGCom;
    public GComponent SelfGComPy => this._selfGCom;

    [ShowInInspector]
    private UiWindow _openingChildWindow;
    public UiWindow OpeningChildWindowPy => this._openingChildWindow;
    public void SetOpeningChildWindow(UiWindow openingChildWindow)
    {
        this._openingChildWindow = openingChildWindow;
    }

    public BaseUiWindow(UiLayerEnum uiLayerEnum)
    {
        this._uiLayerEnum = uiLayerEnum;
        this._selfGCom = GComFactory.GenerateGComForBaseWindow(uiLayerEnum);
    }
}
}