using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;
using System;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    public static class TutorialPatchInitializer
    {
        [HarmonyPrepare]
        private static bool Prepare()
        {
            try
            {
                Version gameVersion = new Version(Application.version);
                Version minimumVersion = new Version("0.9.0");
                
                bool isCompatible = gameVersion >= minimumVersion;
                if (!isCompatible)
                {
                    API.Logger.Info($"TutorialPatch не будет применен: версия игры {Application.version} ниже требуемой 0.9.0");
                    return false;
                }
                
                Type tutorialType = Type.GetType("TutorialGameManager, Assembly-CSharp");
                if (tutorialType == null)
                {
                    tutorialType = AccessTools.TypeByName("TutorialGameManager");
                    if (tutorialType == null)
                    {
                        API.Logger.Warning("Тип TutorialGameManager не найден");
                        return false;
                    }
                }
                
                Harmony harmony = new Harmony("com.baldimods.twgsrussifier.tutorialpatches");
                var original = AccessTools.Method(tutorialType, "BeginPlay");
                var postfix = AccessTools.Method(typeof(TutorialPatchHelper), "Postfix");
                
                if (original != null && postfix != null)
                {
                    harmony.Patch(original, postfix: new HarmonyMethod(postfix));
                    API.Logger.Info("TutorialPatch успешно применен");
                    return true;
                }
                else
                {
                    API.Logger.Warning("Не удалось найти методы для патча TutorialGameManager");
                    return false;
                }
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при инициализации TutorialPatch: {ex.Message}");
                return false;
            }
        }
    }

    public static class TutorialPatchHelper
    {
        public static void Postfix(MonoBehaviour __instance)
        {
            try
            {
                __instance.StartCoroutine(ApplyLocalizationWithDelay());
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в TutorialPatch.Postfix: {ex.Message}");
            }
        }

        private static IEnumerator ApplyLocalizationWithDelay()
        {
            yield return new WaitForSeconds(0.5f);
            
            try
            {
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
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в ApplyLocalizationWithDelay: {ex.Message}");
            }
        }
    }
} 