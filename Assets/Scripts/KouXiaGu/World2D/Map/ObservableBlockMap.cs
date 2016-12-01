﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UniRx;

namespace KouXiaGu.World2D.Map
{

    /// <summary>
    /// 根据块加载的地图;
    /// </summary>
    /// <typeparam name="T">节点</typeparam>
    /// <typeparam name="TBlock">地图块类型</typeparam>
    public class ObservableBlockMap<T, TBlock> : IHexMap<ShortVector2, T>
        where T : struct
        where TBlock : IHexMap<ShortVector2, T>
    {
        protected ObservableBlockMap() { }

        public ObservableBlockMap(ShortVector2 partitionSizes)
        {
            blockMap = new BlockMap<TBlock>(partitionSizes);
        }

        BlockMap<TBlock> blockMap;
        NodeChangeReporter<T> nodeChangingReporter = new NodeChangeReporter<T>();

        public BlockMap<TBlock> BlockMap
        {
            get { return blockMap; }
        }

        public IObservable<MapNodeState<T>> observeChanges
        {
            get { return nodeChangingReporter; }
        }

        public IEnumerable<KeyValuePair<ShortVector2, T>> NodePair
        {
            get
            {
                foreach (var block in blockMap.BlocksPair)
                {
                    foreach (var node in block.Value.NodePair)
                    {
                        ShortVector2 position = blockMap.AddressToMapPoint(block.Key, node.Key);
                        yield return new KeyValuePair<ShortVector2, T>(position, node.Value);
                    }
                }
            }
        }

        public T this[ShortVector2 position]
        {
            get
            {
                ShortVector2 realPosition;
                TBlock block = TransformToBlock(position, out realPosition);
                return block[realPosition];
            }
            set
            {
                ShortVector2 realPosition;
                TBlock block = TransformToBlock(position, out realPosition);
                block[realPosition] = value;

                nodeChangingReporter.NodeDataUpdate(ChangeType.Update, position, value);
            }
        }

        public void Add(ShortVector2 position, T item)
        {
            ShortVector2 realPosition;
            TBlock block = TransformToBlock(position, out realPosition);
            block.Add(realPosition, item);

            nodeChangingReporter.NodeDataUpdate(ChangeType.Add, position, item);
        }

        public bool Remove(ShortVector2 position)
        {
            ShortVector2 realPosition;
            TBlock block = TransformToBlock(position, out realPosition);
            if (block.Remove(realPosition))
            {
                nodeChangingReporter.NodeDataUpdate(ChangeType.Remove, position, default(T));
                return true;
            }
            return false;
        }

        public bool Contains(ShortVector2 position)
        {
            ShortVector2 realPosition;
            TBlock block = TransformToBlock(position, out realPosition);
            return block.Contains(realPosition);
        }

        /// <summary>
        /// 尝试获取到此值;
        /// </summary>
        public bool TryGetValue(ShortVector2 position, out T item)
        {
            ShortVector2 realPosition;
            TBlock block;
            ShortVector2 address = blockMap.MapPointToAddress(position, out realPosition);

            if (blockMap.TryGetValue(address, out block))
            {
                return block.TryGetValue(realPosition, out item);
            }

            item = default(T);
            return false;
        }

        /// <summary>
        /// 转换成块的信息;
        /// </summary>
        TBlock TransformToBlock(ShortVector2 position, out ShortVector2 realPosition)
        {
            TBlock block;
            ShortVector2 address = blockMap.MapPointToAddress(position, out realPosition);

            if (blockMap.TryGetValue(address, out block))
            {
                return block;
            }
            throw BlockNotFoundException(address);

        }

        /// <summary>
        /// 实质清除块内容;
        /// </summary>
        void IHexMap<ShortVector2, T>.Clear()
        {
            blockMap.Clear();
        }

        /// <summary>
        /// 返回地图块错误信息;
        /// </summary>
        BlockNotFoundException BlockNotFoundException(ShortVector2 address)
        {
            return new BlockNotFoundException(address.ToString() + "地图块未载入!");
        }

    }

}
