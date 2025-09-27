using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(YCTP), "NewProblem")]
    internal class YCTPPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YCTP), "NewProblem")]
        private static void NewProblem_Postfix(YCTP __instance, bool corrupted)
        {
            if (corrupted && __instance.problemTxt != null)
            {
                var text = __instance.problemTxt.text;
                if (text.StartsWith("Solve Math Q"))
                {
                    string localizedText = Singleton<LocalizationManager>.Instance.GetLocalizedText("TWGS_SolveMathQ3");
                    if (!string.IsNullOrEmpty(localizedText))
                    {
                        text = text.Replace("Solve Math Q", localizedText);
                        __instance.problemTxt.text = text;
                    }
                }
            }
        }
    }
}