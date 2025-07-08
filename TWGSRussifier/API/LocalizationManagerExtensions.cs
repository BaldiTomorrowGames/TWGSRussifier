using System.Collections.Generic;
using HarmonyLib;

namespace TWGSRussifier.API
{
    public static class LocalizationManagerExtensions
    {
		public static bool HasKey(this LocalizationManager localization, string key)
		{
			return Traverse.Create(localization).Field<Dictionary<string, string>>("localizedText").Value.ContainsKey(key);
		}
	}
} 