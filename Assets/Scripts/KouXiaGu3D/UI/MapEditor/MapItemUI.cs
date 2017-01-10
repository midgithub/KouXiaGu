﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KouXiaGu.Terrain3D;

namespace KouXiaGu.UI
{

    /// <summary>
    /// 单个地图显示窗口;
    /// </summary>
    [DisallowMultipleComponent, RequireComponent(typeof(Toggle))]
    public class MapItemUI : MonoBehaviour
    {
        MapItemUI() { }

        [SerializeField]
        Text mapMame;

        [SerializeField]
        Text mapData;

        [SerializeField]
        Text description;

        Toggle toggle;
        public TerrainMap Map { get; private set; }

        MapDescription mapDescription
        {
            get { return Map.Description; }
        }

        public string MapName
        {
            get { return mapMame.text; }
            private set { mapMame.text = value; }
        }

        public string MapData
        {
            get { return mapData.text; }
            private set { mapData.text = value; }
        }

        public string Description
        {
            get { return description.text; }
            private set { description.text = value; }
        }

        public bool IsOn
        {
            get { return toggle.isOn; }
            set { toggle.isOn = value; }
        }


        void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        public void SetDescription(TerrainMap map)
        {
            this.Map = map;

            MapName = mapDescription.Name;
            MapData = new DateTime(mapDescription.SaveTime).ToLongDateString();
            Description = mapDescription.Summary;
        }

    }

}
