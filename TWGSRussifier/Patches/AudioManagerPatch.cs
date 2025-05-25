using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    internal static class AudioManagerPatch
    {
        private static bool initialized = false;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            
            try
            {
                Version gameVersion;
                try
                {
                    gameVersion = new Version(Application.version);
                }
                catch
                {
                    API.Logger.Error($"Не удалось распознать версию игры: {Application.version}");
                    return;
                }
                
                Version minimumVersion = new Version("0.4.0");
                
                if (gameVersion < minimumVersion)
                {
                    API.Logger.Info($"AudioManagerPatch не будет применен: версия игры {Application.version} ниже требуемой 0.4.0");
                    return;
                }

                Type audioManagerType = AccessTools.TypeByName("AudioManager");
                if (audioManagerType == null)
                {
                    API.Logger.Warning("Тип AudioManager не найден");
                    return;
                }

                Type soundObjectType = AccessTools.TypeByName("SoundObject");
                if (soundObjectType == null)
                {
                    API.Logger.Warning("Тип SoundObject не найден");
                    return;
                }

                MethodInfo queueAudioMethod = AccessTools.Method(audioManagerType, "QueueAudio", 
                    new Type[] { soundObjectType, typeof(bool) });
                
                if (queueAudioMethod == null)
                {
                    API.Logger.Warning("Метод QueueAudio не найден в типе AudioManager");
                    return;
                }
                
                Harmony harmony = new Harmony("com.baldimods.twgsrussifier.audiomanagerpatch");
                harmony.Patch(
                    queueAudioMethod,
                    prefix: new HarmonyMethod(typeof(AudioManagerPatch).GetMethod("QueueAudio_Prefix", 
                        BindingFlags.Static | BindingFlags.NonPublic))
                );
                
                API.Logger.Info("AudioManagerPatch успешно применен");
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при инициализации AudioManagerPatch: {ex.Message}");
            }
        }
        

        private static bool QueueAudio_Prefix(object file, bool playImmediately)
        {
            try
            {
                if (file == null) return true;
                
                object soundClip = AccessTools.Property(file.GetType(), "soundClip")?.GetValue(file);
                if (soundClip == null) return true;
                
                string name = AccessTools.Property(soundClip.GetType(), "name")?.GetValue(soundClip) as string;
                
                if (name == "NoL_Minutes")
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в QueueAudio_Prefix: {ex.Message}");
            }
            
            return true;
        }
    }
}
