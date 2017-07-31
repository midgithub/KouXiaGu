﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using KouXiaGu.Grids;
using KouXiaGu.World.Map;
using KouXiaGu.World;

namespace KouXiaGu.Terrain3D.MapEditor
{

    public interface INodeInfoEditer
    {

        /// <summary>
        /// 当选择的点变化时调用;
        /// </summary>
        void OnSelectNodeChanged(List<NodePair> data);

        /// <summary>
        /// 更新这些点的信息;
        /// </summary>
        void OnUpdateSelectNode(List<NodePair> data);
    }

    /// <summary>
    /// 对选中坐标进行编辑;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NodeEditPanel : MonoBehaviour
    {
        NodeEditPanel()
        {
        }

        public SelectNodeList selectNodeList;
        public Button resetButton;
        public Button saveButton;

        public bool IsMapInitialized
        {
            get { return WorldSceneManager.World != null; }
        }

        void Awake()
        {
        }

        bool TryGetMap(out IDictionary<CubicHexCoord, MapNode> map)
        {
            if (WorldSceneManager.World != null)
            {
                map = WorldSceneManager.World.WorldData.MapData.Map;
                return true;
            }
            map = default(IDictionary<CubicHexCoord, MapNode>);
            return false;
        }
    }

    public struct NodePair
    {
        public NodePair(CubicHexCoord position, MapNode data)
        {
            Position = position;
            Data = data;
        }

        public CubicHexCoord Position { get; private set; }
        public MapNode Data { get; set; }
    }
}
