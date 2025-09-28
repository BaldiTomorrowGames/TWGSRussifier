using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    internal class CreditsPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "CreditsBase/Credits_1/Text/Text1", "TWGS_Credits_Text1_1" },
            { "CreditsBase/Credits_2/Text/Text1", "TWGS_Credits_Text2" },
            { "CreditsBase/Credits_1/Text/TextGlitch", "TWGS_Credits_TextGlitch" },
            { "CreditsBase/Credits_3/Text/Text1", "TWGS_Credits_Text1_3" }
        };

        [HarmonyPatch(typeof(Credits), "OnEnable")]
        private static class OnEnablePatch
        {
            [HarmonyPostfix]
            private static void Postfix(Credits __instance)
            {
                ApplyLocalizationToCredits(__instance.transform);
                initialized = true;
            }
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && __instance.name.StartsWith("Credits") && !initialized)
                {
                    Transform parent = __instance.transform;
                    while (parent != null && parent.GetComponent<Credits>() == null)
                    {
                        parent = parent.parent;
                    }
                    
                    if (parent != null)
                    {
                        ApplyLocalizationToCredits(parent);
                        initialized = true;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Credits), "Close")]
        private static class ClosePatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initialized = false;
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

        private static void ApplyLocalizationToCredits(Transform rootTransform)
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
                        Component[] components = targetTransform.GetComponents<Component>();
                        foreach (Component component in components)
                        {
                            if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                            {
                                Object.Destroy(component);
                            }
                        }

                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization();
                        }
                        else
                        {
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }
    }
}