﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KouXiaGu.Grids;
using KouXiaGu.World.RectMap;
using KouXiaGu.World;
using System.Threading;
using UnityEngine.UI;

namespace KouXiaGu.RectTerrain
{

    /// <summary>
    /// 将鼠标所指向地图节点的内容输出为文本;
    /// </summary>
    [DisallowMultipleComponent]
    class MapContentDisplayer : MonoBehaviour, IComponentInitializeHandle
    {

        [SerializeField]
        Text textObject;
        IDictionary<RectCoord, MapNode> map;

        Task IComponentInitializeHandle.StartInitialize(CancellationToken token)
        {
            map = RectMapDataInitializer.Instance.WorldMap.Map;
            enabled = true;
            return null;
        }

        void Awake()
        {
            enabled = false;
        }

        void Update()
        {
            textObject.text = TextUpdate();
        }

        string TextUpdate()
        {
            Vector3 mousePoint;
            if (LandformRay.Instance.TryGetMouseRayPoint(out mousePoint))
            {
                RectCoord pos = mousePoint.ToRectTerrainRect();
                MapNode node;

                if (map.TryGetValue(pos, out node))
                {
                    NodeLandformInfo landform = node.Landform;
                    //RoadNode roadNode = node.Road;
                    //BuildingNode buildingNode = node.Building;
                    return
                        "坐标:" + pos.ToString()
                        + "\nLandform:" + landform.TypeID + ",Angle:" + landform.Angle;
                        //+ "\nRoad:" + roadNode.ID + ",Type:" + roadNode.RoadType + ",ExistRoad:" + roadNode.Exist()
                        //+ "\nBuilding:" + buildingNode.ID + ",Types:" + node.Building.BuildingType + ",ExistBuilding:" + node.Building.Exist()
                        //+ "\nTags:"
                        //+ "\nTown:" + node.Town.TownID;
                }
            }
            return "检测不到地形;";
        }
    }
}
