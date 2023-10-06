using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.PcFSMSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems;
using LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillDrawPileSystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.CharacterSystems.PcSystems;

using Sirenix.OdinInspector;

using UnityEngine;

using ZhuangZhu;

#pragma warning disable CS4014

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems
{
public class PcEntity : CharacterEntity
{
    [Title("Config")]
    [ShowInInspector]
    public const string EntityAddress = "Pc";

    [Title("References")]
    [ShowInInspector]
    private Pc _pc;
    public Pc PcPy => this._pc;
    public override Character CharacterPy => this._pc;
    public void SetPc(Pc pc)
    {
        this._pc = pc;
    }

    [Title("Data")]
    [ShowInInspector]
    private static InputFSM _inputFSM;
    public static InputFSM InputFSMPy => _inputFSM;
    public void SetPcFSM(InputFSM inputFSM)
    {
        _inputFSM = inputFSM;
    }

    [Title("抽技能堆")]
    [ShowInInspector]
    private SkillDrawPile _skillDrawPile;
    public SkillDrawPile SkillDrawPilePy => this._skillDrawPile;
    public void SetSkillDrawPile(SkillDrawPile skillDrawPile)
    {
        this._skillDrawPile = skillDrawPile;
    }
    [Title("技能栏")]
    [ShowInInspector]
    private SkillBar _skillBar;
    public SkillBar SkillBarPy => this._skillBar;
    public void SetSkillBar(SkillBar skillBar)
    {
        this._skillBar = skillBar;
    }

    [Title("自身 Cache")]
    [SerializeField]
    private CircleCollider2D _circleCollider2D;
    public CircleCollider2D CircleCollider2DPy => this._circleCollider2D;

    //减少 Garbage 的 Cache.
    private readonly List<Vector3Int> _entitiesCoords = new List<Vector3Int>(5);
    public List<Vector3Int> EntitiesCoordsPy => this._entitiesCoords;

    public override async UniTask HideAsync()
    {
        this._selfGo.SetActive(false);
        await this._entityMover.FormatAsync();
    }

    public void OnMouseEnter()
    {
        if (!this._hasInitialized) return;
        this._spineController.ShowOutline(true,EOutlineStage.Select);
    }

    private void OnMouseExit()
    {
        if (!this._hasInitialized) return;
        this._spineController.ShowOutline(false,EOutlineStage.Select);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        this._circleCollider2D = this.GetComponent<CircleCollider2D>();
    }
#endif
}
}