using System.Collections.Generic;

using Common.DataTypes;

using Cysharp.Threading.Tasks;

using FairyGUI;

using Sirenix.OdinInspector;

using UICommon;

using UnityEngine;

#pragma warning disable CS4014

namespace LowLevelSystems.UISystems
{
public class DialogueBubblePool
{
    //配置.
    //功能: 一秒几个字.
    private const int _fontNumberPerSec = 10;
    //功能: 一个字间隔多少 ms
    private const int _msPerFont = (int)(1000f / _fontNumberPerSec);
    //功能: 说完后的悬停时间.
    private const int _suspendedMs = 2000;
    //功能: 泡泡的世界偏移坐标.
    private readonly Vector3 _bubbleWorldOffset = new Vector3(0.6f,1.7f,0f);

    //功能: 对话泡泡.
    //Cache. 泡泡需要一个 Parent GComponent.
    [ShowInInspector]
    private GComponent _parentGComponent;
    //功能: 对话泡泡是一个对象池.
    [ShowInInspector]
    private ObjectPool<UI_Component_DialogueBubble> _dialogueBubblePool;
    //功能: 记录当前显示着的泡泡.
    [ShowInInspector]
    private List<UI_Component_DialogueBubble> _visibleBubble;

    //外部 Cache.
    private readonly Camera _camera;
    private readonly GRoot _gRoot;

    private UI_Component_DialogueBubble CreateDialogueBubble()
    {
        UI_Component_DialogueBubble dialogueBubble = UI_Component_DialogueBubble.CreateInstance();
        dialogueBubble.InitializeTypingEffect(new TypingEffect(dialogueBubble.Text_FrameFd));
        this._parentGComponent.AddChild(dialogueBubble);
        return dialogueBubble;
    }

    public DialogueBubblePool(GComponent parentGComponent,Camera camera,GRoot gRoot)
    {
        this._parentGComponent = parentGComponent;
        this._dialogueBubblePool = new ObjectPool<UI_Component_DialogueBubble>(10,this.CreateDialogueBubble);
        this._visibleBubble = new List<UI_Component_DialogueBubble>(10);

        this._camera = camera;
        this._gRoot = gRoot;
    }

    // 设计: 要关闭一个 Bubble, 只需要 cancel 其 TypingEffect 即可, 该 Bubble 生命周期末尾会自动关闭并回到 Pool 中.
    public UI_Component_DialogueBubble ShowDialogueBubbleAsync(Transform targetTransform,string text,string name)
    {
        //获取一个 UI_Component_DialogueBubble.
        UI_Component_DialogueBubble dialogueBubble = this._dialogueBubblePool.GetItemFromPool();
        //更新显示名字
        dialogueBubble.GTextField_Name.text = name;
        //更新显示内容.
        dialogueBubble.Text_FrameFd.text = text;
        //显示 Dialogue.
        dialogueBubble.visible = true;
        //跟随 target.
        this.UpdateUIPositionAsync(dialogueBubble,targetTransform);
        //打字效果.
        this.PrintAndBackToPoolAsync(dialogueBubble);
        //记录进当前显示着的泡泡.
        this._visibleBubble.Add(dialogueBubble);

        return dialogueBubble;
    }

    private async UniTask UpdateUIPositionAsync(UI_Component_DialogueBubble dialogueBubble,Transform targetTransform)
    {
        while (dialogueBubble.visible)
        {
            //计算 FGUI 坐标.
            Vector3 headWorldPosition = targetTransform.position + this._bubbleWorldOffset;
            Vector2 screenPosition = this._camera.WorldToScreenPoint(headWorldPosition);
            Vector2 fguiScreenPosition = new Vector2(screenPosition.x,Screen.height - screenPosition.y);
            Vector2 uiPosition = this._gRoot.GlobalToLocal(fguiScreenPosition);

            //设置坐标. 
            dialogueBubble.SetXY(uiPosition.x,uiPosition.y);
            await UniTask.NextFrame();
        }
    }

    //功能: 文本显示效果.
    private async UniTask PrintAndBackToPoolAsync(UI_Component_DialogueBubble dialogueBubble)
    {
        dialogueBubble.TypingEffectPy.Start();

        while (true)
        {
            //功能: 打完字或者该窗口隐藏时, 不用继续效果.
            if (!dialogueBubble.TypingEffectPy.Print()) break;
            if (!dialogueBubble.visible) break;

            await UniTask.Delay(_msPerFont);
        }

        //打字效果完成后, 悬停一段时间再 关闭.
        await UniTask.Delay(_suspendedMs);

        //功能: 返回进对象池, 且从当前记录的打开着的泡泡中移除.
        dialogueBubble.visible = false;
        dialogueBubble.TypingEffectPy.Cancel();
        this._dialogueBubblePool.ReturnItemToPool(dialogueBubble);
        this._visibleBubble.Remove(dialogueBubble);
    }
}
}