using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    [HarmonyPatch(typeof(OptionsMenu), "Awake")]
    internal class OptionsMenuPatch
    {
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("General/FlashToggle/ToggleText", new Vector2(-8f, 12f)),
            new KeyValuePair<string, Vector2>("Data/Main/DeleteFileButton", new Vector2(0f, -123f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetTripScoresButton", new Vector2(0f, -65f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetEndlessScoresButton", new Vector2(0f, -10f)),
            new KeyValuePair<string, Vector2>("Graphics/ApplyButton", new Vector2(115f, -160f)),
            new KeyValuePair<string, Vector2>("Graphics/PixelFilterToggle/HotSpot", new Vector2(-14f, -20f)),

            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton/MapperButton", new Vector2(5f, 0f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamInputButton", new Vector2(4f, 0f)),
            
            new KeyValuePair<string, Vector2>("Graphics/FullScreenToggle/ToggleText", new Vector2(-6f, 0f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Graphics/ApplyButton/ApplyText", new Vector2(132f, 32f)),
            new KeyValuePair<string, Vector2>("General/FlashToggle/HotSpot", new Vector2(232f, 60f)),
            new KeyValuePair<string, Vector2>("Graphics/PixelFilterToggle/HotSpot", new Vector2(300f, 60f)),
            new KeyValuePair<string, Vector2>("Graphics/ApplyButton", new Vector2(140f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton", new Vector2(370f, 32f)),
            new KeyValuePair<string, Vector2>("Graphics/FullScreenToggle/HotSpot", new Vector2(265f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton/MapperButton", new Vector2(400f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamInputButton", new Vector2(360f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton", new Vector2(355f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamDesc", new Vector2(340f, 128f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMinTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Data/Main/ResetEndlessScoresButton", new Vector2(-160f, -60f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetTripScoresButton", new Vector2(-160f, -110f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMaxTargets = new List<KeyValuePair<string, Vector2>>
        {
        };

        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "ControlsTemp/MapperButton/MapperButton", "TWGS_Menu_MapperButtonText" },
            { "ControlsTemp/SteamButton/SteamInputButton", "TWGS_Menu_SteamInputText" },
            { "ControlsTemp/SteamButton/SteamDesc", "TWGS_Menu_SteamDescText" },
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
        [HarmonyPatch(typeof(OptionsMenu), "Awake")]
        private static void Awake_Postfix(OptionsMenu __instance)
        {
            ApplyChanges(__instance);
            
            ApplyLocalization(__instance);
        }

        private static void ApplyChanges(OptionsMenu optionsMenuInstance)
        {
            Transform optionsTransform = optionsMenuInstance.transform;

            ProcessTargets(optionsTransform, AnchoredPositionTargets, (rect, value) => rect.anchoredPosition = value, "anchoredPosition");

            ProcessTargets(optionsTransform, SizeDeltaTargets, (rect, value) => rect.sizeDelta = value, "sizeDelta");

            ProcessTargets(optionsTransform, OffsetMinTargets, (rect, value) => rect.offsetMin = value, "offsetMin");

            ProcessTargets(optionsTransform, OffsetMaxTargets, (rect, value) => rect.offsetMax = value, "offsetMax");
        }

        private static void ApplyLocalization(OptionsMenu optionsMenuInstance)
        {
            if (optionsMenuInstance == null) return;
            
            Transform optionsTransform = optionsMenuInstance.transform;
            
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;
                
                Transform targetTransform = FindInChildrenIncludingInactive(optionsTransform, relativePath);
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

        private static void ProcessTargets(Transform root, List<KeyValuePair<string, Vector2>> targets, System.Action<RectTransform, Vector2> applyAction, string propertyName)
        {
            foreach (var target in targets)
            {
                Transform elementTransform = FindInChildrenIncludingInactive(root, target.Key);

                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        applyAction(rectTransform, target.Value);
                    }
                }
            }
        }
    }
} 