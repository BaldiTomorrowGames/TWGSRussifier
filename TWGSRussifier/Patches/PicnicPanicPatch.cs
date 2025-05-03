using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace TWGSRussifier
{
    internal class PicnicPanicPatch
    {
        private static bool patchApplied = false;
        
        [HarmonyPatch(typeof(MinigameBase), "StartMinigame")]
        private static class StartMinigamePatch
        {
            static void Postfix(MinigameBase __instance)
            {
                if (__instance != null && __instance.name.Contains("Picnic"))
                {
                    __instance.StartCoroutine(ApplyPatchWithDelay(__instance));
                }
            }
        }
        
        private static IEnumerator ApplyPatchWithDelay(MinigameBase minigameBase)
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
                    GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
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
            catch (System.Exception)
            {
            }
        }
    }
} 