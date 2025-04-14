using HarmonyLib;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(AudioManager), "QueueAudio", new System.Type[] { typeof(SoundObject) })]
    public static class AudioManagerPatch
    {
        [HarmonyPrefix]
        static bool Prefix(SoundObject file)
        {
            if (file?.soundClip?.name == "NoL_Minutes")
            {
                return false;
            }
            return true;
        }
    }
}
