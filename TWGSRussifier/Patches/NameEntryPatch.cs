using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    internal class NameEntryPatch
    {
        private static bool buttonFixesApplied = false;
        private static bool localizationApplied = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "SaveError/Text (TMP)", "TWGS_Menu_SaveErrorText" } 
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

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "NameEntry")
                {
                     if (value)
                     {
                          if(!localizationApplied)
                          {
                              ApplyLocalization(__instance.transform);
                              localizationApplied = true;
                          }
                     }
                     else
                     {
                         localizationApplied = false;
                         buttonFixesApplied = false; 
                     }
                }
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Awake")]
        private static class NameManagerAwakePatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                buttonFixesApplied = false;
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Load")]
        private static class LoadPatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                buttonFixesApplied = false; 
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        private static void ApplyNewFileButtonFixes(NameManager nameManager)
        {
            if (nameManager == null || nameManager.newFileButton == null || buttonFixesApplied) return;
            
            RectTransform buttonRect = nameManager.newFileButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.sizeDelta = new Vector2(158f, 30f);
            }
            
            Transform textComponentTransform = nameManager.newFileButton.transform.Find("Text (TMP)");
            if (textComponentTransform != null)
            {
                RectTransform textRect = textComponentTransform.GetComponent<RectTransform>();
                if (textRect != null)
                {
                    textRect.anchoredPosition = new Vector2(1f, 0f);
                    textRect.sizeDelta = new Vector2(150f, 32f);
                    
                    buttonFixesApplied = true; 
                }
            }
        }
        private static void ApplyLocalization(Transform rootTransform)
        {
             if (rootTransform == null) return;
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;

                Transform targetTransform = FindInChildrenIncludingInactive(rootTransform, relativePath);
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                        }
                        
                        localizer.key = localizationKey;
                        localizer.RefreshLocalization(); 
                    }
                     else
                    {
                    }
                }
                 else
                {
                }
            }
        }
    }
} 