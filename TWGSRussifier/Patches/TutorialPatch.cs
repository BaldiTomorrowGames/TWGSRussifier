using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;
// using TWGSRussifier.Runtime;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(TutorialGameManager))]
    [HarmonyPatch("BeginPlay")]
    public static class TutorialPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TutorialGameManager __instance)
        {
            // RestoreAudio(__instance);
            __instance.StartCoroutine(ApplyLocalizationWithDelay());
        }
        
        private static void RestoreAudio(TutorialGameManager instance)
        {
            // if (ConfigManager.AreSoundsEnabled() && LanguageManager.instance != null)
            // {
            //     LanguageManager.instance.UpdateAudio();
                
            //     instance.StartCoroutine(RestoreAudioAfterMinimalDelay());
            // }
        }
        
        private static IEnumerator RestoreAudioAfterMinimalDelay()
        {
            yield return new WaitForSeconds(0.1f);
            
            // if (ConfigManager.AreSoundsEnabled() && LanguageManager.instance != null)
            // {
            //     LanguageManager.instance.UpdateAudio();
            // }
        }

        private static IEnumerator ApplyLocalizationWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            
            GameObject tutorialManager = GameObject.Find("TutorialGameManager(Clone)");
            if (tutorialManager != null)
            {
                Transform textTransform = tutorialManager.transform.Find("DefaultCanvas/Text");
                if (textTransform != null)
                {
                    TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        TextLocalizer existingLocalizer = textTransform.GetComponent<TextLocalizer>();
                        if (existingLocalizer == null)
                        {
                            TextLocalizer localizer = textTransform.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = "TWGS_Tutorial_DefaultCanvas";
                        }
                    }
                    else
                    {
                        // Debug.LogWarning("TextMeshProUGUI компонент не найден на тексте в туториале");
                    }
                }
                else
                {
                    // Debug.LogWarning("Текст в туториале не найден по ожидаемому пути DefaultCanvas/Text");
                }
            }
            else
            {
                // Debug.LogWarning("TutorialGameManager(Clone) не найден");
            }
        }
    }
} 