﻿using KouXiaGu.Concurrent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using KouXiaGu.Resources;

namespace KouXiaGu.Globalization
{

    /// <summary>
    /// 本地化;
    /// </summary>
    public static class Localization
    {
        public static LanguagePack Pack { get; private set; }
        readonly static HashSet<ILocalizationText> observers = new HashSet<ILocalizationText>();

        public static string Language
        {
            get { return Pack != null ? Pack.Language : string.Empty; }
        }

        public static string LanguagePackName
        {
            get { return Pack != null ? Pack.Name : string.Empty; }
        }

        /// <summary>
        /// 在非Unity线程编辑可能会导致异常;
        /// </summary>
        internal static IDictionary<string, string> TextDictionary
        {
            get { return Pack != null ? Pack.TextDictionary : null; }
        }

        /// <summary>
        /// 初始化,在运行中调用重置本地化数据(线程安全);
        /// </summary>
        public static void Initialize()
        {
            LanguagePackXmlSearcher searcher = new LanguagePackXmlSearcher();
            LanguagePackXmlSerializer serializer = new LanguagePackXmlSerializer();
            ConfigReader configReader = new ConfigReader();

            var config = configReader.ReadOrDefault();
            var packs = searcher.EnumeratePacks().ToList();
            try
            {
                var stream = Find(packs, config);
                var pack = serializer.Deserialize(stream);
                SetLanguage_internal(pack);
            }
            finally
            {
                LanguagePackStream.CloseAll(packs);
            }
        }

        /// <summary>
        /// Unity线程检查,若不为Unity线程这返回异常;
        /// </summary>
        internal static void ValidateThread()
        {
            if (!XiaGu.IsUnityThread)
                throw new InvalidOperationException("仅允许在Unity线程调用;");
        }

        /// <summary>
        /// 设置本地化文本(线程安全);
        /// </summary>
        public static void SetLanguage(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            LanguagePackXmlSerializer serializer = new LanguagePackXmlSerializer();
            var pack = serializer.Deserialize(stream);
            SetLanguage_internal(pack);
        }

        /// <summary>
        /// 设置指定的语言,需要在Unity线程调用;
        /// </summary>
        public static void SetLanguage(LanguagePack pack)
        {
            if (pack == null)
                throw new ArgumentNullException("pack");
            ValidateThread();

            Pack = pack;
            ObserverTracker();
        }

        /// <summary>
        /// 指定语言,若不在Unity线程调用,则延迟更新;
        /// </summary>
        /// <param name="pack"></param>
        static void SetLanguage_internal(LanguagePack pack)
        {
            if (XiaGu.IsUnityThread)
            {
                SetLanguage(pack);
            }
            else
            {
                UnityAsyncRequestDispatcher.Instance.Add(() => SetLanguage(pack));
            }
        }

        /// <summary>
        /// 将语言包推送到所有订阅者;
        /// </summary>
        static void ObserverTracker()
        {
            foreach (var observer in observers)
            {
                observer.OnLanguageUpdate(Pack);
            }
        }

        /// <summary>
        /// 订阅到语言,需要在Unity线程调用;
        /// </summary>
        public static IDisposable Subscribe(ILocalizationText observer)
        {
            ValidateThread();

            if (!observers.Add(observer))
            {
                throw new ArgumentException("已经订阅到;");
            }
            return new CollectionUnsubscriber<ILocalizationText>(observers, observer);
        }

        /// <summary>
        /// 获取到对应文本,若不存在则返回key;
        /// </summary>
        public static string GetText(string key)
        {
            ValidateThread();

            if (Pack != null)
            {
                string value;
                if (Pack.TextDictionary.TryGetValue(key, out value))
                {
                    return value;
                }
            }
            return key;
        }

        /// <summary>
        /// 尝试获取到对应文本;
        /// </summary>
        public static bool TryGetText(string key, out string value)
        {
            ValidateThread();

            if (Pack != null)
            {
                return Pack.TextDictionary.TryGetValue(key, out value);
            }
            value = string.Empty;
            return false;
        }

        #region FindLanguagePack

        /// <summary>
        /// 获取到指定语言文件,若未能找到合适的语言包则返回Null;
        /// </summary>
        static T Find<T>(ICollection<T> packs, LocalizationConfig config)
            where T : LanguagePackHead
        {
            if (packs == null)
                throw new ArgumentNullException("packs");

            T pack;
            if (config != null)
            {
                if (!string.IsNullOrEmpty(config.LanguageName))
                {
                    pack = FindLanguageName(packs, config.LanguageName);
                    if (pack != null)
                        return pack;
                }
                if (!string.IsNullOrEmpty(Language))
                {
                    pack = FindLanguage(packs, Language);
                    if (pack != null)
                        return pack;
                }
            }
            pack = FindSystemLanguage(packs);
            return pack;
        }

        /// <summary>
        /// 根据语言包名字获取到语言包;
        /// </summary>
        static T FindLanguageName<T>(IEnumerable<T> packs, string name)
            where T : LanguagePackHead
        {
            foreach (var pack in packs)
            {
                if (pack.Name == name)
                    return pack;
            }
            return null;
        }

        /// <summary>
        /// 根据语言获取到语言包;
        /// </summary>
        static T FindLanguage<T>(IEnumerable<T> packs, string language)
             where T : LanguagePackHead
        {
            foreach (var pack in packs)
            {
                if (pack.Language == language)
                    return pack;
            }
            return null;
        }

        /// <summary>
        /// 根据系统语言获取到语言包;
        /// </summary>
        static T FindSystemLanguage<T>(IEnumerable<T> packs)
            where T : LanguagePackHead
        {
            string language = Application.systemLanguage.ToString().ToLower();
            return FindLanguage(packs, language);
        }

        #endregion
    }
}
