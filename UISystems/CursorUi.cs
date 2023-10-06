using System;

using Cysharp.Threading.Tasks;

using LowLevelSystems.CharacterEntitySystems.NpcEntitySystems;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LowLevelSystems.UISystems
{
public class CursorUi
{
    [Title("Config")]
    private const string _cursorPrefabAddress = "Cursor";
    private static readonly Color _walkableColor = Color.white;
    private static readonly Color _notWalkableColor = Color.red;

    //鼠标指示.
    [Title("Data")]
    [ShowInInspector]
    private GameObject _cursorGo;
    public GameObject CursorGoPy => this._cursorGo;
    [ShowInInspector]
    private Transform _cursorTransform;
    public Transform CursorTransformPy => this._cursorTransform;
    [ShowInInspector]
    private GameObject _walkableCursorGo;
    public GameObject WalkableCursorGoPy => this._walkableCursorGo;
    [ShowInInspector]
    private SpriteRenderer _cursorWalkableRenderer;
    [ShowInInspector]
    private GameObject _interactableCursorGo;
    [ShowInInspector]
    public static event Action<NpcEntity> InteractableCursorShowed;
    [ShowInInspector]
    public static event Action InteractableCursorHidden;

    public async UniTask InitializeAsync(Transform parentTransform)
    {
        this._cursorGo = await Addressables.InstantiateAsync(_cursorPrefabAddress,parentTransform);
        this._cursorTransform = this._cursorGo.transform;
        this._walkableCursorGo = this._cursorTransform.Find("Walkable").gameObject;
        this._cursorWalkableRenderer = this._walkableCursorGo.transform.Find("Sign").GetComponent<SpriteRenderer>();
        this._interactableCursorGo = this._cursorTransform.Find("Interactable").gameObject;
    }

    public void ChangeColorBy(bool isWalkable)
    {
        this._cursorWalkableRenderer.color = isWalkable ? _walkableColor : _notWalkableColor;
    }

    public void ShowInteractableCursor(NpcEntity npcEntity)
    {
        this._interactableCursorGo.SetActive(true);
        InteractableCursorShowed?.Invoke(npcEntity);
    }
    public void HideInteractableCursor()
    {
        this._interactableCursorGo.SetActive(false);
        InteractableCursorHidden?.Invoke();
    }
}
}