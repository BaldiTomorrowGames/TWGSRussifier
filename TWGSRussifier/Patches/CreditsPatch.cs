using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(Credits))]
    public class CreditsPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>
        {
            // Main Credits
            { "Text", "TWGS_Credits_MainTitleText" },
            { "TrademarkText", "TWGS_Credits_UnityDisclaimerText" },

            // Voices & Artists
            { "Text (1)", "TWGS_Credits_ArtistsText" },
            
            // Testing, Tutorials, Other Testers
            { "Text (2)", "TWGS_Credits_OtherTestersText" },
            
            // Tools & Assets
            { "Text (3)", "TWGS_Credits_ToolsText" },
            
            // Warner Disclaimer
            { "Main Credits (3.5)/TrademarkText", "TWGS_Credits_WarnerDisclaimerText" },
            { "Main Credits (3.5)/Text", "TWGS_Credits_SoundsFromText" },
            
            // Open Source
            { "Main Credits (3.75)/Text", "TWGS_Credits_OpenSourceText" },
            { "Main Credits (3.75)/LicenseText", "TWGS_Credits_LicenseText" },
            
            // Music & Special Thanks
            { "Main Credits (4)/Text", "TWGS_Credits_MusicText" },
            { "Main Credits (4)/Text (1)", "TWGS_Credits_SpecialThanksText" },
            { "Main Credits (4)/Text (2)", "TWGS_Credits_BibleVerseText" },
            
            // Thank You
            { "Main Credits (5)/Text", "TWGS_Credits_ThankYouText" }
        };

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void LocalizeCreditsOnStart(Credits __instance)
        {
            if (__instance.screens == null) return;

            foreach (var screenCanvas in __instance.screens)
            {
                if (screenCanvas == null) continue;

                var textComponents = screenCanvas.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var textComponent in textComponents)
                {
                    string path = GetTransformPath(textComponent.transform, screenCanvas.transform);
                    if (LocalizationKeys.TryGetValue(path, out string key))
                    {
                        ApplyLocalization(textComponent, key);
                    }
                    else if (LocalizationKeys.TryGetValue(textComponent.name, out key)) // Fallback for simple names
                    {
                        ApplyLocalization(textComponent, key);
                    }
                }
            }
        }

        private static void ApplyLocalization(TextMeshProUGUI textComponent, string key)
        {
            TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
            if (localizer == null)
            {
                localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
            }

            if (localizer.key != key)
            {
                localizer.key = key;
                localizer.RefreshLocalization();
            }
        }
        
        private static string GetTransformPath(Transform target, Transform parent)
        {
            if (target == parent)
                return target.name;

            System.Text.StringBuilder path = new System.Text.StringBuilder(target.name);
            Transform current = target.parent;

            while (current != null && current != parent)
            {
                path.Insert(0, "/");
                path.Insert(0, current.name);
                current = current.parent;
            }
            return path.ToString();
        }
    }
} 