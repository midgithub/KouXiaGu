﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using KouXiaGu.Collections;

namespace KouXiaGu.Localizations
{

    /// <summary>
    /// 本地化配置;
    /// </summary>
    [XmlType("LocalizationConfiguration"), Serializable]
    public sealed class LocalizationConfig
    {

        /// <summary>
        /// 配置文件名;
        /// </summary>
        const string DESCRIPTION_NAME = "Localization.xml";


        static readonly XmlSerializer serializer = new XmlSerializer(typeof(LocalizationConfig));


        public static string ConfigFilePath
        {
            get { return Path.Combine(Resources.ResDirectoryPath, DESCRIPTION_NAME); }
        }

        public static string SysLanguage
        {
            get { return Application.systemLanguage.ToString(); }
        }


        public static LocalizationConfig Read()
        {
            var descr = (LocalizationConfig)serializer.DeserializeXiaGu(ConfigFilePath);
            return descr;
        }

        public static void Write(LocalizationConfig config)
        {
            serializer.SerializeXiaGu(ConfigFilePath, config);
        }


        public LocalizationConfig()
        {
        }

        public LocalizationConfig(bool isFollowSystemLanguage, string[] languagePrioritys)
        {
            if (languagePrioritys == null)
                throw new ArgumentNullException();
            if (languagePrioritys.Length < 1)
                throw new ArgumentOutOfRangeException();

            this.isFollowSystemLanguage = isFollowSystemLanguage;
            this.languagePrioritys = languagePrioritys;
        }

        public LocalizationConfig(bool isFollowSystemLanguage, string firstLanguage)
        {
            this.isFollowSystemLanguage = isFollowSystemLanguage;
            this.Language = firstLanguage;
        }


        /// <summary>
        /// 是否跟随系统语言;
        /// </summary>
        [SerializeField]
        bool isFollowSystemLanguage = true;

        /// <summary>
        /// 语言读取顺序
        /// </summary>
        [SerializeField]
        string[] languagePrioritys = new string[]
            {
                "English",
                "English",
                "ChineseSimplified",
                "Chinese",
            };


        [XmlElement("IsFollowSystemLanguage")]
        public bool IsFollowSystemLanguage
        {
            get { return isFollowSystemLanguage; }
            private set { isFollowSystemLanguage = value; }
        }

        [XmlArray("LanguagePriority"), XmlArrayItem("Language")]
        public string[] LanguagePrioritys
        {
            get { return languagePrioritys; }
            private set { languagePrioritys = value; }
        }

        [XmlIgnore]
        public string Language
        {
            get { return LanguagePrioritys[0]; }
            set { LanguagePrioritys[0] = value; }
        }


        /// <summary>
        /// 获取到优先级最高的语言 下标,若不存在则返回 -1;
        /// </summary>
        /// <param name="languages">存在的语言;</param>
        public int FindIndex(IList<string> languages)
        {
            string[] prioritys = GetPrioritys();
            return FindIndex(languages, prioritys);
        }

        /// <summary>
        /// 获取到优先级最高的语言 下标,若不存在则返回 -1;
        /// </summary>
        /// <param name="languages">存在的语言;</param>
        /// <param name="prioritys">优先级降序排序;</param>
        public int FindIndex(IList<string> languages, IEnumerable<string> prioritys)
        {
            int index = -1;
            foreach (var priority in prioritys)
            {
                index = languages.IndexOf(priority);
                if (index != -1)
                    return index;
            }
            return index;
        }

        /// <summary>
        /// 获取到语言优先顺序;
        /// </summary>
        public string[] GetPrioritys()
        {
            if (IsFollowSystemLanguage)
            {
                return LanguagePrioritys.AddFirst(SysLanguage);
            }
            else
            {
                return LanguagePrioritys;
            }
        }

    }

}
