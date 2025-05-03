using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System; 

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    internal class ElevatorScreenPatch
    {
        private static bool fixesApplied = false;
        private static bool errorLocalizationApplied = false; 
        
        private static readonly Dictionary<string, string> ResultLocalizationKeys = new Dictionary<string, string>()
        {
            { "ResultsText", "TWGS_Elevator_ResultsText" },
            { "TimeText", "TWGS_Elevator_TimeText" },
            { "YTPText", "TWGS_Elevator_YTPText" },
            { "TotalText", "TWGS_Elevator_TotalText" },
            { "GradeText", "TWGS_Elevator_GradeText" },
            { "MultiplierText", "TWGS_Elevator_MultiplierText" },
            { "TimeBonusText", "TWGS_Elevator_TimeBonusText" },
            { "GradeBonusText", "TWGS_Elevator_GradeBonusText" },
            { "TimeBonusValue", "TWGS_Elevator_TimeBonusValue" },
            { "GradeBonusValue", "TWGS_Elevator_GradeBonusValue" }
        };
        
        private static readonly Dictionary<string, string> ErrorLocalizationKeys = new Dictionary<string, string>()
        {
            { "errorText", "TWGS_GeneratorError_Text" }
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("ResultsText", new Vector2(-64f, 104f)),
            new KeyValuePair<string, Vector2>("YTPText", new Vector2(30f, 74.5f)),
            new KeyValuePair<string, Vector2>("YTPValue", new Vector2(38f, 74.5f)),
            new KeyValuePair<string, Vector2>("GradeText", new Vector2(30f, -30.5f)),
            new KeyValuePair<string, Vector2>("MultiplierText", new Vector2(30f, 39.5f)),
            new KeyValuePair<string, Vector2>("MultiplierValue", new Vector2(35f, 39.5f)),
            new KeyValuePair<string, Vector2>("GradeValue", new Vector2(35f, -30.5f)),
            new KeyValuePair<string, Vector2>("TimeBonusText", new Vector2(70f, -65f)),
            new KeyValuePair<string, Vector2>("TimeBonusValue", new Vector2(85f, -79f)),
            new KeyValuePair<string, Vector2>("GradeBonusText", new Vector2(60f, -31f)),
            new KeyValuePair<string, Vector2>("GradeBonusValue", new Vector2(85f, -45f))
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("ResultsText", new Vector2(137.01f, 33.45f)),
            new KeyValuePair<string, Vector2>("YTPText", new Vector2(202f, 30f)),
            new KeyValuePair<string, Vector2>("TimeBonusText", new Vector2(94f, 30f)),
            new KeyValuePair<string, Vector2>("GradeBonusText", new Vector2(103f, 30f))
        };
        
        [HarmonyPatch(typeof(ElevatorScreen), "Start")]
        [HarmonyPostfix]
        static void ElevatorScreenStartPostfix(ElevatorScreen __instance)
        {
            fixesApplied = false;
            errorLocalizationApplied = false;
            
            BigScreen bigScreen = __instance.GetComponentInChildren<BigScreen>(true);

            if (bigScreen != null)
            {
                ApplyPatchesToBigScreen(__instance, bigScreen);
            }
            else
            {
            }
        }
        
        [HarmonyPatch(typeof(ElevatorScreen), "StartGame")]
        [HarmonyPrefix]
        static void ElevatorScreenStartGamePrefix()
        {
            fixesApplied = false;
            errorLocalizationApplied = false;
        }
        
        [HarmonyPatch(typeof(Animator), "Play", new Type[] { typeof(string), typeof(int), typeof(float) })]
        private static class AnimatorPlayPatch
        {
            [HarmonyPostfix]
            private static void Postfix(Animator __instance, string stateName)
            {
                ElevatorScreen elevatorScreen = __instance.GetComponentInParent<ElevatorScreen>();
                
                if (elevatorScreen != null && __instance == AccessTools.Field(typeof(ElevatorScreen), "buttonAnimator").GetValue(elevatorScreen) as Animator)
                {
                    if (stateName == "ButtonRise" && 
                        Singleton<CoreGameManager>.Instance != null && 
                        Singleton<CoreGameManager>.Instance.levelGenError && 
                        !errorLocalizationApplied)
                    {
                        bool isErrorStateActive = (bool)AccessTools.Field(typeof(ElevatorScreen), "error").GetValue(elevatorScreen);
                        
                        ApplyErrorLocalization(elevatorScreen);
                        errorLocalizationApplied = true;
                    }
                }
            }
        }

        private static void ApplyErrorLocalization(ElevatorScreen instance)
        {
            try
            {
                TMP_Text errorText = AccessTools.Field(typeof(ElevatorScreen), "errorText").GetValue(instance) as TMP_Text;

                if (errorText != null)
                {
                     AddOrUpdateLocalizer(errorText.gameObject, ErrorLocalizationKeys["errorText"]);
                }
                else 
                {
                }
            }
            catch (System.Exception ex)
            {
            }
        }
        
        private static Transform FindInChildrenIncludingInactive(Transform parent, string path)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child == parent) continue;
                if (DoesPathMatch(parent, child, path))
                {
                    return child;
                }
            }
            return null;
        }

        private static bool DoesPathMatch(Transform parent, Transform target, string expectedPath)
        {
            if (target == null || parent == null || target == parent) return false;
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;
            while (current != null && current != parent)
            {
                if (pathBuilder.Length > 0)
                    pathBuilder.Insert(0, "/");
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }
            if (current != parent) return false;
            return pathBuilder.ToString() == expectedPath;
        }
        
        private static void ApplyPatchesToBigScreen(ElevatorScreen elevatorScreen, BigScreen bigScreen)
        {
            if (fixesApplied || bigScreen == null) return;
            
            ApplyChanges(bigScreen.transform);
            ApplyResultLocalization(bigScreen.transform);
            
            fixesApplied = true;
        }
        
        private static void ApplyChanges(Transform bigScreenTransform)
        {
            ProcessTargets(bigScreenTransform, AnchoredPositionTargets, (rect, value) => rect.anchoredPosition = value);
            ProcessTargets(bigScreenTransform, SizeDeltaTargets, (rect, value) => rect.sizeDelta = value);
        }

        private static void ProcessTargets(Transform root, List<KeyValuePair<string, Vector2>> targets, System.Action<RectTransform, Vector2> applyAction)
        {
            foreach (var target in targets)
            {
                Transform elementTransform = root.Find(target.Key);
                if (elementTransform == null)
                {
                    elementTransform = FindInChildrenIncludingInactive(root, target.Key);
                }

                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        applyAction(rectTransform, target.Value);
                    }
                }
            }
        }
        
        private static void ApplyResultLocalization(Transform bigScreenTransform)
        {
            foreach (var entry in ResultLocalizationKeys)
            {
                string elementName = entry.Key;
                string localizationKey = entry.Value;
                
                Transform elementTransform = bigScreenTransform.Find(elementName);
                if (elementTransform == null)
                {
                    elementTransform = FindInChildrenIncludingInactive(bigScreenTransform, elementName);
                }
                
                if (elementTransform != null)
                {   
                    AddOrUpdateLocalizer(elementTransform.gameObject, localizationKey);
                }
            }
        }
        
        private static void AddOrUpdateLocalizer(GameObject targetObject, string localizationKey)
        {
             if (targetObject == null) return;

             TextMeshProUGUI textComponent = targetObject.GetComponent<TextMeshProUGUI>();
             if (textComponent != null)
             {
                 TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                 if (localizer == null)
                 {
                     localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                     localizer.key = localizationKey;
                     localizer.RefreshLocalization(); 
                 }
                 else if (localizer.key != localizationKey)
                 {
                     localizer.key = localizationKey;
                     localizer.RefreshLocalization();
                 }
             }
             else
             {
             }
        }
    }
} 