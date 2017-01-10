﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Terrain3D;
using UnityEngine;
using UnityEngine.UI;

namespace KouXiaGu.UI
{

    [DisallowMultipleComponent]
    public sealed class MapEditorUI : MonoBehaviour
    {
        MapEditorUI() { }

        [SerializeField]
        Transform mapItemPrefabParent;

        [SerializeField]
        MapItemUI mapItemPrefab;

        [SerializeField]
        Button btnUpdateDescr;

        [SerializeField]
        MapDescriptionUI mapDescriptionUI;


        /// <summary>
        /// 所有实例化的地图模块;
        /// </summary>
        List<MapItemUI> activates;

        /// <summary>
        /// 选中的地图模块;
        /// </summary>
        MapItemUI selected;

        /// <summary>
        /// 选中的地图,若未选中或者不存在则返回 default();
        /// </summary>
        public TerrainMap Map
        {
            get { return selected == null ? null : selected.Map; }
        }


        void Awake()
        {
            activates = new List<MapItemUI>();

            MapFiler.OnMapUpdate += RecreateMapItems;
            btnUpdateDescr.onClick.AddListener(OnClickUpdateDescr);
        }

        void Start()
        {
            RecreateMapItems();
        }

        /// <summary>
        /// 重新创建所有地图元素;
        /// </summary>
        void RecreateMapItems()
        {
            Clear();
            foreach (var item in MapFiler.ReadOnlyMaps)
            {
                CreateMapItem(item);
            }
        }

        void CreateMapItem(TerrainMap map)
        {
            MapItemUI item = Instantiate<MapItemUI>(mapItemPrefab, mapItemPrefabParent);
            item.gameObject.SetActive(true);

            item.Init(map, activates.Count);
            item.OnPitch += OnSelected;

            activates.Add(item);
        }

        /// <summary>
        /// 当发生选中事件调用;
        /// </summary>
        void OnSelected(MapItemUI item)
        {
            selected = item;
            mapDescriptionUI.Set(item.Map.Description);
        }

        void Clear()
        {
            foreach (var item in activates)
            {
                Destroy(item.gameObject);
            }
            activates.Clear();
        }

        /// <summary>
        /// 更新所选中的描述文件信息,若未选中则不响应;
        /// </summary>
        void OnClickUpdateDescr()
        {
            if (selected != null)
            {
                MapDescription descr = mapDescriptionUI.Get();
                selected.Map.UpdateDescription(descr);
                selected.UpdateDescription();
            }
        }

    }

}
