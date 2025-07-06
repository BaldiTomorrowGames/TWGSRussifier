using BepInEx;
using TWGSRussifier.API;
using TWGSRussifier.Core;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.AssetTools;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;

namespace TWGSRussifier
{
    [Serializable]
    public class PosterTextTable
    {
        public List<PosterTextData> items = new List<PosterTextData>();
    }

    [BepInPlugin(RussifierTemp.ModGUID, RussifierTemp.ModName, RussifierTemp.ModVersion)]
    [BepInProcess("BALDI.exe")]
    public class TPPlugin : BaseUnityPlugin
    {
        public static TPPlugin Instance { get; private set; } = null!;
        private Harmony harmonyInstance = null!;
        private const string expectedGameVersion = "0.11";

        private static readonly string[] menuTextureNames =
        {
            "About_Lit", "About_Unlit",
            "Options_Lit", "Options_Unlit",
            "Play_Lit", "Play_Unlit",
            "TempMenu_Low"
        };

        void Awake()
        {
            Instance = this;
            API.Logger.Init(Logger);
            ConfigManager.Initialize(this, Logger);

            API.Logger.Info($"Плагин {RussifierTemp.ModName} инициализирован.");
            API.Logger.Info($"Текстуры: {(ConfigManager.AreTexturesEnabled() ? "Включены" : "Отключены")}, " +
                           $"Звуки: {(ConfigManager.AreSoundsEnabled() ? "Включены" : "Отключены")}, " +
                           $"Логирование: {(ConfigManager.IsLoggingEnabled() ? "Включено" : "Отключено")}");

            harmonyInstance = new Harmony(RussifierTemp.ModGUID);
            harmonyInstance.PatchAll();

            VersionCheck.CheckGameVersion(expectedGameVersion, Info);

            LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetsLoaded(), false);
            
            gameObject.AddComponent<MenuTextureManager>();
        }

        private IEnumerator OnAssetsLoaded()
        {
            yield return 4; // Общее количество шагов загрузки

            yield return "Загрузка ресурсов русификатора..."; // Начальный текст
            API.Logger.Info("Загрузка русифицированных ассетов...");

            string modPath = AssetLoader.GetModPath(this);

            // Шаг 1: Загрузка локализации
            yield return "Загрузка локализации...";
            string langPath = Path.Combine(modPath, "Language", "Russian");
            if (Directory.Exists(langPath))
            {
                API.Logger.Info($"Обнаружена папка локализации: {langPath}");
                AssetLoader.LoadLocalizationFolder(langPath, Language.English);
            }

            // Шаг 2: Загрузка и замена текстур
            yield return "Загрузка текстур...";
            ApplyAllTextures();

            // Шаг 3: Загрузка и замена звуков
            yield return "Загрузка звуков...";
            if (ConfigManager.AreSoundsEnabled())
            {
                string audiosPath = Path.Combine(modPath, "Audios");
                if (Directory.Exists(audiosPath))
                {
                    API.Logger.Info($"Обнаружена папка со звуками: {audiosPath}, производится замена...");
                    SoundObject[] allSounds = Resources.FindObjectsOfTypeAll<SoundObject>();
                    foreach (SoundObject soundObject in allSounds)
                    {
                        string clipPath = Path.Combine(audiosPath, soundObject.name + ".wav"); // Предполагаем wav, можно добавить и ogg
                        if (!File.Exists(clipPath))
                        {
                            clipPath = Path.Combine(audiosPath, soundObject.name + ".ogg");
                        }

                        if (File.Exists(clipPath))
                        {
                            AudioClip newClip = AssetLoader.AudioClipFromFile(clipPath);
                            if (newClip)
                            {
                                soundObject.soundClip = newClip;
                                API.Logger.Info($"Звук '{soundObject.name}' заменен.");
                            }
                        }
                    }
                }
            }

            // Шаг 4: Обновление постеров
            yield return "Обновление плакатов...";
            UpdatePosters(modPath);

            API.Logger.Info("Загрузка ассетов завершена!");
        }

