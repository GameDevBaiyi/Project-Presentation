using System;
using System.Collections.Generic;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
[Serializable]
public class TerrainConfig
{
    [SerializeField]
    private List<TileConfig> _tileConfigRows;
    private Dictionary<TileEnum,TileConfig> _tileEnum_tileConfig;
    public Dictionary<TileEnum,TileConfig> TileEnum_TileConfigPy => this._tileEnum_tileConfig;
    public void SetTileConfigRows(List<TileConfig> tileConfigRows)
    {
        this._tileConfigRows = tileConfigRows;
    }

    public void Initialize()
    {
        this._tileEnum_tileConfig = new Dictionary<TileEnum,TileConfig>(this._tileConfigRows.Count);

        foreach (TileConfig tileConfigRow in this._tileConfigRows)
        {
            this._tileEnum_tileConfig[tileConfigRow.TileEnumPy] = tileConfigRow;
        }
    }
}

[Serializable]
public struct TileConfig
{
    [SerializeField]
    private TileEnum _tileEnum;
    public TileEnum TileEnumPy => this._tileEnum;
    public void SetTileEnum(TileEnum tileEnum)
    {
        this._tileEnum = tileEnum;
    }

    [SerializeField]
    private int _apCostMultiplier;
    public int APCostMultiplierPy => this._apCostMultiplier;
    public void SetAPCostMultiplier(int apCostMultiplier)
    {
        this._apCostMultiplier = apCostMultiplier;
    }
}
}