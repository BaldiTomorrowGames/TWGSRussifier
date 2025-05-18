using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.Events;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch] 
    internal class MainMenuPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "StartTest", "TWGS_Menu_TestMapText" },     
            { "StartTest_1", "TWGS_Menu_TestMapText_1" },
            { "Reminder", "TWGS_Menu_Reminder" },
            { "ModInfo", "TWGS_Menu_ModInfo" }
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("StartTest", new Vector2(210f, 32f)),
            new KeyValuePair<string, Vector2>("StartTest_1", new Vector2(228f, 32f))
        };

        private static Transform FindInChildrenIncludingInactive(Transform parent, string path)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == path)
                {
                    return child;
                }
            }
            
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
                    ApplySizeChanges(__instance.transform);
                    ApplyLocalization(__instance.transform);
                    CreateModInfoButton(__instance.transform);
                }
            }
        }

        private static void CreateModInfoButton(Transform rootTransform)
        {
            if (rootTransform == null) return;

            Transform reminderTransform = rootTransform.Find("Reminder");
            if (reminderTransform == null) return;

            Transform existingModInfo = rootTransform.Find("ModInfo");
            if (existingModInfo != null) return;

            GameObject modInfo = GameObject.Instantiate(reminderTransform.gameObject, rootTransform);
            modInfo.name = "ModInfo";

            modInfo.transform.localPosition = new Vector3(-180f, 155f, 0f);
            
            modInfo.transform.SetSiblingIndex(15);
            
            RectTransform rectTransform = modInfo.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(118f, 50f);
                rectTransform.offsetMin = new Vector2(-239f, 160f);
            }

            TextMeshProUGUI textComponent = modInfo.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.raycastTarget = true;

                TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                }
                
                localizer.key = "TWGS_Menu_ModInfo";
                localizer.RefreshLocalization();

                StandardMenuButton button = textComponent.gameObject.AddComponent<StandardMenuButton>();
                button.OnPress = new UnityEvent();
                button.OnHighlight = new UnityEvent();
                button.OnRelease = new UnityEvent();
                button.OffHighlight = new UnityEvent();
                button.underlineOnHigh = true;
                button.text = textComponent;
                button.gameObject.tag = "Button";

                button.OnPress.AddListener(() => { Application.OpenURL("https://t.me/translate_balda"); });
            }
        }

        private static void ApplySizeChanges(Transform rootTransform)
        {
            if (rootTransform == null) return;
            
            foreach (var target in SizeDeltaTargets)
            {
                Transform elementTransform = FindInChildrenIncludingInactive(rootTransform, target.Key); 
                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (rectTransform.sizeDelta != target.Value)
                            rectTransform.sizeDelta = target.Value;
                    }
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
                    
                    if (textComponent == null) 
                    {
                        Transform textChild = targetTransform.Find("Text (TMP)");
                        if (textChild != null)
                        {
                             textComponent = textChild.GetComponent<TextMeshProUGUI>();
                        }
                    }

                    if (textComponent != null)
                    {
                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization(); 
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