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
            TMP_Text textBox = Traverse.Create(textLocalizer).Field(nameof(textBox)).GetValue<TMP_Text>();
            if ((!textLocalizer.onlySetIfBlank || textBox.text.Length == 0) && textLocalizer.key != "")
            {
                textLocalizer.GetLocalizedText(textLocalizer.key);
            }
        }
    }
}
