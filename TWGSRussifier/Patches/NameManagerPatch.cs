using TWGSRussifier.Runtime;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NameManager))]
    [HarmonyPatch("Awake")]
    public static class NameManagerPatch
    {
        static void Postfix(NameManager __instance)
        {
            FieldInfo welcome = AccessTools.Field(typeof(NameManager), "audWelcome");
            FieldInfo source = AccessTools.Field(typeof(NameManager), "audSource");
            AudioClip oldClip = (AudioClip)welcome.GetValue(__instance);
            AudioSource audioSource = (AudioSource)source.GetValue(__instance);

            welcome.SetValue(__instance, LanguageManager.instance.GetClip(oldClip.name));
            AudioClip newStartup = LanguageManager.instance.GetClip(audioSource.clip.name);
            if (audioSource.clip.name.Contains("WelcomeClickOn"))
            {
                if (audioSource.clip != newStartup)
                {
                    audioSource.clip = newStartup;
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }
}
