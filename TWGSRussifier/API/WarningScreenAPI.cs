using System;
using TWGSRussifier.Patches;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class WarningScreenAPI
    {
        public static void AddWarningScreen(string text, bool fatal)
        {
            if (fatal)
            {
                WarningScreenContainer.criticalScreens.Add(text);
            }
            else
            {
                WarningScreenContainer.nonCriticalScreens.Add(text);
            }
        }
    }
} 