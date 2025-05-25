using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace TWGSRussifier
{
    internal static class PicnicPanicInitializer
    {
        private static bool initialized = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            
            try
            {
                Version gameVersion;
                try
                {
                    gameVersion = new Version(Application.version);
                }
                catch
                {
                    API.Logger.Error($"Не удалось распознать версию игры: {Application.version}");
                    return;
                }
                
                Version minimumVersion = new Version("0.7.0");
                
                if (gameVersion < minimumVersion)
                {
                    API.Logger.Info($"PicnicPanicPatch не будет применен: версия игры {Application.version} ниже требуемой 0.7.0");
                    return;
                }
                
                Type minigameType = AccessTools.TypeByName("MinigameBase");
                if (minigameType == null)
                {
                    API.Logger.Warning("PicnicPanicPatch не применен: тип MinigameBase не найден");
                    return;
                }
                
                MethodInfo method = AccessTools.Method(minigameType, "StartMinigame");
                if (method == null)
                {
                    API.Logger.Warning("PicnicPanicPatch не применен: метод StartMinigame не найден в типе MinigameBase");
                    return;
                }
                
                Harmony harmony = new Harmony("com.baldimods.twgsrussifier.picnicpanicpatch");
                harmony.Patch(
                    method,
                    postfix: new HarmonyMethod(typeof(PicnicPanicPatch).GetMethod("StartMinigame_Postfix", BindingFlags.Static | BindingFlags.Public))
                );
                
                API.Logger.Info("PicnicPanicPatch успешно применен");
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при инициализации PicnicPanicPatch: {ex.Message}");
            }
        }
    }

    internal class PicnicPanicPatch
    {
        private static bool patchApplied = false;
        
        public static void StartMinigame_Postfix(MonoBehaviour __instance)
        {
            try
            {
                if (__instance != null && __instance.name.Contains("Picnic"))
                {
                    __instance.StartCoroutine(ApplyPatchWithDelay(__instance));
                }
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в StartMinigame_Postfix: {ex.Message}");
            }
        }
        
        private static IEnumerator ApplyPatchWithDelay(MonoBehaviour minigameBase)
        {
            yield return new WaitForSeconds(0.5f);
            ApplyPatchForScoreIndicator();
            
            yield return new WaitForSeconds(1.0f);
            patchApplied = false; 
            ApplyPatchForScoreIndicator();
            
            yield return new WaitForSeconds(2.0f);
            patchApplied = false;
            ApplyPatchForScoreIndicator();
        }
        
        private static void ApplyPatchForScoreIndicator()
        {
            if (patchApplied)
                return;
                
            try
            {
                GameObject minigameObj = GameObject.Find("Minigame_PicnicPanic(Clone)");
                if (minigameObj == null)
                {
                    GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
                    foreach (GameObject obj in allObjects)
                    {
                        if (obj.name.Contains("Minigame_PicnicPanic"))
                        {
                            minigameObj = obj;
                            break;
                        }
                    }
                    
                    if (minigameObj == null)
                    {
                        return;
                    }
                }
                
                Transform gameCanvas = minigameObj.transform.Find("GameCanvas");
                if (gameCanvas == null)
                {
                    return;
                }
                
                Transform game = gameCanvas.Find("Game");
                if (game == null)
                {
                    return;
                }
                
                Transform minigameHUD = game.Find("MinigameHUD");
                if (minigameHUD == null)
                {
                    return;
                }
                
                Transform scoreIndicatorBase = minigameHUD.Find("ScoreIndicatorBase");
                if (scoreIndicatorBase == null)
                {
                    return;
                }
                
                Transform scoreIndicator = scoreIndicatorBase.Find("ScoreIndicator");
                if (scoreIndicator == null)
                {
                    return;
                }
                
                RectTransform rectTransform = scoreIndicator.GetComponent<RectTransform>();
                if (rectTransform == null)
                {
                    return;
                }
                
                Vector2 newSize = new Vector2(103f, 32f);
                rectTransform.sizeDelta = newSize;
                
                TMP_Text textComponent = scoreIndicator.GetComponent<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.enableAutoSizing = true;
                    textComponent.fontSizeMin = 8f;
                    textComponent.fontSizeMax = 32f;
                }
                
                patchApplied = true;
            }
            catch (System.Exception ex)
            {
                API.Logger.Error($"Ошибка в ApplyPatchForScoreIndicator: {ex.Message}");
            }
        }
    }
} 