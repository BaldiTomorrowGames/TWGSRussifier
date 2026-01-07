using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System.IO;

namespace TWGSRussifier.API
{
    public static class ConfigManager
    {
        public static ConfigEntry<bool> EnableTextures { get; private set; } = null!;
        public static ConfigEntry<bool> EnableSounds { get; private set; } = null!;
        public static ConfigEntry<bool> EnableLogging { get; private set; } = null!;
#if DEBUG
        public static ConfigEntry<bool> EnableDevMode { get; private set; } = null!;
#endif
        private static ManualLogSource _logger = null!;

        public static void Initialize(BaseUnityPlugin plugin, ManualLogSource logger)
        {
            _logger = logger;
            
            EnableTextures = plugin.Config.Bind("General", "Enable Textures", true, "Enable or disable texture replacement.");
            EnableSounds = plugin.Config.Bind("General", "Enable Sounds", true, "Enable or disable sound replacement.");
            EnableLogging = plugin.Config.Bind("General", "Enable Logging", false, "Enable or disable logging.");
#if DEBUG
            EnableDevMode = plugin.Config.Bind("Development", "Enable Dev Mode", false, "Enable development mode (scans and exports new posters). DISABLE FOR RELEASE!");
#endif

            _logger.LogInfo("Config loaded successfully.");
        }

        public static bool AreTexturesEnabled()
        {
            return EnableTextures.Value;
        }

        public static bool AreSoundsEnabled()
        {
            return EnableSounds.Value;
        }

        public static bool IsLoggingEnabled()
        {
            return EnableLogging.Value;
        }

        public static bool IsDevModeEnabled()
        {
#if DEBUG
            return EnableDevMode.Value;
#else
            return false;
#endif
        }
    }
} 