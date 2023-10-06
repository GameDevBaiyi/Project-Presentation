using System;
using System.Collections.Generic;

using Common.DataTypes;
using Common.Extensions;
using Common.Utilities;

using LowLevelSystems.PathfindingSystems;

using Sirenix.OdinInspector;

using UnityEngine;

namespace LowLevelSystems.CharacterEntitySystems.PcEntitySystems.Components.SkillBarSystems
{
public class PathfindingForSkillBarModule
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

        [ShowInInspector]
        public NodeOfModuleFlags NodeOfModuleFlags;

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

    [ShowInInspector]
    private int _mapWidth;
    [ShowInInspector]
    private int _mapHeight;
    private ObjectPool<AStarNode> _nodePool = new ObjectPool<AStarNode>(100,() => new AStarNode());
    [ShowInInspector]
    private AStarNode[][] _aStarGrid;
    public AStarNode[][] AStarGridPy => this._aStarGrid;

    [Title("Cache")]
    private readonly Heap<AStarNode> _openCollection = new Heap<AStarNode>(100);
    private readonly List<Vector3Int> _neighborCoordsCache = new List<Vector3Int>(6);

    /// <summary>
    /// 注意, 给出的 Path 是从  targetCoord 倒着指向 startCoord.
    /// </summary>
    public bool TryFindPath(Vector3Int startCoord,Vector3Int targetCoord,List<Vector3Int> path)
    {
        if (path == null)
        {
            Debug.LogError("List<Vector3Int> path 传入前要初始化.");
            return false;
        }

        if (startCoord == targetCoord) return false;
        if (!this.CheckIfInRange(startCoord)) return false;
        AStarNode startNode = this._aStarGrid[startCoord.x][startCoord.y];
        if (!this.CheckIfInRange(targetCoord)) return false;
        AStarNode targetNode = this._aStarGrid[targetCoord.x][targetCoord.y];
        if (!this.CheckIfWalkable(targetNode)) return false;

        //清理之前的 Cache.
        this.ResetAllNodeFlags();
        this._openCollection.Clear();

        //功能: Open 中添加起始点.
        startNode.GCost = 0;
        this.PushAndSetFlag(startNode);

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

            //功能: 拿到周围的格子, 范围外的和 Close 中的不拿, 但是 Open 中即使是重复的还是需要拿.
            this.GetNeighbors(currentCoord,this._neighborCoordsCache);

            foreach (Vector3Int neighborCoord in this._neighborCoordsCache)
            {
                AStarNode neighborNode = this._aStarGrid[neighborCoord.x][neighborCoord.y];

                int newGCost = currentNode.GCost + 1;

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

            //最后还是没有路.
            if (this._openCollection.Count == 0)
            {
                return false;
            }
        }

        void RetracePath()
        {
            path.Clear();
            AStarNode currentNode = targetNode;

            while (currentNode != startNode)
            {
                if (currentNode.NodeOfModuleFlags.HasFlag(NodeOfModuleFlags.IsSugarPoint))
                {
                    path.Add(currentNode.Coord);
                }

                currentNode = currentNode.ParentNode;
            }

            //功能: 添加起始点.
            path.Add(startNode.Coord);
        }
    }

    public bool CheckIfInRange(Vector3Int coord)
    {
        return this.CheckIfInRange(coord.x,coord.y);
    }
    public bool CheckIfInRange(int x,int y)
    {
        return x.IsInRange(0,this._mapWidth,ExclusiveFlags.MaxExclusive) && y.IsInRange(0,this._mapHeight,ExclusiveFlags.MaxExclusive);
    }

    private bool CheckIfWalkable(AStarNode aStarNode)
    {
        return aStarNode.NodeOfModuleFlags.HasFlag(NodeOfModuleFlags.IsWalkable);
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

    private void GetNeighbors(Vector3Int currentCoord,List<Vector3Int> neighborCoordsCache)
    {
        neighborCoordsCache.Clear();
        int xCoord = currentCoord.x;
        int yCoord = currentCoord.y;
        Vector3Int coord;

        coord = new Vector3Int(xCoord + 1,yCoord,0);
        if (CheckCoordAndSetFlag(coord))
        {
            neighborCoordsCache.Add(coord);
        }

        coord = new Vector3Int(xCoord - 1,yCoord,0);
        if (CheckCoordAndSetFlag(coord))
        {
            neighborCoordsCache.Add(coord);
        }

        coord = new Vector3Int(xCoord,yCoord + 1,0);
        if (CheckCoordAndSetFlag(coord))
        {
            neighborCoordsCache.Add(coord);
        }

        coord = new Vector3Int(xCoord,yCoord - 1,0);
        if (CheckCoordAndSetFlag(coord))
        {
            neighborCoordsCache.Add(coord);
        }

        bool CheckCoordAndSetFlag(Vector3Int coordLc)
        {
            //功能: GetRing 的Selector 要注意.
            //功能: 不在范围内, 不选.
            if (!this.CheckIfInRange(coordLc)) return false;
            AStarNode aStarNode = this._aStarGrid[coordLc.x][coordLc.y];
            //功能: 已经 Close, 不选.
            if (aStarNode.AStarFlags.HasFlag(AStarFlags.IsInClose)) return false;
            //功能: 之前未计算过, 初次计算, 结果表示不可行走, 除了不选, 还要标记计算结果, 减少之后的计算量.
            if (!this.CheckIfWalkable(aStarNode))
            {
                aStarNode.AStarFlags = aStarNode.AStarFlags.AddFlags(AStarFlags.IsInClose);
                return false;
            }

            return true;
        }
    }

    private int CalculateHCost(Vector3Int coordA,Vector3Int coordB)
    {
        return OffsetUtilities.CalculateSteps(coordA,coordB);
    }

    //功能: 在 OpenCollection 中放入一个 Node, 并且标记 Flags.
    private void PushAndSetFlag(AStarNode node)
    {
        this._openCollection.Push(node);
        node.AStarFlags = node.AStarFlags.AddFlags(AStarFlags.IsInOpen);
    }

    public void ResetPathfinding(NodeOfModuleFlags[][] nodeFlagsGrid)
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

        //准备新数据.
        this._mapWidth = nodeFlagsGrid.Length;
        this._mapHeight = nodeFlagsGrid[0].Length;

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
                currentNode.NodeOfModuleFlags = nodeFlagsGrid[x][y];
                this._aStarGrid[x][y] = currentNode;
            }
        }
    }
}
}