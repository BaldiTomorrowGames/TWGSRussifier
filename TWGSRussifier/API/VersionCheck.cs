using UnityEngine;

namespace TWGSRussifier.API
{
    public static class VersionCheck
    {
        public static bool CheckGameVersion(string expectedVersion)
        {
            if (Application.version != expectedVersion)
            {
                string warningMessage = $"Версия игры ({Application.version}) не соответствует требуемой версии ({expectedVersion}). Мод может работать некорректно.";
                WarningScreenAPI.AddWarningScreen(warningMessage, false);
                return false;
            }
            return true;
        }
    }
} 