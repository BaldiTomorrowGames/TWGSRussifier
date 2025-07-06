using HarmonyLib;
using MTM101BaldAPI.AssetTools;
using System.Reflection;
using TWGSRussifier;
using TWGSRussifier.API;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NoLateTeacher), "Initialize")]
    public static class NoLJustInTimePatch
    {
        [HarmonyPostfix]
        public static void Postfix(NoLateTeacher __instance)
        {
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }

            AudioClip newClip = AssetLoader.AudioClipFromMod(TPPlugin.Instance, new string[] { "Audios", "NoL_JustInTime.wav" });

            if (newClip == null)
            {
                return;
            }

            string[] possibleFieldNames = { "audInTime", "audJustInTime" };
            
            foreach (string fieldName in possibleFieldNames)
            {
                FieldInfo field = AccessTools.Field(typeof(NoLateTeacher), fieldName);
                if (field != null)
                {
                    if (field.GetValue(__instance) is SoundObject originalSound)
                    {
                        SoundObject newSound = Object.Instantiate(originalSound);
                        newSound.soundClip = newClip;
                        field.SetValue(__instance, newSound);
                    }
                }
            }
        }
    }
} 