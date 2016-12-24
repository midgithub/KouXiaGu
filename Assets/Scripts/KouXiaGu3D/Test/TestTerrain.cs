﻿using KouXiaGu.Grids;
using KouXiaGu.Terrain3D;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace KouXiaGu.Test
{

    
    public class TestTerrain : MonoBehaviour
    {

        [SerializeField]
        Text textObject;

        Vector3 currentPixelPosition;
        string checkPointText;

        void Awake()
        {
            textObject = textObject ?? GetComponent<Text>();
        }

        void Start()
        {
            this.ObserveEveryValueChanged(_ => UnityEngine.Input.mousePosition).
                SubscribeToText(textObject, TextUpdate);

            this.ObserveEveryValueChanged(_ => Input.GetKeyDown(KeyCode.Mouse0)).
                Subscribe(_ => currentPixelPosition = MouseConvert.MouseToPixel());

        }

        string TextUpdate(Vector3 mousePosition)
        {
            string str = "";

            str += GetScreenPoint(mousePosition);
            str += GetTestPointsLog(mousePosition);
            str += OnMouseDown();

            return str;
        }

        string GetScreenPoint(Vector2 mousePosition)
        {
            string str = "";

            str += "视窗坐标 :" + mousePosition;

            return str;
        }


        string GetTestPointsLog(Vector3 mousePosition)
        {
            Vector3 pixel = MouseConvert.MouseToPixel();
            //ShortVector2 offset = HexGrids.PixelToOffset(pixel);
            CubicHexCoord cube = GridConvert.Grid.GetCubic(pixel);

            //Vector3 offsetPixel = HexGrids.OffsetToPixel(offset);
            //CubicHexCoord offsetCube = HexGrids.OffsetToHex(offset);

            Vector3 cubePixel = GridConvert.Grid.GetPixel(cube);
            //ShortVector2 cubeOffset = HexGrids.HexToOffset(cube);

            RectCoord terrainBlockCoord = Terrain3D.TerrainChunk.ChunkGrid.GetCoord(pixel);
            Vector3 terrainBlockCenter = Terrain3D.TerrainChunk.ChunkGrid.GetCenter(terrainBlockCoord);
            CubicHexCoord terrainBlockHexCenter = Terrain3D.TerrainChunk.GetHexCenter(terrainBlockCoord);
            //Rect terrainBlockRect = Terrain3D.TerrainData.RectGrid.GetRect(terrainBlockCenter);

            Vector2 terrainBlockLocal = Terrain3D.TerrainChunk.ChunkGrid.GetLocal(pixel, out terrainBlockCoord);
            Vector2 terrainBlockUV = Terrain3D.TerrainChunk.ChunkGrid.GetUV(pixel, out terrainBlockCoord);
            float terrainHeight = Terrain3D.TerrainData.GetHeight(pixel);
            RectCoord[] terrainBlocks = Terrain3D.TerrainChunk.GetBelongChunks(terrainBlockHexCenter);

            string str = "";

            str += "\n基本数值: 像素:" + pixel
                //+ "偏移:" + offset
                + "立方:" + cube

                //+ "\n偏移转换: 中心:" + offsetPixel
                //+ "立方:" + offsetCube

                + "\n立方转换: 中心:" + cubePixel
                //+ "偏移:" + cubeOffset

                + "\n地貌块: 块编号:" + terrainBlockCoord
                + "中心:" + terrainBlockCenter
                + "立方:" + terrainBlockHexCenter
                //+ "矩形:" + terrainBlockRect
                + "\n块坐标:" + terrainBlockLocal
                + "UV:" + terrainBlockUV
                + "高度:" + terrainHeight + ";"
                + "所属1:" + terrainBlocks[0]
                + "所属2:" + terrainBlocks[1];

            return str;
        }

        string OnMouseDown()
        {
            string str = "";

            //CubicHexCoord hex = GridConvert.ToHexCubic(currentPixelPosition);

            //GridsExtensions.GetNeighboursAndSelf(CubicHexCoord.Self);
            //CubicHexCoord.Self.GetNeighboursAndSelf<CubicHexCoord, HexDirections>();

            //foreach (var item in hex.GetNeighboursAndSelf<CubicHexCoord, HexDirections>())
            //{
            //    str += "\n" + item.ToString();
            //}

            return str;
        }

    }


}
