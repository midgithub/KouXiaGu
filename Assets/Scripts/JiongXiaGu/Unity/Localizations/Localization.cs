﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace JiongXiaGu.Unity.Localizations
{

    /// <summary>
    /// 本地化信息(线程安全);
    /// </summary>
    public static class Localization
    {
        /// <summary>
        /// 异步锁;
        /// </summary>
        private static readonly object asyncLock = new object();

        /// <summary>
        /// 所有可用的语言包文件;
        /// </summary>
        public static List<LanguagePackFileInfo> AvailableLanguagePackFiles { get; private set; }

        /// <summary>
        /// 观察者合集;
        /// </summary>
        private static readonly ObserverCollection<LanguageChangedEvent> observers = new ObserverLinkedList<LanguageChangedEvent>();

        /// <summary>
        /// 观察者订阅,并返回取消处置器;若已经加入了观察者合集,则返回Null;
        /// </summary>
        public static IDisposable Subscribe(IObserver<LanguageChangedEvent> observer)
        {
            lock (asyncLock)
            {
                if (!observers.Contains(observer))
                {
                    return observers.Add(observer);
                }
                return null;
            }
        }

        /// <summary>
        /// 移除订阅者;此方法不会调用观察者的 OnCompleted() 方法;
        /// </summary>
        public static bool Unsubscribe(IObserver<LanguageChangedEvent> observer)
        {
            lock (asyncLock)
            {
                return observers.Remove(observer);
            }
        }
    }


    ///// <summary>
    ///// 本地化静态类(线程安全);
    ///// </summary>
    //public static class Localization
    //{
    //    private static readonly object asyncLock = new object();

    //    /// <summary>
    //    /// 所有可用的语言包文件;
    //    /// </summary>
    //    public static List<LanguagePackFileInfo> AvailableLanguagePackFiles { get; private set; }

    //    /// <summary>
    //    /// 语言包合集;
    //    /// </summary>
    //    internal static LanguageGroup LanguagePackGroup { get; private set; }

    //    /// <summary>
    //    /// 观察者合集;
    //    /// </summary>
    //    private static readonly ObserverCollection<LanguageChangedEvent> observers = new ObserverLinkedList<LanguageChangedEvent>();

    //    /// <summary>
    //    /// 当前使用的文本字典;(若不存在则为Null)
    //    /// </summary>
    //    public static IReadOnlyLanguageDictionary Dictionary
    //    {
    //        get { return LanguagePackGroup; }
    //    }

    //    /// <summary>
    //    /// 组件是否准备完成?
    //    /// </summary>
    //    public static bool IsReady
    //    {
    //        get { return LanguagePackGroup != null; }
    //    }

    //    /// <summary>
    //    /// 当前语言类型,若未进行设置则返回 "Unknow";
    //    /// </summary>
    //    public static string Language
    //    {
    //        get { return IsReady ? LanguagePackGroup.Language : "Unknow"; }
    //    }

    //    /// <summary>
    //    /// 所有语言包;(若不存在则为Null)
    //    /// </summary>
    //    public static IReadOnlyCollection<LanguagePack> LanguagePacks
    //    {
    //        get { return LanguagePackGroup; }
    //    }

    //    /// <summary>
    //    /// 获取到对应文本,若未能获取到则返回 key;
    //    /// </summary>
    //    public static bool TryTranslate(string key, out string value)
    //    {
    //        if (IsReady)
    //        {
    //            return LanguagePackGroup.TryTranslate(key, out value);
    //        }
    //        value = default(string);
    //        return false;
    //    }

    //    /// <summary>
    //    /// 设置新的语言;
    //    /// </summary>
    //    public static void SetLanguage(LanguageGroup group)
    //    {
    //        if (group == null)
    //            throw new ArgumentNullException(nameof(group));
    //        if (group == LanguagePackGroup)
    //            return;

    //        LanguagePackGroup = group;
    //        OnLanguageChanged();
    //    }

    //    /// <summary>
    //    /// 在语言发生变化时调用;
    //    /// </summary>
    //    private static void OnLanguageChanged()
    //    {
    //        LanguageChangedEvent changedEvent = new LanguageChangedEvent()
    //        {
    //            LanguageDictionary = Dictionary,
    //        };
    //        observers.NotifyNext(changedEvent);
    //    }

    //    /// <summary>
    //    /// 订阅到观察者,并返回取消处置器;若已经加入了观察者合集,则返回Null(仅在Unity线程调用);
    //    /// </summary>
    //    public static IDisposable Subscribe(IObserver<LanguageChangedEvent> handler)
    //    {
    //        if (!observers.Contains(handler))
    //        {
    //            return observers.Add(handler);
    //        }
    //        return null;
    //    }
    //}
}
