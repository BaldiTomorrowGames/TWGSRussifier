using HarmonyLib;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(EndlessMapOverview))]
    class EndlessMapOverviewPatch
    {
		[HarmonyPostfix]
		[HarmonyPatch("LoadScores")]
		private static void LoadScoresPostfix(EndlessMapOverview __instance)
		{
			var levels = Traverse.Create(__instance).Field<EndlessLevelTypeContainer[]>("level").Value;
			var currentSize = Traverse.Create(__instance).Field<int>("currentSize").Value;
			var currentType = Traverse.Create(__instance).Field<int>("currentType").Value;

			string typeKey = levels[currentType].typeKey;
            string sizeKey = levels[currentType].size[currentSize].sizeKey;

            string processedTypeKey = typeKey.Replace("Level_Type_", "");
            string processedSizeKey = sizeKey.Replace("Level_Size_", "");

			string combinedKey = $"TWGS_Floor_EndlessName_{processedTypeKey}_{processedSizeKey}";

			var locMan = Singleton<LocalizationManager>.Instance;
			if (locMan.HasKey(combinedKey))
			{
				__instance.levelName.text = locMan.GetLocalizedText(combinedKey);
				return;
			}
			__instance.levelName.text = locMan.GetLocalizedText(sizeKey) + " " + locMan.GetLocalizedText(typeKey);
		}
    }
} 