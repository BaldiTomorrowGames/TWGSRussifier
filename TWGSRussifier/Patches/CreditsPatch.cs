using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    public class CreditsPatch
    {
        private static bool initialized = false;
        private static readonly Dictionary<string, string> localizationKeys = new Dictionary<string, string>
        {
            { "Main Credits (5)/Text", "TWGS_Credits_ThankYouText" },
            { "Main Credits (3.5)/Text", "TWGS_Credits_SoundsFromText" },
            { "Main Credits (3.5)/TrademarkText", "TWGS_Credits_WarnerDisclaimerText" },
            { "Main Credits (2)/Text", "TWGS_Credits_TestingFeedbackText" },
            { "Main Credits (2)/Text (1)", "TWGS_Credits_TutorialsText" },
            { "Main Credits (2)/Text (2)", "TWGS_Credits_OtherTestersText" },
            { "Main Credits (1)/Text", "TWGS_Credits_VoicesText" },
            { "Main Credits (1)/Text (1)", "TWGS_Credits_ArtistsText" },
            { "Main Credits (4)/Text", "TWGS_Credits_MusicText" },
            { "Main Credits (4)/Text (1)", "TWGS_Credits_SpecialThanksText" },
            { "Main Credits (4)/Text (2)", "TWGS_Credits_BibleVerseText" },
            { "Main Credits (3)/Text", "TWGS_Credits_ToolsText" },
            { "Main Credits (3)/Text (1)", "TWGS_Credits_AssetsText" },
            { "Main Credits (3.75)/Text", "TWGS_Credits_OpenSourceText" },
            { "Main Credits (3.75)/LicenseText", "TWGS_Credits_LicenseText" },
            { "Main Credits/Text", "TWGS_Credits_MainTitleText" },
            { "Main Credits/TrademarkText", "TWGS_Credits_UnityDisclaimerText" }
        };

        [HarmonyPatch(typeof(SceneManager), "LoadScene", new[] { typeof(string) })]
        private static class LoadScenePatch
        {
            [HarmonyPostfix]
            private static void Postfix(string sceneName)
            {
                if (sceneName == "Credits")
                {
                    GameObject patchInitializer = new GameObject("CreditsPatchInitializer");
                    patchInitializer.AddComponent<CreditsPatchInitializer>();
                    Object.DontDestroyOnLoad(patchInitializer);
                }
            }
        }

        [HarmonyPatch(typeof(Credits), "Start")]
        private static class CreditsStartPatch
        {
            [HarmonyPostfix]
            [HarmonyPriority(Priority.Low)]
            private static void Postfix(Credits __instance)
            {
                if (!initialized)
                {
                    __instance.StartCoroutine(InitializeLocalization(__instance));
                }
            }

            private static IEnumerator InitializeLocalization(Credits credits)
            {
                yield return null;
                
                ApplyLocalizationToAllCreditsObjects();
                
                initialized = true;
            }
        }

        [HarmonyPatch(typeof(Credits), "CreditsScroll")]
        private static class CreditsScrollPatch
        {
            [HarmonyPrefix]
            private static void Prefix(Credits __instance)
            {
                if (!initialized)
                {
                    ApplyLocalizationToAllCreditsObjects();
                    initialized = true;
                }
            }
        }
        
        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class GameObjectSetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && SceneManager.GetActiveScene().name == "Credits" && 
                    __instance.name.StartsWith("Main Credits"))
                {
                    ApplyLocalizationDirectly(__instance.transform);
                }
            }
        }

        public static void ApplyLocalizationToCredits()
        {
            ApplyLocalizationToAllCreditsObjects();
        }
        
        private static void ApplyLocalizationToAllCreditsObjects()
        {
            
            Canvas[] screens = Resources.FindObjectsOfTypeAll<Canvas>();
            foreach (Canvas screen in screens)
            {
                if (screen.name.StartsWith("Main Credits"))
                {
                    ApplyLocalizationDirectly(screen.transform);
                    
                    ProcessChildren(screen.transform);
                }
            }
            
            initialized = true;
        }
        
        private static void ProcessChildren(Transform parent)
        {
            foreach (Transform child in parent)
            {
                ApplyLocalizationDirectly(child);
                ProcessChildren(child);
            }
        }
        
        private static void ApplyLocalizationDirectly(Transform obj)
        {
            foreach (var kvp in localizationKeys)
            {
                string fullPath = GetFullPath(obj);
                
                if (fullPath == kvp.Key)
                {
                    ApplyLocalizationToComponent(obj.gameObject, kvp.Value);
                    break;
                }
            }
        }
        
        private static string GetFullPath(Transform obj)
        {
            if (obj.parent == null || obj.parent.name.Contains("Canvas"))
            {
                return obj.name;
            }
            else
            {
                return obj.parent.name + "/" + obj.name;
            }
        }
        
        private static void ApplyLocalizationToComponent(GameObject textObject, string key)
        {
            TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TextLocalizer localizer = textObject.GetComponent<TextLocalizer>() ?? textObject.AddComponent<TextLocalizer>();
                localizer.key = key;
                
                localizer.RefreshLocalization();
            }
        }
    }

    public class CreditsPatchInitializer : MonoBehaviour
    {
        private int frameCounter = 0;
        private readonly int framesToWait = 2;
        
        private void Update()
        {
            frameCounter++;
            
            if (frameCounter > framesToWait)
            {
                CreditsPatch.ApplyLocalizationToCredits();
                Destroy(gameObject);
            }
        }
    }
}