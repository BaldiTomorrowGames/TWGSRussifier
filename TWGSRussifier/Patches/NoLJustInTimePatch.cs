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
        private static AudioClip? loadedClip = null;
        
        private static readonly string[] supportedExtensions = new string[] { "wav", "ogg", "mp3" };

        [HarmonyPostfix]
        public static void Postfix(NoLateTeacher __instance)
        {
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }
            
            TryLoadAudioClip();
            TryReplaceAudio(__instance);
        }

        private static void TryLoadAudioClip()
        {
            if (audioLoaded && loadedClip != null) return;
            
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }

            // string basePath = RussifierTemp.GetAudioPath();
            
            // if (!Directory.Exists(basePath))
            // {
            //     GameUtils.InsertDirectory(basePath);
            //     API.Logger.Warning($"Директория для аудио не найдена, создана: {basePath}");
            //     return;
            // }
            
            // foreach (string ext in supportedExtensions)
            // {
            //     string fullPath = Path.Combine(basePath, $"{audioFileName}.{ext}");
                
            //     if (File.Exists(fullPath))
            //     {
            //         loadedClip = AssetLoader.AudioClipFromFile(fullPath);
                    
            //         if (loadedClip != null)
            //         {
            //             audioLoaded = true;
            //             API.Logger.Info($"Успешно загружен аудиоклип {audioFileName} из пути: {fullPath}");
            //             return;
            //         }
            //     }
            // }
            
            // API.Logger.Warning($"Не удалось найти аудиоклип {audioFileName} в {basePath}");
        }

        private static void TryReplaceAudio(NoLateTeacher teacher)
        {
            if (!audioLoaded || loadedClip == null) return;
            
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }

            try
            {
                string[] possibleFieldNames = new string[] { "audInTime", "audJustInTime" };
                
                foreach (string fieldName in possibleFieldNames)
                {
                    FieldInfo? field = AccessTools.Field(typeof(NoLateTeacher), fieldName);
                    if (field != null)
                    {
                        SoundObject? soundObj = field.GetValue(teacher) as SoundObject;
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
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при замене аудио Just In Time: {ex.Message}");
            }
        }
        
        [HarmonyPatch(typeof(NoLateTeacher), "InTime")]
        [HarmonyPrefix]
        public static void BeforeInTime(NoLateTeacher __instance)
        {
            if (!ConfigManager.AreSoundsEnabled())
            {
                return;
            }
            
            if (!audioLoaded || loadedClip == null) TryLoadAudioClip();
            if (audioLoaded && loadedClip != null)
            {
                FieldInfo? field = AccessTools.Field(typeof(NoLateTeacher), "audInTime");
                if (field != null)
                {
                    SoundObject? oldSound = field.GetValue(__instance) as SoundObject;
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