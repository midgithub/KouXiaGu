﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KouXiaGu.Grids;
using KouXiaGu.World;
using KouXiaGu.World.Map;
using UnityEngine;
using KouXiaGu.Terrain3D.DynamicMesh;

namespace KouXiaGu.Terrain3D
{
    /// <summary>
    /// 地形道路烘培;
    /// </summary>
    [Serializable]
    class BakeRoad
    {
        [SerializeField]
        DynamicMeshScript prefab = null;
        [SerializeField]
        Shader diffuseShader = null;
        [SerializeField]
        Shader heightShader = null;

        List<Pack> sceneObjects;
        RoadMeshPool objectPool;

        BakeCamera bakeCamera;
        IWorld world;
        CubicHexCoord chunkCenter;
        IEnumerable<CubicHexCoord> displays;
        public RenderTexture DiffuseRT { get; private set; }
        public RenderTexture HeightRT { get; private set; }

        IReadOnlyDictionary<CubicHexCoord, MapNode> map
        {
            get { return world.WorldData.MapData.ReadOnlyMap; }
        }

        IDictionary<int, RoadInfo> infos
        {
            get { return world.BasicData.BasicResource.Terrain.Road; }
        }

        public void Initialise()
        {
            sceneObjects = new List<Pack>();
            objectPool = new RoadMeshPool(prefab, "BakeRoadMesh");
        }

        public void BakeCoroutine(BakeCamera bakeCamera, IWorld world, CubicHexCoord chunkCenter, LandformRenderer result)
        {
            this.bakeCamera = bakeCamera;
            this.world = world;
            this.chunkCenter = chunkCenter;
            this.displays = ChunkPartitioner.GetRoad(chunkCenter);

            PrepareScene();
            BakeDiffuse();
            BakeHeight();

            var diffuseMap = bakeCamera.GetDiffuseTexture(DiffuseRT, TextureFormat.ARGB32);
            var heightMap = bakeCamera.GetHeightTexture(HeightRT);
            result.SetRoadDiffuseMap(diffuseMap);
            result.SetRoadHeightMap(heightMap);

            ClearScene();
            Reset();
        }

        //public IEnumerator BakeCoroutine(BakeCamera bakeCamera, IWorld world, CubicHexCoord chunkCenter, LandformRenderer result, IState state)
        //{
        //    this.bakeCamera = bakeCamera;
        //    this.world = world;
        //    this.chunkCenter = chunkCenter;
        //    this.displays = ChunkPartitioner.GetRoad(chunkCenter);

        //    PrepareScene();
        //    yield return null;
        //    if (state.IsCanceled)
        //    {
        //        goto _End_;
        //    }

        //    BakeDiffuse();
        //    yield return null;
        //    if (state.IsCanceled)
        //    {
        //        goto _End_;
        //    }


        //    BakeHeight();
        //    yield return null;
        //    if (state.IsCanceled)
        //    {
        //        goto _End_;
        //    }

        //    var diffuseMap = bakeCamera.GetDiffuseTexture(DiffuseRT, TextureFormat.ARGB32);
        //    var heightMap = bakeCamera.GetHeightTexture(HeightRT);
        //    result.SetRoadDiffuseMap(diffuseMap);
        //    result.SetRoadHeightMap(heightMap);

        //    _End_:
        //    ClearScene();
        //    Reset();
        //}

        /// <summary>
        /// 释放所有该实例创建的 RenderTexture 类型的资源;
        /// </summary>
        void Reset()
        {
            bakeCamera.ReleaseTemporary(DiffuseRT);
            DiffuseRT = null;

            bakeCamera.ReleaseTemporary(HeightRT);
            HeightRT = null;
        }

        void PrepareScene()
        {
            foreach (var display in displays)
            {
                MapNode node;
                if (map.TryGetValue(display, out node))
                {
                    if (node.Road.Exist())
                    {
                        int roadType = node.Road.RoadType;
                        RoadInfo info;
                        if (infos.TryGetValue(roadType, out info))
                        {
                            RoadResource resource = info.Terrain;
                            var meshs = CreateMesh(display).ToArray();
                            if (meshs.Length > 0)
                            {
                                var pack = new Pack(resource, meshs);
                                sceneObjects.Add(pack);
                            }
                        }
                        else
                        {
                            Debug.LogWarning("未找到对应道路贴图,ID:[" + roadType + "];");
                        }
                    }
                }
            }
        }

        IEnumerable<Pack<DynamicMeshScript, MeshRenderer>> CreateMesh(CubicHexCoord target)
        {
            var routes = GetPeripheralRoutes(target);
            foreach (var route in routes)
            {
                var spline = ConvertSpline(target, route);
                var roadMesh = objectPool.Get();
                roadMesh.Value1.transform.position = target.GetTerrainPixel();
                roadMesh.Value1.Transformation(spline);
                yield return roadMesh;
            }
        }

