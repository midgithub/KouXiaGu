﻿using System.Collections.Generic;
using System.IO;

namespace JiongXiaGu.Unity.Resources
{

    /// <summary>
    /// 对应内容搜索;
    /// </summary>
    public abstract class ContentSearcher<T>
    {

        /// <summary>
        /// 相对目录名;
        /// </summary>
        protected abstract string DirectoryName { get; }

        /// <summary>
        /// 文件搜索模式;
        /// </summary>
        protected abstract string SearchPattern { get; }

        /// <summary>
        /// 反序列化内容;
        /// </summary>
        protected abstract T Deserialize(LoadableContent content, ILoadableEntry entry);

        /// <summary>
        /// 枚举所有符合要求的语言文件;
        /// </summary>
        public List<T> Find(IEnumerable<LoadableContent> loadableContents)
        {
            List<T> items = new List<T>();

            foreach (var load in loadableContents)
            {
                var packs = Enumerate(load);
                items.AddRange(packs);
            }

            return items;
        }

        /// <summary>
        /// 枚举所有可用的语言文件;文件命名需要符合要求;
        /// </summary>
        public IEnumerable<T> Enumerate(LoadableContent contentConstruct, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            foreach (ILoadableEntry entry in contentConstruct.EnumerateFiles(DirectoryName, SearchPattern, searchOption))
            {
                T item = default(T);
                bool isSuccess = false;

                try
                {
                    item = Deserialize(contentConstruct, entry);
                    isSuccess = true;
                }
                catch
                {
                    continue;
                }

                if (isSuccess)
                {
                    yield return item;
                }
            }
        }
    }
}
