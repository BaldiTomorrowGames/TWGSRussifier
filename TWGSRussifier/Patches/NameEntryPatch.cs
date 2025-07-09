using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier
{
    internal class NameEntryPatch
    {
        private static bool buttonFixesApplied = false;
        private static bool localizationApplied = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "SaveError/Text (TMP)", "TWGS_Menu_SaveErrorText" },
            { "Version Number", "TWGS_Menu_Version" }
        };

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "NameEntry")
                {
                     if (value)
                     {
                          if(!localizationApplied)
                          {
                              ApplyLocalization(__instance.transform);
                              localizationApplied = true;
                          }
                     }
                     else
                     {
                         localizationApplied = false;
                         buttonFixesApplied = false; 
                     }
                }
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Awake")]
        private static class NameManagerAwakePatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                buttonFixesApplied = false;
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Load")]
        private static class LoadPatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                buttonFixesApplied = false; 
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        private static void ApplyNewFileButtonFixes(NameManager nameManager)
        {
            if (nameManager == null || nameManager.newFileButton == null || buttonFixesApplied) return;
            
            Transform buttonTransform = nameManager.newFileButton.transform;
            if (buttonTransform != null)
            {
                buttonTransform.SetSizeDeltas(new[] { new KeyValuePair<string, Vector2>("", new Vector2(158f, 30f)) });
                buttonTransform.SetAnchoredPositions(new[] { new KeyValuePair<string, Vector2>("Text (TMP)", new Vector2(1f, 0f)) });
                buttonTransform.SetSizeDeltas(new[] { new KeyValuePair<string, Vector2>("Text (TMP)", new Vector2(150f, 32f)) });
                
                buttonFixesApplied = true;
            }
        }
        private static void ApplyLocalization(Transform rootTransform)
        {
            rootTransform.ApplyLocalizations(LocalizationKeys, true);
        }
    }
} 