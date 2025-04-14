using System.IO;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class AssetLoader
    {
        public static Texture2D TextureFromFile(string path)
        {
            if (!File.Exists(path))
                return null;
                
            byte[] data = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(data))
                return texture;
                
            return null;
        }
        public static AudioClip AudioClipFromFile(string path)
        {
            if (!File.Exists(path))
                return null;
                
            string extension = Path.GetExtension(path).ToLower();
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            WWW www = new WWW("file://" + path);
            while (!www.isDone)
                System.Threading.Thread.Sleep(1);
                
            AudioClip clip = www.GetAudioClip(false, false);
            clip.name = fileName;
            
            return clip;
        }
        public static Texture2D AttemptConvertTo(Texture2D source, TextureFormat format)
        {
            if (source == null)
                return null;
                
            Texture2D result = new Texture2D(source.width, source.height, format, false);
            Color[] pixels = source.GetPixels();
            result.SetPixels(pixels);
            result.Apply();
            
            return result;
        }
    }
} 