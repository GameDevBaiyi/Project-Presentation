using System;
using System.Collections.Generic;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
[Serializable]
public class ModuleOfSkillBarConfig
{
    [SerializeField]
    private ModuleOfSkillBarEnum _moduleOfSkillBarEnum;
    public ModuleOfSkillBarEnum ModuleOfSkillBarEnumPy => this._moduleOfSkillBarEnum;
#region EditorOnly
#if UNITY_EDITOR
    public void SetModuleOfSkillBarEnum(ModuleOfSkillBarEnum moduleOfSkillBarEnum)
    {
        this._moduleOfSkillBarEnum = moduleOfSkillBarEnum;
    }
#endif
#endregion

    [SerializeField]
    private int _mapWidth;
    public int MapWidthPy => this._mapWidth;
#region EditorOnly
#if UNITY_EDITOR
    public void SetMapWidth(int mapWidth)
    {
        this._mapWidth = mapWidth;
    }
#endif
#endregion

    [SerializeField]
    private int _mapHeight;
    public int MapHeightPy => this._mapHeight;
#region EditorOnly
#if UNITY_EDITOR
    public void SetMapHeight(int mapHeight)
    {
        this._mapHeight = mapHeight;
    }
#endif
#endregion

    [SerializeField]
    private NodeOfModuleFlags[] _nodeFlagsList;
    private NodeOfModuleFlags[][] _nodeFlagsGrid;
    public NodeOfModuleFlags[][] NodeFlagsGridPy => this._nodeFlagsGrid;
#region EditorOnly
#if UNITY_EDITOR
    public void SetNodeFlagsList(NodeOfModuleFlags[][] nodeFlagsGrid)
    {
        int mapWidth = nodeFlagsGrid.Length;
        int mapHeight = nodeFlagsGrid[0].Length;
        this.SetMapWidth(mapWidth);
        this.SetMapHeight(mapHeight);
        this._nodeFlagsList = new NodeOfModuleFlags[mapWidth * mapHeight];
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                this._nodeFlagsList[j + i * mapHeight] = nodeFlagsGrid[i][j];
            }
        }
    }
#endif
#endregion

    [SerializeField]
    private List<Vector3Int> _starPoses;
    public List<Vector3Int> StarPosesPy => this._starPoses;
#region EditorOnly
#if UNITY_EDITOR
    public void SetStarPoses(List<Vector3Int> starPoses)
    {
        this._starPoses = starPoses;
    }
#endif
#endregion

    public void Initialize()
    {
        this._nodeFlagsGrid = new NodeOfModuleFlags[this._mapWidth][];
        for (int i = 0; i < this._mapWidth; i++)
        {
            this._nodeFlagsGrid[i] = new NodeOfModuleFlags[this._mapHeight];
        }
        for (int i = 0; i < this._mapWidth; i++)
        {
            for (int j = 0; j < this._mapHeight; j++)
            {
                this._nodeFlagsGrid[i][j] = this._nodeFlagsList[j + i * this._mapHeight];
            }
        }
    }
}
}