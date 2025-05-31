using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;
using TWGSRussifier.Runtime;
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
            // Сразу восстанавливаем озвучку перед всеми остальными операциями
            RestoreAudio(__instance);
            
            // Затем запускаем корутину для работы с текстом
            __instance.StartCoroutine(ApplyLocalizationWithDelay());
        }
        
        // Метод для немедленного восстановления озвучки
        private static void RestoreAudio(TutorialGameManager instance)
        {
            if (ConfigManager.AreSoundsEnabled() && LanguageManager.instance != null)
            {
                // Обновляем все звуки для восстановления озвучки без задержки
                LanguageManager.instance.UpdateAudio();
                API.Logger.Info("Озвучка восстановлена при входе в Туториал (немедленно)");
                
                // Дополнительно запускаем корутину с минимальной задержкой для перестраховки
                instance.StartCoroutine(RestoreAudioAfterMinimalDelay());
            }
        }
        
        // Метод для восстановления озвучки с минимальной задержкой
        private static IEnumerator RestoreAudioAfterMinimalDelay()
        {
            // Используем минимальную задержку, чтобы подождать остальную инициализацию
            yield return new WaitForSeconds(0.1f);
            
            // Восстанавливаем озвучку, если она включена в настройках
            if (ConfigManager.AreSoundsEnabled() && LanguageManager.instance != null)
            {
                // Обновляем все звуки для восстановления озвучки
                LanguageManager.instance.UpdateAudio();
                API.Logger.Info("Озвучка восстановлена при входе в Туториал (с минимальной задержкой)");
            }
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