﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KouXiaGu.Collections;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 动态墙体网格;
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    public sealed class DynamicWallMesh : MonoBehaviour
    {
        DynamicWallMesh()
        {
        }



        [ContextMenu("Build")]
        public void Build()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            Mesh mesh = meshFilter.sharedMesh;
            Build(mesh.vertices);
        }

        /// <summary>
        /// 构建节点记录;
        /// </summary>
        public void Build(Vector3[] vertices)
        {
            SortedList<Vector3> verticeSortedList = new SortedList<Vector3>(vertices, VerticeComparer_x.instance);


        }

        /// <summary>
        /// 对比坐标的x值;
        /// </summary>
        class VerticeComparer_x : IComparer<Vector3>
        {
            public static readonly VerticeComparer_x instance = new VerticeComparer_x();

            /// <summary>
            /// 小于零  x 小于 y。
            /// 零      x 等于 y。
            /// 大于零  x 大于 y。
            /// </summary>
            public int Compare(Vector3 x, Vector3 y)
            {
                return (x.x - y.x) < 0 ? -1 : 1;
            }
        }
    }

}
