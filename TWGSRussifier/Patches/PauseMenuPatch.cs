using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

namespace TWGSRussifier
{
    internal class PauseMenuPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> localizationKeys = new Dictionary<string, string>()
        {
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/ResumeButton", "TWGS_Pause_ResumeButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/OptionsButton", "TWGS_Pause_OptionsButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/QuitButton", "TWGS_Pause_QuitButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/PauseLabel", "TWGS_Pause_PauseLabel" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/SeedLabel", "TWGS_Pause_SeedButton" }
        };

        [HarmonyPatch(typeof(CoreGameManager), "Start")]
        private static class CoreGameManagerStartPatch 
        {
            [HarmonyPostfix]
            private static void Postfix(CoreGameManager __instance) 
            {
                if (__instance.pauseScreen != null) 
                {
                    __instance.StartCoroutine(InitPauseScreenComponents(__instance));
                }
            }

            private static IEnumerator InitPauseScreenComponents(CoreGameManager manager)
            {
                for (int i = 0; i < 5; i++)
                    yield return null;
                
                bool originalState = manager.pauseScreen.activeSelf;
                
                if (!originalState)
                    manager.pauseScreen.SetActive(true);
                    
                PrepareLocalizationComponents();
                
                if (!originalState)
                    manager.pauseScreen.SetActive(false);
                    
                initialized = true;
            }
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class PauseScreenSetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && __instance.name == "PauseScreen" && !initialized)
                {
                    PrepareLocalizationComponents();
                    initialized = true;
                }
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "Pause")]
        private static class PauseMethodPatch
        {
            [HarmonyPostfix]
            private static void Postfix(bool openScreen, CoreGameManager __instance, bool ___paused)
            {
                if (openScreen && ___paused && !initialized)
                {
                    PrepareLocalizationComponents();
                    initialized = true;
                }
            }
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