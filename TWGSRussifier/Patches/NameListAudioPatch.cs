using HarmonyLib;
// using TWGSRussifier.Runtime;
using UnityEngine;
using System.Reflection;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NameManager), "NewFileButtonPressed")]
    public static class NewFileButtonPressedPatch
    {
        static void Postfix(object __instance)
        {
            FieldInfo sourceField = AccessTools.Field(__instance.GetType(), "audSource");
            AudioSource audSource = (AudioSource)sourceField.GetValue(__instance);

            // AudioClip ruClip = LanguageManager.instance.GetClip("BAL_WelcomeTypeIn");
            // if (ruClip != null)
            // {
            //     audSource.clip = ruClip;
            //     if (!audSource.isPlaying)
            //         audSource.Play();
            //         }
            //     {
            // }
        }
    }
}
