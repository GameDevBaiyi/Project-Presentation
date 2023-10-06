using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components;
using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems.Components.NpcAIForLivingSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.NpcSystems;
using LowLevelSystems.InteractableSystems;

using Sirenix.OdinInspector;

using UICommon;

using UnityEngine;

using ZhuangZhu;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.NpcEntitySystems
{
public class NpcEntity : CharacterEntity,IInteractable
{
    [Title("Config")]
    [ShowInInspector]
    public const float WalkSpeed = 1f;
    [ShowInInspector]
    public const float RunSpeed = 5f;
    [ShowInInspector]
    public const string EntityAddress = "Npc";
    // 城镇内的单次移动距离.
    [ShowInInspector]
    public const int MovementRange = 3;
    // 城镇内的移动半径, 每次移动都不可以超过初始点最远该距离.
    [ShowInInspector]
    public const int MovementRadius = 5;

    [Title("References")]
    [ShowInInspector]
    private Npc _npc;
    public Npc NpcPy => this._npc;
    public override Character CharacterPy => this._npc;
    public void SetNpc(Npc npc)
    {
        this._npc = npc;
    }

    [Title("Data")]
    [ShowInInspector]
    private NpcAIForLiving _npcAIForLiving;
    public NpcAIForLiving NpcAIForLivingPy => this._npcAIForLiving;
    public void SetNpcAIForLiving(NpcAIForLiving npcAIForLiving)
    {
        this._npcAIForLiving = npcAIForLiving;
    }

    [ShowInInspector]
    private BtForBattle _btForBattle;
    public BtForBattle BtForBattlePy => this._btForBattle;
    public void SetBtForBattle(BtForBattle btForBattle)
    {
        this._btForBattle = btForBattle;
    }

    //对话时的 Bubble.
    [ShowInInspector]
    public UI_Component_DialogueBubble Bubble;

    public override async UniTask HideAsync()
    {
        this._selfGo.SetActive(false);
        await this._npcAIForLiving.StopAIAndFormatAsync();
        await this._entityMover.FormatAsync();
    }

    private void OnMouseEnter()
    {
        if (!this._hasInitialized) return;
        this._spineController.ShowOutline(true,EOutlineStage.Select);
    }

    private void OnMouseExit()
    {
        if (!this._hasInitialized) return;
        this._spineController.ShowOutline(false,EOutlineStage.Select);
    }

    public Vector3Int CalculateTargetCoord()
    {
        return DetailsOfNpcEntity.CalculateTargetCoord(this);
    }
    public void InteractWithCurrentPcEntity()
    {
        DetailsOfNpcEntity.InteractWithCurrentPcEntity(this);
    }
}
}