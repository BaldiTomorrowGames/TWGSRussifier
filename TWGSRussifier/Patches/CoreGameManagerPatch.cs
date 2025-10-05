using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace TWGSRussifier.Patches
{
    internal class CoreGameManagerPatch
    {
        private static bool initialized = false;

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("PauseMenuScreens/ClassicPauseMenu/Confirm/ConfirmTitle", new Vector2(212f, 128f))
        };

        [HarmonyPatch(typeof(CoreGameManager), "Pause")]
        private static class PausePatch
        {
            [HarmonyPostfix]
            private static void Postfix(CoreGameManager __instance, bool openScreen)
            {
                if (openScreen && !initialized)
                {
                    ApplyRectTransformChanges(__instance);
                    initialized = true;
                }
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

        private static void ApplyRectTransformChanges(CoreGameManager coreGameManager)
        {
            Transform coreGameManagerTransform = coreGameManager.transform;
            ProcessSizeDeltaTargets(coreGameManagerTransform, SizeDeltaTargets);
        }

        private static void ProcessSizeDeltaTargets(Transform root, List<KeyValuePair<string, Vector2>> targets)
        {
            foreach (var target in targets)
            {
                Transform? elementTransform = FindInChildrenIncludingInactive(root, target.Key);

                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.sizeDelta = target.Value;
                    }
                }
            }
        }

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
    }
}