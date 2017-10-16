﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.Unity.Resources
{

    /// <summary>
    /// 模组信息提供器;
    /// </summary>
    public class ModSearcher
    {

        /// <summary>
        /// 枚举所有模组;
        /// </summary>
        public IEnumerable<ModInfo> EnumerateModInfos()
        {
            Debug.LogWarning("还未实现Mod搜寻;");
            return new List<ModInfo>();
        }
    }
}