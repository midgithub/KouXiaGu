﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KouXiaGu.Grids;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 进行烘焙时的最小单位,代表地图节点;
    /// </summary>
    public struct BakingNode
    {

        /// <summary>
        /// 地貌地图节点;
        /// </summary>
        TerrainNode mapNode;

        /// <summary>
        /// 地貌信息;
        /// </summary>
        Landform123 landform;

        /// <summary>
        /// 节点在地图上的坐标;
        /// </summary>
        public CubicHexCoord Position { get; private set; }

        /// <summary>
        /// 地形贴图旋转角度;
        /// </summary>
        public float RotationY
        {
            get { return mapNode.RotationAngle; }
        }

        public Texture DiffuseTex
        {
            get { return landform.DiffuseTex; }
        }

        public Texture DiffuseBlendTex
        {
            get { return landform.DiffuseBlendTex; }
        }

        public Texture HeightTex
        {
            get { return landform.HeightTex; }
        }

        public Texture HeightBlendTex
        {
            get { return landform.HeightBlendTex; }
        }

        public BakingNode(CubicHexCoord position, TerrainNode mapNode) : this()
        {
            this.Position = position;
            this.mapNode = mapNode;
            this.landform = GetLandform(mapNode);
        }

        /// <summary>
        /// 根据地貌节点获取到地貌信息;
        /// </summary>
        Landform123 GetLandform(TerrainNode landformNode)
        {
            if (landformNode.ID == 0)
                return default(Landform123);

            Landform123 landform;
            try
            {
                landform = Landform123.GetLandform(landformNode.ID);
            }
            catch (KeyNotFoundException)
            {
                landform = Landform123.GetRandomLandform();
                Debug.LogWarning("获取到不存在的地貌ID: " + landformNode.ID + ";随机替换为: " + landform.ID);
            }
            return landform;
        }

    }

}
