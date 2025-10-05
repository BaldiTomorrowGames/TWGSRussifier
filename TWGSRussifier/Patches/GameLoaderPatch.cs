using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System.Reflection;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(GameLoader), "UpdateFunSettings")]
    internal class GameLoaderPatch
    {
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("FunSettings", new Vector2(-128f, -23f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting1", new Vector2(0f, -35f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting2", new Vector2(0f, -71f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting3", new Vector2(0f, -107f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting4", new Vector2(0f, -143f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting1/UnlockedText", new Vector2(16f, 0f)),
            new KeyValuePair<string, Vector2>("FunSettings/FunSetting4/UnlockedText", new Vector2(16f, 0f)),
            new KeyValuePair<string, Vector2>("CurrentStyle/Text (TMP)", new Vector2(1f, 0f)),
        };

        private static readonly List<KeyValuePair<string, float>> FontSizeTargets = new List<KeyValuePair<string, float>>
        {
            new KeyValuePair<string, float>("FunSettings", 20f),
            new KeyValuePair<string, float>("FunSettings/FunSetting1/UnlockedText", 21f),
            new KeyValuePair<string, float>("FunSettings/FunSetting4/UnlockedText", 22f),
            new KeyValuePair<string, float>("CurrentStyle/Text (TMP)", 21f),
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMinTargets = new List<KeyValuePair<string, Vector2>>
        {
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMaxTargets = new List<KeyValuePair<string, Vector2>>
        {
        };

        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameLoader), "UpdateFunSettings")]
        private static void UpdateFunSettings_Postfix(GameLoader __instance)
        {
            ApplyChanges(__instance);
            ApplyLocalization(__instance);
        }

        private static void ApplyChanges(GameLoader gameLoaderInstance)
        {
            Transform gameLoaderTransform = gameLoaderInstance.transform;

            var menuCanvasField = typeof(GameLoader).GetField("menuCanvas");
            if (menuCanvasField != null)
            {
                GameObject menuCanvas = (GameObject)menuCanvasField.GetValue(gameLoaderInstance);
                if (menuCanvas != null)
                {
                    Transform menuCanvasTransform = menuCanvas.transform;

                    ProcessTargets(menuCanvasTransform, AnchoredPositionTargets, (rect, value) => rect.anchoredPosition = value, "anchoredPosition");

                    ProcessTargets(menuCanvasTransform, SizeDeltaTargets, (rect, value) => rect.sizeDelta = value, "sizeDelta");

                    ProcessTargets(menuCanvasTransform, OffsetMinTargets, (rect, value) => rect.offsetMin = value, "offsetMin");

                    ProcessTargets(menuCanvasTransform, OffsetMaxTargets, (rect, value) => rect.offsetMax = value, "offsetMax");

                    ProcessFontSizeTargets(menuCanvasTransform, FontSizeTargets);
                }
            }
            else
            {
                ProcessTargets(gameLoaderTransform, AnchoredPositionTargets, (rect, value) => rect.anchoredPosition = value, "anchoredPosition");
            }
        }

        private static void ApplyLocalization(GameLoader gameLoaderInstance)
        {
            if (gameLoaderInstance == null) return;

            Transform gameLoaderTransform = gameLoaderInstance.transform;

            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key;
                string localizationKey = entry.Value;

                Transform? targetTransform = FindInChildrenIncludingInactive(gameLoaderTransform, relativePath);
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
                Transform? elementTransform = FindInChildrenIncludingInactive(root, target.Key);

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

        private static void ProcessFontSizeTargets(Transform root, List<KeyValuePair<string, float>> targets)
        {
            foreach (var target in targets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(root, target.Key);

                if (elementTransform != null)
                {
                    TextMeshProUGUI textComponent = elementTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        textComponent.fontSize = target.Value;
                    }
                }
            }
        }

    }
}