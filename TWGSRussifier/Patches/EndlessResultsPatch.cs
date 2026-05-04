using HarmonyLib;
using TMPro;
using TWGSRussifier.Runtime;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(ClassicEndlessResults))]
    internal class EndlessResultsPatch
    {
        [HarmonyPatch(typeof(ClassicEndlessResults), "Initialize")]
        private static class InitializePatch
        {
            [HarmonyPostfix]
            private static void Postfix(ClassicEndlessResults __instance, int score)
            {
                TMP_Text scoreTmp = __instance.scoreTmp;
                if (scoreTmp == null) return;

                string declined = GetDeclinedForm(score);
                scoreTmp.text = score + " " + declined;
            }
        }

        private static string GetDeclinedForm(int n)
        {
            int abs = n < 0 ? -n : n;
            int mod100 = abs % 100;
            int mod10 = abs % 10;

            string key;
            if (mod100 >= 11 && mod100 <= 19)
                key = "TWGS_Notebook_5";
            else if (mod10 == 1)
                key = "TWGS_Notebook_1";
            else if (mod10 >= 2 && mod10 <= 4)
                key = "TWGS_Notebook_234";
            else
                key = "TWGS_Notebook_5";

            return GetFromJson(key, n);
        }

        private static string GetFromJson(string key, int n)
        {
            if (LanguageManager.instance != null && LanguageManager.instance.ContainsData(key))
                return LanguageManager.instance.GetKeyData(key)!;

            if (Singleton<LocalizationManager>.Instance != null)
            {
                string loc = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
                if (!string.IsNullOrEmpty(loc) && loc != key)
                    return loc;
            }

            return n == 1 ? "Notebook" : "Notebooks";
        }
    }
}
