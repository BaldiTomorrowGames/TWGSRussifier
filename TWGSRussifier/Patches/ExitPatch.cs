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
            FieldInfo source = AccessTools.Field(typeof(MainMenu), "audioSource");
            AudioSource audioSource = (AudioSource)source.GetValue(__instance);
            AudioClip rusClip = LanguageManager.instance.GetClip(audioSource.clip.name);
            if (rusClip != null)
            {
                audioSource.clip = rusClip;

            }
            audioSource.Play();
        }
    }
}
