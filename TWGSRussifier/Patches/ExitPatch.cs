// using TWGSRussifier.Runtime;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using TWGSRussifier;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(MainMenu))]
    [HarmonyPatch("Quit")]
    public static class ExitPatch
    {
        static void Postfix(MainMenu __instance)
        {
            FieldInfo source = AccessTools.Field(typeof(MainMenu), "audioSource");
            AudioSource audioSource = (AudioSource)source.GetValue(__instance);
            
            if (audioSource.clip != null && TPPlugin.AllClips.TryGetValue(audioSource.clip.name, out AudioClip rusClip))
            {
                audioSource.clip = rusClip;
            }
            audioSource.Play();
        }
    }
}
