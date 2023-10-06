using Common.Utilities;

using CommonPromptPackage;

using FairyGUI;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.UISystems
{
public class PromptOnMousePos
{
    [Title("Data")]
    private readonly UI_TipsPrompt _promptGComponent;

    public PromptOnMousePos(GComponent parentGComponent)
    {
        this._promptGComponent = UI_TipsPrompt.CreateInstance();
        parentGComponent.AddChild(this._promptGComponent);
        this._promptGComponent.visible = false;
    }

    // 机制: 在鼠标位置显示, 做动画, 然后隐藏.
    private void Hide()
    {
        this._promptGComponent.visible = false;
    }
    public void Show(string text)
    {
        Vector2 fguiPos = FGUIUtilities.MousePosToFGUI();
        this._promptGComponent.SetXY(fguiPos.x,fguiPos.y);
        this._promptGComponent.GTextField_TipsFd.text = text;
        this._promptGComponent.visible = true;
        this._promptGComponent.t0.Stop();
        this._promptGComponent.t0.Play(this.Hide);
    }
}
}