﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace KouXiaGu.Collections
{

    public static class DictionaryExtensions
    {

        /// <summary>
        /// 将元素加入到,若已经存在,则进行替换;
        /// </summary>
        public static void AddOrUpdate<TKey,TValue>(
            this IDictionary<TKey, TValue> dictionary, 
            IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
            {
                try
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
                catch (ArgumentException)
                {
                    dictionary[pair.Key] = pair.Value;
                }
            }
        }

        /// <summary>
        /// 将元素加入到,若已经存在,则进行替换;
        /// 若为加入则返回true,替换返回false;
        /// </summary>
        public static bool AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value)
        {
            try
            {
                dictionary.Add(key, value);
                return true;
            }
            catch (ArgumentException)
            {
                dictionary[key] = value;
                return false;
            }
        }

        /// <summary>
        /// 将元素加入到,若已经存在,则进行替换;
        /// </summary>
        public static void AddOrUpdate<TKey, TValue, T>(
            this IDictionary<TKey, TValue> dictionary,
            IEnumerable<T> collection,
            Func<T, KeyValuePair<TKey, TValue>> func)
        {
            foreach (var value in collection)
            {
                KeyValuePair<TKey, TValue> pair = func(value);
                try
                {
                    dictionary.Add(pair);
                }
                catch (ArgumentException)
                {
                    dictionary[pair.Key] = pair.Value;
                }
            }
        }

        /// <summary>
        /// 将元素加入到,若已经存在,则进行替换;
        /// </summary>
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IEnumerable<TValue> collection, 
            Func<TValue, TKey> func)
        {
            foreach (var value in collection)
            {
                TKey key = func(value);
                try
                {
                    dictionary.Add(key, value);
                }
                catch (ArgumentException)
                {
                    dictionary[key] = value;
                }
            }
        }


        /// <summary>
        /// 将值加入到链表合集;若不存在则创建到;
        /// </summary>
        public static void Add<TKey, TValue>(
            this IDictionary<TKey, List< TValue>> dictionary,
            TKey key, TValue value, int capacity)
        {
            List<TValue> list;
            if (dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TValue>(capacity);
                dictionary.Add(key, list);
            }
        }


        /// <summary>
        /// 获取到,若不存在则返回默认值;
        /// </summary>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            try
            {
                return dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                return default(TValue);
            }
        }

        /// <summary>
        /// 转换成字典,若存在相同的元素则返回异常;
        /// </summary>
        public static Dictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(this IEnumerable<T> collection, Func<T, KeyValuePair<TKey,TValue>> func)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            IDictionary<TKey, TValue> dictionaryI = dictionary;
            foreach (var key in collection)
            {
                var pair = func(key);
                dictionaryI.Add(pair);
            }
            return dictionary;
        }


    }

}