        public void ApplyMenuTextures()
        {
            if (!ConfigManager.AreTexturesEnabled()) return;

            string modPath = AssetLoader.GetModPath(this);
            string texturesPath = Path.Combine(modPath, "Textures");

            if (Directory.Exists(texturesPath))
            {
                API.Logger.Info("Применение текстур главного меню...");
                Texture2D[] allGameTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                foreach (string textureName in menuTextureNames)
                {
                    Texture2D originalTexture = allGameTextures.FirstOrDefault(t => t.name == textureName);
                    if (originalTexture != null)
                    {
                        string textureFile = Path.Combine(texturesPath, textureName + ".png");
                        if (File.Exists(textureFile))
                        {
                            try
                            {
                                Texture2D newTexture = AssetLoader.TextureFromFile(textureFile);
                                if (newTexture != null)
                                {
                                    if (originalTexture.width != newTexture.width || originalTexture.height != newTexture.height)
                                    {
                                        API.Logger.Warning($"Размер текстуры '{textureName}' ({newTexture.width}x{newTexture.height}) не совпадает с оригиналом ({originalTexture.width}x{originalTexture.height}). Замена отменена.");
                                        continue;
                                    }
                                    
                                    newTexture = AssetLoader.AttemptConvertTo(newTexture, originalTexture.format);
                                    AssetLoader.ReplaceTexture(originalTexture, newTexture);
                                }
                            }
                            catch (Exception e)
                            {
                                API.Logger.Error($"Ошибка при замене текстуры '{textureName}': {e.Message}");
                            }
                        }
                    }
                }
            }
        }

        public void ApplyAllTextures()
        {
            if (!ConfigManager.AreTexturesEnabled()) return;

            string modPath = AssetLoader.GetModPath(this);
            string texturesPath = Path.Combine(modPath, "Textures");

            if (Directory.Exists(texturesPath))
            {
                API.Logger.Info($"Обнаружена папка с текстурами: {texturesPath}, производится замена...");
                
                Texture2D[] allGameTextures = Resources.FindObjectsOfTypeAll<Texture2D>();
                string[] textureFiles = Directory.GetFiles(texturesPath, "*.png", SearchOption.AllDirectories);

                foreach (string textureFile in textureFiles)
                {
                    string textureName = Path.GetFileNameWithoutExtension(textureFile);
                    Texture2D originalTexture = allGameTextures.FirstOrDefault(t => t.name == textureName);

                    if (originalTexture != null)
                    {
                        try
                        {
                            Texture2D newTexture = AssetLoader.TextureFromFile(textureFile);
                            if (newTexture != null)
                            {
                                if (originalTexture.width != newTexture.width || originalTexture.height != newTexture.height)
                                {
                                    API.Logger.Warning($"Размер текстуры '{textureName}' ({newTexture.width}x{newTexture.height}) не совпадает с оригиналом ({originalTexture.width}x{originalTexture.height}). Замена отменена.");
                                    continue;
                                }
                                
                                newTexture = AssetLoader.AttemptConvertTo(newTexture, originalTexture.format);
                                AssetLoader.ReplaceTexture(originalTexture, newTexture);
                                API.Logger.Info($"Текстура '{textureName}' заменена.");
                            }
                        }
                        catch (Exception e)
                        {
                            API.Logger.Error($"Ошибка при замене текстуры '{textureName}': {e.Message}");
                        }
                    }
                    else
                    {
                        API.Logger.Warning($"Не найдена соответствующая текстура для файла: {textureName}");
                    }
                }
            }
        }

        private void UpdatePosters(string modPath)
        {
            string postersPath = Path.Combine(modPath, "PosterFiles");
            if (!Directory.Exists(postersPath))
            {
                API.Logger.Warning("Папка с постерами не найдена, замена не будет произведена.");
                return;
            }

            API.Logger.Info("Начало обновления постеров...");
            PosterObject[] allPosters = Resources.FindObjectsOfTypeAll<PosterObject>();
            foreach (PosterObject poster in allPosters)
            {
                string posterDataPath = Path.Combine(postersPath, poster.name, "PosterData.json");
                if (File.Exists(posterDataPath))
                {
                    try
                    {
                        PosterTextTable? posterData = JsonUtility.FromJson<PosterTextTable>(File.ReadAllText(posterDataPath));
                        
                        if (posterData != null)
                        {
                            for (int i = 0; i < Math.Min(posterData.items.Count, poster.textData.Length); i++)
                            {
                                var sourceData = poster.textData[i];
                                var modifiedData = posterData.items[i];
                                
                                sourceData.textKey = modifiedData.textKey;
                                sourceData.position = modifiedData.position;
                                sourceData.size = modifiedData.size;
                                sourceData.fontSize = modifiedData.fontSize;
                                sourceData.color = modifiedData.color;
                            }
                        
                            API.Logger.Info($"Обновлен плакат: {poster.name} с локализованными данными");
                        }
                    }
                    catch (Exception ex)
                    {
                        API.Logger.Error($"Ошибка замены данных плаката для {poster.name}: {ex.Message}");
                    }
                }
            }
            API.Logger.Info("Обновление постеров завершено.");
        }

        void OnDestroy()
        {
            if (harmonyInstance != null)
            {
                harmonyInstance.UnpatchSelf();
                harmonyInstance = null;
            }
        }
    }
}
