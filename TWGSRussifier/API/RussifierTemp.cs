using UnityEngine;
using System.IO;
using BepInEx;

namespace TWGSRussifier.API
{
    public static class RussifierTemp
    {
        public const string ModGUID = "twgs.plus.russifier";
        public const string ModName = "Baldi's Basics Plus Russifier";
        public const string ModVersion = "1.0.2.1";

        public static string OverwritesFile = "Overwrites.json";
        public static string PostersFile = "PosterSettings.json";
        public static string SubtitilesFile = "Subtitles_Russian.json";

        public static string GetBasePath()
        {
            return Path.Combine(Paths.PluginPath, "TWGSRussifier", ModGUID);
        }

        public static string GetAudioPath()
        {
            return Path.Combine(GetBasePath(), "Audios");
        }

        public static string GetTexturePath()
        {
            return Path.Combine(GetBasePath(), "Textures");
        }

        public static string GetPostersPath()
        {
            return Path.Combine(GetBasePath(), "PosterFiles");
        }

        public static void UpdateClipData(SoundObject obj, AudioClip newClip)
        {
            if (obj != null && newClip != null)
            {
                if (obj.name == newClip.name)
                {
                    obj.soundClip = newClip;
                }
            }
        }
    }
}
