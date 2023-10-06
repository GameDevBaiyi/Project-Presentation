using UnityEngine;

namespace LowLevelSystems.UISystems
{
public class CellUiMb : MonoBehaviour
{
    [SerializeField]
    private GameObject _selfGo;
    public GameObject SelfGoPy => this._selfGo;

    [SerializeField]
    private Transform _selfTransform;
    public Transform SelfTransformPy => this._selfTransform;

#region EditorOnly
#if UNITY_EDITOR
    private void OnValidate()
    {
        this._selfGo = this.gameObject;
        this._selfTransform = this.transform;
    }
#endif
#endregion
}
}