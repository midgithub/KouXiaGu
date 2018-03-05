﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiongXiaGu.Collections
{

    /// <summary>
    /// 提供合集对比方法的工具类;
    /// </summary>
    public static class Contrast
    {

        /// <summary>
        /// 判断两个字典结构内容是否相同;
        /// </summary>
        public static void AreEqual<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> target)
        {
            AreEqual(source, target, EqualityComparer<TValue>.Default);
        }

        /// <summary>
        /// 判断两个字典结构内容是否相同;
        /// </summary>
        public static void AreEqual<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> target, IEqualityComparer<TValue> valueComparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source.Count != target.Count)
                throw CreateDifferentCollectionCount(source.Count, target.Count);

            foreach (var item1 in source)
            {
                TValue value2;
                if (target.TryGetValue(item1.Key, out value2))
                {
                    if (!valueComparer.Equals(item1.Value, value2))
                    {
                        throw new ArgumentException(string.Format("[{0}]对应的值不相同;", item1.Key));
                    }
                }
                else
                {
                    throw new ArgumentException("缺少键值:" + item1.Key);
                }
            }
        }

        /// <summary>
        /// 当合集数目不同返回的异常;
        /// </summary>
        private static Exception CreateDifferentCollectionCount(int count1, int count2)
        {
            return new ArgumentException(string.Format("两个合集元素总数不同 [{0} , {1}]", count1, count2));
        }
    }
}
