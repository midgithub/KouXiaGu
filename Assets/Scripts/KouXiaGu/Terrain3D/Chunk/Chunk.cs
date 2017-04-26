﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Grids;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 地形块;需要通过静态方法创建;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Chunk : MonoBehaviour
    {

        #region Static;

        /// <summary>
        /// 地形块挂载的脚本;
        /// </summary>
        static readonly Type[] ChunkScripts = new Type[]
            {
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(MeshCollider),
                typeof(Chunk)
            };


#if UNITY_EDITOR
        [MenuItem("GameObject/Create Other/TerrainChunk")]
#endif
        static void _CraeteLandformChunk()
        {
            var item = Create();
        }

        /// <summary>
        /// 使用默认名实例一个地形块;
        /// </summary>
        public static Chunk Create()
        {
            return Create("TerrainChunk");
        }

        /// <summary>
        /// 指定实例名创建一个地形块;
        /// </summary>
        public static Chunk Create(string name)
        {
            GameObject gameObject = new GameObject(name, ChunkScripts);
            Chunk chunk = gameObject.GetComponent<Chunk>();
            return chunk;
        }

        #endregion


        Chunk()
        {
        }

        public bool IsInitialized { get; private set; }
        public LandformMesh Mesh { get; private set; }
        public LandformRenderer Renderer { get; private set; }
        public LandformTrigger Trigger { get; private set; }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        void Awake()
        {
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var meshCollider = GetComponent<MeshCollider>();

            Mesh = new LandformMesh(meshFilter);
            Renderer = new LandformRenderer(meshRenderer);
            Trigger = new LandformTrigger(meshCollider, Renderer);
            gameObject.layer = LandformRay.Instance.RayLayer;
        }

        /// <summary>
        /// 当地形块高度变化时调用;
        /// </summary>
        void OnHeightChanged(LandformRenderer renderer)
        {
            Trigger.RebuildCollisionMesh();
        }

        /// <summary>
        /// 清除所有引用,备下次重复使用;
        /// </summary>
        public void Clear()
        {
            Renderer.Clear();
        }

        /// <summary>
        /// 销毁Unity实例,并清除所有贴图资源;
        /// </summary>
        public void Destroy()
        {
            Renderer.Destroy();
            Destroy(gameObject);
        }

    }

}
