using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(MainMenu), "Start")]
    internal class MainMenuPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "Version", "TWGS_MainMenu_Version" },
        };

        private static Transform FindInChildrenIncludingInactive(Transform parent, string path)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == parent) continue;
                if (DoesPathMatch(parent, child, path))
                {
                    return child;
                }
            }
            return null;
        }

        private static bool DoesPathMatch(Transform parent, Transform target, string expectedPath)
        {
            if (target == null || parent == null || target == parent) return false;
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;
            while (current != null && current != parent)
            {
                if (pathBuilder.Length > 0)
                    pathBuilder.Insert(0, "/");
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }
            if (current != parent) return false;
            return pathBuilder.ToString() == expectedPath;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenu), "Start")]
        private static void Start_Postfix(MainMenu __instance)
        {
            ApplyLocalization(__instance);
        }

        private static void ApplyLocalization(MainMenu mainMenuInstance)
        {
            if (mainMenuInstance == null) return;

            Transform mainMenuTransform = mainMenuInstance.transform;

            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;

                Transform targetTransform = FindInChildrenIncludingInactive(mainMenuTransform, relativePath);
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = localizationKey;
                        }
                        else if (localizer.key != localizationKey)
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