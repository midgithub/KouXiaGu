﻿using KouXiaGu.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace KouXiaGu.Terrain3D
{

    [XmlType("TerrainBuilding")]
    public class TerrainBuildingInfo
    {
        /// <summary>
        /// 建筑预制物体名;
        /// </summary>
        [XmlElement("PrefabName")]
        public string PrefabName { get; set; }
    }

    public class BuildingResource : IDisposable
    {
        public BuildingResource(TerrainBuildingInfo info, GameObject prefab)
        {
            Info = info;
            Prefab = prefab;
        }

        public TerrainBuildingInfo Info { get; private set; }
        public GameObject Prefab { get; internal set; }

        public bool IsLoadComplete
        {
            get { return Prefab != null; }
        }

        public void Dispose()
        {
            GameObject.Destroy(Prefab);
            Prefab = null;
        }
    }

    public class BuildingReader : AsyncOperation<Dictionary<int, BuildingResource>>
    {
        public BuildingReader(ISegmented stopwatch, AssetBundle assetBundle, IEnumerable<BuildingInfo> infos)
        {
            this.stopwatch = stopwatch;
            this.assetBundle = assetBundle;
            this.infos = infos;
        }

        readonly ISegmented stopwatch;
        readonly AssetBundle assetBundle;
        readonly IEnumerable<BuildingInfo> infos;

        /// <summary>
        /// 在协程读取;
        /// </summary>
        public IEnumerator ReadAsync()
        {
            Result = new Dictionary<int, BuildingResource>();
            foreach (BuildingInfo info in infos)
            {
                BuildingResource resource;
                TryRead(info.Terrain, out resource);
                Result.Add(info.ID, resource);

                if (stopwatch.Await())
                {
                    yield return null;
                    stopwatch.Restart();
                }
            }
            OnCompleted();
        }

        public bool TryRead(TerrainBuildingInfo info, out BuildingResource item)
        {
            GameObject prefab = assetBundle.LoadAsset<GameObject>(info.PrefabName);
            item = new BuildingResource(info, prefab);

            if (item.IsLoadComplete)
            {
                return true;
            }
            else
            {
                Debug.LogWarning("[地形建筑读取]未找到对应的预制物体;Name:" + info.PrefabName);
                return false;
            }
        }
    }

}
