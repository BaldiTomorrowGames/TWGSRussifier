using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    internal class ModeSelectPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "PlusButtonStuff/BaldiHidden/Text (TMP)", "TWGS_ModeSelect_BaldiHidden" },
            { "PlusButtonStuff/BaldiShown/Text (TMP)", "TWGS_ModeSelect_BaldiShown" }
        };

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && __instance.name == "ModeSelect")
                {
                    ApplyLocalizationToModeSelect(__instance.transform);
                    initialized = true;
                }
            }
        }

        [HarmonyPatch(typeof(GameLoader), "SetStyle")]
        private static class SetStylePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameLoader __instance, int style)
            {
                if (style == 2 && !initialized)
                {
                    GameObject modeSelectObject = GameObject.Find("ModeSelect");
                    if (modeSelectObject != null)
                    {
                        ApplyLocalizationToModeSelect(modeSelectObject.transform);
                        initialized = true;
                    }
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

        private static void ApplyLocalizationToModeSelect(Transform rootTransform)
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