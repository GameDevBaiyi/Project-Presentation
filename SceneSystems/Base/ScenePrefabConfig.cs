using System;
using System.Collections.Generic;
using System.Linq;

using Common.Extensions;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.CharacterSystems;

using UnityEngine;
using UnityEngine.Serialization;

namespace LowLevelSystems.SceneSystems.Base
{
[Serializable]
public class ScenePrefabConfig
{
    public enum EditorTileEnum
    {
        None,
        SpawnPoint,
        Upstairs,
    }

    public static EditorTileEnum[] AllEditorTileEnums = ((EditorTileEnum[])Enum.GetValues(typeof(EditorTileEnum))).Where(t => t != EditorTileEnum.None).ToArray();

    [Serializable]
    public class EditorTileEnumAndCoords
    {
        [SerializeField]
        private EditorTileEnum _editorTileEnum;
        public EditorTileEnum EditorTileEnumPy => this._editorTileEnum;
        public void SetEditorTileEnum(EditorTileEnum editorTileEnum)
        {
            this._editorTileEnum = editorTileEnum;
        }

        [SerializeField]
        private List<Vector3Int> _coords;
        public List<Vector3Int> CoordsPy => this._coords;
        public void SetCoords(List<Vector3Int> coords)
        {
            this._coords = coords;
        }

        public EditorTileEnumAndCoords(EditorTileEnum editorTileEnum,List<Vector3Int> coords)
        {
            this._editorTileEnum = editorTileEnum;
            this._coords = coords;
        }
    }

    [Serializable]
    public class BackAndEntitiesCoords
    {
        [SerializeField]
        private Vector3Int _backCoord;
        public Vector3Int BackCoordPy => this._backCoord;
        public void SetBackCoord(Vector3Int backCoord)
        {
            this._backCoord = backCoord;
        }

        [SerializeField]
        private List<Vector3Int> _entitiesCoords;
        public List<Vector3Int> EntitiesCoordsPy => this._entitiesCoords;
        public void SetEntitiesCoords(List<Vector3Int> entitiesCoords)
        {
            this._entitiesCoords = entitiesCoords;
        }
    }

    [Serializable]
    public struct NpcEnumAndCoord
    {
        [SerializeField]
        private CharacterEnum _characterEnum;
        public CharacterEnum CharacterEnumPy => this._characterEnum;
        public void SetNpcEnum(CharacterEnum characterEnum)
        {
            this._characterEnum = characterEnum;
        }

        [SerializeField]
        private Vector3Int _coord;
        public Vector3Int CoordPy => this._coord;
        public void SetNpcInitialCoord(Vector3Int coord)
        {
            this._coord = coord;
        }
    }

    [Serializable]
    public class BattleTypeAndSpecialCoordsList
    {
        [SerializeField]
        private BattleConfig.BattleTypeEnum _battleTypeEnum;
        public BattleConfig.BattleTypeEnum BattleTypeEnumPy => this._battleTypeEnum;
        public void SetBattleTypeEnum(BattleConfig.BattleTypeEnum battleTypeEnum)
        {
            this._battleTypeEnum = battleTypeEnum;
        }

        [SerializeField]
        private List<SpecialCoords> _specialCoordsList = new List<SpecialCoords>();
        public List<SpecialCoords> SpecialCoordsListPy => this._specialCoordsList;
        public void SetSpecialCoordsList(List<SpecialCoords> specialCoordsList)
        {
            this._specialCoordsList = specialCoordsList;
        }
    }

    [SerializeField]
    private ScenePrefabEnum _scenePrefabEnum;
    public ScenePrefabEnum ScenePrefabEnumPy => this._scenePrefabEnum;
    public void SetScenePrefabEnum(ScenePrefabEnum scenePrefabEnum)
    {
        this._scenePrefabEnum = scenePrefabEnum;
    }

    [SerializeField]
    private string _sceneEntityAddress;
    public string SceneEntityAddressPy => this._sceneEntityAddress;
    public void SetSceneEntityAddress(string sceneEntityAddress)
    {
        this._sceneEntityAddress = sceneEntityAddress;
    }

    [SerializeField]
    private int _mapWidth;
    public int MapWidthPy => this._mapWidth;
    public void SetMapWidth(int mapWidth)
    {
        this._mapWidth = mapWidth;
    }

    [SerializeField]
    private int _mapHeight;
    public int MapHeightPy => this._mapHeight;
    public void SetMapHeight(int mapHeight)
    {
        this._mapHeight = mapHeight;
    }

    [SerializeField]
    private TerrainStaticCell[] _terrainStaticCellsList;
    private TerrainStaticCell[][] _terrainStaticGrid;
    public TerrainStaticCell[][] TerrainStaticGridPy => this._terrainStaticGrid;
    public void SetTerrainStaticCellsList(TerrainStaticCell[] terrainStaticCellsList)
    {
        this._terrainStaticCellsList = terrainStaticCellsList;
    }

