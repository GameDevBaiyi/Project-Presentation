using Common.Template;

using Cysharp.Threading.Tasks;

using FairyGUI;

using LowLevelSystems.CharacterEntitySystems.Components;
using LowLevelSystems.CharacterEntitySystems.Components.CharacterAnimationSystems;
using LowLevelSystems.CharacterEntitySystems.Components.EntityMoverSystems;
using LowLevelSystems.CharacterEntitySystems.Components.HpUiSystems;
using LowLevelSystems.CharacterSystems;
using LowLevelSystems.SkillSystems.SkillBuffSystems;

using Sirenix.OdinInspector;

using Spine.Unity;

using UnityEngine;

using ZhuangZhu;

// ReSharper disable InconsistentNaming

namespace LowLevelSystems.CharacterEntitySystems
{
[InlineEditor]
public abstract class CharacterEntity : MonoBehaviour,IInstance
{
    [Title("Data")]
    [ShowInInspector]
    protected int _instanceId;
    public int InstanceIdPy => this._instanceId;
    public void SetInstanceId(int instanceId)
    {
        this._instanceId = instanceId;
    }

    [ShowInInspector]
    protected Character.CharacterTypeEnum _characterTypeEnum;
    public Character.CharacterTypeEnum CharacterTypeEnumPy => this._characterTypeEnum;
    public void SetCharacterTypeEnum(Character.CharacterTypeEnum characterTypeEnum)
    {
        this._characterTypeEnum = characterTypeEnum;
    }

    [ShowInInspector]
    protected CharacterAnimationSystem _characterAnimationSystem;
    public CharacterAnimationSystem CharacterAnimationSystemPy => this._characterAnimationSystem;
    public void SetCharacterAnimationSystem(CharacterAnimationSystem characterAnimationSystem)
    {
        this._characterAnimationSystem = characterAnimationSystem;
    }

    [ShowInInspector]
    protected EntityMover _entityMover;
    public EntityMover EntityMoverPy => this._entityMover;
    public void SetEntityMover(EntityMover entityMover)
    {
        this._entityMover = entityMover;
    }

    [ShowInInspector]
    protected CharacterPanelController _characterPanelController;
    public CharacterPanelController CharacterPanelControllerPy => this._characterPanelController;
    public void SetCharacterPanelController(CharacterPanelController characterPanelController)
    {
        this._characterPanelController = characterPanelController;
    }

    [ShowInInspector]
    protected HpUiSystem HpUiSystem;
    public HpUiSystem HpUiSystemPy => this.HpUiSystem;
    public void SetHpUiSystem(HpUiSystem hpUiSystem)
    {
        this.HpUiSystem = hpUiSystem;
    }

    [ShowInInspector]
    protected BuffPool _buffPool;
    public BuffPool BuffPoolPy => this._buffPool;
    public void SetBuffPool(BuffPool buffPool)
    {
        this._buffPool = buffPool;
    }

    [ShowInInspector]
    protected CharacterEfxHolder CharacterEfxHolder;
    public CharacterEfxHolder CharacterEfxHolderPy => this.CharacterEfxHolder;
    public void SetEfxHolder(CharacterEfxHolder characterEfxHolder)
    {
        this.CharacterEfxHolder = characterEfxHolder;
    }

    [Title("自身 Cache")]
    [SerializeField]
    protected GameObject _selfGo;
    public GameObject SelfGoPy => this._selfGo;

    [SerializeField]
    protected Transform _selfTransform;
    public Transform SelfTransformPy => this._selfTransform;

    [SerializeField]
    protected SkeletonAnimation _skeletonAnimation;
    public SkeletonAnimation SkeletonAnimationPy => this._skeletonAnimation;

    [SerializeField]
    protected MeshRenderer _meshRenderer;
    public MeshRenderer MeshRendererPy => this._meshRenderer;

    [SerializeField]
    protected Transform _particleHolder;
    public Transform ParticleHolderPy => this._particleHolder;

    [SerializeField]
    protected SpineController _spineController;
    public SpineController SpineControllerPy => this._spineController;

    [SerializeField]
    protected UIPanel _uiPanel;
    public UIPanel UiPanelPy => this._uiPanel;

    [ShowInInspector]
    protected bool _hasInitialized;
    public void SetHasInitialized()
    {
        this._hasInitialized = true;
    }

    [Title("Methods")]
    [ShowInInspector]
    public CharacterId CharacterIdPy => new CharacterId(this._instanceId);

    [ShowInInspector]
    public Vector3 HeadPosPy => new Vector3(this._selfTransform.position.x,this._meshRenderer.bounds.max.y,0f);
    [ShowInInspector]
    public Vector3 CenterPosPy => new Vector3(this._selfTransform.position.x,this._meshRenderer.bounds.center.y,0f);
    [ShowInInspector]
    public Vector3 BottomPy => new Vector3(this._selfTransform.position.x,this._meshRenderer.bounds.min.y,0f);

    public abstract UniTask HideAsync();
    public abstract Character CharacterPy { get; }

    public void ChangeDirection(int direction)
    {
        this.CharacterPy.CoordSystemPy.SetDirectionIndex(direction);
        this._characterPanelController.RefreshDirection();
        if (direction == 0
         || direction == 3) return;
        this._characterAnimationSystem.SkeletonAnimationPy.skeleton.ScaleX = direction is 1 or 2 ? -1f : 1f;
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        this._selfGo = this.gameObject;
        this._selfTransform = this.transform;
        this._skeletonAnimation = this.GetComponentInChildren<SkeletonAnimation>();
        Transform spineTrans = this.transform.Find("Spine");
        if (!spineTrans.TryGetComponent(out MeshRenderer meshRenderer))
        {
            Debug.LogError($"未找到 Spine 上的 MeshRenderer.");
        }
        this._meshRenderer = meshRenderer;
        this._particleHolder = this.transform.Find("ParticleHolder");
        this._spineController = this.GetComponent<SpineController>();
        this._uiPanel = this.GetComponentInChildren<UIPanel>();
    }
#endif
}
}