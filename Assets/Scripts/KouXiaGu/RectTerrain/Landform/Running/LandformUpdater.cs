﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KouXiaGu.Grids;
using KouXiaGu.World.RectMap;

namespace KouXiaGu.RectTerrain
{

    public class LandformUpdater : TerrainUpdater<RectCoord, LandformChunkRenderer>
    {
        public LandformUpdater(LandformBuilder builder, TerrainGuiderGroup<RectCoord> guiderGroup, WorldMap map) : base(builder, guiderGroup)
        {
            worldMap = map;
            mapChangedRecorder = new DictionaryChangedRecorder<RectCoord, MapNode>();
            unsubscriber = map.ObservableMap.Subscribe(mapChangedRecorder);
        }

        WorldMap worldMap;
        IDisposable unsubscriber;
        DictionaryChangedRecorder<RectCoord, MapNode> mapChangedRecorder;

        IDictionary<RectCoord, MapNode> map
        {
            get { return worldMap.Map; }
        }

        protected override void GetPointsToUpdate(ref ICollection<RectCoord> needUpdatePoints)
        {
            base.GetPointsToUpdate(ref needUpdatePoints);

            RecordeItem<RectCoord, MapNode> recorde;
            while (mapChangedRecorder.TryDequeue(out recorde))
            {
                MapNode node;
                if (map.TryGetValue(recorde.Key, out node))
                {
                    if (HasChanged(node, recorde.OriginalValue))
                    {
                        needUpdatePoints.Add(recorde.Key);
                    }
                }
            }
        }

        bool HasChanged(MapNode node, MapNode original)
        {
            return  node.Landform != original.Landform 
                || node.Road != original.Road;
        }
    }
}
