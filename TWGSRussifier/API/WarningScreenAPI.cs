using System;
using TWGSRussifier.Patches;
using UnityEngine;

namespace MTM101BaldAPI
{
    public static class MTM101BaldiDevAPI
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