using BepInEx.Configuration;
using TWGSRussifier.API;
using BepInEx.Logging;

namespace TWGSRussifier.API
{
    public static class ConfigManager
    {
        public static ConfigEntry<bool> EnableTextures { get; private set; } = null!;
        public static ConfigEntry<bool> EnableSounds { get; private set; } = null!;
        public static ConfigEntry<bool> EnableLogging { get; private set; } = null!;
        private static ManualLogSource _logger = null!;

        public static void Initialize(BepInEx.BaseUnityPlugin plugin, ManualLogSource logger)
        {
            _logger = logger;
            
            EnableTextures = plugin.Config.Bind("Main", 
                "EnableTextures", 
                true, 
                "Включить замену текстур");

            EnableSounds = plugin.Config.Bind("Main", 
                "EnableSounds", 
                true, 
                "Включить замену звуков");

            EnableLogging = plugin.Config.Bind("Main", 
                "EnableLogging", 
                false, 
                "Включить логирование");

            _logger.LogInfo($"Конфигурация загружена. Текстуры: {EnableTextures.Value}, Звуки: {EnableSounds.Value}, Логирование: {EnableLogging.Value}");
        }

        public static bool AreTexturesEnabled()
        {
            return EnableTextures?.Value ?? false;
        }

        public static bool AreSoundsEnabled()
        {
            return EnableSounds?.Value ?? false;
        }

        public static bool IsLoggingEnabled()
        {
            return EnableLogging?.Value ?? true;
        }
    }
} 