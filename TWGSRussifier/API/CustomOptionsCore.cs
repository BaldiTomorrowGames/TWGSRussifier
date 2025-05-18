using System;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class CustomOptionsCore
    {
        public static event Action<OptionsMenu, CustomOptionsHandler> OnMenuInitialize;
        public static event Action<OptionsMenu, CustomOptionsHandler> OnMenuClose;

        public static void CallOnMenuInitialize(OptionsMenu menu, CustomOptionsHandler handler)
        {
            if (OnMenuInitialize != null)
            {
                OnMenuInitialize(menu, handler);
            }
        }

        public static void CallOnMenuClose(OptionsMenu menu, CustomOptionsHandler handler)
        {
            if (OnMenuClose != null)
            {
                OnMenuClose(menu, handler);
            }
        }
    }

    public class CustomOptionsHandler : MonoBehaviour
    {
        public struct OptionsCategory
        {
            public string localizationName;
            public GameObject gameObject;

            public OptionsCategory(string name, GameObject page)
            {
                localizationName = name;
                gameObject = page;
            }
        }
    }
} 