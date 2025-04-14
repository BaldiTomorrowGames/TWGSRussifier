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
            try
            {
                string directoryPath = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log($"Directory created: {directoryPath}");
                }

                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    config = JsonUtility.FromJson<LocalizationConfig>(jsonContent);
                }
                else
                {
                    config = new LocalizationConfig();
                    string jsonContent = JsonUtility.ToJson(config, true);
                    File.WriteAllText(configPath, jsonContent);
                    Debug.Log($"Default config created: {configPath}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error in reLoadConfig: {ex.Message}");
            }
        }
    }
}
