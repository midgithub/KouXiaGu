﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class TerrainChangedCreater : ITerrainMapObserver
    {
        static readonly TerrainChangedCreater instance = new TerrainChangedCreater();


        public static void Initialize(TerrainMap map)
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
            TerrainChunk.GetBelongChunks(coord, ref chunksCoord);

            TerrainCreater.UpdateChunk(chunksCoord[0]);
            TerrainCreater.UpdateChunk(chunksCoord[1]);
        }

    }

}
