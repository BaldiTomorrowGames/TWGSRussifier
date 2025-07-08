using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    internal class PickChallengeMenuPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Stealthy", new Vector2(175f, 32f))
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
                
                if (__instance.name == "PickChallenge" && value && !fixesApplied)
                {
                    ApplyButtonSizeFixes(__instance.transform);
                    fixesApplied = true;
                }
            }
        }
        
        private static void ApplyButtonSizeFixes(Transform pickChallengeTransform)
        {
            foreach (var target in SizeDeltaTargets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(pickChallengeTransform, target.Key);
                
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