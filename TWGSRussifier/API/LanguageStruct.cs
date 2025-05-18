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
             Logger.Warning($"Одинаковый ключ {key} уже найден! \n Используйте language_LANGCODENAME.config для настройки того, что нужно сделать, замените ключи и значения на новые или пропустите.");
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
