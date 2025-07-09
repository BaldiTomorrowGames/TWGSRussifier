using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class RectTransformExtensions
    {
        public static Transform? FindTransform(this Transform parent, string path)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == parent) continue;
                if (GetPath(parent, child) == path)
                {
                    return child;
                }
            }
            return null;
        }

        private static string GetPath(Transform parent, Transform target)
        {
            if (target == null || parent == null || target == parent) return "";
            
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;
            while (current != null && current != parent)
            {
                if (pathBuilder.Length > 0)
                    pathBuilder.Insert(0, "/");
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }
            
            return (current == parent) ? pathBuilder.ToString() : "";
        }

        private static void ProcessTargets(this Transform root, IEnumerable<KeyValuePair<string, Vector2>> targets, System.Action<RectTransform, Vector2> applyAction)
        {
            foreach (var target in targets)
            {
                Transform? elementTransform = root.FindTransform(target.Key);

                if (elementTransform != null)
                {
                    RectTransform? rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        applyAction(rectTransform, target.Value);
                    }
                }
            }
        }

        public static void SetAnchoredPositions(this Transform root, IEnumerable<KeyValuePair<string, Vector2>> targets)
        {
            root.ProcessTargets(targets, (rect, value) => rect.anchoredPosition = value);
        }

        public static void SetSizeDeltas(this Transform root, IEnumerable<KeyValuePair<string, Vector2>> targets)
        {
            root.ProcessTargets(targets, (rect, value) => rect.sizeDelta = value);
        }

        public static void SetOffsetMins(this Transform root, IEnumerable<KeyValuePair<string, Vector2>> targets)
        {
            root.ProcessTargets(targets, (rect, value) => rect.offsetMin = value);
        }

        public static void SetOffsetMaxs(this Transform root, IEnumerable<KeyValuePair<string, Vector2>> targets)
        {
            root.ProcessTargets(targets, (rect, value) => rect.offsetMax = value);
        }
    }
} 