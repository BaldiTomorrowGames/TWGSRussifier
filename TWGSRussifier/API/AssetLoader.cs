using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace TWGSRussifier.API
{
    public static class AssetLoader
    {
        public static Texture2D TextureFromFile(string path, TextureFormat format)
        {
            try
            {
                if (File.Exists(path))
                {
                    byte[] data = File.ReadAllBytes(path);
                    Texture2D tex = new Texture2D(2, 2, format, false);
                    tex.LoadImage(data);
                    tex.name = Path.GetFileNameWithoutExtension(path);
                    return tex;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка загрузки текстуры из {path}: {ex.Message}");
            }
            return null;
        }

        public static Texture2D TextureFromFile(string path)
        {
            return TextureFromFile(path, TextureFormat.RGBA32);
        }

        private static Dictionary<AudioType, string[]> audioExtensions = new Dictionary<AudioType, string[]>
        {
            { AudioType.OGGVORBIS, new string[] { "ogg" } },
            { AudioType.WAV, new string[] { "wav" } },
            { AudioType.MPEG, new string[] { "mp3" } }
        };

        public static AudioType GetAudioType(string path)
        {
            string extension = Path.GetExtension(path).ToLower().Replace(".", "").Trim();

            foreach (var pair in audioExtensions)
            {
                if (Array.IndexOf(pair.Value, extension) >= 0)
                {
                    return pair.Key;
                }
            }

            Logger.Error($"Неподдерживаемый формат аудио: {extension}");
            return AudioType.UNKNOWN;
        }

        private static string[] fallbacks = new string[]
        {
            "",
            "file://",
            "file:///",
            Path.Combine("File:///", ""),
            Path.Combine("File://", "")
        };

        public static AudioClip AudioClipFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Error($"Файл не найден: {path}");
                return null;
            }

            AudioType audioType = GetAudioType(path);
            if (audioType == AudioType.UNKNOWN)
            {
                return null;
            }

            return AudioClipFromFile(path, audioType);
        }

        public static AudioClip AudioClipFromFile(string path, AudioType type)
        {
            try
            {
                AudioClip clip = null;
                UnityWebRequest audioRequest = null;
                
                foreach (string fallback in fallbacks)
                {
                    try
                    {
                        string fullPath = fallback + path;
                        audioRequest = UnityWebRequestMultimedia.GetAudioClip(fullPath, type);
                        audioRequest.SendWebRequest();
                        
                        while (!audioRequest.isDone) { }
                        
                        if (audioRequest.result == UnityWebRequest.Result.Success)
                        {
                            clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                            clip.name = Path.GetFileNameWithoutExtension(path);
                            return clip;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning($"Ошибка при попытке загрузить аудио с префиксом {fallback}: {ex.Message}");
                    }
                    finally
                    {
                        if (audioRequest != null)
                        {
                            audioRequest.Dispose();
                        }
                    }
                }
                
                Logger.Error($"Не удалось загрузить аудио: {path}");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка загрузки аудио из {path}: {ex.Message}");
                return null;
            }
        }

        public static Texture2D AttemptConvertTo(Texture2D toConvert, TextureFormat format)
        {
            if (toConvert == null) return null;
            if (toConvert.format == format) return toConvert;

            try
            {
                Texture2D newTex = new Texture2D(toConvert.width, toConvert.height, format, false);
                Color[] pixels = toConvert.GetPixels();
                newTex.SetPixels(pixels);
                newTex.Apply();
                return newTex;
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка преобразования формата текстуры: {ex.Message}");
                return toConvert;
            }
        }
    }
} 