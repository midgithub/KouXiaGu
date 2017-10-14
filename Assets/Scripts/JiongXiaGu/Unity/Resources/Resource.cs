﻿using JiongXiaGu.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.Unity.Resources
{

    /// <summary>
    /// 对游戏资源文件路径进行定义,需要手动初始化;
    /// </summary>
    [ConsoleMethodsClass]
    public static class Resource
    {
        private static bool isInitialized = false;

        /// <summary>
        /// 核心数据和配置文件的文件夹;
        /// </summary>
        public static ModInfo CoreDirectoryInfo { get; private set; }

        /// <summary>
        /// 用于存放用户配置文件的文件夹;
        /// </summary>
        public static DirectoryInfo UserConfigDirectoryInfo { get; private set; }

        /// <summary>
        /// 存放存档的文件夹路径;
        /// </summary>
        public static DirectoryInfo ArchivesDirectoryInfo { get; private set; }

        /// <summary>
        /// 模组资源,在未指定时为null;
        /// </summary>
        internal static ModResource ModResource { get; set; }

        /// <summary>
        /// 在游戏开始时初始化;
        /// </summary>
        internal static Task Initialize()
        {
            if (!isInitialized)
            {
                CoreDirectoryInfo = ReadCoreDirectoryInfo();
                UserConfigDirectoryInfo = ReadUserConfigDirectoryInfo();
                ArchivesDirectoryInfo = ReadArchivesDirectoryInfo();
                isInitialized = true;
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 主要数据和配置文件的文件夹;
        /// </summary>
        public static string CoreDirectory
        {
            get { return CoreDirectoryInfo.DirectoryInfo.FullName; }
        }

        /// <summary>
        /// 用于存放用户配置文件的文件夹;
        /// </summary>
        public static string UserConfigDirectory
        {
            get { return UserConfigDirectoryInfo.FullName; }
        }

        /// <summary>
        /// 存放存档的文件夹路径;
        /// </summary>
        public static string ArchivesDirectory
        {
            get { return ArchivesDirectoryInfo.FullName; }
        }

        /// <summary>
        /// 获取到核心数据和配置文件的文件夹;
        /// </summary>
        public static ModInfo ReadCoreDirectoryInfo()
        {
            string directory = Application.streamingAssetsPath;
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            directoryInfo.ThrowIfDirectoryNotExisted();
            var dataDirectoryInfo = new ModInfo(directoryInfo);
            return dataDirectoryInfo;
        }

        public static DirectoryInfo ReadUserConfigDirectoryInfo()
        {
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", Application.productName);
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            directoryInfo.Create();
            return directoryInfo;
        }

        public static DirectoryInfo ReadArchivesDirectoryInfo()
        {
            var userConfigDirectory = UserConfigDirectory;
            if (userConfigDirectory == null)
            {
                userConfigDirectory = ReadUserConfigDirectoryInfo().FullName;
            }

            string directory = Path.Combine(userConfigDirectory, "Saves");
            DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            directoryInfo.Create();
            return directoryInfo;
        }
        
        [ConsoleMethod("log_resource_path_info", "显示所有资源路径;", IsDeveloperMethod = true)]
        public static void LogInfoAll()
        {
            string str =
                "\nDataDirectoryPath : " + CoreDirectory +
                "\nUserConfigDirectoryPath" + UserConfigDirectory +
                "\nArchiveDirectoryPath : " + ArchivesDirectory;
            Debug.Log(str);
        }
    }
}
