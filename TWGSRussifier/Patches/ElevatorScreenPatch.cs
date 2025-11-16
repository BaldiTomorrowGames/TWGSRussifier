using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(ElevatorScreen))]
    internal class ElevatorScreenPatch
    {
        private static bool fixesApplied = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "ResultsText", "TWGS_Elevator_ResultsText" },
            { "TimeText", "TWGS_Elevator_TimeText" },
            { "YTPText", "TWGS_Elevator_YTPText" },
            { "TotalText", "TWGS_Elevator_TotalText" },
            { "StickerText", "TWGS_Elevator_StickerBonuses" },
            { "MultiplierText", "TWGS_Elevator_MultiplierText" },
            { "TimeBonusText", "TWGS_Elevator_TimeBonusText" },
            { "GradeBonusText", "TWGS_Elevator_GradeBonusText" },
            { "TimeBonusValue", "TWGS_Elevator_TimeBonusValue" },
            { "GradeBonusValue", "TWGS_Elevator_GradeBonusValue" }
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("ResultsText", new Vector2(-64f, 104f)),
            // new KeyValuePair<string, Vector2>("TimeText", new Vector2(50f, 4.5f)),
            // new KeyValuePair<string, Vector2>("TimeValue", new Vector2(58f, 4.5f)),
            new KeyValuePair<string, Vector2>("YTPText", new Vector2(50f, 74.5f)),
            new KeyValuePair<string, Vector2>("YTPValue", new Vector2(58f, 74.5f)),
            new KeyValuePair<string, Vector2>("TotalText", new Vector2(52f, -30.5f)),
            new KeyValuePair<string, Vector2>("TotalValue", new Vector2(60f, -30.5f)),
            new KeyValuePair<string, Vector2>("StickerText", new Vector2(52f, 4.5f)),
            new KeyValuePair<string, Vector2>("StickerValue", new Vector2(60f, 4.5f)),
            new KeyValuePair<string, Vector2>("MultiplierText", new Vector2(50f, 39.5f)),
            new KeyValuePair<string, Vector2>("MultiplierValue", new Vector2(55f, 39.5f)),
            new KeyValuePair<string, Vector2>("TimeBonusText", new Vector2(90f, -65f)),
            new KeyValuePair<string, Vector2>("TimeBonusValue", new Vector2(105f, -79f)),
            new KeyValuePair<string, Vector2>("GradeBonusText", new Vector2(80f, -31f)),
            new KeyValuePair<string, Vector2>("GradeBonusValue", new Vector2(105f, -45f))
        };
        
        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("ResultsText", new Vector2(137.01f, 33.45f)),
            new KeyValuePair<string, Vector2>("YTPText", new Vector2(202f, 30f)),
            new KeyValuePair<string, Vector2>("TotalText", new Vector2(202f, 30f)),
            new KeyValuePair<string, Vector2>("StickerText", new Vector2(213f, 30f)),
            new KeyValuePair<string, Vector2>("TimeBonusText", new Vector2(94f, 30f)),
            new KeyValuePair<string, Vector2>("GradeBonusText", new Vector2(103f, 30f))
        };
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void StartPostfix(ElevatorScreen __instance)
        {
            fixesApplied = false;
            
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
        
        private static void ApplyPatchesToBigScreen(ElevatorScreen elevatorScreen)
        {
            if (fixesApplied) return;
            
            BigScreen? bigScreen = elevatorScreen.GetComponentInChildren<BigScreen>(true);
            if (bigScreen == null) return;
            
            ApplyChanges(bigScreen.transform);
            ApplyLocalization(bigScreen.transform);
            
            LocalizeErrorText(elevatorScreen.transform);
            
            fixesApplied = true;
        }
        
        private static void ApplyChanges(Transform bigScreenTransform)
        {
            bigScreenTransform.SetAnchoredPositions(AnchoredPositionTargets);
            bigScreenTransform.SetSizeDeltas(SizeDeltaTargets);
        }
        
        private static void ApplyLocalization(Transform bigScreenTransform)
        {
            var standardKeys = new Dictionary<string, string>();
            var customKeys = new Dictionary<string, string>();

            foreach(var entry in LocalizationKeys)
            {
                if (entry.Key == "TimeBonusValue" || entry.Key == "GradeBonusValue")
                {
                    customKeys.Add(entry.Key, entry.Value);
                }
                else
                {
                    standardKeys.Add(entry.Key, entry.Value);
                }
            }

            bigScreenTransform.ApplyLocalizations(standardKeys);

            foreach (var entry in customKeys)
            {
                string elementName = entry.Key;
                string localizationKey = entry.Value;
                
                Transform? elementTransform = bigScreenTransform.FindTransform(elementName);
                
                if (elementTransform != null)
                {
                    TextMeshProUGUI? textComponent = elementTransform.GetComponent<TextMeshProUGUI>();
                    
                    if (textComponent == null)
                    {
                        textComponent = elementTransform.GetComponentInChildren<TextMeshProUGUI>();
                    }
                    
                    if (textComponent != null)
                    {
                        Component[] components = textComponent.GetComponents<Component>();
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
                }
            }
        }
        
        private static void LocalizeErrorText(Transform elevatorScreenTransform)
        {
            Transform? errorTransform = elevatorScreenTransform.FindTransform("ElevatorTransission/Error");
            if (errorTransform != null)
            {
                TextMeshProUGUI? textComponent = errorTransform.GetComponent<TextMeshProUGUI>();
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
                    
                    TextLocalizer? localizer = textComponent.GetComponent<TextLocalizer>();
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
            public string key = null!;
            public string originalText = null!;
            public string localizedText = null!;
            
            private TextMeshProUGUI textComponent = null!;
            
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
    }
} 