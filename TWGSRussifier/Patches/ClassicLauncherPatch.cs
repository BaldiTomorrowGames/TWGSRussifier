using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using TWGSRussifier.API;
using TWGSRussifier.Runtime;
using System.IO;
using System.Reflection;
using System.Linq;

namespace TWGSRussifier.Patches
{
    internal class ClassicLauncherPatch
    {
        private static bool initialized = false;
        
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "PlayButton", "TWGS_ClassicLauncher_PlayButton" },
            { "WebsiteButton", "TWGS_ClassicLauncher_WebsiteButton" },
            { "ExitButton", "TWGS_ClassicLauncher_ExitButton" },
        };

        [HarmonyPatch(typeof(ClassicLauncher), "Start")]
        private static class ClassicLauncherStartPatch
        {
            [HarmonyPostfix]
            private static void Postfix(ClassicLauncher __instance)
            {
                if (!initialized)
                {
                    EnsureSubtitlesLoaded();
                    ApplyLocalizationToClassicLauncher();
                    initialized = true;
                }
                
                LoadClassicLauncherTexture();
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                initialized = false;
            }
        }

        private static void LoadClassicLauncherTexture()
        {
            string textureName = "ClassicLaunhcerImage";

            try
            {
                Texture2D? originalTexture = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == textureName);
                if (originalTexture == null)
                {
                    return;
                }

                string modPath = RussifierTemp.GetTexturePath();
                string filePath = Path.Combine(modPath, textureName + ".png");

                Texture2D? newTexture = AssetLoader.TextureFromFile(filePath);

                if (newTexture == null || originalTexture.width != newTexture.width || originalTexture.height != newTexture.height)
                {
                    if (newTexture != null) Object.Destroy(newTexture);
                    return;
                }

                newTexture = AssetLoader.AttemptConvertTo(newTexture, originalTexture.format);
                if (newTexture != null)
                {
                    ReplaceTexturePixels(originalTexture, newTexture);
                }
            }
            catch (System.Exception)
            {
            }
        }

        private static void ReplaceTexturePixels(Texture2D originalTexture, Texture2D newTexture)
        {
            if (originalTexture == null || newTexture == null) return;

            try
            {
                Texture2D readableTexture = new Texture2D(originalTexture.width, originalTexture.height, originalTexture.format, false);
                
                RenderTexture renderTexture = RenderTexture.GetTemporary(originalTexture.width, originalTexture.height);
                Graphics.Blit(originalTexture, renderTexture);
                
                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = renderTexture;
                readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
                readableTexture.Apply();
                RenderTexture.active = previous;
                
                Color[] newPixels = newTexture.GetPixels();
                
                readableTexture.SetPixels(newPixels);
                readableTexture.Apply();
                
                Graphics.CopyTexture(readableTexture, originalTexture);

                RenderTexture.ReleaseTemporary(renderTexture);
                Object.Destroy(readableTexture);

                Object.Destroy(newTexture);
            }
            catch (System.Exception)
            {
            }
        }
        private static void EnsureSubtitlesLoaded()
        {
            if (Singleton<LocalizationManager>.Instance != null)
            {
                FieldInfo localTextField = AccessTools.Field(typeof(LocalizationManager), "localizedText");
                var localizedText = localTextField?.GetValue(Singleton<LocalizationManager>.Instance) as Dictionary<string, string>;
                
                if (localizedText != null && localizedText.Count > 0 && 
                    localizedText.ContainsKey("TWGS_ClassicLauncher_PlayButton"))
                {
                    return;
                }
            }

            if (LanguageManager.instance != null && LanguageManager.instance.languageData.Count > 0)
            {
                UpdateLocalizationManager(LanguageManager.instance.languageData);
                return;
            }

            LoadSubtitlesDirectly();
        }

        private static void LoadSubtitlesDirectly()
        {
            try
            {
                string basePath = RussifierTemp.GetBasePath();
                string subtitlePath = Path.Combine(basePath, RussifierTemp.SubtitilesFile);
                
                if (File.Exists(subtitlePath))
                {
                    string fileData = File.ReadAllText(subtitlePath);
                    LocalizationData rawJson = JsonUtility.FromJson<LocalizationData>(fileData);
                    
                    Dictionary<string, string> subtitleData = new Dictionary<string, string>();
                    for (int i = 0; i < rawJson.items.Length; i++)
                    {
                        subtitleData[rawJson.items[i].key] = rawJson.items[i].value;
                    }
                    
                    UpdateLocalizationManager(subtitleData);
                }
            }
            catch (System.Exception)
            {
            }
        }

        private static void UpdateLocalizationManager(Dictionary<string, string> subtitleData)
        {
            if (Singleton<LocalizationManager>.Instance != null && subtitleData != null)
            {
                FieldInfo localTextField = AccessTools.Field(typeof(LocalizationManager), "localizedText");
                localTextField?.SetValue(Singleton<LocalizationManager>.Instance, subtitleData);
            }
        }

        private static void ApplyLocalizationToClassicLauncher()
        {
            TextMeshProUGUI[] allTexts = Object.FindObjectsOfType<TextMeshProUGUI>();
            
            foreach (var text in allTexts)
            {
                if (text.transform.parent != null)
                {
                    string parentName = text.transform.parent.name;
                    
                    foreach (var kvp in LocalizationKeys)
                    {
                        string buttonName = kvp.Key;
                        string localizationKey = kvp.Value;
                        
                        if (parentName.Contains(buttonName))
                        {
                            ApplyLocalizationToText(text, localizationKey);
                            break;
                        }
                    }
                }
            }
        }

        private static void ApplyLocalizationToText(TextMeshProUGUI textComponent, string localizationKey)
        {
            if (textComponent == null) return;

            Component[] components = textComponent.GetComponents<Component>();
            foreach (Component component in components)
            {
                if (component != null && component.GetType().Name == "TextLocalizer" && component.GetType() != typeof(TextLocalizer))
                {
                    Object.Destroy(component);
                }
            }

            TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
            if (localizer == null)
            {
                localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                localizer.key = localizationKey;
                localizer.RefreshLocalization();
            }
            else
            {
                localizer.key = localizationKey;
                localizer.RefreshLocalization();
            }
        }
    }
}