﻿using KouXiaGu.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KouXiaGu.World
{

    /// <summary>
    /// 游戏场景初始化器;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorldInitializer : SceneSington<WorldInitializer>
    {
        WorldInitializer()
        {
        }

        [SerializeField]
        DataInitializer worldDataInitializer;
        List<Task> tasks;
        IInitializer[] initializers;
        internal Task InitializeTask { get; private set; }

        public bool IsCompleted
        {
            get { return InitializeTask != null ? InitializeTask.IsCompleted : false; }
        }

        public bool IsFaulted
        {
            get { return InitializeTask != null ? InitializeTask.IsFaulted : false; }
        }

        public AggregateException Exception
        {
            get { return InitializeTask != null ? InitializeTask.Exception : null; }
        }

        public bool IsRunning { get; private set; }
        public CancellationTokenSource TokenSource { get; private set; }

        void Awake()
        {
            tasks = new List<Task>();
            initializers = GetComponentsInChildren<IInitializer>();
            TokenSource = new CancellationTokenSource();
        }

        void Start()
        {
            StartInitialize();
        }

        async void StartInitialize()
        {
            if (IsCompleted)
            {
                return;
            }

            IsRunning = true;

            while (!worldDataInitializer.IsCompleted)
            {
                await Task.Delay(500);
            }

            foreach (var initializer in initializers)
            {
                Task task = initializer.StartInitialize(TokenSource.Token);
                tasks.Add(task);
            }

            try
            {
                InitializeTask = Task.WhenAll(tasks);
                await InitializeTask;
                Debug.Log("[场景初始化]完成;");
            }
            catch
            {
                Debug.LogError("[场景初始化]时遇到错误:" + InitializeTask.Exception);
            }
            finally
            {
                tasks = null;
                initializers = null;
                InitializeTask = null;
                IsRunning = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TokenSource.Cancel();
        }
    }
}
