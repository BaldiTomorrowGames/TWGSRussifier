using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Linq;

namespace TWGSRussifier.Patches
{
    internal class ChallengeWinPatch
    {
        private const string WinTextKey = "TWGS_ChallengeWin_Text";

        [HarmonyPatch(typeof(ChallengeWin), "Start")]
        private static class ChallengeWinStartPatch
        {
            [HarmonyPostfix]
            private static void Postfix(ChallengeWin __instance)
            {
                // We search for the text component within the ChallengeWin hierarchy.
                // GetComponentsInChildren(true) is robust and finds components even if they are inactive.
                var textComponents = __instance.GetComponentsInChildren<TextMeshProUGUI>(true);
                var targetText = textComponents.FirstOrDefault(t => t.name == "Text (TMP)");

                if (targetText != null)
                {
                    ApplyLocalizer(targetText);
                }
            }
        }

        private static void ApplyLocalizer(TextMeshProUGUI textComponent)
        {
            TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
            if (localizer == null)
            {
                localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
            }

            if (localizer.key != WinTextKey)
            {
                localizer.key = WinTextKey;
                localizer.RefreshLocalization();
            }
        }
    }
} 