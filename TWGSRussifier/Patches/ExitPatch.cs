using TWGSRussifier.Runtime;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("Quit")]
    public static class ExitPatch
    {
        static void Postfix(MainMenu __instance)
        {
            try
            {
                if (LanguageManager.instance == null)
                {
                    Debug.Log("Skipping ExitPatch: LanguageManager not initialized");
                    return;
                }
                FieldInfo audManField = AccessTools.Field(typeof(MainMenu), "audMan");
                if (audManField == null)
                {
                    Debug.LogWarning("ExitPatch: audMan field not found in MainMenu class");
                    return;
                }
                
                AudioManager audioManager = (AudioManager)audManField.GetValue(__instance);
                if (audioManager == null)
                {
                    Debug.LogWarning("ExitPatch: audMan is null");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"ExitPatch error: {ex.Message}");
            }
        }
    }
}
