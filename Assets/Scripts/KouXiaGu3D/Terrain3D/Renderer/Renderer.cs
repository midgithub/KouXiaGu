﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KouXiaGu.Grids;
using System.Collections;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 基本贴图信息渲染,负责将传入的请求渲染出基本的高度图和地貌贴图;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class Renderer : UnitySington<Renderer>
    {
        Renderer() { }

        /// <summary>
        /// 负责渲染的摄像机;
        /// </summary>
        [SerializeField]
        Camera bakingCamera;

        [SerializeField]
        BakingParameter parameter = new BakingParameter(120, 0, 1);

        [SerializeField]
        NormalMapper normalMapper;

        [SerializeField]
        RenderTerrain terrain;

        [SerializeField]
        DecorateRoad road;

        [SerializeField]
        DecorateBlend decorateBlend;

        static Coroutine bakingCoroutine;

        /// <summary>
        /// 将要进行烘焙的队列;
        /// </summary>
        static readonly LinkedList<IBakeRequest> bakingQueue = new LinkedList<IBakeRequest>();

        /// <summary>
        /// 是否运行中?
        /// </summary>
        public static bool IsRunning
        {
            get { return bakingCoroutine != null; }
        }

        /// <summary>
        /// 烘焙时的参数;
        /// </summary>
        public static BakingParameter Parameter
        {
            get { return GetInstance.parameter; }
            set { GetInstance.parameter = value; }
        }

        static Camera BakingCamera
        {
            get { return GetInstance.bakingCamera; }
        }

        /// <summary>
        /// 烘焙请求队列;
        /// </summary>
        public static LinkedList<IBakeRequest> BakingRequests
        {
            get { return bakingQueue; }
        }

        public static void Clear()
        {
            bakingQueue.Clear();
        }

        protected override void Awake()
        {
            base.Awake();
            terrain.Awake();
            road.Awake();
            decorateBlend.Awake();
        }

        void Start()
        {
            InitBakingCamera();
            StartCoroutine();
        }

        void OnValidate()
        {
            parameter.Reset();
        }

        /// <summary>
        /// 开始烘焙协程;
        /// </summary>
        public void StartCoroutine()
        {
            if (!IsRunning)
            {
                bakingCoroutine = StartCoroutine(Baking());
            }
        }

        /// <summary>
        /// 停止烘焙的协程,清空请求队列;
        /// </summary>
        public void StopCoroutine()
        {
            StopCoroutine(bakingCoroutine);
            bakingQueue.Clear();
        }

        /// <summary>
        /// 初始化烘焙相机参数;
        /// </summary>
        [ContextMenu("初始化相机")]
        void InitBakingCamera()
        {
            bakingCamera.aspect = BakingParameter.CameraAspect;
            bakingCamera.orthographicSize = BakingParameter.CameraSize;
            bakingCamera.transform.rotation = BakingParameter.CameraRotation;
            bakingCamera.clearFlags = CameraClearFlags.SolidColor;  //必须设置为纯色,否则摄像机渲染贴图会有(残图?);

            bakingCamera.backgroundColor = Color.black;
        }

        /// <summary>
        /// 在协程内队列中进行烘焙;
        /// 流程:
        /// 建筑高度平整图->建筑地表图->
        /// 地形高度图->地形地表图->
        /// 混合高度->混合地表->
        /// 高度生成法线图->完成
        /// </summary>
        IEnumerator Baking()
        {
            CustomYieldInstruction bakingYieldInstruction = new WaitWhile(() => bakingQueue.Count == 0);

            IBakeRequest request = null;
            RenderTexture normalMapRT = null;
            RenderTexture heightMapRT = null;
            RenderTexture diffuseMapRT = null;

            Texture2D normalMap = null;
            Texture2D heightMap = null;
            Texture2D diffuse = null;

            while (true)
            {
                yield return bakingYieldInstruction;
                try
                {
                    request = bakingQueue.Dequeue();
                    var cover = GetCover(request);

                    terrain.Render(request, cover);
                    road.Render(request, cover);

                    heightMapRT = decorateBlend.BlendHeight(terrain.HeightRT, road.HeightRT);
                    diffuseMapRT = decorateBlend.BlendDiffuse(terrain.DiffuseRT, road.DiffuseRT);

                    normalMapRT = normalMapper.Rander(heightMapRT);

                    heightMap = GetHeightTexture(heightMapRT);
                    normalMap = normalMapper.GetTexture(normalMapRT);
                    diffuse = GetDiffuseTexture(diffuseMapRT);

                    request.OnComplete(diffuse, heightMap, normalMap);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("烘焙时出现错误:" + ex);

                    Destroy(diffuse);
                    Destroy(heightMap);
                    Destroy(normalMap);

                    if(request != null)
                        request.OnError(ex);
                }
                finally
                {
                    terrain.Dispose();
                    road.Dispose();
                    RenderTexture.ReleaseTemporary(normalMapRT);
                    RenderTexture.ReleaseTemporary(heightMapRT);
                    RenderTexture.ReleaseTemporary(diffuseMapRT);
                }
            }
        }

        /// <summary>
        /// 使用摄像机烘焙;
        /// </summary>
        public static void CameraRender(RenderTexture rt)
        {
            BakingCamera.targetTexture = rt;
            BakingCamera.Render();
            BakingCamera.targetTexture = null;
        }

        /// <summary>
        /// 使用摄像机烘焙;
        /// </summary>
        public static void CameraRender(RenderTexture rt, MeshDisplay display)
        {
            SetBakingCamera(display);
            CameraRender(rt);
        }

        /// <summary>
        /// 使用摄像机指定背景颜色烘焙;
        /// </summary>
        public static void CameraRender(RenderTexture rt, MeshDisplay display, Color backgroundColor)
        {
            SetBakingCamera(display);
            CameraRender(rt, backgroundColor);
        }

        /// <summary>
        /// 使用摄像机指定背景颜色烘焙;
        /// </summary>
        public static void CameraRender(RenderTexture rt, Color backgroundColor)
        {
            Color current = BakingCamera.backgroundColor;
            BakingCamera.backgroundColor = backgroundColor;
            CameraRender(rt);
            BakingCamera.backgroundColor = current;
        }


        /// <summary>
        /// 设置烘焙相机到对应位置;
        /// </summary>
        static void SetBakingCamera(MeshDisplay display)
        {
            Camera bakingCamera = GetInstance.bakingCamera;
            Vector3 cameraPoint = TerrainConvert.Grid.GetPixel(display.Center, 5);
            bakingCamera.transform.position = cameraPoint;
        }

        /// <summary>
        /// 获取到覆盖到的坐标;
        /// </summary>
        IEnumerable<CubicHexCoord> GetCover(IBakeRequest request)
        {
            return TerrainChunk.GetChunkCover(request.ChunkCoord);
        }

        public static Texture2D GetHeightTexture(RenderTexture rt)
        {
            RenderTexture.active = rt;
            Texture2D heightMap = new Texture2D(Parameter.HeightMapWidth, Parameter.HeightMapHeight, TextureFormat.RGB24, false);
            heightMap.ReadPixels(Parameter.HeightReadPixel, 0, 0, false);
            heightMap.wrapMode = TextureWrapMode.Clamp;
            heightMap.Apply();
            return heightMap;
        }

        public static Texture2D GetDiffuseTexture(RenderTexture rt)
        {
            RenderTexture.active = rt;
            Texture2D diffuseTex = new Texture2D(Parameter.DiffuseTexWidth, Parameter.DiffuseTexHeight, TextureFormat.RGB24, false);
            diffuseTex.ReadPixels(Parameter.DiffuseReadPixel, 0, 0, false);
            diffuseTex.wrapMode = TextureWrapMode.Clamp;
            diffuseTex.Apply();
            return diffuseTex;
        }


    }

}
