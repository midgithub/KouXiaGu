﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KouXiaGu.Terrain3D
{

    /// <summary>
    /// 对场景创建管理进行更新;
    /// </summary>
    public class BuildRequestUpdater : IUnityThreadBehaviour<Action>
    {
        public BuildRequestUpdater(BuildRequestManager buildRequestManager)
        {
            this.buildRequestManager = buildRequestManager;
            this.SubscribeUpdate();
        }

        readonly BuildRequestManager buildRequestManager;

        object IUnityThreadBehaviour<Action>.Sender
        {
            get { return "场景的地形块创建销毁管理"; }
        }

        Action IUnityThreadBehaviour<Action>.Action
        {
            get { return buildRequestManager.SendDisplay; }
        }

    }

}