using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace TWGSRussifier
{
    public static class LocalizationManager_Extensions
    {
        private static readonly FieldInfo _localizedText = AccessTools.Field(typeof(LocalizationManager), "localizedText");

        public static bool HasKey(this LocalizationManager manager, string key)
        {
            if (_localizedText == null) {
                API.Logger.Error("Unable to access LocalizationManager.localizedText via reflection!");
                return false; 
            }
            var dictionary = (Dictionary<string, string>)_localizedText.GetValue(manager);
            return dictionary != null && dictionary.ContainsKey(key);
        }
    }
} 