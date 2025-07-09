using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using TWGSRussifier.API;

namespace TWGSRussifier
{
    internal class PicnicPanicPatch
    {
        private static bool patchApplied = false;

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("GameCanvas/Game/MinigameHUD/ScoreIndicatorBase/ScoreIndicator", new Vector2(103f, 32f))
        };
        
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
                
                minigameObj.transform.SetSizeDeltas(SizeDeltaTargets);
                
                Transform? scoreIndicator = minigameObj.transform.FindTransform("GameCanvas/Game/MinigameHUD/ScoreIndicatorBase/ScoreIndicator");
                if (scoreIndicator == null)
                {
                    return;
                }
                
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