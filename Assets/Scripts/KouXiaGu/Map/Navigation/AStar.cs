﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace KouXiaGu.Map.Navigation
{

    /// <summary>
    /// A*寻路;
    /// </summary>
    public sealed class AStar<TMover>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Mover"></param>
        /// <param name="map"></param>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="maxTime">需要大于或等于2</param>
        public AStar(TMover Mover, INavigationMap<TMover> map, IntVector2 current, IntVector2 target, uint maxTime = uint.MaxValue)
        {
            this.map = map;
            this.Current = current;
            this.Target = target;
            this.ResidualTime = maxTime;

            IsDone = false;
        }

        private INavigationMap<TMover> map;
        private Dictionary<IntVector2, PathNode> openPoints;
        private HashSet<IntVector2> closePoints;
        private PathNode currentNode;

        public TMover Mover { get; private set; }
        public uint ResidualTime { get; private set; }
        public Path WayPath { get; private set; }
        public IntVector2 Current { get; private set; }
        public IntVector2 Target { get; private set; }

        public bool IsDone { get; private set; }
        public Exception Error { get; private set; }

        public void Start()
        {
            if (Error is PathNotFoundException)
                return;

            openPoints = new Dictionary<IntVector2, PathNode>();
            closePoints = new HashSet<IntVector2>();
            AddToOpenPoints(Current, null);
            Pathfinding();
        }

        public void Continue()
        {
            if (Error is PathNotFoundException || IsDone)
                return;

            Current = WayPath.Last.Value;

            openPoints = new Dictionary<IntVector2, PathNode>();
            closePoints = new HashSet<IntVector2>();
            AddToOpenPoints(Current, null);
            Pathfinding();
        }

        private void Pathfinding()
        {
            try
            {
                for (; ResidualTime > uint.MinValue; ResidualTime--)
                {
                    currentNode = GetMinPathNode();
                    var around = map.GetAround(currentNode.Position).
                        Where(pair => !closePoints.Contains(pair.Key));

                    foreach (KeyValuePair<IntVector2, INavigationNode<TMover>> info in around)
                    {
                        AddToOpenPoints(info.Key, currentNode, info.Value);
                    }

                    if (TryGetTarget())
                        return;
                    RemovePoint(currentNode.Position);
                }

                WayPath = NodeToPath(currentNode);
            }
            catch (Exception e)
            {
                Error = e;
                IsDone = false;
            }
        }

        private void AddToOpenPoints(IntVector2 position, PathNode previous)
        {
            INavigationNode<TMover> node = map.GetAt(position);
            AddToOpenPoints(position, previous, node);
        }

        private void AddToOpenPoints(IntVector2 position, PathNode previous, INavigationNode<TMover> node)
        {
            PathNode beforeNode;
            PathNode newNode;

            if (openPoints.TryGetValue(position, out beforeNode))
            {
                beforeNode.TryChangePrevious(currentNode);
            }
            else
            {
                newNode = new PathNode(position, previous, node, Mover, Target);
                openPoints.Add(position, newNode);
            }
        }

        private void RemovePoint(IntVector2 position)
        {
            openPoints.Remove(position);
            closePoints.Add(position);
        }

        private PathNode GetMinPathNode()
        {
            try
            {
                return openPoints.Values.Min();
            }
            catch (InvalidOperationException e)
            {
                throw new PathNotFoundException("已经遍历完所有可行走节点!", e);
            }
        }

        /// <summary>
        /// 尝试获取到终点;获取到了返回true,否则返回false;
        /// </summary>
        /// <returns></returns>
        private bool TryGetTarget()
        {
            PathNode node;
            if (openPoints.TryGetValue(Target, out node))
            {
                WayPath = NodeToPath(node);
                IsDone = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        private Path NodeToPath(PathNode node)
        {
            Path path = new Path();
            for (; node != null; node = node.Previous)
            {
                path.AddFirst(node.Position);
            }
            return path;
        }

        private class PathNode : IComparable<PathNode>
        {
            public PathNode() { }

            public PathNode(IntVector2 position, PathNode previous, INavigationNode<TMover> node, TMover mover, IntVector2 target)
            {
                nodeCost = GetNodeCost(position, node, mover, target);
                this.Position = position;
                this.Previous = previous;
            }

            private float nodeCost;

            public float PathCost { get { return Previous.PathCost + nodeCost; } }
            public IntVector2 Position { get; private set; }
            public PathNode Previous { get; private set; }

            /// <summary>
            /// 尝试替换本结点的 父节点;
            /// 若 挑战者 路径值小于原本 父节点,则替换,并返回true;
            /// </summary>
            /// <param name="challenger"></param>
            /// <returns></returns>
            public bool TryChangePrevious(PathNode challenger)
            {
                if (Previous.PathCost > challenger.PathCost)
                {
                    Previous = challenger;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private float GetNodeCost(IntVector2 position, INavigationNode<TMover> node, TMover mover, IntVector2 target)
            {
                float nodeCost = node.GetCost(mover);
                return nodeCost;
            }

            int IComparable<PathNode>.CompareTo(PathNode other)
            {
                if (other.nodeCost > nodeCost)
                    return -1;
                else if (other.nodeCost < nodeCost)
                    return 1;
                else
                    return 0;
            }

        }

    }

}