    [SerializeField]
    private List<EditorTileEnumAndCoords> _editorTileEnumAndCoordsList;
    private Dictionary<EditorTileEnum,List<Vector3Int>> _editorTileEnum_coords;
    public Dictionary<EditorTileEnum,List<Vector3Int>> EditorTileEnum_CoordsPy => this._editorTileEnum_coords;
    public void SetEditorTileEnumAndCoordsList(List<EditorTileEnumAndCoords> editorTileEnumAndCoordsList)
    {
        this._editorTileEnumAndCoordsList = editorTileEnumAndCoordsList;
    }

    [SerializeField]
    private List<BackAndEntitiesCoords> _backAndEntitiesCoordsList;
    private Dictionary<Vector3Int,HashSet<Vector3Int>> _backCoord_entitiesCoords;
    public Dictionary<Vector3Int,HashSet<Vector3Int>> BackCoord_EntitiesCoordsPy => this._backCoord_entitiesCoords;
    public void SetBackAndEntitiesCoordsList(List<BackAndEntitiesCoords> backAndEntitiesCoordsList)
    {
        this._backAndEntitiesCoordsList = backAndEntitiesCoordsList;
    }

    [SerializeField]
    private List<Vector3Int> _allWalkableCoords;
    public List<Vector3Int> AllWalkableCoordsPy => this._allWalkableCoords;
    public void SetAllWalkableCoords(List<Vector3Int> allWalkableCoords)
    {
        this._allWalkableCoords = allWalkableCoords;
    }

    [SerializeField]
    private List<BattleTypeAndSpecialCoordsList> _battleTypeAndSpecialCoordsLists = new List<BattleTypeAndSpecialCoordsList>();
    private Dictionary<BattleConfig.BattleTypeEnum,List<SpecialCoords>> _battleType_specialCoordsList;
    public void SetBattleTypeAndSpecialCoordsLists(List<BattleTypeAndSpecialCoordsList> battleTypeAndSpecialCoordsLists)
    {
        this._battleTypeAndSpecialCoordsLists = battleTypeAndSpecialCoordsLists;
    }
    public SpecialCoords GetRandomSpecialCoordsByBattleType(BattleConfig.BattleTypeEnum battleTypeEnum)
    {
        if (!this._battleType_specialCoordsList.TryGetValue(battleTypeEnum,out List<SpecialCoords> specialCoordsList))
        {
            Debug.LogError($"该 Scene: {this._scenePrefabEnum} 未找到 BattleTypeEnum: {battleTypeEnum} 对应的 SpecialCoordsList.");
            return null;
        }

        return specialCoordsList.GetRandomItem();
    }

    public void Initialize()
    {
        this._terrainStaticGrid = new TerrainStaticCell[this._mapWidth][];
        for (int i = 0; i < this._mapWidth; i++)
        {
            this._terrainStaticGrid[i] = new TerrainStaticCell[this._mapHeight];
        }
        for (int x = 0; x < this._mapWidth; x++)
        {
            for (int y = 0; y < this._mapHeight; y++)
            {
                this._terrainStaticGrid[x][y] = this._terrainStaticCellsList[x * this._mapHeight + y];
            }
        }

        this._editorTileEnum_coords = new Dictionary<EditorTileEnum,List<Vector3Int>>(ScenePrefabConfig.AllEditorTileEnums.Length);
        foreach (EditorTileEnum editorTileEnum in ScenePrefabConfig.AllEditorTileEnums)
        {
            this._editorTileEnum_coords[editorTileEnum] = new List<Vector3Int>();
        }
        foreach (EditorTileEnumAndCoords editorTileEnumAndCoords in this._editorTileEnumAndCoordsList)
        {
            this._editorTileEnum_coords[editorTileEnumAndCoords.EditorTileEnumPy] = editorTileEnumAndCoords.CoordsPy;
        }

        this._backCoord_entitiesCoords = new Dictionary<Vector3Int,HashSet<Vector3Int>>(this._backAndEntitiesCoordsList.Count);
        foreach (BackAndEntitiesCoords backCoordEntitiesCoords in this._backAndEntitiesCoordsList)
        {
            HashSet<Vector3Int> vector3Ints = new HashSet<Vector3Int>(backCoordEntitiesCoords.EntitiesCoordsPy.Count);
            foreach (Vector3Int vector3Int in backCoordEntitiesCoords.EntitiesCoordsPy)
            {
                vector3Ints.Add(vector3Int);
            }
            this._backCoord_entitiesCoords[backCoordEntitiesCoords.BackCoordPy] = vector3Ints;
        }

        this._battleType_specialCoordsList = new Dictionary<BattleConfig.BattleTypeEnum,List<SpecialCoords>>(this._battleTypeAndSpecialCoordsLists.Count);
        foreach (BattleTypeAndSpecialCoordsList battleTypeAndSpecialCoordsList in this._battleTypeAndSpecialCoordsLists)
        {
            this._battleType_specialCoordsList[battleTypeAndSpecialCoordsList.BattleTypeEnumPy] = battleTypeAndSpecialCoordsList.SpecialCoordsListPy;
        }
    }
}
}