﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using KouXiaGu.Collections;
using UnityEngine;

namespace KouXiaGu.Globalization
{

    /// <summary>
    /// 负责读取本地化文本;
    /// </summary>
    public sealed class LanguagePackReader : MonoBehaviour, IAsync
    {
        static LanguagePackReader()
        {
            Filer = new XmlTextPackFiler();
            IsInitialized = false;
        }

        internal static LanguagePackFiler Filer { get; private set; }

        public static string MainLanguagePackDirectory
        {
            get { return Path.Combine(Application.streamingAssetsPath, "Localization"); }
        }

        /// <summary>
        /// 是否已经在读取中?
        /// </summary>
        public static bool IsInitialized { get; private set; }
        static LanguagePack effective;
        static IDictionary<string, string> textDictionary;

        /// <summary>
        /// 默认读取的语言;
        /// </summary>
        const string DefaultLocName = "zh-cn";

        /// <summary>
        /// 读取到新的语言文件;
        /// </summary>
        public static IAsync Read(LanguagePack pack, IDictionary<string, string> dictionary)
        {
            effective = pack;
            textDictionary = dictionary;
            return StartRead();
        }

        /// <summary>
        /// 根据预留的语言信息,读取到最适合的语言文件;
        /// </summary>
        public static IAsync Read(out List<LanguagePack> packs, out int choice)
        {
            Config config;
            packs = GetMainLanguagePacks();

            if (packs.Count == 0)
                Debug.LogWarning("Not Found Any LanguagePack In :" + MainLanguagePackDirectory);

            try
            {
                config = Config.Read();
            }
            catch (FileNotFoundException)
            {
                config = WriteDefaultConfigFile();
            }

            effective = GetPack(packs, config.LocName, out choice);
            textDictionary = new Dictionary<string, string>();

            return StartRead();
        }

        static LanguagePack GetPack(List<LanguagePack> packs, string locName, out int index)
        {
            index = packs.FindIndex(pack => pack.LocName == locName);
            if (index < 0)
            {
                index = 0;
            }
            return packs[index];
        }

        static Config WriteDefaultConfigFile()
        {
            Config config = new Config()
            {
                LocName = DefaultLocName,
            };

            Config.Write(config);
            return config;
        }

        public static List<LanguagePack> GetMainLanguagePacks()
        {
            var packs = Filer.GetPacks(MainLanguagePackDirectory);
            return new List<LanguagePack>(packs);
        }

        static IAsync StartRead()
        {
            if (IsInitialized)
                throw new ArgumentException("语言文件已经在读取中;");

            IsInitialized = true;
            var instance = new GameObject("LanguagePackReader", typeof(LanguagePackReader));
            var i = instance.GetComponent<LanguagePackReader>();
            return i;
        }


        LanguagePackReader()
        {
        }

        public bool IsCompleted { get; private set; }
        public bool IsFaulted { get; private set; }
        public Exception Ex { get; private set; }

        void Awake()
        {
            IsCompleted = false;
            IsFaulted = false;
            Ex = null;
            ReadLanguagePack();
        }

        void ReadLanguagePackAsync()
        {
            throw new NotImplementedException();
        }

        void ReadLanguagePack()
        {
            try
            {
                var texts = Filer.Read(effective);
                textDictionary.AddOrUpdate(texts);

                Localization.SetTextDictionary(textDictionary);
                Localization.TrackAll();
                Clear();

                IsCompleted = true;
            }
            catch (Exception ex)
            {
                IsCompleted = true;
                IsFaulted = true;
                Ex = ex;
            }
        }

        void Clear()
        {
            IsInitialized = false;
            effective = default(LanguagePack);
            textDictionary = null;
            Destroy(gameObject);
        }


        [XmlType("Config")]
        public struct Config
        {

            static readonly XmlSerializer serializer = new XmlSerializer(typeof(Config));

            public static string ConfigFilePath
            {
                get { return Path.Combine(Application.streamingAssetsPath, "Localization/Config.xml"); }
            }

            public static Config Read()
            {
                return (Config)serializer.DeserializeXiaGu(ConfigFilePath);
            }

            public static void Write(Config item)
            {
                string path = ConfigFilePath;
                string directoryPath = Path.GetDirectoryName(path);

                if (Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                serializer.SerializeXiaGu(ConfigFilePath, item);
            }

            [XmlElement("LocName")]
            public string LocName;

        }

    }


}