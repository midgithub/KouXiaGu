﻿using JiongXiaGu.Unity.Resources;
using System;
using System.IO;
using System.Collections.Generic;

namespace JiongXiaGu.Unity.RectMaps
{

    [Obsolete]
    public class MapDataSerializer : ResourcesReader<MapData, MapData>
    {
        public MapDataSerializer(ISerializer<MapData> serializer, ResourcesSearcher resourceSearcher) : base(serializer, resourceSearcher)
        {
        }

        protected override MapData Combine(List<MapData> sources)
        {
            if (sources .Count > 0)
            {
                MapData main = sources[0];
                for (int i = 1; i < sources.Count; i++)
                {
                    MapData other = sources[i];
                    throw new NotImplementedException();
                }
                return main;
            }
            throw new FileNotFoundException("未找到对应地图文件;");
        }

        protected override MapData Convert(MapData result)
        {
            return result;
        }
    }
}
