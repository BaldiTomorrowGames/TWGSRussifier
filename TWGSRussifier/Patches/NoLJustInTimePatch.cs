using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Reflection;
using TWGSRussifier.API;
using System;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NoLateTeacher), "Initialize")]
    public class NoLJustInTimePatch
    {
        private static readonly string audioFileName = "NoL_JustInTime";
        private static bool audioLoaded = false;
        private static AudioClip loadedClip = null;
        
        private static readonly string[] supportedExtensions = new string[] { "wav", "ogg", "mp3" };

        [HarmonyPostfix]
        public static void Postfix(NoLateTeacher __instance)
        {
            TryLoadAudioClip();
            TryReplaceAudio(__instance);
        }

        private static void TryLoadAudioClip()
        {
            if (audioLoaded && loadedClip != null) return;

            string[] basePaths = new string[]
            {
                Path.Combine(Application.streamingAssetsPath, "Mods", RussifierTemp.ModGUID, "Audios"),
                Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "Audios"),
                Path.Combine(BepInEx.Paths.PluginPath, RussifierTemp.ModGUID, "Audios"),
                Path.Combine(Application.dataPath, "..", "Mods", RussifierTemp.ModGUID, "Audios")
            };

            foreach (string basePath in basePaths)
            {
                foreach (string ext in supportedExtensions)
                {
                    string fullPath = Path.Combine(basePath, $"{audioFileName}.{ext}");
                    
                    if (File.Exists(fullPath))
                    {
                        loadedClip = AssetLoader.AudioClipFromFile(fullPath);
                        
                        if (loadedClip != null)
                        {
                            audioLoaded = true;
                            return;
                        }
                    }
                }
            }
        }

        private static void TryReplaceAudio(NoLateTeacher teacher)
        {
            if (!audioLoaded || loadedClip == null) return;

            try
            {
                string[] possibleFieldNames = new string[] { "audInTime", "audJustInTime" };
                
                foreach (string fieldName in possibleFieldNames)
                {
                    FieldInfo field = AccessTools.Field(typeof(NoLateTeacher), fieldName);
                    if (field != null)
                    {
                        SoundObject soundObj = field.GetValue(teacher) as SoundObject;
                        if (soundObj != null)
                        {
                            SoundObject newObj = ScriptableObject.CreateInstance<SoundObject>();
                            newObj.name = soundObj.name;
                            foreach (FieldInfo propField in typeof(SoundObject).GetFields())
                            {
                                if (propField.Name != "soundClip")
                                {
                                    propField.SetValue(newObj, propField.GetValue(soundObj));
                                }
                            }
                            newObj.soundClip = loadedClip;
                            
                            field.SetValue(teacher, newObj);
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        
        [HarmonyPatch(typeof(NoLateTeacher), "InTime")]
        [HarmonyPrefix]
        public static void BeforeInTime(NoLateTeacher __instance)
        {
            if (!audioLoaded || loadedClip == null) TryLoadAudioClip();
            if (audioLoaded && loadedClip != null)
            {
                FieldInfo field = AccessTools.Field(typeof(NoLateTeacher), "audInTime");
                if (field != null)
                {
                    SoundObject oldSound = field.GetValue(__instance) as SoundObject;
                    if (oldSound != null)
                    {
                        SoundObject newSound = ScriptableObject.CreateInstance<SoundObject>();
                        newSound.name = oldSound.name;
                        foreach (FieldInfo propField in typeof(SoundObject).GetFields())
                        {
                            if (propField.Name != "soundClip")
                            {
                                propField.SetValue(newSound, propField.GetValue(oldSound));
                            }
                        }
                        newSound.soundClip = loadedClip;
                        
                        field.SetValue(__instance, newSound);
                    }
                }
            }
        }
    }
} 