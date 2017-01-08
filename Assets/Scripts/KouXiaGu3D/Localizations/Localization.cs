﻿using System;
using KouXiaGu.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace KouXiaGu.Localizations
{

    /// <summary>
    /// 文本内容;
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Localization : UnitySington<Localization>
    {
        static Localization()
        {
            Initialized = false;
            LanguageIndex = -1;
        }

        Localization() { }

        /// <summary>
        /// 配置信息;
        /// </summary>
        [SerializeField]
        LocalizationConfig config = new LocalizationConfig();


        static readonly TextDictionary textDictionary = new TextDictionary();

        static readonly HashSet<ITextObserver> textObservers = new HashSet<ITextObserver>();


        public static IReadOnlyDictionary<string, string> TextDictionary
        {
            get { return textDictionary; }
        }

        /// <summary>
        /// 是否初始化完毕?
        /// </summary>
        public static bool Initialized { get; private set; }

        public static LocalizationConfig Config
        {
            get { return GetInstance.config; }
            private set { GetInstance.config = value; }
        }

        /// <summary>
        /// 所有语言包(只读);
        /// </summary>
        public static List<XmlLanguageFile> ReadOnlyLanguageFiles { get; private set; }

        /// <summary>
        /// 所有语言(只读),和 ReadOnlyLanguageFiles 对应;
        /// </summary>
        public static List<string> ReadOnlyLanguages { get; private set; }

        /// <summary>
        /// 当前读取的语言下标;
        /// </summary>
        public static int LanguageIndex { get; private set; }

        /// <summary>
        /// 当前读取的语言;
        /// </summary>
        public static string Language
        {
            get { return ReadOnlyLanguages[LanguageIndex]; }
        }

        /// <summary>
        /// 当前读取的语言;
        /// </summary>
        public static XmlLanguageFile LanguageFile
        {
            get { return ReadOnlyLanguageFiles[LanguageIndex]; }
        }


        /// <summary>
        /// 初始化所有信息;
        /// </summary>
        public static void Init()
        {
            Config = LocalizationConfig.Read();

            ReadOnlyLanguageFiles = Resources.FindLanguageFiles().ToList();
            ReadOnlyLanguages = ReadOnlyLanguageFiles.Select(item => item.Language).ToList();

            Initialized = true;
        }


        public static IDisposable Subscribe(ITextObserver observer)
        {
            if (observer == null)
                throw new ArgumentNullException("订阅者为null;");
            if (!textObservers.Add(observer))
                throw new ArgumentException("重复订阅;");

            UpdateTextObserver(observer);

            return new CollectionUnsubscriber<ITextObserver>(textObservers, observer);
        }


        /// <summary>
        /// 更新所有文本内容,在主线程内调用;
        /// </summary>
        public static void UpdateTextObservers()
        {
            foreach (var textObserver in textObservers)
            {
                UpdateTextObserver(textObserver);
            }
        }

        static void UpdateTextObserver(ITextObserver textObserver)
        {
            string text;
            if (textDictionary.TryGetValue(textObserver.Key, out text))
            {
                textObserver.SetText(text);
                return;
            }
            textObserver.OnTextNotFound();
        }


        /// <summary>
        /// 设置优先读取语言,并且重新读取所有文本;
        /// </summary>
        public static void SetConfig(int languageIndex, bool isFollowSystemLanguage)
        {
            var languageFile = ReadOnlyLanguageFiles[languageIndex];
            LocalizationConfig config = new LocalizationConfig(isFollowSystemLanguage, languageFile.Language);
            SetConfig(config);
        }

        /// <summary>
        /// 更新配置信息,并且重新读取所有文本;
        /// </summary>
        public static void SetConfig(LocalizationConfig config)
        {
            LocalizationConfig.Write(config);
            Config = config;
            Read(config);
        }

        /// <summary>
        /// 读取到所有文本;
        /// </summary>
        public static void Read()
        {
            Read(Config);
        }

        /// <summary>
        /// 读取\重新读取 所有文本;
        /// </summary>
        static void Read(LocalizationConfig config)
        {
            ITextReader reader = FindReader(config);

            ClearTexts();
            ReadTexts(reader);
            UpdateTextObservers();
        }

        /// <summary>
        /// 寻找到当前最适合读取的语言接口;
        /// </summary>
        static ITextReader FindReader(LocalizationConfig config)
        {
            LanguageIndex = config.FindIndex(ReadOnlyLanguages);
            var reader = Resources.GetReader(LanguageFile);
            return reader;
        }

        static void ReadTexts(ITextReader reader)
        {
            foreach (var item in reader.ReadTexts())
            {
                if (!textDictionary.Add(item))
                    Debug.LogWarning("重复加入的文本条目:" + item);
            }

            Debug.Log("语言读取完毕:" + reader.ToString());
        }

        public static void ClearTexts()
        {
            textDictionary.Clear();
        }


        protected override void Awake()
        {
            base.Awake();
            Init();
            Read();
        }



    }

}
