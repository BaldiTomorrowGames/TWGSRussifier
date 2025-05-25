using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    internal class TextLocalizerPatchInitializer
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
                    API.Logger.Error($"Не удалось распознать версию игры: {Application.version}");
                    return false;
                }
                
                Version minimumVersion = new Version("0.6.0");
                
                if (gameVersion < minimumVersion)
                {
                    API.Logger.Info($"TextLocalizerPatches не будет применен: версия игры {Application.version} ниже требуемой 0.6.0");
                    return false;
                }

                Type textLocalizerType = AccessTools.TypeByName("TextLocalizer");
                if (textLocalizerType == null)
                {
                    API.Logger.Warning("Тип TextLocalizer не найден");
                    return false;
                }

                FieldInfo onlySetIfBlankField = AccessTools.Field(textLocalizerType, "onlySetIfBlank");
                if (onlySetIfBlankField == null)
                {
                    API.Logger.Warning("Поле onlySetIfBlank не найдено в типе TextLocalizer. Патч не будет применен.");
                    return false;
                }
                
                API.Logger.Info("TextLocalizerPatches готов к применению");
                return true;
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка при инициализации TextLocalizerPatches: {ex.Message}");
                return false;
            }
        }
    }

    [HarmonyPatch(typeof(global::TextLocalizer))]
    internal class TextLocalizerPatches
    {
        [HarmonyPatch(typeof(global::TextLocalizer), "Awake")] [HarmonyPostfix]
        private static void TranslateOnAwakeInstead(global::TextLocalizer __instance)
        {
            Start(__instance);
        }
        [HarmonyPatch(typeof(global::TextLocalizer), "Start")] [HarmonyPrefix]
        private static bool PreventLocalizationOnStart()
        {
            return false;
        }
        private static void Start(global::TextLocalizer textLocalizer)
        {
            try
            {
                TMP_Text textBox = Traverse.Create(textLocalizer).Field(nameof(textBox)).GetValue<TMP_Text>();
                bool shouldSetText = true;
                
                try
                {
                    bool onlySetIfBlank = Traverse.Create(textLocalizer).Field("onlySetIfBlank").GetValue<bool>();
                    if (onlySetIfBlank && textBox.text.Length > 0)
                    {
                        shouldSetText = false;
                    }
                }
                catch (Exception)
                {
                }
                
                if (shouldSetText && textLocalizer.key != "")
                {
                    textLocalizer.GetLocalizedText(textLocalizer.key);
                }
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка в TextLocalizerPatches.Start: {ex.Message}");
            }
        }
    }
}
