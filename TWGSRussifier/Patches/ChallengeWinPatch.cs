using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using System.Reflection;
using TWGSRussifier.Runtime;

namespace TWGSRussifier.Patches
{
    internal class ChallengeWinPatch
    {
        private static bool initialized = false;
        private static readonly string[] challengeManagerPrefixes = new string[]
        {
            "SpeedyChallengeManager",
            "GrappleChallengeManager",
            "StealthyChallengeManager"
        };

        [HarmonyPatch(typeof(SpeedyChallengeManager), "Initialize")]
        [HarmonyPatch(typeof(GrappleChallengeManager), "Initialize")]
        [HarmonyPatch(typeof(StealthyChallengeManager), "Initialize")]
        private static class ChallengeInitializePatch
        {
            [HarmonyPostfix]
            private static void Postfix(BaseGameManager __instance)
            {
                FieldInfo winScreenField = __instance.GetType().GetField("winScreen", 
                    BindingFlags.Public | BindingFlags.Instance);
                
                if (winScreenField != null)
                {
                    ChallengeWin winScreen = winScreenField.GetValue(__instance) as ChallengeWin;
                    if (winScreen != null)
                    {
                        __instance.StartCoroutine(FindAndApplyLocalizationToWinScreen(winScreen, __instance.GetType().Name));
                    }
                }
            }
        }
        
        private static IEnumerator FindAndApplyLocalizationToWinScreen(ChallengeWin winScreen, string managerName)
        {
            yield return null; 
            
            Transform canvasTransform = winScreen.transform.Find("Canvas");
            if (canvasTransform != null)
            {
                Transform textTransform = canvasTransform.Find("Text (TMP)");
                if (textTransform != null)
                {
                    ApplyLocalizerToTransform(textTransform);
                    yield break;
                }
            }
            
            foreach (string prefix in challengeManagerPrefixes)
            {
                string objectPath = $"{prefix}(Clone)/ChallengeWin/Canvas/Text (TMP)";
                GameObject textObject = GameObject.Find(objectPath);
                if (textObject != null)
                {
                    ApplyLocalizerToGameObject(textObject);
                    yield break;
                }
            }
            
            yield return new WaitForSeconds(0.5f);
            
            string specificPath = $"{managerName}(Clone)/ChallengeWin/Canvas/Text (TMP)";
            GameObject specificTextObject = GameObject.Find(specificPath);
            if (specificTextObject != null)
            {
                ApplyLocalizerToGameObject(specificTextObject);
            }
        }
        
        private static void ApplyLocalizerToTransform(Transform transform)
        {
            TextMeshProUGUI textComponent = transform.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TextLocalizer localizer = transform.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = transform.gameObject.AddComponent<TextLocalizer>();
                    localizer.key = "TWGS_ChallengeWin_Text";
                    localizer.RefreshLocalization();
                }
                else
                {
                    localizer.key = "TWGS_ChallengeWin_Text";
                    localizer.RefreshLocalization();
                }
            }
        }
        
        private static void ApplyLocalizerToGameObject(GameObject gameObject)
        {
            TextMeshProUGUI textComponent = gameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TextLocalizer localizer = gameObject.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = gameObject.AddComponent<TextLocalizer>();
                    localizer.key = "TWGS_ChallengeWin_Text";
                    localizer.RefreshLocalization();
                }
                else
                {
                    localizer.key = "TWGS_ChallengeWin_Text";
                    localizer.RefreshLocalization();
                }
            }
        }
        
        [HarmonyPatch(typeof(ChallengeWin), "Start")]
        private static class ChallengeWinStartPatch
        {
            [HarmonyPostfix]
            private static void Postfix(ChallengeWin __instance)
            {
                __instance.StartCoroutine(FindAndApplyLocalizationToWinScreen(__instance, ""));
            }
        }
    }
} 