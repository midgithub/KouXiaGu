﻿using JiongXiaGu.Collections;
using JiongXiaGu.Grids;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace JiongXiaGu.Unity.RectMaps
{

    /// <summary>
    /// 地图数据;
    /// </summary>
    public class Map
    {
        /// <summary>
        /// 描述;
        /// </summary>
        public MapDescription Description { get; private set; }

        /// <summary>
        /// 地图数据;
        /// </summary>
        public MapData MapData { get; set; }

        public Map(MapDescription description)
        {
            Description = description;
            MapData = new MapData();
        }
        
        public Map(MapDescription description, MapData mapData) 
        {
            Description = description;
            MapData = mapData;
        }
    }
}
