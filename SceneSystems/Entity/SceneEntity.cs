using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using DG.Tweening;

using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;
using LowLevelSystems.SceneSystems.CitySystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

namespace LowLevelSystems.SceneSystems.Entity
{
public class SceneEntity : MonoBehaviour
{
    //配置.
    private const float _minAlpha = 0.6f;
    private const float _alphaTransitionDuration = 0.5f;

    [Title("自身 Cache")]
    [SerializeField]
    private GameObject _gameObject;
    public GameObject GameObjectPy => this._gameObject;
    [SerializeField]
    private Tilemap _entitiesTilemap;
    public Tilemap EntitiesTilemapPy => this._entitiesTilemap;
    [SerializeField]
    private Tilemap _treeEntityTilemap; //BaiyiTODO. 后续删除. 

    [SerializeField]
    private PolygonCollider2D _polygonCollider2D;
    public PolygonCollider2D PolygonCollider2DPy => this._polygonCollider2D;
    [SerializeField]
    private List<Light2D> _light2Ds = new List<Light2D>(20);

    [Title("自身 Cache, 运行时直接 Destroy 掉.")]
    [SerializeField]
    private GameObject[] _editorTilemapGOsToDestroy;
    public GameObject[] EditorTilemapGOsToDestroyPy => this._editorTilemapGOsToDestroy;

    [Title("Data")]
    [SerializeField]
    private ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;
    public void SetScenePrefabEnum(ScenePrefabEnum scenePrefabEnum)
    {
        this._scenePrefabEnum = scenePrefabEnum;
    }

    public async UniTask ShowAsync(bool isDayTime,City city = null)
    {
        this._gameObject.SetActive(true);
        this.SwitchLights(!isDayTime);
        if (city != null)
        {
            foreach (Building building in city.BuildingHubPy.AllPhasedBuildingsPy)
            {
                BuildingInstanceConfig buildingInstanceConfig = building.BuildingInstanceConfigPy;
                string tileAddress = buildingInstanceConfig.TileAddressPy;
                TileBase tileBase = string.IsNullOrEmpty(tileAddress) ? null : await Addressables.LoadAssetAsync<TileBase>(tileAddress);
                this._entitiesTilemap.SetTile(buildingInstanceConfig.CoordPy,tileBase);
            }
        }
    }
    public void Hide()
    {
        this._gameObject.SetActive(false);
    }

    //BaiyiTODO. 后续换透视方式. 
    public void ReduceEntityAlpha(Vector3Int entityCoord)
    {
        this._entitiesTilemap.RemoveTileFlags(entityCoord,TileFlags.LockColor);
        DOTween.ToAlpha(() => this._entitiesTilemap.GetColor(entityCoord),t => this._entitiesTilemap.SetColor(entityCoord,t),_minAlpha,_alphaTransitionDuration);
        if (_treeEntityTilemap == null)
        {
            return;
        }
        this._treeEntityTilemap.RemoveTileFlags(entityCoord,TileFlags.LockColor);
        DOTween.ToAlpha(() => this._treeEntityTilemap.GetColor(entityCoord),t => this._treeEntityTilemap.SetColor(entityCoord,t),_minAlpha,_alphaTransitionDuration);
    }
    public void RestoreEntityAlpha(Vector3Int entityCoord)
    {
        this._entitiesTilemap.RemoveTileFlags(entityCoord,TileFlags.LockColor);
        DOTween.ToAlpha(() => this._entitiesTilemap.GetColor(entityCoord),t => this._entitiesTilemap.SetColor(entityCoord,t),1f,_alphaTransitionDuration);
        if (_treeEntityTilemap == null)
        {
            return;
        }
        this._treeEntityTilemap.RemoveTileFlags(entityCoord,TileFlags.LockColor);
        DOTween.ToAlpha(() => this._treeEntityTilemap.GetColor(entityCoord),t => this._treeEntityTilemap.SetColor(entityCoord,t),1f,_alphaTransitionDuration);
    }

    public void SwitchLights(bool isEnabled)
    {
        this._light2Ds.Clear();
        this._entitiesTilemap.GetComponentsInChildren(this._light2Ds);
        foreach (Light2D light2D in this._light2Ds)
        {
            light2D.enabled = isEnabled;
        }
    }

#region EditorOnly
#if UNITY_EDITOR
    private void OnValidate()
    {
        this._gameObject = this.gameObject;
        List<Tilemap> entitiesTilemap = this.GetComponentsInChildren<Tilemap>()
                                            .Where(t => t.GetComponent<TilemapRenderer>().sortingLayerName == "Entities" && t.GetComponent<TilemapRenderer>().sortingOrder == 1)
                                            .ToList();
        this._entitiesTilemap = entitiesTilemap[0];
        this._treeEntityTilemap = entitiesTilemap.Find(t => t.gameObject.name == "Tree");
        if (this.TryGetComponent(out PolygonCollider2D polygonCollider2D))
        {
            this._polygonCollider2D = polygonCollider2D;
        }

        IEnumerable<GameObject> editorTilemapsQuery = from tilemap in this._gameObject.GetComponentsInChildren<Tilemap>()
                                                      where tilemap.GetComponent<TilemapRenderer>().sortingLayerName == "Editor"
                                                      select tilemap.gameObject;
        this._editorTilemapGOsToDestroy = editorTilemapsQuery.ToArray();
    }
#endif
#endregion
}
}