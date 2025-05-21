using BepInEx;
using TWGSRussifier.API;
using TWGSRussifier.Runtime;
using TWGSRussifier.Patches;
using HarmonyLib;
using UnityEngine;
using System.IO;

namespace TWGSRussifier
{
    [BepInPlugin(RussifierTemp.ModGUID, RussifierTemp.ModName, RussifierTemp.ModVersion)]
    [BepInProcess("Baldi.exe")]
    public class TPPlugin : BaseUnityPlugin
    {
        private Harmony harmonyInstance;
        private const string expectedGameVersion = "0.10.2"; 

        public void Awake()
        {
            API.Logger.Init(this.Logger);
            ConfigManager.Initialize(this, this.Logger);
            
            API.Logger.Info($"Плагин {RussifierTemp.ModName} инициализирован.");
            API.Logger.Info($"Текстуры: {(ConfigManager.AreTexturesEnabled() ? "Включены" : "Отключены")}, " +
                           $"Звуки: {(ConfigManager.AreSoundsEnabled() ? "Включены" : "Отключены")}, " +
                           $"Логирование: {(ConfigManager.IsLoggingEnabled() ? "Включено" : "Отключено")}");
            
            CreateModDirectories();
            harmonyInstance = new Harmony(RussifierTemp.ModGUID);
            harmonyInstance.PatchAll();

            VersionCheck.CheckGameVersion(expectedGameVersion);

            GameUtils.CreateInstance<ModSceneManager>();
            GameUtils.CreateInstance<PostersManager>();
            GameUtils.CreateInstance<RussifierController>();
            GameUtils.CreateInstance<LanguageManager>();
        }
        
        private void CreateModDirectories()
        {
            string basePath = RussifierTemp.GetBasePath();
            GameUtils.InsertDirectory(basePath);
            
            GameUtils.InsertDirectory(RussifierTemp.GetAudioPath());
            GameUtils.InsertDirectory(RussifierTemp.GetTexturePath());
            GameUtils.InsertDirectory(RussifierTemp.GetPostersPath());
            
            API.Logger.Info($"Созданы директории для ассетов мода в: {basePath}");
        }
        
        public void OnDestroy()
        {
            if (harmonyInstance != null)
            {
                harmonyInstance.UnpatchSelf();
                harmonyInstance = null;
            }
        }
    }
}
