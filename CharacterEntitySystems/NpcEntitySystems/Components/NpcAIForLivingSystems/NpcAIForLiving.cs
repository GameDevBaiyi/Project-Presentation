using System.Collections.Generic;

using Common.BehaviourTree;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems
{
/// <summary>
/// 非战斗条件下的 AI
/// </summary>
public class NpcAIForLiving
{
    [Title("Data")]
    [ShowInInspector]
    private readonly NpcEntity _npcEntity;
    public NpcEntity NpcEntityPy => this._npcEntity;

    [Title("Cache")]
    public readonly List<Vector3Int> Ring = new List<Vector3Int>(NpcEntity.MovementRange * 6);

    [Title("BT")]
    [ShowInInspector]
    private Sequence _idleThenHangOut;

    [Title("闲逛 相关")]
    [ShowInInspector]
    private TaskNode _hangOut;
    [ShowInInspector]
    private bool _hasHungOut;
    public bool HasHungOutPy => this._hasHungOut;
    public void SetHasHungOut(bool hasHungOut)
    {
        this._hasHungOut = hasHungOut;
    }

    [Title("发呆 相关")]
    [ShowInInspector]
    private TaskNode _idle;
    [ShowInInspector]
    private bool _hasBeenIdle;
    public bool HasBeenIdlePy => this._hasBeenIdle;
    public void SetHasBeenIdle(bool hasBeenIdle)
    {
        this._hasBeenIdle = hasBeenIdle;
    }
    [ShowInInspector]
    private float _idleTimer;
    public float IdleTimerPy => this._idleTimer;
    public void SetIdleTimer(float idleTimer)
    {
        this._idleTimer = idleTimer;
    }
    [ShowInInspector]
    private float _idleTimeSpan = 1f;
    public float IdleTimeSpanPy => this._idleTimeSpan;
    public void SetIdleTimeSpan(float idleTimeSpan)
    {
        this._idleTimeSpan = idleTimeSpan;
    }

    public NpcAIForLiving(NpcEntity npcEntity)
    {
        this._npcEntity = npcEntity;

        this._idle = new TaskNode(() => NpcAIForLivingDetails.Idle(this));
        this._hangOut = new TaskNode(() => NpcAIForLivingDetails.HangOut(this));
        this._idleThenHangOut = new Sequence(this._idle,this._hangOut);
    }

    [Title("UniTask Trace")]
    [ShowInInspector]
    private bool _isProcessing;
    [ShowInInspector]
    private bool _canAI;
    [ShowInInspector]
    private bool _isStoppingAI;

    [Title("Methods")]
    public async UniTask ProcessAsync()
    {
        this._isProcessing = true;
        this._canAI = true;
        while (true)
        {
            this._idleThenHangOut.Update();

            if (!this._canAI) break;
#if UNITY_EDITOR
            if (!Application.isPlaying) break;
#endif
            await UniTask.NextFrame();
        }

        this._canAI = false;
        this._isProcessing = false;
    }
    public async UniTask StopAIAndFormatAsync()
    {
        this._isStoppingAI = true;
        this._canAI = false;

        await UniTask.WaitUntil(() => !this._isProcessing);
        this.Format();
        this._isStoppingAI = false;
    }

    public void Format()
    {
        this._hasHungOut = default(bool);
        this._hasBeenIdle = default(bool);
        this._idleTimer = default(float);
        this._idleTimeSpan = 1f;
    }
}
}