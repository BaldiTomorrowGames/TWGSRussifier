using TWGSRussifier.API;
using System.IO;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    public class ConfigManager : MonoBehaviour
    {
        public static ConfigManager instance;
        public string configPath;
        public LocalizationConfig config;

        public void Awake()
        {
            configPath = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "gameStruct.json");
            instance = this;
            DontDestroyOnLoad(gameObject);
            config = new LocalizationConfig();

            AssetsReloader.Instance.onAssetsReload += reLoadConfig;

            reLoadConfig();
        }
        public void reLoadConfig()
        {
            if (File.Exists(configPath))
            {
                string jsonContent = File.ReadAllText(configPath);
                config = JsonUtility.FromJson<LocalizationConfig>(jsonContent);
            }
            else
            {
                string jsonContent = JsonUtility.ToJson(config, true);
                File.WriteAllText(configPath, jsonContent);
            }
        }
    }
}
