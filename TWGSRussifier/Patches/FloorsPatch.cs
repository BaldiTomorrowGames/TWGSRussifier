using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    public class FloorsPatch
    {
        private static bool initialized = false;
        private static readonly Dictionary<string, string> floorTitles = new Dictionary<string, string>
        {
            { "MainLevel_1", "Э1" },
            { "MainLevel_2", "Э2" },
            { "MainLevel_3", "Э3" },
            { "MainLevel_4", "Э4" },
            { "MainLevel_5", "Э5" },
            
            { "PlaceholderEnding", "УРА" },
            { "Pitstop", "ПИТ" },
            { "Tutorial", "ОБЧ" },
            { "EndlessRandomMedium", "БСК" },
            { "EndlessPremadeMedium", "БСК" },
            
            { "Farm", "ФРМ" },
            { "Camping", "ЛГР" },
            
            { "StealthyChallenge", "И1" },
            { "GrappleChallenge", "И3" },
            { "SpeedyChallenge", "И2" }
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
                
                if (floorTitles.TryGetValue(currentScene.name, out string newTitle))
                {
                    currentScene.levelTitle = newTitle;
                }
            }
            
            return true;
        }

        private static void ApplyFloorTitlePatches()
        {
            SceneObject[] allScenes = Resources.FindObjectsOfTypeAll<SceneObject>();
            
            foreach (SceneObject scene in allScenes)
            {
                if (floorTitles.TryGetValue(scene.name, out string newTitle))
                {
                    scene.levelTitle = newTitle;
                }
            }
        }
    }
} 