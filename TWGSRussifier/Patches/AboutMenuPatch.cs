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
        
        private static Dictionary<string, Transform> BuildTransformPathMap(Transform parent)
        {
            var map = new Dictionary<string, Transform>();
            var children = parent.GetComponentsInChildren<Transform>(true);

            foreach (var child in children)
            {
                if (child == parent) continue;
        
                StringBuilder pathBuilder = new StringBuilder();
                Transform current = child;
                while (current != null && current != parent)
                {
                    if (pathBuilder.Length > 0)
                        pathBuilder.Insert(0, "/");
                    pathBuilder.Insert(0, current.name);
                    current = current.parent;
                }
        
                if (current == parent)
                {
                    string path = pathBuilder.ToString();
                    if (!map.ContainsKey(path))
                    {
                        map.Add(path, child);
                    }
                }
            }
            return map;
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
                    var transformMap = BuildTransformPathMap(__instance.transform);
                    ApplyLocalization(transformMap);
                    ApplySizeDeltaChanges(transformMap);
                    fixesApplied = true;
                    
                    ForceRefreshLocalization(transformMap);
                }
            }
        }
        
        private static void ForceRefreshLocalization(Dictionary<string, Transform> transformMap)
        {
            // Debug.Log("[AboutMenuPatch] Принудительная перезагрузка локализации");
            foreach (var entry in LocalizationKeys)
            {
                if (transformMap.TryGetValue(entry.Key, out Transform targetTransform))
                {
                    TextLocalizer localizer = targetTransform.GetComponent<TextLocalizer>();
                    if (localizer != null)
                    {
                        localizer.RefreshLocalization();
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {entry.Key} в меню About");
                }
            }
        }
        
        private static void ApplySizeDeltaChanges(Dictionary<string, Transform> transformMap)
        {
           // Debug.Log("[AboutMenuPatch] Применение изменений размеров");
            
            foreach (var target in SizeDeltaTargets)
            {
                if (transformMap.TryGetValue(target.Key, out Transform elementTransform))
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.sizeDelta = target.Value;
                       // Debug.Log($"[AboutMenuPatch] Применено изменение размеров {target.Value} к {target.Key}");
                    }
                    else
                    {
                       // Debug.LogWarning($"[AboutMenuPatch] RectTransform компонент не найден на {target.Key}");
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {target.Key} в меню About");
                }
            }
        }
        
        private static void ApplyLocalization(Dictionary<string, Transform> transformMap)
        {
           // Debug.Log("[AboutMenuPatch] Применение локализации к меню About");
            foreach (var entry in LocalizationKeys)
            {
                if (transformMap.TryGetValue(entry.Key, out Transform targetTransform))
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
                            localizer.key = entry.Value;
                           // Debug.Log($"[AboutMenuPatch] Добавлен TextLocalizer к {entry.Key} с ключом {entry.Value}");
                        }
                        else if (localizer.key != entry.Value)
                        {
                            localizer.key = entry.Value;
                            localizer.RefreshLocalization();
                           // Debug.Log($"[AboutMenuPatch] Обновлен ключ TextLocalizer для {entry.Key} на {entry.Value}");
                        }
                    }
                    else
                    {
                      //  Debug.LogWarning($"[AboutMenuPatch] TextMeshProUGUI компонент не найден на {entry.Key}");
                    }
                }
                else
                {
                   // Debug.LogWarning($"[AboutMenuPatch] Не удалось найти путь {entry.Key} в меню About");
                }
            }
        }
    }
} 