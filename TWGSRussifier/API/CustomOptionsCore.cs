using System;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class CustomOptionsCore
    {
        public static Action<OptionsMenu, CustomOptionsHandler> OnMenuInitialize;
        public class CustomOptionsCategory
        {
            public virtual void Build() { }
        }
    }
    public class CustomOptionsHandler
    {
        public void RegisterCategory(CustomOptionsCore.CustomOptionsCategory category) { }
    }
} 