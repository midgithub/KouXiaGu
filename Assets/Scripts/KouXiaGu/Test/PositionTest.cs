﻿using System;
using KouXiaGu.World2D;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace KouXiaGu.Test
{

    /// <summary>
    /// 游戏视窗位置测试;
    /// </summary>
    [SerializeField]
    public class PositionTest : MonoBehaviour
    {

        private Text textObject;

        private void Awake()
        {
            textObject = GetComponent<Text>();
        }

        private void Start()
        {
            this.ObserveEveryValueChanged(_ => UnityEngine.Input.mousePosition).
                SubscribeToText(textObject, TextUpdate);
        }

        private string TextUpdate(Vector3 mousePosition)
        {
            string str = "";

            str += GetScreenPoint(mousePosition);
            str += GetPlanePoint(mousePosition);

            return str;
        }

        private string GetScreenPoint(Vector3 mousePosition)
        {
            string str = "";

            str += "视窗坐标 :" + mousePosition;

            return str;
        }

        private string GetPlanePoint(Vector3 mousePosition)
        {
            try
            {
                Vector2 planePoint = WorldConvert.MouseToPlane();
                var pointPair = WorldConvert.PlaneToHexPair(planePoint);
                string str = "";

                str += "六边形中心坐标 :" + pointPair.HexCenter
                    + "地图坐标 :" + pointPair.MapPoint
                    + "平面坐标 :" + planePoint;

                return str;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

    }

}
