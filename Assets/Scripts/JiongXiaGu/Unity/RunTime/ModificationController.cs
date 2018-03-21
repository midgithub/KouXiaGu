﻿using JiongXiaGu.Unity.Initializers;
using JiongXiaGu.Unity.Resources;
using JiongXiaGu.Unity.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.Unity.RunTime
{

    /// <summary>
    /// 模组管理;
    /// </summary>
    public static class ModificationController
    {
        /// <summary>
        /// 所有模组信息,包括核心资源;
        /// </summary>
        public static List<ModificationInfo> ModificationInfos { get; private set; }

        /// <summary>
        /// 寻找所有模组;
        /// </summary>
        internal static void SearcheAll()
        {
            ModificationFactory contentSearcher = new ModificationFactory();
            ModificationInfos = new List<ModificationInfo>();

            string directory = Path.Combine(Resource.StreamingAssetsPath, "Data");
            Core = contentSearcher.Read(directory);

            var mods = contentSearcher.EnumerateModifications(Resource.ModDirectory);
            ModificationInfos.AddRange(mods);

            var userMods = contentSearcher.EnumerateModifications(Resource.UserModDirectory);
            ModificationInfos.AddRange(userMods);
        }

        /// <summary>
        /// 根据预先定义的模组顺序获取到激活的模组(按先后读取顺序);
        /// </summary>
        public static List<ModificationInfo> GetActiveModificationInfos()
        {
            try
            {
                ModificationLoadOrder order;
                ModificationLoadOrderSerializer serializer = new ModificationLoadOrderSerializer();

                order = serializer.Deserialize();
                return GetActiveModificationInfos(order);
            }
            catch
            {
                List<ModificationInfo> newList = new List<ModificationInfo>();
                return newList;
            }
        }

        /// <summary>
        /// 根据模组顺序获取到激活的模组(按先后读取顺序);
        /// </summary>
        public static List<ModificationInfo> GetActiveModificationInfos(ModificationLoadOrder activeModification)
        {
            List<ModificationInfo> newList = new List<ModificationInfo>();

            if (ModificationInfos != null)
            {
                foreach (var id in activeModification.IDList)
                {
                    int index = ModificationInfos.FindIndex(info => info.Description.ID == id);
                    if (index >= 0)
                    {
                        ModificationInfo info = ModificationInfos[index];
                        newList.Add(info);
                    }
                }
            }

            return newList;
        }

        /// <summary>
        /// 筛选模组;
        /// </summary>
        public static List<ModificationInfo> GetIdleModificationInfos(IList<ModificationInfo> activeModificationInfos)
        {
            var idleModificationInfos = new List<ModificationInfo>();

            if (ModificationInfos != null)
            {
                foreach (var modificationInfo in ModificationInfos)
                {
                    if (!activeModificationInfos.Contains(modificationInfo))
                    {
                        idleModificationInfos.Add(modificationInfo);
                    }
                }
            }

            return idleModificationInfos;
        }

        /// <summary>
        /// 尝试获取到对应模组信息;
        /// </summary>
        public static bool TryGetInfo(string id, out ModificationInfo info)
        {
            int index = ModificationInfos.FindIndex(item => item.Description.ID == id);
            if (index >= 0)
            {
                info = ModificationInfos[index];
                return true;
            }
            else
            {
                info = default(ModificationInfo);
                return false;
            }
        }




        /// <summary>
        /// 核心资源;
        /// </summary>
        public static Modification Core { get; private set; }

        /// <summary>
        /// 需要读取的模组资源,包括核心资源;
        /// </summary>
        internal static List<Modification> ModificationContents { get; private set; }

        private static Task initializeTask;
        private static CancellationTokenSource cancellationTokenSource;
        public static TaskStatus InitializeTaskStatus => initializeTask != null ? initializeTask.Status : TaskStatus.WaitingToRun;

        /// <summary>
        /// 进行初始化,若已经初始化,初始化中则无任何操作;
        /// </summary>
        public static Task Initialize(IProgress<ProgressInfo> progress)
        {
            UnityThread.ThrowIfNotUnityThread();

            if (initializeTask == null || initializeTask.IsCompleted)
            {
                cancellationTokenSource = new CancellationTokenSource();
                initializeTask = InternalInitialize(progress, cancellationTokenSource.Token);
            }
            else if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException("初始化正在被取消!");
            }

            return initializeTask;
        }

        private static async Task InternalInitialize(IProgress<ProgressInfo> progress, CancellationToken token)
        {
            progress?.Report(new ProgressInfo(0.1f, "程序初始化"));
            await Program.WorkTask;

            progress?.Report(new ProgressInfo(0.2f, "模组排序"));
            ModificationContents = GetModificationContent();

            progress?.Report(new ProgressInfo(0.5f, "模组初始化"));
            var resourceProgress = new LocalProgress(progress, 0.5f, 1f);
            await ModificationInitializer.StartInitialize(ModificationContents, resourceProgress, token);

            progress?.Report(new ProgressInfo(1f, "初始化完毕"));
        }

        /// <summary>
        /// 取消初始化;
        /// </summary>
        public static Task Cancel(IProgress<ProgressInfo> progress)
        {
            UnityThread.ThrowIfNotUnityThread();
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            if (initializeTask != null)
            {
                cancellationTokenSource.Cancel();
                return initializeTask.ContinueWith(delegate (Task task)
                {
                    cancellationTokenSource = null;
                    initializeTask = null;
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }


        /// <summary>
        /// 根据预先定义的模组顺序获取到激活的模组(按先后读取顺序),包含核心模组;
        /// </summary>
        private static List<Modification> GetModificationContent()
        {
            try
            {
                ModificationLoadOrder order;
                ModificationLoadOrderSerializer serializer = new ModificationLoadOrderSerializer();

                order = serializer.Deserialize();
                return GetModificationContent(order);
            }
            catch (FileNotFoundException)
            {
                List<Modification> newList = new List<Modification>();
                newList.Add(Core);
                return newList;
            }
        }

        /// <summary>
        /// 根据模组顺序获取到激活的模组(按先后读取顺序),包含核心模组;
        /// </summary>
        private static List<Modification> GetModificationContent(ModificationLoadOrder activeModification)
        {
            List<Modification> newList = new List<Modification>();
            newList.Add(Core);
            ModificationFactory factory = new ModificationFactory();

            if (ModificationContents == null)
            {
                foreach (var id in activeModification.IDList)
                {
                    int index = ModificationInfos.FindIndex(_info => _info.Description.ID == id);
                    if (index >= 0)
                    {
                        var info = ModificationInfos[index];
                        Modification content = factory.Read(info.ModificationDirectory);
                        newList.Add(content);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("未找到ID为[{0}]的模组;", id));
                    }
                }
            }
            else
            {
                List<Modification> old = ModificationContents;

                foreach (var info in activeModification.IDList)
                {
                    Modification content;
                    int index = old.FindIndex(oldContent => oldContent.Description.ID == info);
                    if (index >= 0)
                    {
                        content = old[index];
                        old[index] = null;
                    }
                    else
                    {
                        content = factory.Read(info);
                    }

                    newList.Add(content);
                }

                foreach (var mod in old)
                {
                    if (mod != null)
                    {
                        mod.Dispose();
                    }
                }
            }

            return newList;
        }
    }
}
