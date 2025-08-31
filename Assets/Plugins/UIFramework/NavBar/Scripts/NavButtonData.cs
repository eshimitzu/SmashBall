using System;
using UnityEngine;

namespace UIFramework.Components.NavBar
{
    [Serializable]
    public class NavButtonData
    {
        public string name;
        public Sprite icon = null;
        public NavBarScreen prefab;
    }
}