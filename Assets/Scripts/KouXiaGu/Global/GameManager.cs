﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KouXiaGu.Globalization;
using KouXiaGu.KeyInput;
using UnityEngine;

namespace KouXiaGu
{

    /// <summary>
    /// 负责对游戏资源初始化;
    /// </summary>
    [DisallowMultipleComponent]
    public class GameManager : Initializer
    {
        GameManager()
        {
        }

        /// <summary>
        /// 对内容进行初始化;
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            InitInput();
            InitLanguage();
        }

        /// <summary>
        /// 初始化自定义按键模块;
        /// </summary>
        void InitInput()
        {
            CustomInput.ReadFromFile();

            var emptyKeys = CustomInput.GetEmptyKeys().ToList();
            if (emptyKeys.Count != 0)
            {
                Debug.LogWarning("未定义的按键:" + emptyKeys.ToLog());
            }
        }

        /// <summary>
        /// 初始化语言模块;
        /// </summary>
        void InitLanguage()
        {
            var operater = Localization.Init();
            AddOperater(operater);
        }

    }

}