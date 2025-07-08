using System;
using UnityEngine;
using TMPro;

namespace TWGSRussifier
{
    public class TextLocalizer : MonoBehaviour
    {
        public string key = null!;
        private TextMeshProUGUI textComponent = null!;
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

        public object? RefreshLocalization()
        {
            return ApplyLocalization();
        }

        private object? ApplyLocalization()
        {
            if (textComponent != null && !string.IsNullOrEmpty(key) && Singleton<LocalizationManager>.Instance != null)
            {
                if (Singleton<LocalizationManager>.Instance.HasKey(key))
                {
                    string localizedText = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
                    if (!string.IsNullOrEmpty(localizedText) && textComponent.text != localizedText)
                    {
                        textComponent.text = localizedText;
                        return localizedText;
                    }
                }
                return textComponent.text;
            }
            return null;
        }
    }
} 