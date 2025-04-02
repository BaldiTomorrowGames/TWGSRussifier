using UnityEngine;
using System.Collections.Generic;

namespace TWGSRussifier.API
{
    [System.Serializable]
   public class LanguageStruct
    {
        public string LanguageCodeName = "English";

        private Dictionary<string,string> localizationTable = new Dictionary<string,string>(); 
        public void AddKey(string key , string value)
        {
            if (!localizationTable.ContainsKey(key))
            {
                localizationTable.Add(key, value);
            }
            else
            {
             Debug.LogWarning($"Same key {key} is already found! \n Use language_LANGCODENAME.config to setup what engine needs to do, replace keys and values by new one or skip.");
            }
        }
        public bool ContainsKey(string key)
        {
            return localizationTable.ContainsKey(key);
        }
        public bool ContainsValue(string value)
        {
            return localizationTable.ContainsValue(value);
        }
        public string GetLocalizatedText(string key)
        {
            return localizationTable[key];
        }
    }
}
