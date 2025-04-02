using HarmonyLib;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(AudioManager), "QueueAudio", new System.Type[] { typeof(SoundObject), typeof(bool) })]
    public static class AudioManagerPatch
    {
        [HarmonyPrefix]
        static bool Prefix(SoundObject file, bool playImmediately)
        {
            if (file?.soundClip?.name == "NoL_Minutes")
            {
                return false;
            }
            return true;
        }
    }
}
