using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.Runtime;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    internal class DetentionUiPatch
    {
        private static readonly Dictionary<int, TMP_Text?> mainTextCache = new Dictionary<int, TMP_Text?>();

        // Возвращает правильную форму слова "секунда" для числа n.
        // 1, 21, 31... -> TWGS_Detention_Sec1 ("секунда задержания")
        // 2-4, 22-24... -> TWGS_Detention_Sec234 ("секунды задержания")
        // 5-20, 0, 25... -> TWGS_Detention_Sec5 ("секунд задержания")
        public static string GetDeclinedForm(int n)
        {
            int abs = n < 0 ? -n : n;
            int mod100 = abs % 100;
            int mod10 = abs % 10;

            string key;
            if (mod100 >= 11 && mod100 <= 19)
                key = "TWGS_Detention_Sec5";
            else if (mod10 == 1)
                key = "TWGS_Detention_Sec1";
            else if (mod10 >= 2 && mod10 <= 4)
                key = "TWGS_Detention_Sec234";
            else
                key = "TWGS_Detention_Sec5";

            return GetFromJson(key);
        }

        private static string GetFromJson(string key)
        {
            if (LanguageManager.instance != null && LanguageManager.instance.ContainsData(key))
                return LanguageManager.instance.GetKeyData(key)!;
            return key;
        }

        private static TMP_Text? FindMainText(DetentionUi instance, int instanceId)
        {
            if (!mainTextCache.TryGetValue(instanceId, out TMP_Text? cached))
            {
                Transform found = instance.transform.Find("MainText");
                if (found != null)
                {
                    RectTransform rect = found.GetComponent<RectTransform>();
                    if (rect != null)
                        rect.sizeDelta = new Vector2(250f, 64f);
                    cached = found.GetComponent<TMP_Text>();
                }
                else
                    cached = null;
                mainTextCache[instanceId] = cached;
            }
            return cached;
        }

        private static void RepositionTimer(DetentionUi instance)
        {
            Transform time = instance.transform.Find("Time");
            if (time != null)
            {
                RectTransform rect = time.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = new Vector2(-130f, -28f);
            }
        }

        [HarmonyPatch(typeof(DetentionUi), "Initialize")]
        private static class InitializePatch
        {
            [HarmonyPostfix]
            private static void Postfix(DetentionUi __instance, float time)
            {
                RepositionTimer(__instance);
                int instanceId = __instance.GetInstanceID();
                TMP_Text? mainText = FindMainText(__instance, instanceId);
                if (mainText != null)
                    mainText.text = GetDeclinedForm(Mathf.CeilToInt(time));
            }
        }

        [HarmonyPatch(typeof(DetentionUi), "Update")]
        private static class UpdatePatch
        {
            [HarmonyPostfix]
            private static void Postfix(DetentionUi __instance)
            {
                int instanceId = __instance.GetInstanceID();
                TMP_Text? mainText = FindMainText(__instance, instanceId);
                if (mainText == null) return;

                int roundedTime = Traverse.Create(__instance).Field("roundedTime").GetValue<int>();
                string newText = GetDeclinedForm(roundedTime);
                if (mainText.text != newText)
                    mainText.text = newText;
            }
        }
    }
}
