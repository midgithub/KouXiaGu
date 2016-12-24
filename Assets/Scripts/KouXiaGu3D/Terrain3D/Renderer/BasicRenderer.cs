﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;


namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 基本贴图信息渲染,负责将传入的请求渲染出基本的高度图和地貌贴图;
    /// </summary>
    [DisallowMultipleComponent, CustomEditorTool]
    public sealed class BasicRenderer : UnitySingleton<BasicRenderer>
    {
        BasicRenderer() { }

        /// <summary>
        /// 负责渲染的摄像机;
        /// </summary>
        [SerializeField]
        Camera bakingCamera;
        /// <summary>
        /// 在烘焙过程中放置在场景内的物体;
        /// </summary>
        [SerializeField]
        RenderDisplayMeshPool ovenDisplayMeshPool;

        [SerializeField, HideInInspector]
        BakingParameter prm = new BakingParameter(150);

        [SerializeField]
        Shader mixer;
        [SerializeField]
        Shader height;
        [SerializeField]
        Shader normalMap;
        [SerializeField]
        NormalMapper normalMapper;
        [SerializeField]
        Shader heightToAlpha;
        [SerializeField]
        Shader diffuse;
        [SerializeField]
        Shader blur;

        static Material mixerMaterial;
        static Material heightMaterial;
        static Material normalMapMaterial;
        static Material heightToAlphaMaterial;
        static Material diffuseMaterial;
        static Material blurMaterial;

        static Coroutine bakingCoroutine;

        /// <summary>
        /// 将要进行烘焙的队列(对外只提供查询,以允许移除;);
        /// </summary>
        static readonly LinkedList<BakeRequest> bakingQueue = new LinkedList<BakeRequest>();

        public static bool IsRunning
        {
            get { return bakingCoroutine != null; }
        }

        public static LinkedList<BakeRequest> BakingRequests
        {
            get { return bakingQueue; }
        }

        [ExposeProperty]
        public float TextureSize
        {
            get { return prm.TextureSize; }
            set { prm = new BakingParameter(value); }
        }

        public static void Clear()
        {
            bakingQueue.Clear();
        }

        void Awake()
        {
            ovenDisplayMeshPool.Awake();
        }

        void Start()
        {
            InitBakingCamera();
            InitMaterial();

            StartCoroutine();
        }

        /// <summary>
        /// 加入到烘焙队列;
        /// </summary>
        public static void Enqueue(BakeRequest request)
        {
            bakingQueue.AddLast(request);
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

        void InitMaterial()
        {
            mixerMaterial = new Material(mixer);
            mixerMaterial.hideFlags = HideFlags.HideAndDontSave;

            heightMaterial = new Material(height);
            heightMaterial.hideFlags = HideFlags.HideAndDontSave;

            normalMapMaterial = new Material(normalMap);
            normalMapMaterial.hideFlags = HideFlags.HideAndDontSave;

            heightToAlphaMaterial = new Material(heightToAlpha);
            heightToAlphaMaterial.hideFlags = HideFlags.HideAndDontSave;

            diffuseMaterial = new Material(diffuse);
            diffuseMaterial.hideFlags = HideFlags.HideAndDontSave;

            blurMaterial = new Material(blur);
            blurMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// 在协程内队列中进行烘焙;
        /// </summary>
        /// <returns></returns>
        IEnumerator Baking()
        {
            CustomYieldInstruction bakingYieldInstruction = new WaitWhile(() => bakingQueue.Count == 0);

            while (true)
            {
                yield return bakingYieldInstruction;

                var request = bakingQueue.Dequeue();

                IEnumerable<KeyValuePair<BakingNode, MeshRenderer>> bakingNodes = PrepareBaking(request);

                RenderTexture mixerRT = null;
                RenderTexture heightRT = null;
                RenderTexture normalMapRT = null;
                RenderTexture diffuseRT = null;
                Texture2D normalMap;
                Texture2D heightMap;
                Texture2D diffuse;

                try
                {
                    mixerRT = BakingMixer(bakingNodes);
                    heightRT = BakingHeight(bakingNodes, mixerRT);
                    normalMapRT = BakingNormalMap(heightRT);
                    diffuseRT = BakingDiffuse(bakingNodes, mixerRT, normalMapRT);

                    normalMap = GetNormalMap(normalMapRT);
                    heightMap = GetHeightMap(heightRT);
                    diffuse = GetDiffuseTexture(diffuseRT);

                    request.TextureComplete(diffuse, heightMap, normalMap);

                    //diffuse.SavePNG(@"123");
                    //heightMap.SavePNG(@"123");
                    //normalMap.SavePNG(@"123");

                }
                finally
                {
                    RenderTexture.ReleaseTemporary(mixerRT);
                    RenderTexture.ReleaseTemporary(heightRT);
                    RenderTexture.ReleaseTemporary(normalMapRT);
                    RenderTexture.ReleaseTemporary(diffuseRT);
                }
            }
        }

        /// <summary>
        /// 烘焙前的准备,返回烘焙对应的网格;
        /// </summary>
        List<KeyValuePair<BakingNode, MeshRenderer>> PrepareBaking(BakeRequest request)
        {
            bakingCamera.transform.position = request.CameraPosition;

            IEnumerable<BakingNode> bakingNodes = request.BakingNodes;
            List<KeyValuePair<BakingNode, MeshRenderer>> list = new List<KeyValuePair<BakingNode, MeshRenderer>>();

            ovenDisplayMeshPool.RecoveryActive();

            int indexY = -2;

            foreach (var node in bakingNodes)
            {
                Vector3 position = new Vector3(node.Position.x, indexY--, node.Position.z);
                var mesh = ovenDisplayMeshPool.Dequeue(position, node.RotationY);
                list.Add(new KeyValuePair<BakingNode, MeshRenderer>(node, mesh));
            }

            return list;
        }


        /// <summary>
        /// 对混合贴图进行烘焙;传入需要设置到的地貌节点;
        /// </summary>
        RenderTexture BakingMixer(IEnumerable<KeyValuePair<BakingNode, MeshRenderer>> bakingNodes)
        {
            foreach (var pair in bakingNodes)
            {
                BakingNode node = pair.Key;
                MeshRenderer hexMesh = pair.Value;

                if (hexMesh.material != null)
                    GameObject.Destroy(hexMesh.material);

                hexMesh.material = mixerMaterial;
                hexMesh.material.mainTexture = node.MixerTexture;
            }

            RenderTexture mixerRT = RenderTexture.GetTemporary(prm.rDiffuseMapWidth, prm.rDiffuseMapHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            Render(mixerRT);
            return mixerRT;
        }

        /// <summary>
        /// 混合高度贴图;
        /// </summary>
        RenderTexture BakingHeight(IEnumerable<KeyValuePair<BakingNode, MeshRenderer>> bakingNodes, Texture mixer)
        {
            foreach (var pair in bakingNodes)
            {
                BakingNode node = pair.Key;
                MeshRenderer hexMesh = pair.Value;

                if (hexMesh.material != null)
                    GameObject.Destroy(hexMesh.material);

                hexMesh.material = heightMaterial;
                hexMesh.material.SetTexture("_MainTex", node.HeightTexture);
                hexMesh.material.SetTexture("_Mixer", node.MixerTexture);
                hexMesh.material.SetTexture("_GlobalMixer", mixer);
            }

            RenderTexture heightRT = RenderTexture.GetTemporary(prm.rHeightMapWidth, prm.rHeightMapHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            Render(heightRT);
            return heightRT;
        }

        /// <summary>
        /// 将高度图从灰度通道转到阿尔法通道上;
        /// </summary>
        [Obsolete]
        RenderTexture BakingHeightToAlpha(Texture height)
        {
            RenderTexture alphaHeightRT = RenderTexture.GetTemporary(height.width, height.height, 0, RenderTextureFormat.ARGB32);

            height.filterMode = FilterMode.Bilinear;
            alphaHeightRT.filterMode = FilterMode.Bilinear;

            Graphics.Blit(height, alphaHeightRT, heightToAlphaMaterial, 0);
            return alphaHeightRT;
        }

        /// <summary>
        /// 根据高度图生成法线贴图,把高度图的高度信息转移到输出的 阿尔法通道上;
        /// </summary>
        RenderTexture BakingNormalMap(Texture height)
        {
            return normalMapper.Rander(height);
        }

        /// <summary>
        /// 烘焙材质贴图;
        /// </summary>
        RenderTexture BakingDiffuse(IEnumerable<KeyValuePair<BakingNode, MeshRenderer>> bakingNodes, Texture globalMixer, Texture globalHeight)
        {
            foreach (var pair in bakingNodes)
            {
                BakingNode node = pair.Key;
                MeshRenderer hexMesh = pair.Value;

                if (hexMesh.material != null)
                    GameObject.Destroy(hexMesh.material);

                hexMesh.material = diffuseMaterial;

                hexMesh.material.SetTexture("_MainTex", node.DiffuseTexture);
                hexMesh.material.SetTexture("_Mixer", node.MixerTexture);
                hexMesh.material.SetTexture("_Height", node.HeightTexture);
                //hexMesh.material.SetTexture("_GlobalMixer", globalMixer);
                //hexMesh.material.SetTexture("_ShadowsAndHeight", globalHeight);
                //hexMesh.material.SetFloat("_Sea", 0f);
                hexMesh.material.SetFloat("_Centralization", 1.0f);
            }

            RenderTexture diffuseRT = RenderTexture.GetTemporary(prm.rDiffuseMapWidth, prm.rDiffuseMapHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
            Render(diffuseRT);
            return diffuseRT;
        }

        Texture2D GetHeightMap(RenderTexture rt)
        {
            RenderTexture.active = rt;
            Texture2D heightMap = new Texture2D(prm.HeightMapWidth, prm.HeightMapHeight, TextureFormat.ARGB32, false);
            heightMap.ReadPixels(prm.HeightReadPixel, 0, 0, false);
            heightMap.wrapMode = TextureWrapMode.Clamp;
            heightMap.Apply();

            return heightMap;
        }

        Texture2D GetNormalMap(RenderTexture rt)
        {
            RenderTexture.active = rt;
            Texture2D normalMap = new Texture2D(prm.HeightMapWidth, prm.HeightMapHeight, TextureFormat.ARGB32, false);
            normalMap.ReadPixels(prm.HeightReadPixel, 0, 0, false);
            normalMap.wrapMode = TextureWrapMode.Clamp;
            normalMap.Apply();
            
            return normalMap;
        }

        Texture2D GetDiffuseTexture(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            Texture2D diffuse = new Texture2D(prm.DiffuseMapWidth, prm.DiffuseMapHeight, TextureFormat.RGB24, false);
            diffuse.ReadPixels(prm.DiffuseReadPixel, 0, 0, false);
            diffuse.wrapMode = TextureWrapMode.Clamp;
            diffuse.Apply();

            return diffuse;
        }

        void Render(RenderTexture rt)
        {
            bakingCamera.targetTexture = rt;
            bakingCamera.Render();
            bakingCamera.targetTexture = null;
        }

    }


    /// <summary>
    /// 地形烘焙时的贴图大小参数;
    /// </summary>
    [Serializable]
    public struct BakingParameter
    {

        /// <summary>
        /// 烘焙时的边框比例(需要裁剪的部分比例);
        /// </summary>
        public static readonly float OutlineScale = 1f / 12f;

        /// <summary>
        /// 完整预览整个地图块的摄像机旋转角度;
        /// </summary>
        public static readonly Quaternion CameraRotation = Quaternion.Euler(90, 0, 0);

        /// <summary>
        /// 完整预览整个地图块的摄像机大小;
        /// </summary>
        public static readonly float CameraSize = 
            ((TerrainChunk.CHUNK_HEIGHT + (TerrainChunk.CHUNK_HEIGHT * OutlineScale)) / 2);

        /// <summary>
        /// 完整预览整个地图块的摄像机比例(W/H);
        /// </summary>
        public static readonly float CameraAspect = 
            (TerrainChunk.CHUNK_WIDTH + TerrainChunk.CHUNK_WIDTH * OutlineScale) / 
            (TerrainChunk.CHUNK_HEIGHT + TerrainChunk.CHUNK_HEIGHT * OutlineScale);

        [SerializeField]
        float textureSize;

        /// <summary>
        /// 贴图大小;
        /// </summary>
        public float TextureSize
        {
            get { return textureSize; }
            private set { textureSize = value; }
        }

        /// <summary>
        /// 图片裁剪后的尺寸;
        /// </summary>
        public int DiffuseMapWidth { get; private set; }
        public int DiffuseMapHeight { get; private set; }
        public int HeightMapWidth { get; private set; }
        public int HeightMapHeight { get; private set; }

        /// <summary>
        /// 烘焙时的尺寸;
        /// </summary>
        public int rDiffuseMapWidth { get; private set; }
        public int rDiffuseMapHeight { get; private set; }
        public int rHeightMapWidth { get; private set; }
        public int rHeightMapHeight { get; private set; }

        /// <summary>
        /// 裁剪区域;
        /// </summary>
        public Rect DiffuseReadPixel { get; private set; }
        public Rect HeightReadPixel { get; private set; }

        public BakingParameter(float textureSize) : this()
        {
            SetTextureSize(textureSize);
        }

        void SetTextureSize(float size)
        {
            float chunkWidth = TerrainChunk.CHUNK_WIDTH * size;
            float chunkHeight = TerrainChunk.CHUNK_HEIGHT * size;

            this.DiffuseMapWidth = (int)(chunkWidth);
            this.DiffuseMapHeight = (int)(chunkHeight);
            this.HeightMapWidth = (int)(chunkWidth);
            this.HeightMapHeight = (int)(chunkHeight);

            this.rDiffuseMapWidth = (int)(DiffuseMapWidth + DiffuseMapWidth * OutlineScale);
            this.rDiffuseMapHeight = (int)(DiffuseMapHeight + DiffuseMapHeight * OutlineScale);
            this.rHeightMapWidth = (int)(HeightMapWidth + HeightMapWidth * OutlineScale);
            this.rHeightMapHeight = (int)(HeightMapHeight + HeightMapHeight * OutlineScale);

            this.DiffuseReadPixel = 
                new Rect(
                    DiffuseMapWidth * OutlineScale / 2,
                    DiffuseMapHeight * OutlineScale / 2, 
                    DiffuseMapWidth,
                    DiffuseMapHeight);

            this.HeightReadPixel = 
                new Rect(
                    HeightMapWidth * OutlineScale / 2,
                    HeightMapHeight * OutlineScale / 2,
                    HeightMapWidth,
                    HeightMapHeight);

            this.TextureSize = size;
        }

    }

}
