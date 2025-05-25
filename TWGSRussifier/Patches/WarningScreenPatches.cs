using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using System.Reflection;

namespace TWGSRussifier.Patches
{
    static internal class WarningScreenContainer
    {
        static internal (string, bool)[] screens => nonCriticalScreens.Select(x => (x, false)).ToArray().AddRangeToArray(criticalScreens.Select(x => (x, true)).ToArray()).ToArray();

        static internal List<string> nonCriticalScreens = new List<string>();

        static internal List<string> criticalScreens = new List<string>();

        static internal int currentPage = 0;

        static internal string pressAny = "";
    }

    internal class WarningScreenStartPatchFor09Plus
    {
        [HarmonyPrepare]
        private static bool Prepare()
        {
            try
            {
                Version gameVersion;
                try
                {
                    gameVersion = new Version(Application.version);
                }
                catch
                {
                    return false; 
                }

                Version minVersion = new Version("0.9.0");
                if (gameVersion < minVersion)
                {
                    API.Logger.Info($"WarningScreenStartPatchFor09Plus не будет применен: версия игры {Application.version} ниже требуемой 0.9.0");
                    return false;
                }

                Type inputManagerType = AccessTools.TypeByName("InputManager");
                if (inputManagerType != null)
                {
                    MethodInfo methodInfo = AccessTools.Method(inputManagerType, "GetInputButtonName", 
                        new Type[] { typeof(string), typeof(string), typeof(bool) });
                    
                    if (methodInfo == null)
                    {
                        API.Logger.Warning("Метод GetInputButtonName с тремя параметрами не найден. Патч для 0.9+ не будет применен.");
                        return false;
                    }
                }

                API.Logger.Info($"WarningScreenStartPatchFor09Plus успешно применен для версии {Application.version}");
                return true;
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при проверке версии для WarningScreenStartPatchFor09Plus: {ex.Message}");
                return false;
            }
        }

        [HarmonyPatch(typeof(WarningScreen), "Start")]
        [HarmonyPrefix]
        private static bool Prefix09Plus(WarningScreen __instance)
        {
            try
            {
                string text = "";
                
                if (Singleton<InputManager>.Instance.SteamInputActive)
                {
                    string buttonName = Singleton<InputManager>.Instance.GetInputButtonName("MouseSubmit", "Interface", false);
                    text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), buttonName);
                    WarningScreenContainer.pressAny = string.Format("НАЖМИТЕ {0} ДЛЯ ПРОДОЛЖЕНИЯ", buttonName);
                }
                else
                {
                    text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), "ЛЮБУЮ КЛАВИШУ");
                    WarningScreenContainer.pressAny = string.Format("НАЖМИТЕ {0} ДЛЯ ПРОДОЛЖЕНИЯ", "ЛЮБУЮ КЛАВИШУ");
                }
                
                WarningScreenContainer.nonCriticalScreens.Insert(0, text);
                __instance.textBox.text = text;
                return false;
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в WarningScreenStartPatchFor09Plus: {ex.Message}");
                return true; 
            }
        }
    }

    [HarmonyPatch]
    internal class WarningScreenStartPatchFor08
    {
        [HarmonyPrepare]
        private static bool Prepare()
        {
            try
            {
                Version gameVersion;
                try
                {
                    gameVersion = new Version(Application.version);
                }
                catch
                {
                    return true; 
                }

                Version minVersion = new Version("0.9.0");
                if (gameVersion >= minVersion)
                {
                    API.Logger.Info($"WarningScreenStartPatchFor08 не будет применен: версия игры {Application.version} выше 0.9.0");
                    return false;
                }

                API.Logger.Info($"WarningScreenStartPatchFor08 успешно применен для версии {Application.version}");
                return true;
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при проверке версии для WarningScreenStartPatchFor08: {ex.Message}");
                return false;
            }
        }

        [HarmonyPatch(typeof(WarningScreen), "Start")]
        [HarmonyPrefix]
        private static bool Prefix08(WarningScreen __instance)
        {
            try
            {
                string text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), "ЛЮБУЮ КЛАВИШУ");
                WarningScreenContainer.pressAny = string.Format("НАЖМИТЕ {0} ДЛЯ ПРОДОЛЖЕНИЯ", "ЛЮБУЮ КЛАВИШУ");
                
                WarningScreenContainer.nonCriticalScreens.Insert(0, text);
                __instance.textBox.text = text;
                return false;
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в WarningScreenStartPatchFor08: {ex.Message}");
                
                string text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), "ЛЮБУЮ КЛАВИШУ");
                WarningScreenContainer.pressAny = string.Format("НАЖМИТЕ {0} ДЛЯ ПРОДОЛЖЕНИЯ", "ЛЮБУЮ КЛАВИШУ");
                WarningScreenContainer.nonCriticalScreens.Insert(0, text);
                __instance.textBox.text = text;
                
                return false; 
            }
        }
    }
    
    [HarmonyPatch(typeof(WarningScreen))]
    [HarmonyPatch("Advance")]
    [HarmonyPriority(800)]
    static class WarningScreenAdvancePatch
    {
        static bool Prefix(WarningScreen __instance)
        {
            if (WarningScreenContainer.currentPage >= WarningScreenContainer.screens.Length)
            {
                return true;
            }
            if ((WarningScreenContainer.screens[WarningScreenContainer.currentPage].Item2) && ((WarningScreenContainer.currentPage + 1) >= WarningScreenContainer.screens.Length))
            {
                return false; 
            }
            WarningScreenContainer.currentPage++;
            if (WarningScreenContainer.currentPage >= WarningScreenContainer.screens.Length)
            {
                return true;
            }
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.01666667f);
            if (!WarningScreenContainer.screens[WarningScreenContainer.currentPage].Item2)
            {
                __instance.textBox.text = "<color=yellow>ВНИМАНИЕ!</color>\n" + WarningScreenContainer.screens[WarningScreenContainer.currentPage].Item1 + "\n\n" + WarningScreenContainer.pressAny;
            }
            else
            {
                if (((WarningScreenContainer.currentPage + 1) < WarningScreenContainer.screens.Length))
                {
                    __instance.textBox.text = "<color=red>ОШИБКА!</color>\n" + WarningScreenContainer.screens[WarningScreenContainer.currentPage].Item1 + "\n\n" + WarningScreenContainer.pressAny;
                }
                else
                {
                    __instance.textBox.text = "<color=red>ОШИБКА!</color>\n" + WarningScreenContainer.screens[WarningScreenContainer.currentPage].Item1 + "\n\nНАЖМИТЕ ALT+F4 ЧТОБЫ ВЫЙТИ";
                }
            }
            return false;
        }
    }
} 