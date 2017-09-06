﻿using System;
using System.Xml.Serialization;
using UnityEngine;
using KouXiaGu.Concurrent;

namespace KouXiaGu.RectTerrain
{

    /// <summary>
    /// 地貌资源信息;
    /// </summary>
    [XmlRoot("LandformResourceInfo")]
    public sealed class LandformResource
    {
        public LandformResource()
        {
        }

        /// <summary>
        /// 高度调整贴图;
        /// </summary>
        [XmlElement("HeightTex")]
        public TextureInfo HeightTex { get; set; }

        /// <summary>
        /// 高度调整的权重贴图;
        /// </summary>
        [XmlElement("HeightBlendTex")]
        public TextureInfo HeightBlendTex { get; set; }

        /// <summary>
        /// 漫反射贴图名;
        /// </summary>
        [XmlElement("DiffuseTex")]
        public TextureInfo DiffuseTex { get; set; }

        /// <summary>
        /// 漫反射混合贴图名;
        /// </summary>
        [XmlElement("DiffuseBlendTex")]
        public TextureInfo DiffuseBlendTex { get; set; }

        /// <summary>
        /// 是否为空?
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return
                    DiffuseTex.Texture == null &&
                    DiffuseBlendTex.Texture == null &&
                    HeightTex.Texture == null &&
                    HeightBlendTex.Texture == null;
            }
        }

        /// <summary>
        /// 是否所有都不为空?
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return
                    DiffuseTex.IsCompleted &&
                    DiffuseBlendTex.IsCompleted &&
                    HeightTex.IsCompleted &&
                    HeightBlendTex.IsCompleted;
            }
        }

        /// <summary>
        /// 销毁所有贴图;
        /// </summary>
        public void Destroy()
        {
            if (DiffuseTex.Texture != null)
            {
                Destroy(DiffuseTex.Texture);
                DiffuseTex = null;
            }

            if (DiffuseBlendTex.Texture != null)
            {
                Destroy(DiffuseBlendTex.Texture);
                DiffuseBlendTex = null;
            }

            if (HeightTex.Texture != null)
            {
                Destroy(HeightTex.Texture);
                HeightTex = null;
            }

            if (HeightBlendTex.Texture != null)
            {
                Destroy(HeightBlendTex.Texture);
                HeightBlendTex = null;
            }
        }

        void Destroy(UnityEngine.Object item)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(item);
#else
            GameObject.Destroy(item);
#endif
        }
    }

    /// <summary>
    /// 序列化之后处理程序;
    /// </summary>
    public class LandformResourceAfterSerialization
    {

    }
}
