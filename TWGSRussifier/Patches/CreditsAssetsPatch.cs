using HarmonyLib;
using UnityEngine;
using System.IO;
using System.Linq;
using MTM101BaldAPI.AssetTools;
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

            string textureName = "AwaitingSubmission";
            string fileName = textureName + ".png";
            string texturesPath = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "Textures");
            string filePath = Path.Combine(texturesPath, fileName);

            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                Texture2D loadedTexture = AssetLoader.TextureFromFile(filePath);
                if (loadedTexture == null)
                {
                    return;
                }
                loadedTexture.name = textureName + "_Loaded"; 

                Texture2D originalTexture = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(t => t.name == textureName);

                if (originalTexture == null)
                {
                    UnityEngine.Object.Destroy(loadedTexture); 
                    return;
                }

                Texture2D convertedTexture = AssetLoader.AttemptConvertTo(loadedTexture, originalTexture.format);

                if (convertedTexture != null)
                {
                    Graphics.CopyTexture(convertedTexture, originalTexture);
                    textureReplaced = true; 
                    
                    UnityEngine.Object.Destroy(loadedTexture);
                    if(convertedTexture != loadedTexture) 
                         UnityEngine.Object.Destroy(convertedTexture);
                }
                else 
                {
                     UnityEngine.Object.Destroy(loadedTexture);
                }
            }
            catch (System.Exception ex)
            {
            }
        }
    }
} 