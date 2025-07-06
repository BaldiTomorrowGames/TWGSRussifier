using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
// using TWGSRussifier.Runtime;
using TWGSRussifier.API;

namespace TWGSRussifier
{
    internal class MainModePatch
    {
        private static bool fixesApplied = false;
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("MainContinue", new Vector2(380f, 32f))
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
        
        [HarmonyPatch(typeof(MenuButton), "Press")]
        private static class MainButtonPressPatch
        {
            [HarmonyPostfix]
            private static void Postfix(MenuButton __instance)
            {
                if (__instance != null && __instance.name == "Main")
                {
                    CoroutineHelper.StartCoroutine(WaitAndCheckHideSeekMenu());
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
                
                if (__instance.name == "HideSeekMenu" && value && !fixesApplied)
                {
                    ApplyButtonSizeFixes(__instance.transform);
                    
                    fixesApplied = true;
                }
            }
        }
        
        private static System.Collections.IEnumerator WaitAndCheckHideSeekMenu()
        {
            yield return new WaitForSeconds(0.2f);
            
            GameObject hideSeekMenu = GameObject.Find("HideSeekMenu");
            if (hideSeekMenu != null)
            {
                ApplyButtonSizeFixes(hideSeekMenu.transform);
                
                fixesApplied = true;
            }
        }
        
        private static void ApplyButtonSizeFixes(Transform hideSeekMenuTransform)
        {
            foreach (var target in SizeDeltaTargets)
            {
                Transform elementTransform = FindInChildrenIncludingInactive(hideSeekMenuTransform, target.Key);
                
                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
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
    
    public static class CoroutineHelper
    {
        private static CoroutineExecutor executor;

        public static Coroutine StartCoroutine(System.Collections.IEnumerator coroutine)
        {
            if (executor == null)
            {
                GameObject go = new GameObject("CoroutineExecutor");
                Object.DontDestroyOnLoad(go);
                executor = go.AddComponent<CoroutineExecutor>();
            }
            
            return executor.StartCoroutine(coroutine);
        }

        private class CoroutineExecutor : MonoBehaviour
        {
        }
    }
} 