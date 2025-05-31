using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TWGSRussifier.Runtime;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorScreenPatch
    {
        private static bool fixesApplied = false;
        private static bool audioRestoredOnGameStart = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
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
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPostfix(ElevatorScreen __instance)
        {
            fixesApplied = false;
            
            if (!audioRestoredOnGameStart)
            {
                RestoreAudio();
                audioRestoredOnGameStart = true;
                API.Logger.Info("Озвучка восстановлена при первом запуске игры");
            }
            
            __instance.OnLoadReady += () => {
                ApplyPatchesToBigScreen(__instance);
            };
        }
        
        [HarmonyPatch("StartGame")]
        [HarmonyPrefix]
        static void StartGamePrefix()
        {
            fixesApplied = false;
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
        
        private static void ApplyPatchesToBigScreen(ElevatorScreen elevatorScreen)
        {
            if (fixesApplied) return;
            
            BigScreen bigScreen = elevatorScreen.GetComponentInChildren<BigScreen>(true);
            if (bigScreen == null) return;
            
            ApplyChanges(bigScreen.transform);
            ApplyLocalization(bigScreen.transform);
            
            LocalizeErrorText(elevatorScreen.transform);
            
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
        
        private static void ApplyLocalization(Transform bigScreenTransform)
        {
            foreach (var entry in LocalizationKeys)
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
                    TextMeshProUGUI textComponent = elementTransform.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        if (elementName == "TimeBonusValue" || elementName == "GradeBonusValue")
                        {
                            string originalText = textComponent.text;
                            
                            Component[] components = elementTransform.GetComponents<Component>();
                            foreach (Component component in components)
                            {
                                if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                                {
                                    Object.Destroy(component);
                                }
                            }
                            
                            CustomTextLocalizer customLocalizer = textComponent.gameObject.AddComponent<CustomTextLocalizer>();
                            customLocalizer.key = localizationKey;
                            customLocalizer.originalText = "YTPs";
                            customLocalizer.localizedText = "ОТМ";
                        }
                        else
                        {
                            Component[] components = elementTransform.GetComponents<Component>();
                            foreach (Component component in components)
                            {
                                if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                                {
                                    Object.Destroy(component);
                                }
                            }
                            
                            TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                            if (localizer == null)
                            {
                                localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                                localizer.key = localizationKey;
                            }
                            else if (localizer.key != localizationKey)
                            {
                                localizer.key = localizationKey;
                                localizer.RefreshLocalization();
                            }
                        }
                    }
                }
            }
        }
        
        private static void LocalizeErrorText(Transform elevatorScreenTransform)
        {
            Transform errorTransform = FindInChildrenIncludingInactive(elevatorScreenTransform, "ElevatorTransission/Error");
            if (errorTransform != null)
            {
                TextMeshProUGUI textComponent = errorTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    Component[] components = errorTransform.GetComponents<Component>();
                    foreach (Component component in components)
                    {
                        if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                        {
                            Object.Destroy(component);
                        }
                    }
                    
                    TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                    if (localizer == null)
                    {
                        localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                        localizer.key = "TWGS_GeneratorError_Text";
                    }
                    else if (localizer.key != "TWGS_GeneratorError_Text")
                    {
                        localizer.key = "TWGS_GeneratorError_Text";
                        localizer.RefreshLocalization();
                    }
                }
            }
        }
        
        private class CustomTextLocalizer : MonoBehaviour
        {
            public string key;
            public string originalText;
            public string localizedText;
            
            private TextMeshProUGUI textComponent;
            
            private void Awake()
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }
            
            private void Start()
            {
                if (textComponent != null)
                {
                    ReplaceText();
                }
            }
            
            private void Update()
            {
                if (textComponent != null && textComponent.text.Contains(originalText))
                {
                    ReplaceText();
                }
            }
            
            private void ReplaceText()
            {
                if (!string.IsNullOrEmpty(textComponent.text))
                {
                    textComponent.text = textComponent.text.Replace(originalText, localizedText);
                }
            }
        }
        
        private static void RestoreAudio()
        {
            if (ConfigManager.AreSoundsEnabled() && LanguageManager.instance != null)
            {
                LanguageManager.instance.UpdateAudio();
            }
        }
    }
} 