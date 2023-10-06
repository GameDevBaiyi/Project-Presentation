using System.Collections.Generic;

using Common.DataTypes;

using Cysharp.Threading.Tasks;

using LowLevelSystems.Common;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LowLevelSystems.UISystems
{
public class CellUiShower
{
    [Title("References")]
    [ShowInInspector]
    private readonly Transform _parentTransform;

    [Title("Data")]
    [ShowInInspector]
    private readonly string _address;
    [ShowInInspector]
    private readonly ObjectPoolAsync<CellUiMb> _cellUIPoolAsync;
    private async UniTask<CellUiMb> GenerateCellUIMonoAsync()
    {
        GameObject gameObject = await Addressables.InstantiateAsync(this._address,this._parentTransform);
        if (!gameObject.TryGetComponent(out CellUiMb cellUIMono))
        {
            Debug.LogError($"{this._address} 的 Prefab 上 未找到 CellUiMono. ");
        }
        return cellUIMono;
    }

    [ShowInInspector]
    private readonly List<CellUiMb> _visibleUIs = new List<CellUiMb>(30);

    public CellUiShower(Transform parentTransform,string address)
    {
        this._parentTransform = parentTransform;

        this._address = address;
        this._cellUIPoolAsync = new ObjectPoolAsync<CellUiMb>(30,this.GenerateCellUIMonoAsync);
    }

    /// <summary>
    /// 机制1. 隐藏当前显示的 所有 Uis. 返回到 Pool 中.
    /// </summary>
    [Title("Methods")]
    public void Hide()
    {
        foreach (CellUiMb cellUIMono in this._visibleUIs)
        {
            cellUIMono.SelfGoPy.SetActive(false);
            this._cellUIPoolAsync.ReturnItemToPool(cellUIMono);
        }
        this._visibleUIs.Clear();
    }

    // 机制2. 显示.
    /// <summary>
    /// 显示单个.
    /// </summary>
    private async UniTask ShowSingleAsync(Vector3Int offsetCoord)
    {
        CellUiMb cellUiMb = await this._cellUIPoolAsync.GetItemFromPoolAsync();
        cellUiMb.SelfGoPy.SetActive(true);
        cellUiMb.SelfTransformPy.position = offsetCoord.ToWorldPos();
        this._visibleUIs.Add(cellUiMb);
    }

    /// <summary>
    /// 隐藏当前所有 Uis, 在新的地方显示. 
    /// </summary>
    public void Show(IEnumerable<Vector3Int> offsetCoords)
    {
        this.Hide();
        foreach (Vector3Int offsetCoord in offsetCoords)
        {
#pragma warning disable CS4014
            this.ShowSingleAsync(offsetCoord);
#pragma warning restore CS4014
        }
    }
}
}