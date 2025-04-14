using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(WarningScreen))]
    [HarmonyPatch("Start")]
    [HarmonyPriority(800)]
    public static class WarningScreenStartPatch
    {
        public static bool Prefix(WarningScreen __instance)
        {
            string text = "";
            if (Singleton<InputManager>.Instance.SteamInputActive)
            {
                text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), 
                    Singleton<InputManager>.Instance.GetInputButtonName("MouseSubmit", "Interface", false));
            }
            else
            {
                text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), "ANY BUTTON");
            }
            
            WarningScreenAPI.AddWarningScreen(text, false);
            
            __instance.textBox.text = text;
            
            return false;
        }
    }
    
    [HarmonyPatch(typeof(WarningScreen))]
    [HarmonyPatch("Advance")]
    [HarmonyPriority(800)]
    public static class WarningScreenAdvancePatch
    {
        private static int currentPage = 0;
        
        public static bool Prefix(WarningScreen __instance)
        {
            return true;
        }
    }
} 