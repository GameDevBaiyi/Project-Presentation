using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SceneSystems.RoomSystems;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.BuildingSystems
{
[Serializable]
public class BuildingInstanceConfig
{
    [SerializeField]
    private BuildingEnum _buildingEnum;
    public BuildingEnum BuildingEnumPy => this._buildingEnum;
    public void SetBuildingEnum(BuildingEnum buildingEnum)
    {
        this._buildingEnum = buildingEnum;
    }

    [SerializeField]
    private Vector3Int _coord;
    public Vector3Int CoordPy => this._coord;
    public void SetCoord(Vector3Int coord)
    {
        this._coord = coord;
    }

    [SerializeField]
    private List<Vector3Int> _interactions = new List<Vector3Int>();
    public List<Vector3Int> InteractionsPy => this._interactions;
    public void SetInteractions(List<Vector3Int> interactions)
    {
        this._interactions = interactions;
    }

    [SerializeField]
    private TextId _nameId;
    public TextId NameIdPy => this._nameId;
    public void SetNameId(TextId nameId)
    {
        this._nameId = nameId;
    }

    [SerializeField]
    private List<RoomInstanceConfig> _roomConfigs = new List<RoomInstanceConfig>();
    public List<RoomInstanceConfig> RoomConfigsPy => this._roomConfigs;
    public void SetRoomConfigs(List<RoomInstanceConfig> roomConfigs)
    {
        this._roomConfigs = roomConfigs;
    }

    [SerializeField]
    private string _tileAddress;
    public string TileAddressPy => this._tileAddress;
    public void SetTileAddress(string tileAddress)
    {
        this._tileAddress = tileAddress;
    }
}
}