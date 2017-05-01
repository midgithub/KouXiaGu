﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Grids;

namespace KouXiaGu.Terrain3D
{

    public interface ILandformWatcher
    {
        void UpdateDispaly(LandformScene scene);
    }

    /// <summary>
    /// 场景管理,负责对场景地形块的创建和销毁进行管理;
    /// </summary>
    public class LandformScene : IUnityThreadBehaviour<Action>
    {
        public LandformScene(LandformBuilder builder)
        {
            this.builder = builder;
            createCoords = new Dictionary<RectCoord, BakeTargets>();
            destroyCoords = new List<RectCoord>();
            watcherList = new List<ILandformWatcher>();
            OnUpdateSendDisplay();
            this.SubscribeUpdate();
        }

        readonly LandformBuilder builder;
        readonly Dictionary<RectCoord, BakeTargets> createCoords;
        readonly List<RectCoord> destroyCoords;
        readonly List<ILandformWatcher> watcherList;

        IReadOnlyDictionary<RectCoord, ChunkRequest> sceneDisplayedChunks
        {
            get { return builder.SceneDisplayedChunks; }
        }

        IEnumerable<RectCoord> sceneCoords
        {
            get { return sceneDisplayedChunks.Keys; }
        }

        object IUnityThreadBehaviour<Action>.Sender
        {
            get { return "场景的地形块创建销毁管理"; }
        }

        Action IUnityThreadBehaviour<Action>.Action
        {
            get { return OnUpdateSendDisplay; }
        }

        public ICollection<ILandformWatcher> WatcherCollection
        {
            get { return watcherList; }
        }

        public void AddLandformWatcher(ILandformWatcher watcher)
        {
            if (watcherList.Contains(watcher))
                throw new ArgumentException();

            watcherList.Add(watcher);
        }

        public bool RemoveLandformWatcher(ILandformWatcher watcher)
        {
            return watcherList.Remove(watcher);
        }

        public void Display(RectCoord chunkCoord, BakeTargets targets)
        {
            if (createCoords.ContainsKey(chunkCoord))
            {
                createCoords[chunkCoord] |= targets;
            }
            else
            {
                createCoords.Add(chunkCoord, targets);
            }
        }

        void OnUpdateSendDisplay()
        {
            UpdateDispalyCoords();

            ICollection<RectCoord> needDestroyCoords = GetNeedDestroyCoords();
            foreach (var coord in needDestroyCoords)
            {
                this.builder.Destroy(coord);
            }

            IDictionary<RectCoord, BakeTargets> needCreateCoords = GetNeedCreateCoords();
            foreach (var item in needCreateCoords)
            {
                this.builder.CreateOrUpdate(item.Key, item.Value);
            }

            createCoords.Clear();
            destroyCoords.Clear();
        }

        void UpdateDispalyCoords()
        {
            foreach (var item in watcherList)
            {
                item.UpdateDispaly(this);
            }
        }

        ICollection<RectCoord> GetNeedDestroyCoords()
        {
            foreach (var coord in sceneCoords)
            {
                if (!createCoords.ContainsKey(coord))
                    destroyCoords.Add(coord);
            }
            return destroyCoords;
        }

        IDictionary<RectCoord, BakeTargets> GetNeedCreateCoords()
        {
            return createCoords;
        }

    }

}
