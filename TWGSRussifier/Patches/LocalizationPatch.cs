using TWGSRussifier.Runtime;
using HarmonyLib;
using System.Reflection;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(LocalizationManager))]
    [HarmonyPatch("LoadLocalizedText")]
    public static class LocalizationPatch
    {
        static void Postfix(LocalizationManager __instance)
        {
            FieldInfo localText = AccessTools.Field(typeof(LocalizationManager), "localizedText");
            localText.SetValue(__instance, LanguageManager.instance.languageData);
        }
    }
}
