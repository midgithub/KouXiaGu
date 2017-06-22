﻿using KouXiaGu.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using KouXiaGu.Concurrent;
using KouXiaGu.World.Resources;

namespace KouXiaGu.Terrain3D
{

    class TerrainResourcesReader
    {
        public WorldResources Read(GameResourceUnityDispatcher dispatcher, WorldResources resources, IOperationState state)
        {
            if (XiaGu.IsUnityThread)
            {
                throw new ArgumentException("只允许在非Unity线程调用;");
            }

            var assetBundleReader = new AssetBundleReader();
            dispatcher.Add(assetBundleReader);
            while (!assetBundleReader.IsCompleted)
            {
                if (state.IsCanceled)
                    throw new OperationCanceledException();
            }
            AssetBundle assetBundle = assetBundleReader.Result;

            foreach (var landformInfo in resources.Landform.Values)
            {
                LandformResourceReader request = new LandformResourceReader(assetBundle, landformInfo);
                dispatcher.Add(request);
            }

            foreach (var buildingInfo in resources.Building.Values)
            {
                BuildingResourceReader request = new BuildingResourceReader(assetBundle, buildingInfo);
                dispatcher.Add(request);
            }

            foreach (var roadInfo in resources.Road.Values)
            {
                RoadResourceReader request = new RoadResourceReader(assetBundle, roadInfo);
                dispatcher.Add(request);
            }

            while (dispatcher.RequestCount != 0)
            {
                if (state.IsCanceled)
                    throw new OperationCanceledException();
            }

            AssetBundleUnload assetBundleUnloadRequest = new AssetBundleUnload(assetBundle, false);
            dispatcher.Add(assetBundleUnloadRequest);
            return resources;
        }
    }

    class AssetBundleReader : AsyncOperation<AssetBundle>, IAsyncRequest
    {
        const string assetBundleName = "terrain";

        string AssetBundleFilePath
        {
            get { return Path.Combine(Resource.AssetBundleDirectoryPath, assetBundleName); }
        }

        void IAsyncRequest.AddQueue() { }

        void IAsyncRequest.Operate()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetBundleFilePath);
            OnCompleted(assetBundle);
        }
    }

    class AssetBundleUnload : IAsyncRequest
    {
        public AssetBundleUnload(AssetBundle assetBundle, bool unloadAllLoadedObjects)
        {
            this.assetBundle = assetBundle;
        }

        AssetBundle assetBundle;
        bool unloadAllLoadedObjects;

        void IAsyncRequest.AddQueue() { }

        void IAsyncRequest.Operate()
        {
            assetBundle.Unload(unloadAllLoadedObjects);
        }
    }
}