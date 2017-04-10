﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace KouXiaGu
{


    static class GameFile
    {

        /// <summary>
        /// 游戏文件路径;
        /// </summary>
        public static List<string> GameDirectorys = new List<string>();
        
    }

    public interface ICustomFile
    {
        /// <summary>
        /// 获取到主要的文件路径;
        /// </summary>
        string MainFilePath { get; }

        /// <summary>
        /// 获取到所有的文件路径;
        /// </summary>
        IEnumerable<string> GetFilePaths();
    }

    abstract class CustomFilePath : ICustomFile
    {
        public abstract string FileName { get; }

        public string MainFilePath
        {
            get { return Path.Combine(ResourcePath.ConfigDirectoryPath, FileName); }
        }

        /// <summary>
        /// 获取到文件路径;
        /// </summary>
        public IEnumerable<string> GetFilePaths()
        {
            yield return MainFilePath;
            foreach (var dir in GameFile.GameDirectorys)
            {
                yield return Path.Combine(dir, FileName);
            }
        }
    }

}
