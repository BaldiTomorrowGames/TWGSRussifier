using HarmonyLib;
using UnityEngine;
using System.Linq;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(AudioManager), "QueueAudio", new System.Type[] { typeof(SoundObject), typeof(bool) })]
    public static class AllNotebooksPatch
    {
        [HarmonyPrefix]
        static void Prefix(ref SoundObject file, bool playImmediately)
        {
            if (file == null || file.soundKey != "Vfx_BAL_AllNotebooks_1")
            {
                return;
            }

            file = Object.Instantiate(file);

            bool needsSpecialEnding = Singleton<BaseGameManager>.Instance.NotebookTotal == 4;
            string targetKey = needsSpecialEnding ? "Vfx_BAL_AllNotebooks_2_Additional" : "Vfx_BAL_AllNotebooks_2";
            string oldKey = needsSpecialEnding ? "Vfx_BAL_AllNotebooks_2" : "Vfx_BAL_AllNotebooks_2_Additional";

            var keyToChange = file.additionalKeys.FirstOrDefault(k => k.key == oldKey);
            if (keyToChange != null)
            {
                keyToChange.key = targetKey;
            }
        }
    }
} 