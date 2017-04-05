﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.World.Map;

namespace KouXiaGu.World
{

    /// <summary>
    /// 提供初始化的世界信息;
    /// </summary>
    public class WorldInfo
    {
        /// <summary>
        /// 时间;
        /// </summary>
        public WorldTimeInfo Time { get; set; }

        /// <summary>
        /// 使用的地图信息;
        /// </summary>
        public MapFile Map { get; set; }



        public ArchiveWorldInfo ArchiveInfo { get; set; }

        public bool IsInitializeFromArchive
        {
            get { return ArchiveInfo != null; }
        }


        /// <summary>
        /// 人口每日增长比例;
        /// </summary>
        public float ProportionOfDailyGrowth { get; set; }

    }

    /// <summary>
    /// 存档信息;
    /// </summary>
    public class ArchiveWorldInfo
    {

        /// <summary>
        /// 存档地图信息;
        /// </summary>
        public ArchiveMapFile Map { get; set; }

    }


}
