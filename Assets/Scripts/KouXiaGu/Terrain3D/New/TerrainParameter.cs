﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KouXiaGu.Terrain3D
{

    [DisallowMultipleComponent]
    class TerrainParameter : UnitySington<TerrainParameter>
    {

        TerrainParameter()
        {
        }

        /// <summary>
        /// 地形Shader;
        /// </summary>
        public Shader TerrainShader;

        /// <summary>
        /// 地形细分程度;
        /// </summary>
        [SerializeField, Range(0, 32)]
        float tessellation = 16f;

        /// <summary>
        /// 地形高度缩放;
        /// </summary>
        [SerializeField, Range(0, 5)]
        float displacement = 1.3f;

        /// <summary>
        /// 降雪程度;
        /// </summary>
        [SerializeField, Range(0, 20)]
        float snowLevel = 0f;


        /// <summary>
        /// 地形细分程度;
        /// </summary>
        public float Tessellation
        {
            get { return tessellation; }
        }

        public void SetTessellation(float value)
        {
            Shader.SetGlobalFloat("_TerrainTess", value);
            tessellation = value;
        }


        /// <summary>
        /// 地形高度缩放;
        /// </summary>
        public float Displacement
        {
            get { return displacement; }
        }

        public void SetDisplacement(float value)
        {
            Shader.SetGlobalFloat("_TerrainDisplacement", value);
            displacement = value;
        }


        /// <summary>
        /// 降雪程度;
        /// </summary>
        public float SnowLevel
        {
            get { return snowLevel; }
        }

        public void SetSnowLevel(float value)
        {
            Shader.SetGlobalFloat("_TerrainSnow", value);
            snowLevel = value;
        }

    }

}
