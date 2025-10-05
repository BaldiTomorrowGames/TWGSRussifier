using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWGSRussifier.API;
using UnityEngine;

namespace TWGSRussifier.Runtime
{
    [Serializable]
    public class PosterTextTable
    {
        public List<PosterTextData> items = new List<PosterTextData>();
    }

    public class PostersManager : MonoBehaviour
    {
        public static PostersManager instance = null!;

        private List<PosterObject> loadedPosters = new List<PosterObject>();
        private string postersPath = null!;
        private bool initialized = false;

        public void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            postersPath = RussifierTemp.GetPostersPath();
            GameUtils.InsertDirectory(postersPath);
            
            ModSceneManager.instance.onMenuSceneLoadOnce += LoadAndUpdatePosters;
            
            API.Logger.Info($"PostersManager инициализирован по пути: {postersPath}");
        }

        public void LoadAndUpdatePosters()
        {
            if (!initialized)
            {
                initialized = true;
                loadedPosters = Resources.FindObjectsOfTypeAll<PosterObject>().ToList();
                API.Logger.Info($"Найдено {loadedPosters.Count} плакатов в ресурсах игры");
            }

            foreach (PosterObject poster in loadedPosters)
            {
                if (PosterExists(poster))
                {
                    ReplaceJsonData(poster);
                }
                else
                {
                    DumpPoster(poster);
                }
            }
        }

        public void DumpPoster(PosterObject poster)
        {
            string path = Path.Combine(postersPath, poster.name);
            GameUtils.InsertDirectory(path);
            
            PosterTextTable posterData = ConvertDataToList(poster.textData);
            string filePath = Path.Combine(path, "PosterData.json");

            string json = JsonUtility.ToJson(posterData, true);
            File.WriteAllText(filePath, json);
            
            API.Logger.Info($"Загружены данные плаката: {poster.name}");
        }

        public void ReplaceJsonData(PosterObject poster)
        {
            if (!PosterExists(poster))
            {
                API.Logger.Warning($"Плакат {poster.name} не существует в папке");
                API.Logger.Warning($"Плакат {poster.name} не существует в папке modded");
                API.Logger.Warning($"Плакат {poster.name} не существует в папке modded");
                return;
            }

            string path = Path.Combine(postersPath, poster.name, "PosterData.json");
            
            try
            {
                PosterTextTable posterData = JsonUtility.FromJson<PosterTextTable>(File.ReadAllText(path));
                
                for (int i = 0; i < Math.Min(posterData.items.Count, poster.textData.Length); i++)
                {
                    var sourceData = poster.textData[i];
                    var modifiedData = posterData.items[i];
                    
                    sourceData.textKey = modifiedData.textKey;
                    sourceData.position = modifiedData.position;
                    sourceData.size = modifiedData.size;
                    sourceData.fontSize = modifiedData.fontSize;
                    sourceData.color = modifiedData.color;
                }
                
                API.Logger.Info($"Обновлен плакат: {poster.name} с локализованными данными");
            }
            catch (Exception ex)
            {
                API.Logger.Error($"Ошибка замены данных плаката для {poster.name}: {ex.Message}");
            }
        }

        public bool PosterExists(PosterObject poster)
        {
            string path = Path.Combine(postersPath, poster.name);
            return Directory.Exists(path) && File.Exists(Path.Combine(path, "PosterData.json"));
        }

        private PosterTextTable ConvertDataToList(PosterTextData[] inputData)
        {
            PosterTextTable table = new PosterTextTable();
            table.items = new List<PosterTextData>();
            
            HashSet<PosterTextData> uniqueItems = new HashSet<PosterTextData>();
            foreach (var data in inputData)
            {
                if (!uniqueItems.Contains(data))
                {
                    uniqueItems.Add(data);
                    table.items.Add(data);
                }
            }
            
            return table;
        }
    }
} 