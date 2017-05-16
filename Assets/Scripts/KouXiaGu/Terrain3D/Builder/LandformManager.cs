﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Grids;
using KouXiaGu.World;

namespace KouXiaGu.Terrain3D
{

    public interface ILandformWatcher : IChunkWatcher
    {
    }


    public class LandformManager : ChunkWatcherUpdater
    {
        static LandformManager()
        {
            watcherList = new List<ILandformWatcher>();
        }

        static readonly List<ILandformWatcher> watcherList;

        public static List<ILandformWatcher> WatcherList
        {
            get { return watcherList; }
        }


        public LandformManager(IWorldData worldData)
        {
            builder = new LandformBuilder(worldData);
            SendDisplay();
            StartUpdate();
        }

        readonly LandformBuilder builder;

        public LandformBuilder Builder
        {
            get { return builder; }
        }

        protected override IEnumerable<IChunkWatcher> Watchers
        {
            get { return watcherList.Cast<IChunkWatcher>(); }
        }

        protected override IEnumerable<RectCoord> SceneCoords
        {
            get { return builder.SceneDisplayedChunks.Keys; }
        }

        protected override void CreateAt(RectCoord coord)
        {
            builder.Create(coord);
        }

        protected override void DestroyAt(RectCoord coord)
        {
            builder.Destroy(coord);
        }
    }
}
