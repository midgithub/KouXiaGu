﻿using System;
using UnityEngine;
using KouXiaGu.Collections;
using KouXiaGu.Grids;
using UniRx;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 当地图发生变化,且已经渲染到场景,则重新渲染;
    /// </summary>
    [SerializeField]
    public class TerrainChangedCreater : IDataObserver
    {
        static readonly TerrainChangedCreater instance = new TerrainChangedCreater();


        public static void Initialize(OObservableDictionary<CubicHexCoord, TerrainNode> map)
        {
            map.Subscribe(instance);
        }

        void IObserver<DictionaryChange<CubicHexCoord, TerrainNode>>.OnCompleted()
        {
            return;
        }

        void IObserver<DictionaryChange<CubicHexCoord, TerrainNode>>.OnError(Exception error)
        {
            return;
        }

        void IObserver<DictionaryChange<CubicHexCoord, TerrainNode>>.OnNext(DictionaryChange<CubicHexCoord, TerrainNode> value)
        {
            UpdateChunk(value.Key);
        }

        static RectCoord[] chunksCoord = new RectCoord[2];

        void UpdateChunk(CubicHexCoord coord)
        {
            foreach (var around in coord.GetNeighbours())
            {
                LandformChunk.GetBelongChunks(around, ref chunksCoord);
                TerrainData.Creater.Landform.CreateOrUpdate(chunksCoord[0]);
                TerrainData.Creater.Landform.CreateOrUpdate(chunksCoord[1]);
            }
        }

    }

}
