using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    internal class PickEndlessMapPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Random", new Vector2(134f, 100f))
        };
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "Text (TMP)", "TWGS_Menu_EndlessMapText" },
        };
        
        private static Transform? FindInChildrenIncludingInactive(Transform parent, string path)
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
        
        [HarmonyPatch(typeof(MenuButton), "Press")]
        private static class MenuButtonPressPatch
        {
            [HarmonyPostfix]
            private static void Postfix(MenuButton __instance)
            {
                if (__instance != null && __instance.name == "Endless")
                {
                    fixesApplied = false;
                }
            }
        }
        
        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "Menu" && value)
                {
                    fixesApplied = false;
                }
                
                if (__instance.name == "PickEndlessMap" && value && !fixesApplied)
                {
                    ApplyButtonSizeFixes(__instance.transform);
                    ApplyLocalization(__instance.transform);
                    fixesApplied = true;
                    
                    ForceRefreshLocalization(__instance.transform);
                }
            }
        }
        private static void ForceRefreshLocalization(Transform pickEndlessMapTransform)
        {
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                
                Transform? targetTransform = FindInChildrenIncludingInactive(pickEndlessMapTransform, relativePath);
                if (targetTransform != null)
                {
                    TextLocalizer? localizer = targetTransform.GetComponent<TextLocalizer>();
                    if (localizer != null)
                    {
                        localizer.RefreshLocalization();
                    }
                }
            }
        }
        
        private static void ApplyButtonSizeFixes(Transform pickEndlessMapTransform)
        {
            foreach (var target in SizeDeltaTargets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(pickEndlessMapTransform, target.Key);
                
                if (elementTransform != null)
                {
                    RectTransform? rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (rectTransform.sizeDelta != target.Value)
                        {
                            rectTransform.sizeDelta = target.Value;
                        }
                    }
                }
            }
        }
        
        private static void ApplyLocalization(Transform pickEndlessMapTransform)
        {
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;
                
                Transform? targetTransform = FindInChildrenIncludingInactive(pickEndlessMapTransform, relativePath);
                if (targetTransform != null)
                {
                    TextMeshProUGUI? textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer? localizer = textComponent.GetComponent<TextLocalizer>();
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