using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using TWGSRussifier.Runtime;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    public class FloorsPatch
    {
        private static bool initialized = false;
        private static readonly Dictionary<string, string> floorLocalizationKeys = new Dictionary<string, string>
        {
            { "MainLevel_1", "TWGS_Floor_Level1" },
            { "MainLevel_2", "TWGS_Floor_Level2" },
            { "MainLevel_3", "TWGS_Floor_Level3" },
            { "MainLevel_4", "TWGS_Floor_Level4" },
            { "MainLevel_5", "TWGS_Floor_Level5" },
            
            { "PlaceholderEnding", "TWGS_Floor_Ending" },
            { "Pitstop", "TWGS_Floor_Pitstop" },
            { "Tutorial", "TWGS_Floor_Tutorial" },
            { "EndlessRandomMedium", "TWGS_Floor_EndlessRandom" },
            { "EndlessPremadeMedium", "TWGS_Floor_EndlessPremade" },
            
            { "StealthyChallenge", "TWGS_Floor_StealthyChallenge" },
            { "GrappleChallenge", "TWGS_Floor_GrappleChallenge" },
            { "SpeedyChallenge", "TWGS_Floor_SpeedyChallenge" }
        };

        [HarmonyPatch(typeof(MenuInitializer), "Start")]
        [HarmonyPostfix]
        private static void MenuInitializerStartPostfix()
        {
            if (!initialized)
            {
                ApplyFloorTitlePatches();
                initialized = true;
            }
        }
        
        [HarmonyPatch(typeof(ElevatorScreen), "UpdateFloorDisplay")]
        [HarmonyPrefix]
        private static bool ElevatorScreenUpdateFloorDisplayPrefix(ElevatorScreen __instance)
        {
            if (Singleton<CoreGameManager>.Instance != null && 
                Singleton<CoreGameManager>.Instance.sceneObject != null)
            {
                SceneObject currentScene = Singleton<CoreGameManager>.Instance.sceneObject;
                
                UpdateFloorTitle(currentScene);
            }
            
            return true;
        }

        private static void ApplyFloorTitlePatches()
        {
            SceneObject[] allScenes = Resources.FindObjectsOfTypeAll<SceneObject>();
            
            foreach (SceneObject scene in allScenes)
            {
                UpdateFloorTitle(scene);
            }
        }
        
        private static void UpdateFloorTitle(SceneObject scene)
        {
            if (floorLocalizationKeys.TryGetValue(scene.name, out string localizationKey))
            {
                string localizedTitle = GetLocalizedFloorTitle(localizationKey);
                if (!string.IsNullOrEmpty(localizedTitle))
                {
                    scene.levelTitle = localizedTitle;
                }
            }
        }
        
        private static string GetLocalizedFloorTitle(string localizationKey)
        {
            if (LanguageManager.instance != null && LanguageManager.instance.ContainsData(localizationKey))
            {
                string localizedTitle = LanguageManager.instance.GetKeyData(localizationKey);
                if (!string.IsNullOrEmpty(localizedTitle))
                {
                    return localizedTitle;
                }
            }
          
            return string.Empty;
        }
    }
} 