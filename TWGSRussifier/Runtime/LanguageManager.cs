using TWGSRussifier.API;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager instance;

        public Dictionary<string, string> languageData = new Dictionary<string, string>();
        public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        public Dictionary<int, Texture2D> textureAssets = new Dictionary<int, Texture2D>();
        public List<Texture2D> allTextures = new List<Texture2D>();

        private string basePath;
        private string texturesPath;

        public string GetKeyData(string key)
        {
            return languageData.ContainsKey(key) ? languageData[key] : null;
        }

        public AudioClip GetClip(string key)
        {
            return audioClips.ContainsKey(key) ? audioClips[key] : null;
        }

        public Texture2D GetTexture2D(int hashcode)
        {
            return textureAssets.ContainsKey(hashcode) ? textureAssets[hashcode] : null;
        }

        public bool ContainsData(string key)
        {
            return languageData.ContainsKey(key);
        }

        private void UpdateTexts()
        {
            if (languageData == null || languageData.Count == 0) return;

            if (Singleton<LocalizationManager>.Instance != null)
            {
                FieldInfo localText = AccessTools.Field(typeof(LocalizationManager), "localizedText");
                localText.SetValue(Singleton<LocalizationManager>.Instance, languageData);
            }
        }

        public void LateUpdate()
        {
            UpdateTexts();
        }

        public void UpdateAudio()
        {
            SoundObject[] allSounds = Resources.FindObjectsOfTypeAll<SoundObject>();
            foreach (SoundObject soundObject in allSounds)
            {
                API.Logger.Info("Загружен звук: " + soundObject.soundClip.name);
                AudioClip newClip = GetClip(soundObject.soundClip.name);
                RussifierTemp.UpdateClipData(soundObject, newClip);
            }
        }

        public void Start()
        {
            instance = this;
            languageData = new Dictionary<string, string>();
            ModSceneManager.instance.onMenuSceneLoadOnce += UpdateAudio;
            ModSceneManager.instance.onMenuSceneLoadOnce += LoadTextures;
            ModSceneManager.instance.onMenuSceneLoadOnce += ApplyTextures;
            DontDestroyOnLoad(gameObject);

            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            basePath = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID);
            GameUtils.InsertDirectory(basePath);
            LoadLanguageData();
            API.Logger.Info($"Загружены данные языка из {basePath}");
        }

        private void LoadLanguageData()
        {
            string audiosPath = Path.Combine(basePath, "Audios");
            texturesPath = Path.Combine(basePath, "Textures");
            GameUtils.InsertDirectory(audiosPath);
            GameUtils.InsertDirectory(texturesPath);

            API.Logger.Info("Загрузка данных (отсутствие Subtitles_Russian.json приведёт к пустым данным)");
            LoadLanguageAudio(audiosPath);
            LoadLanguageSubtitles(basePath);
        }

        private void LoadLanguageAudio(string audiosPath)
        {
            string[] oggs = Directory.GetFiles(audiosPath, "*.ogg");
            string[] wavs = Directory.GetFiles(audiosPath, "*.wav");
            List<string> customClips = new List<string>();
            customClips.AddRange(oggs);
            customClips.AddRange(wavs);

            foreach (var clip in customClips)
            {
                if (File.Exists(clip))
                {
                    string clipName = Path.GetFileNameWithoutExtension(clip);
                    API.Logger.Info($"Загружен звук: {clipName}!");
                    AudioClip fileClip = AssetLoader.AudioClipFromFile(clip);
                    if (fileClip != null)
                    {
                        audioClips[clipName] = fileClip;
                        API.Logger.Info($"Загружен звук: {clipName}!");
                    }
                }
            }
        }

        private void LoadLanguageSubtitles(string baseFolder)
        {
            string subtitle = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, RussifierTemp.SubtitilesFile);
            if (File.Exists(subtitle))
            {
                string fileData = File.ReadAllText(subtitle);
                LocalizationData rawJson = JsonUtility.FromJson<LocalizationData>(fileData);
                for (int i = 0; i < rawJson.items.Length; i++)
                {
                    languageData[rawJson.items[i].key] = rawJson.items[i].value;
                }
            }
            else
            {
                API.Logger.Error($"{subtitle} не найден!");
            }
        }

        public void LoadTextures()
        {
            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            string[] pngs = Directory.GetFiles(texturesPath, "*.png");

            foreach (var pngPath in pngs)
            {
                string nameToSearch = Path.GetFileNameWithoutExtension(pngPath).Trim();
                Texture2D targetTex = allTextures.FirstOrDefault(x => x.name == nameToSearch);

                if (targetTex == null)
                {
                    API.Logger.Warning($"Не найдена соответствующая текстура для: {nameToSearch}");
                    continue;
                }
                Texture2D generatedTex = AssetLoader.AttemptConvertTo(AssetLoader.TextureFromFile(pngPath), targetTex.format);

                if (!textureAssets.ContainsKey(targetTex.GetHashCode()))
                {
                    textureAssets[targetTex.GetHashCode()] = generatedTex;
                    API.Logger.Info($"Загружена текстура: {targetTex.name}");
                }
                else
                {
                    API.Logger.Warning($"Текстура {targetTex.name} уже загружена.");
                }
            }
        }

        private void ApplyTextures()
        {
            allTextures = Resources.FindObjectsOfTypeAll<Texture2D>().ToList();
            var textureLookup = allTextures.ToDictionary(x => x.GetHashCode(), x => x);

            foreach (var kvp in textureAssets)
            {
                if (textureLookup.TryGetValue(kvp.Key, out Texture2D targetTexture))
                {
                    Graphics.CopyTexture(kvp.Value, targetTexture);
                }
                else
                {
                    API.Logger.Warning($"Не найдена целевая текстура для ключа {kvp.Key}");
                }
            }
            API.Logger.Info($"Копировано {textureAssets.Count} текстур.");
        }

        internal void RegisterClip(string clipName, AudioClip loadedClip)
        {
            throw new NotImplementedException();
        }
    }
}
