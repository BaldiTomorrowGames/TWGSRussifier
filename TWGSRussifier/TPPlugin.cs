using BepInEx;
using TWGSRussifier.API;
using TWGSRussifier.Runtime;
using TWGSRussifier.Patches;
using HarmonyLib;
using UnityEngine;

namespace TWGSRussifier
{
    [BepInPlugin(RussifierTemp.ModGUID, RussifierTemp.ModName, RussifierTemp.ModVersion)]
    [BepInProcess("Baldi.exe")]
    public class TPPlugin : BaseUnityPlugin
    {
        private Harmony harmonyInstance;
        private const string expectedGameVersion = "0.999999999"; 
        public void Awake()
        {
            harmonyInstance = new Harmony(RussifierTemp.ModGUID);
            harmonyInstance.PatchAll();

            if (Application.version != expectedGameVersion)
            {
                string warningMessage = $"<color=red>Внимание!</color> Версия игры ({Application.version}) не соответствует требуемой версии ({expectedGameVersion}). Мод может работать некорректно.";
                WarningScreenAPI.AddWarningScreen(warningMessage, false);
            }

            GameUtils.CreateInstance<AssetsReloader>();
            GameUtils.CreateInstance<ConfigManager>();
            GameUtils.CreateInstance<ModSceneManager>();
            GameUtils.CreateInstance<OverwritesManager>();
            GameUtils.CreateInstance<RussifierController>();
            GameUtils.CreateInstance<LanguageManager>();
            TextFixes.Init();
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
