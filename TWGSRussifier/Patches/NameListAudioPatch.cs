using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using System.Reflection;
using TWGSRussifier;
using TWGSRussifier.API;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NameManager), "NewFileButtonPressed")]
    public static class NewFileButtonPressedPatch
    {
        static void Postfix(NameManager __instance)
        {
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }

            FieldInfo sourceField = AccessTools.Field(typeof(NameManager), "audSource");
            AudioSource audSource = (AudioSource)sourceField.GetValue(__instance);

            AudioClip ruClip = AssetLoader.AudioClipFromMod(TPPlugin.Instance, new string[] { "Audios", "BAL_WelcomeTypeIn.wav" });

            if (ruClip != null)
            {
                audSource.clip = ruClip;
                if (!audSource.isPlaying)
                {
                    audSource.Play();
                }
            }
        }
    }
}
