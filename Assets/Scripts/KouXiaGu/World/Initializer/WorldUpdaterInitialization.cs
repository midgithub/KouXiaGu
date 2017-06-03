﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Terrain3D;

namespace KouXiaGu.World
{


    public class WorldUpdaterInitialization : IWorldUpdater
    {
        public WorldUpdaterInitialization(IWorld world)
        {
            if (world == null)
                throw new ArgumentNullException("world");

            Initialize(world);
        }

        public SceneUpdater LandformUpdater { get; private set; }

        void Initialize(IWorld world)
        {
            LandformUpdater = new SceneUpdater(world);
            var landformUpdaterOperation = LandformUpdater.Start();
            while (!landformUpdaterOperation.IsCompleted)
            {
            }
        }

        public void Dispose()
        {
            if (LandformUpdater != null)
            {
                LandformUpdater.Stop();
                LandformUpdater = null;
            }
        }
    }
}
