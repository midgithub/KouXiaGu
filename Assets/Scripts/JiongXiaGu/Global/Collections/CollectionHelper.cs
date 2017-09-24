﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JiongXiaGu
{
    
    /// <summary>
    /// 合集拓展方法;
    /// </summary>
    public static class CollectionHelper
    {

        static void ValidateCollection<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
        }

        static void ValidateNull(object item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
        }


        /// <summary>
        /// 移除符合要求的第一个元素;
        /// </summary>
        public static bool Remove<T>(this IList<T> list, Func<T, bool> func)
        {
            if (list == null)
                throw new ArgumentNullException("collection");
            if (func == null)
                throw new ArgumentNullException("comparer");

            int index = list.FindIndex(func);
            if (index >= 0)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除元素;
        /// </summary>
        /// <param name="item">要在序列中定位的值</param>
        /// <param name="comparer">一个对值进行比较的相等比较器;</param>
        public static bool Remove<T>(IList<T> collection, T item, IEqualityComparer<T> comparer)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            int index = FindIndex(collection, item, comparer);
            if (index >= 0)
            {
                collection.RemoveAt(index);
                return true;
            }
            return false;
        }



        /// <summary>
        /// 移除指定下标的元素,同 List 的 RemoveAt();
        /// </summary>
        public static void RemoveAt<T>(ref T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            Array.Copy(array, index + 1, array, index, array.Length - index - 1);
            Array.Resize(ref array, array.Length - 1);
        }



        /// <summary>
        /// 移除范围内条件相匹配的所有元素;
        /// </summary>
        /// <param name="startIndex">开始移除的下标</param>
        /// <param name="match">返回ture则移除</param>
        public static void RemoveAll<T>(this IList<T> list, int startIndex, Func<T, bool> match)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (match == null)
                throw new ArgumentNullException("match");
            if (startIndex < 0 || startIndex >= list.Count)
                throw new ArgumentOutOfRangeException("startIndex");

            int index = startIndex;
            while (index < list.Count)
            {
                T item = list[index];
                if (match(item))
                {
                    list.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }



        /// <summary>
        /// 移除合集内相同的元素;
        /// 若存在相同的元素,则保留第一个元素,其余的都移除;
        /// </summary>
        public static void RemoveSame<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            for (int i = 0; i < list.Count;)
            {
                T item = list[i];
                list.RemoveAll(++i, other => item.Equals(other));
            }
        }



        /// <summary>
        /// 获取到对应元素下标,若不存在则返回-1;
        /// </summary>
        public static int FindIndex<T>(this IList<T> collection, T item)
        {
            return FindIndex(collection, item, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 获取到对应元素下标,若不存在则返回-1;
        /// </summary>
        /// <param name="item">要在序列中定位的值</param>
        /// <param name="comparer">一个对值进行比较的相等比较器;</param>
        public static int FindIndex<T>(this IList<T> collection, T item, IEqualityComparer<T> comparer)
        {
            ValidateCollection(collection);
            ValidateNull(comparer);
            for (int i = 0; i < collection.Count; i++)
            {
                T original = collection[i];
                if (comparer.Equals(original, item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 获取到对应元素下标,若不存在则返回-1;
        /// </summary>
        public static int FindIndex<T>(this IList<T> collection, Func<T, bool> func)
        {
            ValidateCollection(collection);
            ValidateNull(func);
            for (int i = 0; i < collection.Count; i++)
            {
                T original = collection[i];
                if (func(original))
                {
                    return i;
                }
            }
            return -1;
        }


        /// <summary>
        /// 这两个合集是否拥有相同的内容?尽管顺序和包含个数不同;
        /// </summary>
        public static bool IsSameContent<T>(this IEnumerable<T> collection, ICollection<T> otherList)
        {
            if (collection == null)
                throw new ArgumentNullException("list");
            if (otherList == null)
                throw new ArgumentNullException("otherList");

            if (collection == otherList)
                return true;

            foreach (var item in collection)
            {
                if (!otherList.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 这两个合集是否完全相同?
        /// </summary>
        public static bool IsClone<T>(this IList<T> list, IList<T> otherList)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (otherList == null)
                throw new ArgumentNullException("otherList");

            if (list == otherList)
                return true;
            if (list.Count != otherList.Count)
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Equals(otherList[i]))
                    return false;
            }
            return true;
        }


        #region IDictionary

        /// <summary>
        /// 将合集加入到dest,若出现相同的Key则替换;
        /// </summary>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dest, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                TKey key = item.Key;
                TValue value = item.Value;
                if (dest.ContainsKey(key))
                {
                    dest[key] = value;
                }
                else
                {
                    dest.Add(item);
                }
            }
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// 区别于 Linq.Min();返回转换结果最小的元素;
        /// </summary>
        public static TSource MinSource<T, TSource>(this IEnumerable<TSource> collection, Func<TSource, T> selector)
        {
            return MinSource(collection, selector, Comparer<T>.Default);
        }

        /// <summary>
        /// 区别于 Linq.Min();返回转换结果最小的元素;
        /// </summary>
        public static TSource MinSource<T, TSource>(this IEnumerable<TSource> collection, Func<TSource, T> selector, IComparer<T> comparer)
        {
            ValidateNull(collection);
            ValidateNull(selector);
            ValidateNull(comparer);

            bool isFirst = true;
            TSource minSource = default(TSource);
            T min = default(T);
            foreach (var item in collection)
            {
                if (isFirst)
                {
                    minSource = item;
                    min = selector(item);
                    isFirst = false;
                    continue;
                }

                T value = selector(item);
                if (comparer.Compare(min, value) > 0)
                {
                    minSource = item;
                    min = value;
                }
            }
            return minSource;
        }


        /// <summary>
        /// 区别于 Linq.Min();返回转换结果最大的元素;
        /// </summary>
        public static TSource MaxSource<T, TSource>(this IEnumerable<TSource> collection, Func<TSource, T> selector)
        {
            return MaxSource(collection, selector, Comparer<T>.Default);
        }

        /// <summary>
        /// 区别于 Linq.Min();返回转换结果最大的元素;
        /// </summary>
        public static TSource MaxSource<T, TSource>(this IEnumerable<TSource> collection, Func<TSource, T> selector, IComparer<T> comparer)
        {
            ValidateNull(collection);
            ValidateNull(selector);
            ValidateNull(comparer);

            bool isFirst = true;
            TSource source = default(TSource);
            T max = default(T);
            foreach (var item in collection)
            {
                if (isFirst)
                {
                    source = item;
                    max = selector(item);
                    isFirst = false;
                    continue;
                }

                T value = selector(item);
                if (comparer.Compare(max, value) < 0)
                {
                    source = item;
                    max = value;
                }
            }
            return source;
        }

        #endregion
    }
}
