using Cysharp.Threading.Tasks;

using FairyGUI;

using Sirenix.OdinInspector;

namespace LowLevelSystems.UISystems
{
public class Typing
{
    //预设.
    [ShowInInspector]
    private readonly int _printInterval;

    //打字功能.
    [ShowInInspector]
    private readonly GTextField _gTextField;
    [ShowInInspector]
    private readonly TypingEffect _typingEffect;
#region 数据恢复.
    public Typing(int charactersPerSec,GTextField gTextField)
    {
        this._printInterval = (int)(1000f / charactersPerSec);

        this._gTextField = gTextField;
        this._typingEffect = new TypingEffect(gTextField);
    }
#endregion

    [ShowInInspector]
    private bool _isTyping;
    public bool IsTypingPy => this._isTyping;

    public async UniTask BeginTypingAsync(string text)
    {
        this._isTyping = true;
        this._gTextField.text = text;
        this._typingEffect.Start();

        while (true)
        {
            bool hasFinished = !this._typingEffect.Print();
            if (hasFinished) break;

            await UniTask.Delay(this._printInterval);
        }

        this.OnTypingStopped();
    }
    public async UniTask StopTypingAsync()
    {
        this._typingEffect.Cancel();

        while (true)
        {
            if (!this._isTyping) break;

            await UniTask.NextFrame();
        }
    }

    private void OnTypingStopped()
    {
        this._typingEffect.Cancel();
        this._isTyping = false;
    }

    public async UniTask ManagedTypingAsync(string text)
    {
        await this.StopTypingAsync();
        await this.BeginTypingAsync(text);
    }
}
}