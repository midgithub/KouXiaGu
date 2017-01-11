﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 用于保存的地形节点结构;
    /// </summary>
    [ProtoContract]
    public struct TerrainNode
    {

        /// <summary>
        /// 代表的地形ID((0,-1作为保留);
        /// </summary>
        [ProtoMember(1)]
        public int Landform;

        /// <summary>
        /// 地形旋转角度;
        /// </summary>
        [ProtoMember(2)]
        public float LandformAngle;

        /// <summary>
        /// 存在地貌信息?
        /// </summary>
        public bool ExistLandform
        {
            get { return Landform != 0; }
        }


        /// <summary>
        /// 道路类型编号?不存在则为0,否则为道路类型编号;
        /// </summary>
        [ProtoMember(3)]
        public int Road;

        /// <summary>
        /// 存在道路?
        /// </summary>
        public bool ExistRoad
        {
            get { return Road != 0; }
        }


        /// <summary>
        /// 建筑物类型编号;
        /// </summary>
        [ProtoMember(4)]
        public int Building;

        /// <summary>
        /// 建筑物旋转的角度;
        /// </summary>
        [ProtoMember(5)]
        public float BuildingAngle;

        /// <summary>
        /// 存在建筑物?
        /// </summary>
        public bool ExistBuild
        {
            get { return Building != 0; }
        }

    }

}