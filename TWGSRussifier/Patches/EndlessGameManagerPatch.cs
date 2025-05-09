using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(EndlessGameManager))]
    internal class EndlessGameManagerPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> localizationKeys = new Dictionary<string, string>()
        {
            { "EndlessGameManager(Clone)/Score/Text", "TWGS_Endless_Text" },
            { "EndlessGameManager(Clone)/Score/Congrats", "TWGS_Endless_Score" },
            { "Medium_EndlessGameManager(Clone)/Score/Text", "TWGS_Endless_Text" },
            { "Medium_EndlessGameManager(Clone)/Score/Congrats", "TWGS_Endless_Score" }
        };

        [HarmonyPatch("RestartLevel")]
        [HarmonyPostfix]
        private static void RestartLevelPostfix(EndlessGameManager __instance)
        {
            if (!initialized)
            {
                __instance.StartCoroutine(InitEndlessScreenComponents());
                initialized = true;
            }
        }

        private static IEnumerator InitEndlessScreenComponents()
        {
            for (int i = 0; i < 3; i++)
                yield return null;
                
            PrepareLocalizationComponents();
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        [HarmonyPrefix]
        private static void ResetInitialization()
        {
            initialized = false;
        }

        private static void PrepareLocalizationComponents()
        {
            foreach (KeyValuePair<string, string> pair in localizationKeys)
            {
                GameObject menuItem = GameObject.Find(pair.Key);
                if (menuItem != null)
                {
                    TextMeshProUGUI textComponent = menuItem.GetComponent<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        string originalText = textComponent.text;
                        
                        Component[] components = menuItem.GetComponents<Component>();
                        foreach (Component component in components)
                        {
                            if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                            {
                                Object.Destroy(component);
                            }
                        }
                        TextLocalizer localizer = menuItem.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = menuItem.AddComponent<TextLocalizer>();
                            localizer.key = pair.Value;
                            
                            localizer.RefreshLocalization();
                        }
                        else
                        {
                            localizer.key = pair.Value;
                            localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }
    }
} 