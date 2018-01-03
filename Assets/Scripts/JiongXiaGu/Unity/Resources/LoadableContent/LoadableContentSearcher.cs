﻿using System;
using System.Collections.Generic;
using System.IO;

namespace JiongXiaGu.Unity.Resources
{

    /// <summary>
    /// 负责对可读的资源进行搜索;
    /// </summary>
    public class LoadableContentSearcher
    {
        /// <summary>
        /// 忽略符,置于名称前缀,用于忽略某文件/文件夹;
        /// </summary>
        private const string IgnoreSymbol = "#ignore_";
        public LoadableContentFactory Factory { get; private set; }

        public LoadableContentSearcher()
        {
            Factory = new LoadableContentFactory();
        }

        public LoadableContentSearcher(LoadableContentFactory factory)
        {
            Factory = factory;
        }

        /// <summary>
        /// 获取目录下的所有可读资源;
        /// </summary>
        /// <param name="modsDirectory">目标目录</param>
        /// <param name="type">指定找到的模组类型</param>
        /// <returns></returns>
        public List<LoadableContent> Find(string modsDirectory)
        {
            List<LoadableContent> list = new List<LoadableContent>();

            list.AddRange(EnumerateDirectory(modsDirectory));
            list.AddRange(EnumerateZipFile(modsDirectory));

            return list;
        }

        /// <summary>
        /// 枚举目录下所有 目录 类型的资源;
        /// </summary>
        public IEnumerable<LoadableContent> EnumerateDirectory(string modsDirectory)
        {
            foreach (var directory in Directory.EnumerateDirectories(modsDirectory, "*", SearchOption.TopDirectoryOnly))
            {
                string directoryName = Path.GetDirectoryName(directory);
                if (directoryName.StartsWith(IgnoreSymbol, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                LoadableContent info = null;

                try
                {
                    info = Factory.Read(directory);
                }
                catch(Exception)
                {
                    continue;
                }

                yield return info;
            }
        }

        /// <summary>
        /// 枚举目录下所有 压缩包 类型的资源;
        /// </summary>
        public IEnumerable<LoadableContent> EnumerateZipFile(string modsDirectory)
        {
            foreach (var filePath in Directory.EnumerateFiles(modsDirectory, "*.zmod", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileName(filePath);
                if (fileName.StartsWith(IgnoreSymbol, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                LoadableContent info = null;

                try
                {
                    info = Factory.ReadZip(filePath);
                }
                catch(Exception)
                {
                    continue;
                }

                yield return info;
            }
        }
    }
}
