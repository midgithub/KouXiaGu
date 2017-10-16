﻿using JiongXiaGu.Unity;
using JiongXiaGu.Unity.Initializers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.Diagnostics
{

    /// <summary>
    /// 负责游戏控制台初始化;
    /// </summary>
    [DisallowMultipleComponent]
    public class XiaGuConsoleInitializer : MonoBehaviour, IGameComponentInitializeHandle
    {
        Task IGameComponentInitializeHandle.Initialize(CancellationToken token)
        {
            Task task = Task.Run(delegate ()
            {
                token.ThrowIfCancellationRequested();
                XiaGuConsole.Initialize();
                OnXiaGuConsoleCompleted();
            }, token);
            return task;
        }

        [Conditional("EDITOR_LOG")]
        void OnXiaGuConsoleCompleted()
        {
            const string prefix = "[控制台组件]";
            string log = " 初始化完成,开发者模式:" + XiaGuConsole.IsDeveloperMode + " ,条目总数:" + XiaGuConsole.CommandCollection.Count;
            UnityEngine.Debug.Log(prefix + log);
        }
    }
}