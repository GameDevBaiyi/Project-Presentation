using System;
using System.Collections.Generic;

using LowLevelSystems.LocalizationSystems;
using LowLevelSystems.SceneSystems.Base;
using LowLevelSystems.SceneSystems.BuildingSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.SceneSystems.RoomSystems
{
[Serializable]
public class Room : Scene,IHasScenePrefab,IHasEntranceToScene,IHasSceneName
{
    private static readonly TextId _unitTextId = new TextId(1000015); // "层" 字.

    [ShowInInspector]
    private readonly ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;

    [ShowInInspector]
    private readonly BuildingId _buildingId;
    public BuildingId BuildingIdPy => this._buildingId;

    [ShowInInspector]
    private readonly int _floorIndex;
    public int FloorIndexPy => this._floorIndex;

    [ShowInInspector]
    private readonly bool _isFinalRoom;
    public bool IsFinalRoomPy => this._isFinalRoom;

    [ShowInInspector]
    private readonly Dictionary<Vector3Int,SceneId> _entranceCoord_sceneId = new Dictionary<Vector3Int,SceneId>(6);
    public Dictionary<Vector3Int,SceneId> EntranceCoord_SceneIdPy => this._entranceCoord_sceneId;

    public Room(int instanceId,ScenePrefabEnum scenePrefabEnum,BuildingId buildingId,
                int floorIndex,bool isFinalRoom) : base(instanceId,SceneTypeEnum.Room)
    {
        this._scenePrefabEnum = scenePrefabEnum;
        this._buildingId = buildingId;
        this._floorIndex = floorIndex;
        this._isFinalRoom = isFinalRoom;
    }

    [Title("Methods")]
    [ShowInInspector]
    public string SceneNamePy => this._buildingId.BuildingPy.BuildingNamePy + $" {this._floorIndex + 1}{_unitTextId.TextPy}";
    [ShowInInspector]
    public ScenePrefabConfig ScenePrefabConfigPy => this._scenePrefabEnum.ScenePrefabConfig();
}
}