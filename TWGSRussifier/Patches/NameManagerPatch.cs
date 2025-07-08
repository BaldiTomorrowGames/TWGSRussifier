using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using System.Reflection;
using TWGSRussifier;
using TWGSRussifier.API;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NameManager))]
    [HarmonyPatch("Awake")]
    public static class NameManagerPatch
    {
        static void Postfix(NameManager __instance)
        {
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }
            
            FieldInfo welcome = AccessTools.Field(typeof(NameManager), "audWelcome");
            FieldInfo source = AccessTools.Field(typeof(NameManager), "audSource");
            AudioClip oldClip = (AudioClip)welcome.GetValue(__instance);
            AudioSource audioSource = (AudioSource)source.GetValue(__instance);

            AudioClip newWelcome = AssetLoader.AudioClipFromMod(TPPlugin.Instance, new string[] { "Audios", oldClip.name + ".wav" });
            if (newWelcome != null)
            {
                welcome.SetValue(__instance, newWelcome);
            }

            if (audioSource.clip != null && audioSource.clip.name.Contains("WelcomeClickOn"))
            {
                AudioClip newStartup = AssetLoader.AudioClipFromMod(TPPlugin.Instance, new string[] { "Audios", audioSource.clip.name + ".wav" });
                if (newStartup != null)
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
}
