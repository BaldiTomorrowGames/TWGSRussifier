using BepInEx;
using MTM101BaldAPI;
using System;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class VersionCheck
    {
        public static bool CheckGameVersion(string expectedVersion, PluginInfo info)
        {
            if (Application.version != expectedVersion)
            {
                string errorMessage = $"Версия игры ({Application.version}) не соответствует требуемой версии ({expectedVersion}). Мод может работать некорректно.";
                API.Logger.Error(errorMessage);
                // Для критической ошибки, можно использовать:
                // MTM101BaldiDevAPI.CauseCrash(info, new Exception(errorMessage));
                return false;
            }
            return true;
        }
    }
} 