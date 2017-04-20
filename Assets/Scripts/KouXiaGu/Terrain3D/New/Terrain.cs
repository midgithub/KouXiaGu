﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.World;
using KouXiaGu.World.Map;
using KouXiaGu.Grids;
using UnityEngine;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 地形控制;
    /// </summary>
    public class Terrain
    {
        public static IAsyncOperation<Terrain> Initialize(IWorld world)
        {
            return new AsyncInitializer(world);
        }

        class AsyncInitializer : AsyncOperation<Terrain>
        {
            public AsyncInitializer(IWorld world)
            {
                var instance = new Terrain();
                instance.World = world;
                instance.TerrainChunk = new TerrainChunkManager();
                OnCompleted(instance);
            }
        }


        Terrain()
        {
        }

        public IWorld World { get; private set; }
        public TerrainChunkManager TerrainChunk { get; private set; }

        public IDictionary<CubicHexCoord, MapNode> Map
        {
            get { return World.Map.Data; }
        }

    }

}
