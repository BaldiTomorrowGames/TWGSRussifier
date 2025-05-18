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
        private const string expectedGameVersion = "0.10.2"; 

        public void Awake()
        {
            API.Logger.Init(this.Logger);
            API.Logger.Info($"Плагин {RussifierTemp.ModName} инициализирован.");
            harmonyInstance = new Harmony(RussifierTemp.ModGUID);
            harmonyInstance.PatchAll();

            VersionCheck.CheckGameVersion(expectedGameVersion);

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
