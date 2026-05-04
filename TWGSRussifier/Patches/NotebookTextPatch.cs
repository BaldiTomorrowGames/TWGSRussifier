using HarmonyLib;
using TMPro;
using TWGSRussifier.Runtime;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    internal class NotebookTextPatch
    {
        private static TMP_Text? notebookText;

        [HarmonyPatch(typeof(HudManager), "Awake")]
        private static class AwakePatch
        {
            [HarmonyPostfix]
            private static void Postfix(HudManager __instance)
            {
                Traverse traverse = Traverse.Create(__instance);
                TMP_Text[]? textBox = traverse.Field("textBox").GetValue<TMP_Text[]>();
                if (textBox != null && textBox.Length > 0)
                {
                    notebookText = textBox[0];
                    if (notebookText != null)
                    {
                        TextLocalizer? localizer = notebookText.gameObject.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = notebookText.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = "Hud_Notebooks";
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), "UpdateText")]
        private static class UpdateTextPatch
        {
            [HarmonyPostfix]
            private static void Postfix(HudManager __instance, int textVal, string text)
            {
                if (textVal != 0) return;
                if (notebookText == null) return;

                string numberPart = ExtractNumberPart(text);
                if (string.IsNullOrEmpty(numberPart)) return;

                int count = ExtractFirstNumber(numberPart);
                string declined = GetDeclinedForm(count);

                notebookText.text = numberPart + " " + declined;
            }
        }

        private static string ExtractNumberPart(string text)
        {
            int lastSpace = text.LastIndexOf(' ');
            if (lastSpace > 0)
                return text.Substring(0, lastSpace);
            return text;
        }

        private static int ExtractFirstNumber(string numberPart)
        {
            int slashIndex = numberPart.IndexOf('/');
            string firstPart = slashIndex >= 0 ? numberPart.Substring(0, slashIndex) : numberPart;
            if (int.TryParse(firstPart.Trim(), out int result))
                return result;
            return 0;
        }

        public static string GetDeclinedForm(int n)
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
