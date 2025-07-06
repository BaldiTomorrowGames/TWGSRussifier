using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

    [HarmonyPatch(typeof(WarningScreen))]
    [HarmonyPatch("Start")]
    [HarmonyPriority(800)]
    static class WarningScreenStartPatch
    {
        static bool Prefix(WarningScreen __instance)
        {
            string text = "";
            if (Singleton<InputManager>.Instance.SteamInputActive)
            {
                text = string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Men_Warning"), Singleton<InputManager>.Instance.GetInputButtonName("MouseSubmit", "Interface", false));
                WarningScreenContainer.pressAny = string.Format("НАЖМИТЕ {0} ДЛЯ ПРОДОЛЖЕНИЯ", Singleton<InputManager>.Instance.GetInputButtonName("MouseSubmit", "Interface", false));
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

    // Этот Postfix-патч сработает *после* того, как MTM101BMDE подготовит экран предупреждения.
    // Мы просто заменяем английский текст на русский.
    [HarmonyPatch(typeof(WarningScreen))]
    internal class WarningScreen_Patch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void Start_Postfix(WarningScreen __instance)
        {
            TranslateWarningText(__instance.textBox);
        }

        [HarmonyPatch("Advance")]
        [HarmonyPostfix]
        private static void Advance_Postfix(WarningScreen __instance)
        {
            TranslateWarningText(__instance.textBox);
        }

        private static void TranslateWarningText(TMP_Text textBox)
        {
            if (textBox == null) return;

            string currentText = textBox.text;

            // Перевод заголовков
            currentText = currentText.Replace("WARNING!", "ВНИМАНИЕ!");
            currentText = currentText.Replace("ERROR!", "ОШИБКА!");

            // Перевод стандартного текста предупреждения (если он есть)
            if (currentText.Contains("This game is not suitable for children or those who are easily disturbed."))
            {
                currentText = "Эта игра не подходит для детей и людей со слабой психикой.\nОна содержит внезапные громкие звуки и пугающие образы.";
            }

            // Перевод инструкции "Press any button to continue"
            if (currentText.Contains("PRESS ANY BUTTON TO CONTINUE"))
            {
                currentText = currentText.Replace("PRESS ANY BUTTON TO CONTINUE", "НАЖМИТЕ ЛЮБУЮ КЛАВИШУ ДЛЯ ПРОДОЛЖЕНИЯ");
            }
            else
            {
                // Для случаев, когда указана конкретная кнопка
                currentText = System.Text.RegularExpressions.Regex.Replace(currentText, @"PRESS (.+) TO CONTINUE", "НАЖМИТЕ $1 ДЛЯ ПРОДОЛЖЕНИЯ");
            }
            
            // Перевод инструкции выхода
            currentText = currentText.Replace("PRESS ALT+F4 TO EXIT", "НАЖМИТЕ ALT+F4 ЧТОБЫ ВЫЙТИ");


            textBox.text = currentText;
        }
    }
} 