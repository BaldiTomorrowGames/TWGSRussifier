using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Linq;
using TWGSRussifier.API; 

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(Credits), "Start")] 
    internal class CreditsAssetsPatch 
    {
        private static bool textureReplaced = false; 

        [HarmonyPostfix]
        private static void CreditsStartPostfix(Credits __instance)
        {
            if (textureReplaced)
            {
                return;
            }
            
            if (!ConfigManager.AreTexturesEnabled())
            {
                return;
            }

            string textureName = "AwaitingSubmission";
            string fileName = textureName + ".png";
            
            string texturesPath = RussifierTemp.GetTexturePath();
            string filePath = Path.Combine(texturesPath, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                Texture2D originalTexture = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == textureName);
                
                if (originalTexture == null)
                {
                    return;
                }
                
                Texture2D loadedTexture = AssetLoader.TextureFromFile(filePath, originalTexture.format);
                if (loadedTexture == null)
                {
                    API.Logger.Warning($"Не удалось загрузить текстуру из {filePath}");
                    return;
                }
                
                loadedTexture.name = textureName + "_Loaded";

                if (loadedTexture.format == originalTexture.format)
                {
                    Graphics.CopyTexture(loadedTexture, originalTexture);
                    textureReplaced = true;
                    UnityEngine.Object.Destroy(loadedTexture);
                }
                else
                {
                    Texture2D convertedTexture = new Texture2D(loadedTexture.width, loadedTexture.height, originalTexture.format, false);
                    if (convertedTexture == null)
                    {
                        API.Logger.Warning($"Не удалось создать преобразованную текстуру");
                        UnityEngine.Object.Destroy(loadedTexture);
                        return;
                    }
                    
                    convertedTexture.SetPixels(loadedTexture.GetPixels());
                    convertedTexture.Apply();
                    
                    Graphics.CopyTexture(convertedTexture, originalTexture);
                    textureReplaced = true;
                    
                    UnityEngine.Object.Destroy(loadedTexture);
                    UnityEngine.Object.Destroy(convertedTexture);
                }
            }
            catch (System.Exception ex)
            {
                API.Logger.Error($"[{RussifierTemp.ModGUID}] Ошибка замены текстуры в титрах: {ex.Message}");
            }
        }
    }
} 