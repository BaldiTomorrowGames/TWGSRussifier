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
    }
}
