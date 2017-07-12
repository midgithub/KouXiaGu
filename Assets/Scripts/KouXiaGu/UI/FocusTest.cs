﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KouXiaGu.UI
{


    [DisallowMultipleComponent]
    [RequireComponent(typeof(SelectablePanel))]
    public class FocusTest : MonoBehaviour
    {
        FocusTest()
        {
        }

        void Awake()
        {
            SelectablePanel panel = GetComponent<SelectablePanel>();
            panel.OnFocusEvent.AddListener(OnFocus);
            panel.OnBlurEvent.AddListener(OnBlur);
        }

        void OnFocus()
        {
            Debug.Log("OnFocus : " + name);
        }

        void OnBlur()
        {
            Debug.Log("OnBlur : " + name);
        }
    }
}
