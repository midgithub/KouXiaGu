﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KouXiaGu.World2D.Navigation
{

    /// <summary>
    /// 提供寻路方法
    /// </summary>
    [DisallowMultipleComponent]
    public class NavController : UnitySingleton<NavController>
    {

        Obstruction obstruction;
        IMap<ShortVector2, WorldNode> worldMap;

        void Start()
        {
            obstruction = Obstruction.GetInstance;
            worldMap = WorldMapData.GetInstance.Map;
        }

        /// <summary>
        /// 获取到导航路径;
        /// </summary>
        public NavPath NavigateTo(ShortVector2 starting, ShortVector2 destination, ShortVector2 maximumRange, INavAction mover)
        {
            AStar<WorldNode, INavAction> astarNav = new AStar<WorldNode, INavAction>(obstruction, worldMap);
            LinkedList<ShortVector2> path = astarNav.Start(starting, destination, maximumRange, mover);
            astarNav.Clear();
            return new NavPath(path, WorldMapData.GetInstance.Map, TopographiessData.GetInstance);
        }



    }

}
