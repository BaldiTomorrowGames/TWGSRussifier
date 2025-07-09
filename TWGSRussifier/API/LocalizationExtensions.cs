using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class LocalizationExtensions
    {
        public static void ApplyLocalizations(this Transform root, IReadOnlyDictionary<string, string> targets, bool forceRefresh = false)
        {
            if (root == null) return;

            foreach (var target in targets)
            {
                Transform? elementTransform = root.FindTransform(target.Key);

                if (elementTransform != null)
                {
                    var textComponent = elementTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        var localizer = textComponent.GetComponent<TextLocalizer>() ?? textComponent.gameObject.AddComponent<TextLocalizer>();
                        
                        if (localizer.key != target.Value || forceRefresh)
                        {
                            localizer.key = target.Value;
                            localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }
    }
} 