using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(global::TextLocalizer))]
    internal class TextLocalizerPatches
    {
        [HarmonyPatch(typeof(global::TextLocalizer), "Awake")] [HarmonyPostfix]
        private static void TranslateOnAwakeInstead(global::TextLocalizer __instance)
        {
            Start(__instance);
        }
        [HarmonyPatch(typeof(global::TextLocalizer), "Start")] [HarmonyPrefix]
        private static bool PreventLocalizationOnStart()
        {
            return false;
        }
        private static void Start(global::TextLocalizer textLocalizer)
        {
            try
            {
                TMP_Text textBox = Traverse.Create(textLocalizer).Field("textBox").GetValue<TMP_Text>();
                if (textBox != null && !string.IsNullOrEmpty(textLocalizer.key))
                {
                    if (Singleton<LocalizationManager>.Instance != null)
                    {
                        textLocalizer.GetLocalizedText(textLocalizer.key);
                    }
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogWarning($"TextLocalizer patch failed for key '{textLocalizer?.key}': {ex.Message}");
            }
        }
    }
}
