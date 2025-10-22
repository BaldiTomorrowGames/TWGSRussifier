using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    internal class PlusInfoPatch
    {
        private static HashSet<int> initializedInstances = new HashSet<int>();
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "ScreenAnchor/Screen2/TMP", "TWGS_PlusInfo_Screen2" },
            { "ScreenAnchor/Screen1/TMP_Upper", "TWGS_PlusInfo_Screen1_Upper" },
            { "ScreenAnchor/Screen1/TMP_Lower", "TWGS_PlusInfo_Screen1_Lower" },
            { "ScreenAnchor/Screen3/TMP", "TWGS_PlusInfo_Screen3" },
            { "ScreenAnchor/Screen4/TMP_0", "TWGS_PlusInfo_Screen4_0" },
            { "ScreenAnchor/Screen4/TMP_1", "TWGS_PlusInfo_Screen4_1" },
            { "ScreenAnchor/Screen4/TMP_2", "TWGS_PlusInfo_Screen4_2" },
            { "ScreenAnchor/Screen5/TMP_Title", "TWGS_PlusInfo_Screen5_Title" },
            { "ScreenAnchor/Screen5/TMP", "TWGS_PlusInfo_Screen5" },
            { "ScreenAnchor/Screen6/TMP", "TWGS_PlusInfo_Screen6" },
            { "ScreenAnchor/Screen6/WebsiteButton/TMP", "TWGS_PlusInfo_Screen6_Website" },
            { "Back/TMP", "TWGS_PlusInfo_Back" },
            { "BackwardButton/TMP", "TWGS_PlusInfo_BackwardButton" },
            { "ForwardButton/TMP", "TWGS_PlusInfo_ForwardButton" }
        };

        [HarmonyPatch(typeof(PlusInfoScreen), "OnEnable")]
        private static class OnEnablePatch
        {
            [HarmonyPostfix]
            private static void Postfix(PlusInfoScreen __instance)
            {
                int instanceId = __instance.GetInstanceID();
                if (!initializedInstances.Contains(instanceId))
                {
                    ApplyLocalizationToPlusInfo(__instance.transform);
                    initializedInstances.Add(instanceId);
                }
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initializedInstances.Clear();
            }
        }

        private static void ApplyLocalizationToPlusInfo(Transform rootTransform)
        {
            if (rootTransform == null) return;

            foreach (var kvp in LocalizationKeys)
            {
                string path = kvp.Key;
                string localizationKey = kvp.Value;
                
                Transform targetTransform = rootTransform.Find(path);
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
                        
                        if (path == "ScreenAnchor/Screen5/TMP")
                        {
                            textComponent.fontSize = 23;
                        }
                        
                        if (path == "ScreenAnchor/Screen6/TMP")
                        {
                            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
                            if (rectTransform != null)
                            {
                                rectTransform.anchoredPosition = new Vector2(0, 10);
                            }
                        }
                        
                        if (path == "ScreenAnchor/Screen6/WebsiteButton/TMP")
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