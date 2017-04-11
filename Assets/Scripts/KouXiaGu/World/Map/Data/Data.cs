﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KouXiaGu.World.Map
{

    public class Data
    {
        public MapData Map { get; private set; }
        public ArchiveMap ArchiveMap { get; private set; }

        public Data(MapData map)
        {
            Map = map;
            ArchiveMap = new ArchiveMap();
            ArchiveMap.Subscribe(Map);
        }

        /// <summary>
        /// 构造;
        /// </summary>
        /// <param name="map">不包含存档内容的地图数据;</param>
        /// <param name="archive">变化内容,存档内容</param>
        public Data(MapData map, ArchiveMap archive)
        {
            Map = map;
            ArchiveMap = archive;
            Map.Update(ArchiveMap);
            ArchiveMap.Subscribe(Map);
        }

        /// <summary>
        /// 重新输出地图(保存修改后的地图);
        /// </summary>
        public void WriteMap()
        {
            MapDataReader.instance.Write(Map);
        }

        /// <summary>
        /// 输出存档;
        /// </summary>
        public void WriteArchived(string archivedDir)
        {
            ArchiveMapReader reader = ArchiveMapReader.Create(archivedDir);
            reader.Write(ArchiveMap);
        }

        public void SetArchiveMap(ArchiveMap archive)
        {
            ArchiveMap.Unsubscribe();
            Map.Update(archive);
            ArchiveMap = archive;
            archive.Subscribe(Map);
        }
    }

}
