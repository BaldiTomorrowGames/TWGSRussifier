using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(TutorialGameManager))]
    [HarmonyPatch("BeginPlay")]
    public static class TutorialPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TutorialGameManager __instance)
        {
            __instance.StartCoroutine(ApplyLocalizationWithDelay());
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
                            // Debug.Log("Локализатор применен к тексту в туториале");
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