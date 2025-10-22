using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(CoreGameManager), "Pause")]
    internal class ClassicPauseMenuPatch
    {
        private static bool initialized = false;

        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/InvincibleToggle/HotSpot", new Vector2(-65f, 0f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/NoClipToggle/HotSpot", new Vector2(-67f, 0f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/SuperSpeedToggle/HotSpot", new Vector2(-67f, 0f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/UnlimitedToggle/HotSpot", new Vector2(-72f, 0f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/UnlimitedToggle/ToggleText", new Vector2(-8f, 17f)),
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/InvincibleToggle/HotSpot", new Vector2(215f, 32f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/NoClipToggle/HotSpot", new Vector2(225f, 32f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/SuperSpeedToggle/HotSpot", new Vector2(225f, 32f)),
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Debug/UnlimitedToggle/HotSpot", new Vector2(240f, 60f)),
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
        private static void Postfix(CoreGameManager __instance, bool openScreen)
        {
            if (openScreen && !initialized)
            {
                ApplyChanges(__instance);
                initialized = true;
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initialized = false;
            }
        }

        private static void ApplyChanges(CoreGameManager coreGameManager)
        {
            Transform coreGameManagerTransform = coreGameManager.transform;

            ProcessTargets(coreGameManagerTransform, AnchoredPositionTargets, (rect, value) => rect.anchoredPosition = value, "anchoredPosition");
            ProcessTargets(coreGameManagerTransform, SizeDeltaTargets, (rect, value) => rect.sizeDelta = value, "sizeDelta");
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
    }
}