﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KouXiaGu.HexTerrain
{


    /// <summary>
    /// 地形烘焙请求;
    /// 烘焙的为一个正六边形网格内的区域;
    /// </summary>
    public struct RenderRequest
    {

        public RenderRequest(IReadOnlyMap<CubicHexCoord, LandformNode> map, ShortVector2 blockCoord)
        {
            this.Map = map;
            this.BlockCoord = blockCoord;
        }

        public IReadOnlyMap<CubicHexCoord, LandformNode> Map { get; set; }

        /// <summary>
        /// 请求烘焙的地图块位置;
        /// </summary>
        public ShortVector2 BlockCoord { get; set; }

        /// <summary>
        /// 摄像机位置;
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return TerrainData.BlockToPixelCenter(BlockCoord) + new Vector3(0, 5, 0); }
        }

        /// <summary>
        /// 获取到这次需要烘焙的所有节点;
        /// </summary>
        public IEnumerable<BakingNode> GetBakingNodes()
        {
            IEnumerable<CubicHexCoord> cover = TerrainData.GetBlockCover(BlockCoord);
            LandformNode node;

            float index = -2;

            foreach (var coord in cover)
            {
                Vector3 pixPoint = HexGrids.HexToPixel(coord, index--);
                if (Map.TryGetValue(coord, out node))
                {
                    yield return new BakingNode(pixPoint, node);
                }
                else
                {
                    yield return default(BakingNode);
                }
            }
        }

        //float GetBakingNodeHeight(CubicHexCoord coord)
        //{
        //    float height = coord.GetHashCode();
        //    return height > 0 ? -height : height;
        //}

        public void BasicTextureComplete(Texture2D diffuse, Texture2D height)
        {
            TerrainData.Create(BlockCoord, diffuse, height);
        }

    }

}