        /// <summary>
        /// 迭代获取到这个点通向周围的路径点,若不存在节点则不进行迭代;
        /// </summary>
        public IEnumerable<CubicHexCoord[]> GetPeripheralRoutes(CubicHexCoord target)
        {
            PeripheralRoute.TryGetPeripheralValue tryGetValue = delegate (CubicHexCoord position, out uint value)
            {
                MapNode node;
                if (map.TryGetValue(position, out node))
                {
                    if (node.Road.Exist())
                    {
                        value = node.Road.ID;
                        return true;
                    }
                }
                value = default(uint);
                return false;
            };
            return PeripheralRoute.GetRoadRoutes(target, tryGetValue);
        }

        /// <summary>
        /// 转换为坐标;
        /// </summary>
        ISpline ConvertSpline(CubicHexCoord target, CubicHexCoord[] route)
        {
            Vector3[] points = new Vector3[route.Length];
            for (int i = 0; i < route.Length; i++)
            {
                CubicHexCoord localPosition = route[i] - target;
                points[i] = localPosition.GetTerrainPixel();
            }
            CatmullRomSpline spline = new CatmullRomSpline(points[0], points[1], points[2], points[3]);
            return spline;
        }


        void ClearScene()
        {
            foreach (var pack in sceneObjects)
            {
                foreach (var mesh in pack.Packs)
                {
                    objectPool.Release(mesh);
                }
            }
            sceneObjects.Clear();
        }

        void BakeDiffuse()
        {
            foreach (var meshRenderer in sceneObjects)
            {
                SetDiffuserMaterial(meshRenderer);
            }

            DiffuseRT = bakeCamera.GetDiffuseTemporaryRender();
            bakeCamera.CameraRender(DiffuseRT, chunkCenter, LandformSettings.BlackTransparent);
        }

        void SetDiffuserMaterial(Pack pack)
        {
            RoadResource res = pack.Res;
            Material material = new Material(diffuseShader);
            material.SetTexture("_MainTex", res.DiffuseTex);
            material.SetTexture("_BlendTex", res.DiffuseBlendTex);

            foreach (var item in pack.Packs)
            {
                if (item.Value2.sharedMaterial != null)
                    GameObject.Destroy(item.Value2.sharedMaterial);

                item.Value2.sharedMaterial = material;
            }
        }


        void BakeHeight()
        {
            foreach (var meshRenderer in sceneObjects)
            {
                SetHeightMaterial(meshRenderer);
            }

            HeightRT = bakeCamera.GetHeightTemporaryRender();
            bakeCamera.CameraRender(HeightRT, chunkCenter);
        }

        void SetHeightMaterial(Pack pack)
        {
            RoadResource res = pack.Res;
            Material material = new Material(heightShader);
            material.SetTexture("_MainTex", res.HeightAdjustTex);

            foreach (var item in pack.Packs)
            {
                if (item.Value2.sharedMaterial != null)
                    GameObject.Destroy(item.Value2.sharedMaterial);

                item.Value2.sharedMaterial = material;
            }
        }

        struct Pack
        {
            public Pack(RoadResource res, IEnumerable<Pack<DynamicMeshScript, MeshRenderer>> rednerer)
            {
                this.Res = res;
                this.Packs = rednerer;
            }

            public RoadResource Res { get; private set; }
            public IEnumerable<Pack<DynamicMeshScript, MeshRenderer>> Packs { get; private set; }
        }


        class RoadMeshPool : ObjectPool<Pack<DynamicMeshScript, MeshRenderer>>
        {
            public RoadMeshPool(DynamicMeshScript prefab, string parentName)
            {
                this.prefab = prefab;
                this.objectParent = new GameObject(parentName).transform;
            }

            readonly DynamicMeshScript prefab;
            readonly Transform objectParent;

            public override Pack<DynamicMeshScript, MeshRenderer> Instantiate()
            {
                var mesh = GameObject.Instantiate(prefab);
                mesh.transform.SetParent(objectParent);
                MeshRenderer renderer = mesh.GetComponent<MeshRenderer>();
                return new Pack<DynamicMeshScript, MeshRenderer>(mesh, renderer);
            }

            public override void Destroy(Pack<DynamicMeshScript, MeshRenderer> item)
            {
                var gameObject = item.Value1.gameObject;
                GameObject.Destroy(gameObject);
            }

            public override void ResetWhenEnterPool(Pack<DynamicMeshScript, MeshRenderer> item)
            {
                //item.Value1.Reset();
                item.Value1.gameObject.SetActive(false);
            }

            public override void ResetWhenOutPool(Pack<DynamicMeshScript, MeshRenderer> item)
            {
                item.Value1.gameObject.SetActive(true);
            }
        }

    }

}