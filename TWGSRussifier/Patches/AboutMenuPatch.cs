using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    internal class AboutMenuPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "DevUpdateTitle", "TWGS_About_DevUpdateTitle" },
            { "DevUpdateText", "TWGS_About_DevUpdateText" },
            { "Credits", "TWGS_About_CreditsText" },
            { "WebsiteButton", "TWGS_About_WebsiteButtonText" },
            { "DevlogsButton", "TWGS_About_DevlogsButtonText" },
            { "BugsButton", "TWGS_About_BugsButtonText" },
            { "SaveFolderButton", "TWGS_About_SaveFolderButtonText" },
            { "AnniversaryButton", "TWGS_About_AnniversaryButtonText" },
            { "RoadmapButton", "TWGS_About_RoadmapButtonText" }
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("SaveFolderButton", new Vector2(150f, 50f))
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
        private static class MenuButtonPressPatch
        {
            [HarmonyPostfix]
            private static void Postfix(MenuButton __instance)
            {
                if (__instance != null && __instance.name == "About")
                {
                    // Debug.Log("[AboutMenuPatch] Кнопка About нажата, сброс fixesApplied");
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
                
                if (__instance.name == "About" && value && !fixesApplied)
                {
                    // Debug.Log("[AboutMenuPatch] Меню About активировано, применяется локализация");
                    ApplyLocalization(__instance.transform);
                    ApplySizeDeltaChanges(__instance.transform);
                    fixesApplied = true;
                    
                    ForceRefreshLocalization(__instance.transform);
                }
            }
        }
        
        private static void ForceRefreshLocalization(Transform aboutTransform)
        {
            // Debug.Log("[AboutMenuPatch] Принудительная перезагрузка локализации");
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                
                Transform targetTransform = FindInChildrenIncludingInactive(aboutTransform, relativePath);
                if (targetTransform != null)
                {
                    TextLocalizer localizer = targetTransform.GetComponent<TextLocalizer>();
                    if (localizer != null)
                    {
                        localizer.RefreshLocalization();
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {relativePath} в меню About");
                }
            }
        }
        
        private static void ApplySizeDeltaChanges(Transform aboutTransform)
        {
           // Debug.Log("[AboutMenuPatch] Применение изменений размеров");
            
            foreach (var target in SizeDeltaTargets)
            {
                string relativePath = target.Key;
                Vector2 sizeDelta = target.Value;
                
                Transform elementTransform = FindInChildrenIncludingInactive(aboutTransform, relativePath);
                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.sizeDelta = sizeDelta;
                       // Debug.Log($"[AboutMenuPatch] Применено изменение размеров {sizeDelta} к {relativePath}");
                    }
                    else
                    {
                       // Debug.LogWarning($"[AboutMenuPatch] RectTransform компонент не найден на {relativePath}");
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {relativePath} в меню About");
                }
            }
        }
        
        private static void ApplyLocalization(Transform aboutTransform)
        {
           // Debug.Log("[AboutMenuPatch] Применение локализации к меню About");
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;
                
                Transform targetTransform = FindInChildrenIncludingInactive(aboutTransform, relativePath);
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
                           // Debug.Log($"[AboutMenuPatch] Добавлен TextLocalizer к {relativePath} с ключом {localizationKey}");
                        }
                        else if (localizer.key != localizationKey)
                        {
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization();
                           // Debug.Log($"[AboutMenuPatch] Обновлен ключ TextLocalizer для {relativePath} на {localizationKey}");
                        }
                    }
                    else
                    {
                      //  Debug.LogWarning($"[AboutMenuPatch] TextMeshProUGUI компонент не найден на {relativePath}");
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {relativePath} в меню About");
                }
            }
        }
    }
} 