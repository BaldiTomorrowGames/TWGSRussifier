using System;
using UnityEngine;
using TMPro;

namespace TWGSRussifier
{
    public class TextLocalizer : MonoBehaviour
    {
        public string key;
        private TextMeshProUGUI textComponent;
        private bool initialized = false;
        
        private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            ApplyLocalization(); 
            initialized = true;
        }

        private void OnEnable()
        {
            if (initialized)
                ApplyLocalization();
        }

        public object RefreshLocalization()
        {
            return ApplyLocalization();
        }

        private object ApplyLocalization()
        {
            if (textComponent != null && !string.IsNullOrEmpty(key) && Singleton<LocalizationManager>.Instance != null)
            {
                string localizedText = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
                if (!string.IsNullOrEmpty(localizedText) && textComponent.text != localizedText)
                {
                    textComponent.text = localizedText;
                    return localizedText;
                }
                return textComponent.text;
            }
            return null;
        }
    }
} 