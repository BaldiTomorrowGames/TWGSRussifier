using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System.Reflection;
using System.Collections;

namespace TWGSRussifier
{
    internal class PickModeMenuPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("FieldTrips", new Vector2(-120f, 3f)),
            new KeyValuePair<string, Vector2>("Tutorial", new Vector2(120f, 3f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton", new Vector2(-85f, -105f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/YesButton", new Vector2(90f, -105f))
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> OffsetMinTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Endless", new Vector2(-216f, 0f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton", new Vector2(160f, 32f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton/Text", new Vector2(155f, 32f))
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
        
        [HarmonyPatch(typeof(GameLoader), "LoadLevel")]
        private static class LoadLevelPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                ApplyFixesToPickMode();
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
                
                if (__instance.name == "PickMode" && value && !fixesApplied)
                {
                    ApplyButtonPositionFixes(__instance.transform);
                    fixesApplied = true;
                }
            }
        }
        
        private static void ApplyFixesToPickMode()
        {
            if (fixesApplied) return;
            
            GameObject pickModeObject = GameObject.Find("PickMode");
            if (pickModeObject != null)
            {
                ApplyButtonPositionFixes(pickModeObject.transform);
                fixesApplied = true;
            }
        }

        private static void ApplyButtonPositionFixes(Transform pickModeTransform)
        {
            foreach (var target in AnchoredPositionTargets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(pickModeTransform, target.Key);
                
                if (elementTransform != null)
                {
                    RectTransform? rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (rectTransform.anchoredPosition != target.Value)
                        {
                            rectTransform.anchoredPosition = target.Value;
                        }
                    }
                }
            }
            
            foreach (var target in OffsetMinTargets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(pickModeTransform, target.Key);
                
                if (elementTransform != null)
                {
                    RectTransform? rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (rectTransform.offsetMin != target.Value)
                        {
                            rectTransform.offsetMin = target.Value;
                        }
                    }
                }
            }

            foreach (var target in SizeDeltaTargets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(pickModeTransform, target.Key);
                
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
    }
} 