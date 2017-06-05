﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KouXiaGu
{


    public static class CollectionHelper
    {

        /// <summary>
        /// 移除元素;
        /// </summary>
        /// <param name="item">要在序列中定位的值</param>
        /// <param name="comparer">一个对值进行比较的相等比较器;</param>
        public static bool Remove<T>(IList<T> collection, T item, IEqualityComparer<T> comparer)
        {
            int index = FindIndex(collection, item, comparer);
            if (index >= 0)
            {
                collection.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取到对应元素下标,若不存在则返回-1;
        /// </summary>
        /// <param name="item">要在序列中定位的值</param>
        /// <param name="comparer">一个对值进行比较的相等比较器;</param>
        public static int FindIndex<T>(this IList<T> collection, T item, IEqualityComparer<T> comparer)
        {
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
        /// 获取到对应元素下标,若不存在则返回-1;
        /// </summary>
        public static int FindIndex<T>(this IList<T> collection, T item)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                T original = collection[i];
                if (item.Equals(original))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 移除指定下标的元素,同 List 的 RemoveAt();
        /// </summary>
        public static void RemoveAt<T>(ref T[] array, int index)
        {
            Array.Copy(array, index + 1, array, index, array.Length - index - 1);
            Array.Resize(ref array, array.Length - 1);
        }

        /// <summary>
        /// 移除符合要求的第一个元素;
        /// </summary>
        public static bool Remove<T>(this IList<T> list, Func<T, bool> func)
        {
            int index = list.FindIndex(func);
            if (index >= 0)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}