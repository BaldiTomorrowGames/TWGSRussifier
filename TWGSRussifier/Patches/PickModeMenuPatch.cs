using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System.Reflection;
using System.Collections;
using TWGSRussifier.API;

namespace TWGSRussifier
{
    internal class PickModeMenuPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("FieldTrips", new Vector2(-120f, 3f)),
            new KeyValuePair<string, Vector2>("Tutorial", new Vector2(120f, 3f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton", new Vector2(-85f, -105f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/YesButton", new Vector2(90f, -105f))
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> OffsetMinTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Endless", new Vector2(-216f, 0f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton", new Vector2(160f, 32f)),
            new KeyValuePair<string, Vector2>("TutorialPrompt/NoButton/Text", new Vector2(155f, 32f))
        };
        
        [HarmonyPatch(typeof(GameLoader), "LoadLevel")]
        private static class LoadLevelPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                ApplyFixesToPickMode();
            }
        }
        
        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "Menu" && value)
                {
                    fixesApplied = false;
                }
                
                if (__instance.name == "PickMode" && value && !fixesApplied)
                {
                    ApplyButtonPositionFixes(__instance.transform);
                    fixesApplied = true;
                }
            }
        }
        
        private static void ApplyFixesToPickMode()
        {
            if (fixesApplied) return;
            
            GameObject pickModeObject = GameObject.Find("PickMode");
            if (pickModeObject != null)
            {
                ApplyButtonPositionFixes(pickModeObject.transform);
                fixesApplied = true;
            }
        }

        private static void ApplyButtonPositionFixes(Transform pickModeTransform)
        {
            pickModeTransform.SetAnchoredPositions(AnchoredPositionTargets);
            pickModeTransform.SetOffsetMins(OffsetMinTargets);
            pickModeTransform.SetSizeDeltas(SizeDeltaTargets);

            foreach (var target in OffsetMinTargets)
            {
                Transform? elementTransform = pickModeTransform.FindTransform(target.Key);
                
                if (elementTransform != null)
                {
                    if (elementTransform.GetComponent<TransformFixator>() == null)
                    {
                        elementTransform.gameObject.AddComponent<TransformFixator>();
                    }
                }
            }
        }
    }
} 