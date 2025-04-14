using TWGSRussifier.Runtime;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(LocalizationManager))]
    [HarmonyPatch("LoadLocalizedText")]
    public static class LocalizationPatch
    {
        static void Postfix(LocalizationManager __instance)
        {
            if (LanguageManager.instance == null || LanguageManager.instance.languageData == null)
            {
                Debug.Log("Skipping LocalizationPatch: LanguageManager not ready yet");
                return;
            }
            
            FieldInfo localText = AccessTools.Field(typeof(LocalizationManager), "localizedText");
            localText.SetValue(__instance, LanguageManager.instance.languageData);
        }
    }
}
