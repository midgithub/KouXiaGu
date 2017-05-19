﻿using KouXiaGu.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KouXiaGu.World.Map
{

    public interface IMapDataReader
    {
        MapData Read(IGameData info);
    }

    /// <summary>
    /// 随机地图创建;
    /// </summary>
    class RandomMapDataCreater : IMapDataReader
    {
        public RandomMapDataCreater(int mapSize)
        {
            MapSize = mapSize;
        }

        public int MapSize { get; set; }

        public MapData Read(IGameData info)
        {
            int[] landformArray = info.Terrain.LandformInfos.Keys.ToArray();
            int[] buildArray = info.Terrain.BuildingInfos.Keys.ToArray();
            Dictionary<CubicHexCoord, MapNode> map = new Dictionary<CubicHexCoord, MapNode>();
            var points = CubicHexCoord.Range(CubicHexCoord.Self, MapSize);

            foreach (var point in points)
            {
                MapNode node = new MapNode()
                {
                    Landform = new LandformNode()
                    {
                        Type = Random(landformArray),
                        Angle = RandomAngle(),
                    },

                    Building = new BuildingNode()
                    {
                        Type = Random(buildArray),
                        Angle = RandomAngle(),
                    },
                };
                map.Add(point, node);
            }

            MapData data = new MapData()
            {
                Map = map,
            };

            return data;
        }

        static readonly System.Random random = new System.Random();

        T Random<T>(T[] array)
        {
            int index = random.Next(0, array.Length);
            return array[index];
        }

        int RandomAngle()
        {
            return random.Next(0, 360);
        }
    }
}
