﻿using System;
using System.Linq;
using JiongXiaGu.Collections;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;

namespace JiongXiaGu.Unity.Resources
{
    /// <summary>
    /// 游戏数据初始化器(在游戏开始前进行初始化,若初始化失败意味着游戏无法开始);
    /// </summary>
    [DisallowMultipleComponent]
    public class ResourceInitializer : MonoBehaviour
    {
        private static readonly GlobalSingleton<ResourceInitializer> singleton = new GlobalSingleton<ResourceInitializer>();
        public static ResourceInitializer Instance => singleton.GetInstance();
        private ResourceLoadHandler loadHandler;

        private void Awake()
        {
            loadHandler = new ResourceLoadHandler()
            {
                IntegrateHandlers = GetComponentsInChildren<IResourceIntegrateHandle>(),
            };
        }

        public Task LoadResource()
        {
            throw new NotImplementedException();
        }

        public Task LoadResource(LoadOrder loadOrder)
        {
            throw new NotImplementedException();
        }
    }
}
