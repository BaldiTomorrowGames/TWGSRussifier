using BepInEx;
using TWGSRussifier.API;
using TWGSRussifier.Runtime;
using TWGSRussifier.Patches;
using HarmonyLib;
using MTM101BaldAPI;
using UnityEngine;

namespace TWGSRussifier
{
    [BepInPlugin(RussifierTemp.ModGUID, RussifierTemp.ModName, RussifierTemp.ModVersion)]
    [BepInProcess("Baldi.exe")]
    public class TPPlugin : BaseUnityPlugin
    {
        private Harmony harmonyInstance;
        private const string expectedGameVersion = "dev"; 

        public void Awake()
        {
            harmonyInstance = new Harmony(RussifierTemp.ModGUID);
            harmonyInstance.PatchAll();

            if (Application.version != expectedGameVersion)
            {
                string warningMessage = $"Версия игры ({Application.version}) не соответствует требуемой версии ({expectedGameVersion}). Мод может работать некорректно.";
                MTM101BaldiDevAPI.AddWarningScreen(warningMessage, false);
            }

            GameUtils.CreateInstance<AssetsReloader>();
            GameUtils.CreateInstance<ConfigManager>();
            GameUtils.CreateInstance<ModSceneManager>();
            GameUtils.CreateInstance<PostersManager>();
            GameUtils.CreateInstance<RussifierController>();
            GameUtils.CreateInstance<LanguageManager>();
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
