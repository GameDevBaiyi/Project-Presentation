using System;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.Base
{
[Flags]
public enum TerrainStaticFlags
{
    None = 0,
    IsWalkable = 1,
    IsBehindEntity = 1 << 1,
    IsSpawnPoint = 1 << 2,
    IsUpstairs = 1 << 3,
    IsInteractionPoint = 1 << 4,
}

[Serializable]
public class TerrainStaticCell
{
    [SerializeField]
    private Vector3Int _coord;
    public Vector3Int CoordPy => this._coord;

    [SerializeField]
    private TerrainStaticFlags _terrainStaticFlags;
    public TerrainStaticFlags TerrainStaticFlagsPy => this._terrainStaticFlags;

    [SerializeField]
    private TileEnum _tileEnum;
    public TileEnum TileEnumPy => this._tileEnum;

#region EditorOnly
#if UNITY_EDITOR
    public void SetCoord(Vector3Int coord)
    {
        this._coord = coord;
    }
    public void SetTerrainStaticFlags(TerrainStaticFlags terrainStaticFlags)
    {
        this._terrainStaticFlags = terrainStaticFlags;
    }
    public void SetTileEnum(TileEnum tileEnum)
    {
        this._tileEnum = tileEnum;
    }
#endif
#endregion
}

/// <summary>
/// 地形的动态 flags.
/// </summary>
[Flags]
public enum TerrainDynamicFlags
{
    None = 0,

    /// <summary>
    /// 设置为障碍物, 目前用于战斗中, 设置边界.
    /// </summary>
    IsObstacle,
}

public static class TerrainDynamicFlagsExtensions
{
    public static TerrainDynamicFlags AddFlags(this TerrainDynamicFlags terrainDynamicFlags,TerrainDynamicFlags flags)
    {
        return terrainDynamicFlags | flags;
    }
    public static TerrainDynamicFlags RemoveFlags(this TerrainDynamicFlags terrainDynamicFlags,TerrainDynamicFlags flags)
    {
        return terrainDynamicFlags & ~flags;
    }
}

/// <summary>
/// 地图的动态 地形 Cell.
/// </summary>
[Serializable]
public class TerrainDynamicCell
{
    [Title("Data")]
    private TerrainDynamicFlags _terrainDynamicFlags;

    [Title("Methods")]
    public void RemoveAllFlags()
    {
        this._terrainDynamicFlags = TerrainDynamicFlags.None;
    }
    public bool GetFlag(TerrainDynamicFlags terrainDynamicFlag)
    {
        return (this._terrainDynamicFlags & terrainDynamicFlag) != TerrainDynamicFlags.None;
    }
    public void SetFlag(bool isAddingFlag,TerrainDynamicFlags targetFlag)
    {
        if (isAddingFlag)
        {
            this._terrainDynamicFlags |= targetFlag;
        }
        else
        {
            this._terrainDynamicFlags &= ~targetFlag;
        }
    }
}
}