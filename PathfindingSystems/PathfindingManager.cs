using System;
using System.Collections.Generic;

using Common.DataTypes;
using Common.Extensions;
using Common.Template;
using Common.Utilities;

using JetBrains.Annotations;

using LowLevelSystems.BattleSystems.Config;
using LowLevelSystems.SceneSystems.Base;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.PathfindingSystems
{
[Flags]
public enum AStarFlags
{
    None = 0,
    //功能: 在 OpenCollection 中的 Node 标记为 IsInOpen.
    IsInOpen = 1,
    //功能: 不可行走的点, 已经从 OpenCollection 中 Pop 出来的点, 都标记为 Close. 
    IsInClose = 1 << 1,
}

public static class AStarFlagsExtensions
{
    public static AStarFlags AddFlags(this AStarFlags aStarFlags,AStarFlags flags)
    {
        return aStarFlags | flags;
    }
    public static AStarFlags RemoveFlags(this AStarFlags aStarFlags,AStarFlags flags)
    {
        return aStarFlags & ~flags;
    }
}

public class PathfindingManager : Singleton<PathfindingManager>
{
    public class AStarNode : IComparable<AStarNode>
    {
        [ShowInInspector]
        public AStarNode ParentNode;

        [ShowInInspector]
        public Vector3Int Coord;

        [ShowInInspector]
        public int GCost;
        [ShowInInspector]
        public int HCost;
        public int FCostPy => this.GCost + this.HCost;

        [ShowInInspector]
        public AStarFlags AStarFlags;

        public TerrainStaticCell StaticCell;
        public TerrainDynamicFlags TerrainDynamicFlags;

        /// <summary>
        /// FCost 小和 HCost 小的排前面. 
        /// </summary>
        public int CompareTo(AStarNode other)
        {
            int comparingResult = this.FCostPy.CompareTo(other.FCostPy);

            if (comparingResult != 0)
            {
                return comparingResult;
            }

            comparingResult = this.HCost.CompareTo(other.HCost);
            return comparingResult;
        }
    }

    [Title("Data")]
    [ShowInInspector]
    private int _mapWidth;
    [ShowInInspector]
    private int _mapHeight;
    private readonly ObjectPool<AStarNode> _nodePool = new ObjectPool<AStarNode>(20000,() => new AStarNode());
    [ShowInInspector]
    private AStarNode[][] _aStarGrid;
    public AStarNode[][] AStarGridPy => this._aStarGrid;
    [ShowInInspector]
    private Dictionary<TileEnum,TileConfig> _tileEnum_tileConfig;

    [Title("Cache")]
    private readonly Heap<AStarNode> _openCollection = new Heap<AStarNode>(10000);
    private readonly List<Vector3Int> _neighborCoordsCache = new List<Vector3Int>(6);
    [ShowInInspector]
    private readonly Collider2D[] _collider2DsCache = new Collider2D[3];
    public Collider2D[] Collider2DsCachePy => this._collider2DsCache;

    [Title("Methods")]
    public void Initialize(Dictionary<TileEnum,TileConfig> tileEnum_tileConfig)
    {
        this._tileEnum_tileConfig = tileEnum_tileConfig;
    }

    public bool TryFindPath(Vector3Int startCoord,Vector3Int targetCoord,[CanBeNull] List<Vector3Int> listToStorePath = null,
                            [CanBeNull] BattleConfig.CampRelations selfRelations = null)
    {
        if (startCoord == targetCoord) return false;
        if (!this.IsInRange(startCoord)) return false;
        AStarNode startNode = this._aStarGrid[startCoord.x][startCoord.y];
        if (!this.IsInRange(targetCoord)) return false;
        AStarNode targetNode = this._aStarGrid[targetCoord.x][targetCoord.y];
        if (!PathfindingDetails.IsWalkable(targetNode,selfRelations,true)) return false;

        //清理之前的 Cache.
        this.ResetAllNodeFlags();
        this._openCollection.Clear();

        //功能: Open 中添加起始点.
        startNode.GCost = 0;
        this.PushAndSetFlag(startNode);

        //功能: 计算方向, 给出一个微小的偏移量.
        Vector3Int lastDirection = Vector3Int.down;

        //功能: 记录循环次数.
        int iterationTimes = 0;

        //开始循环.
        while (true)
        {
            AStarNode currentNode = this.PopAndSetFlags();
            Vector3Int currentCoord = currentNode.Coord;

            //如果达到了终点, 找出路径.
            if (currentCoord == targetCoord)
            {
                RetracePath();
                return true;
            }

            //功能: 计算方向的优化, 这一轮中如果已经计算到了同向点, 就不用再计算了.
            bool hasFoundSameDirection = false;
            bool isSameDirection = false;

            //功能: 计算上一个方向. 初始点不用计算方向.
            if (iterationTimes != 0)
            {
                lastDirection = OffsetUtilities.CalculateDirection(currentNode.ParentNode.Coord,currentCoord);
            }

            //功能: 拿到周围的格子, 范围外的和 Close 中的不拿, 但是 Open 中即使是重复的还是需要拿.
            this.GetNeighbors(currentCoord,this._neighborCoordsCache,selfRelations);

            foreach (Vector3Int neighborCoord in this._neighborCoordsCache)
            {
                AStarNode neighborNode = this._aStarGrid[neighborCoord.x][neighborCoord.y];

                //功能: 计算当前的方向.
                if (!hasFoundSameDirection)
                {
                    Vector3Int currentDirection = OffsetUtilities.CalculateDirection(currentCoord,neighborCoord);
                    isSameDirection = lastDirection == currentDirection;

                    if (isSameDirection)
                    {
                        hasFoundSameDirection = true;
                    }
                }

                //功能: 计算新的 GCost. 末尾添加了一个同方向的偏移量, 用于 Smooth 路径, 但是注意 GCost 的基数要大.
                int newGCost;

                if (isSameDirection)
                {
                    //   newGCost = this.CalculateGCost(currentNode,neighborNode) + 1;
                    newGCost = this.CalculateGCost(currentNode,neighborNode);
                }
                else
                {
                    newGCost = this.CalculateGCost(currentNode,neighborNode);
                }

                if (neighborNode.AStarFlags.HasFlag(AStarFlags.IsInOpen))
                {
                    //当新的 GCost 更近, 此时改变其 GCost 和 Parent.
                    if (newGCost < neighborNode.GCost)
                    {
                        neighborNode.GCost = newGCost;
                        neighborNode.ParentNode = currentNode;
                    }
                }
                //功能: 如果这一点是初次加入 OpenCollection.
                else
                {
                    neighborNode.GCost = newGCost;
                    neighborNode.HCost = this.CalculateHCost(neighborCoord,targetCoord);
                    neighborNode.ParentNode = currentNode;
                    this.PushAndSetFlag(neighborNode);
                }
            }

            //循环次数 ++.
            iterationTimes++;
            // Debug.LogError($"循环次数: {iterationTimes}");

            //最后还是没有路.
            if (this._openCollection.Count == 0)
            {
                return false;
            }
        }

        void RetracePath()
        {
            if (listToStorePath == null) return;
            listToStorePath.Clear();
            AStarNode currentNode = targetNode;

            while (currentNode != startNode)
            {
                listToStorePath.Add(currentNode.Coord);
                currentNode = currentNode.ParentNode;
            }

            //功能: 添加起始点.
            listToStorePath.Add(startNode.Coord);
            listToStorePath.Reverse();
        }
    }

    public bool IsInRange(Vector3Int coord)
    {
        return this.IsInRange(coord.x,coord.y);
    }
    public bool IsInRange(int x,int y)
    {
        return x.IsInRange(0,this._mapWidth,ExclusiveFlags.MaxExclusive) && y.IsInRange(0,this._mapHeight,ExclusiveFlags.MaxExclusive);
    }

    private void ResetAllNodeFlags()
    {
        for (int x = 0; x < this._mapWidth; x++)
        {
            for (int y = 0; y < this._mapHeight; y++)
            {
                this._aStarGrid[x][y].AStarFlags = AStarFlags.None;
            }
        }
    }

    //功能: 从 OpenCollection 中拿出一个 Node, 并且标记 Flags.
    private AStarNode PopAndSetFlags()
    {
        AStarNode currentNode = this._openCollection.Pop();
        currentNode.AStarFlags = currentNode.AStarFlags.RemoveFlags(AStarFlags.IsInOpen);
        currentNode.AStarFlags = currentNode.AStarFlags.AddFlags(AStarFlags.IsInClose);
        return currentNode;
    }

    private void GetNeighbors(Vector3Int currentCoord,List<Vector3Int> neighborCoordsCache,BattleConfig.CampRelations selfRelations)
    {
        neighborCoordsCache.Clear();
        int xCoord = currentCoord.x;
        int yCoord = currentCoord.y;
        Vector3Int coord;

        if ((yCoord % 2) == 0)
        {
            coord = new Vector3Int(xCoord - 1,yCoord + 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord,yCoord + 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord - 1,yCoord,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord + 1,yCoord,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord - 1,yCoord - 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord,yCoord - 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }
        }
        else
        {
            coord = new Vector3Int(xCoord,yCoord + 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord + 1,yCoord + 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord - 1,yCoord,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord + 1,yCoord,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord,yCoord - 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }

            coord = new Vector3Int(xCoord + 1,yCoord - 1,0);
            if (CheckCoordAndSetFlag(coord,selfRelations))
            {
                neighborCoordsCache.Add(coord);
            }
        }

        bool CheckCoordAndSetFlag(Vector3Int coordLc,BattleConfig.CampRelations selfRelationsParam)
        {
            //功能: GetRing 的Selector 要注意.
            //功能: 不在范围内, 不选.
            if (!this.IsInRange(coordLc)) return false;
            AStarNode aStarNode = this._aStarGrid[coordLc.x][coordLc.y];
            //功能: 已经 Close, 不选.
            if (aStarNode.AStarFlags.HasFlag(AStarFlags.IsInClose)) return false;
            //功能: 之前未计算过, 初次计算, 结果表示不可行走, 除了不选, 还要标记计算结果, 减少之后的计算量.
            if (!PathfindingDetails.IsWalkable(aStarNode,selfRelationsParam))
            {
                aStarNode.AStarFlags = aStarNode.AStarFlags.AddFlags(AStarFlags.IsInClose);
                return false;
            }

            return true;
        }
    }

    private int CalculateGCost(AStarNode currentNode,AStarNode neighborNode)
    {
        return currentNode.GCost + 100 * this._tileEnum_tileConfig[neighborNode.StaticCell.TileEnumPy].APCostMultiplierPy;
    }

    private int CalculateHCost(Vector3Int coordA,Vector3Int coordB)
    {
        return OffsetUtilities.CalculateSteps(coordA,coordB) * 100;
    }

    //功能: 在 OpenCollection 中放入一个 Node, 并且标记 Flags.
    private void PushAndSetFlag(AStarNode node)
    {
        this._openCollection.Push(node);
        node.AStarFlags = node.AStarFlags.AddFlags(AStarFlags.IsInOpen);
    }

    public bool CheckIfInRangeAndWalkable(Vector3Int coord,BattleConfig.CampRelations selfRelations,bool isTerminal = false)
    {
        return this.IsInRange(coord) && PathfindingDetails.IsWalkable(this._aStarGrid[coord.x][coord.y],selfRelations,isTerminal);
    }

    public bool CheckIfInRangeAndWalkable(Vector3Int coord,AStarNode node,BattleConfig.CampRelations selfRelations)
    {
        return this.IsInRange(coord) && PathfindingDetails.IsWalkable(node,selfRelations);
    }

    public void ResetPathfinding(int mapWidth,int mapHeight,TerrainStaticCell[][] terrainStaticGrid,
                                 List<Vector3Int> obstacleCoords)
    {
        //先将当前所有的 Nodes 清理进 Pool 中.
        if (this._aStarGrid != null)
        {
            foreach (AStarNode[] row in this._aStarGrid)
            {
                foreach (AStarNode aStarNode in row)
                {
                    this._nodePool.ReturnItemToPool(aStarNode);
                }
            }
        }

        this._mapWidth = mapWidth;
        this._mapHeight = mapHeight;

        this._aStarGrid = new AStarNode[this._mapWidth][];
        for (int i = 0; i < this._mapWidth; i++)
        {
            this._aStarGrid[i] = new AStarNode[this._mapHeight];
        }
        for (int x = 0; x < this._mapWidth; x++)
        {
            for (int y = 0; y < this._mapHeight; y++)
            {
                AStarNode currentNode = this._nodePool.GetItemFromPool();
                currentNode.ParentNode = null;
                currentNode.Coord = new Vector3Int(x,y,0);
                currentNode.StaticCell = terrainStaticGrid[x][y];
                currentNode.TerrainDynamicFlags = TerrainDynamicFlags.None;
                this._aStarGrid[x][y] = currentNode;
            }
        }

        if (obstacleCoords != null)
        {
            foreach (Vector3Int obstacleCoord in obstacleCoords)
            {
                //Debug.
                if (!this.IsInRange(obstacleCoord))
                {
                    Debug.LogError($"设置 {nameof(TerrainDynamicFlags)} 时, 发现有坐标: {obstacleCoord} 不在地图范围内.");
                    continue;
                }

                AStarNode aStarNode = this._aStarGrid[obstacleCoord.x][obstacleCoord.y];
                aStarNode.TerrainDynamicFlags = aStarNode.TerrainDynamicFlags.AddFlags(TerrainDynamicFlags.IsObstacle);
            }
        }
    }
}
}