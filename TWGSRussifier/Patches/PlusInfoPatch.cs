using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    internal class PlusInfoPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "PlusInfo/ScreenAnchor/Screen2/TMP", "TWGS_PlusInfo_Screen2" },
            { "PlusInfo/ScreenAnchor/Screen1/TMP_Upper", "TWGS_PlusInfo_Screen1_Upper" },
            { "PlusInfo/ScreenAnchor/Screen1/TMP_Lower", "TWGS_PlusInfo_Screen1_Lower" },
            { "PlusInfo/ScreenAnchor/Screen3/TMP", "TWGS_PlusInfo_Screen3" },
            { "PlusInfo/ScreenAnchor/Screen4/TMP_0", "TWGS_PlusInfo_Screen4_0" },
            { "PlusInfo/ScreenAnchor/Screen4/TMP_1", "TWGS_PlusInfo_Screen4_1" },
            { "PlusInfo/ScreenAnchor/Screen4/TMP_2", "TWGS_PlusInfo_Screen4_2" },
            { "PlusInfo/ScreenAnchor/Screen5/TMP_Title", "TWGS_PlusInfo_Screen5_Title" },
            { "PlusInfo/ScreenAnchor/Screen5/TMP", "TWGS_PlusInfo_Screen5" },
            { "PlusInfo/ScreenAnchor/Screen6/TMP", "TWGS_PlusInfo_Screen6" },
            { "PlusInfo/ScreenAnchor/Screen6/WebsiteButton/TMP", "TWGS_PlusInfo_Screen6_Website" },
            { "PlusInfo/Back/TMP", "TWGS_PlusInfo_Back" },
            { "PlusInfo/BackwardButton/TMP", "TWGS_PlusInfo_BackwardButton" },
            { "PlusInfo/ForwardButton/TMP", "TWGS_PlusInfo_ForwardButton" }
        };

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "PlusInfo" && value && !initialized)
                {
                    ApplyLocalizationToPlusInfo(__instance.transform);
                    initialized = true;
                }
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initialized = false;
            }
        }

        private static void ApplyLocalizationToPlusInfo(Transform rootTransform)
        {
            if (rootTransform == null) return;

            foreach (var kvp in LocalizationKeys)
            {
                string path = kvp.Key;
                string localizationKey = kvp.Value;

                string searchPath = path.StartsWith("PlusInfo/") ? path.Substring("PlusInfo/".Length) : path;
                
                Transform targetTransform = rootTransform.Find(searchPath);
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer existingLocalizer = textComponent.GetComponent<TextLocalizer>();
                        if (existingLocalizer != null)
                        {
                            Object.DestroyImmediate(existingLocalizer);
                        }

                        TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                        localizer.key = localizationKey;
                        
                        if (path == "PlusInfo/ScreenAnchor/Screen5/TMP")
                        {
                            textComponent.fontSize = 23;
                        }
                        
                        if (path == "PlusInfo/ScreenAnchor/Screen6/TMP")
                        {
                            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
                            if (rectTransform != null)
                            {
                                rectTransform.anchoredPosition = new Vector2(0, 10);
                            }
                        }
                        
                        if (path == "PlusInfo/ScreenAnchor/Screen6/WebsiteButton/TMP")
                        {
                            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
                            if (rectTransform != null)
                            {
                                rectTransform.sizeDelta = new Vector2(209, 32);
                            }
                        }
                        
                        localizer.RefreshLocalization();
                    }
                }
            }
        }
    }
}