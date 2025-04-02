using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TWGSRussifier.API
{
    [System.Serializable]
    public class PosterTextTable
    {
        public List<PosterTextData> items = new List<PosterTextData>();
    }
    public static class PosterDumper
    {
        public static void DumpPoster(PosterObject currentPoster)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "PosterFiles", currentPoster.name);
            GameUtils.InsertDirectory(path);
            var res = ConvertDataToList(currentPoster.textData);
            string filePath = Path.Combine(path, "PosterData.json");

            string json = JsonUtility.ToJson(res, true);
            File.WriteAllText(filePath, json);
        }
        public static PosterTextTable ConvertDataToList(PosterTextData[] input)
        {
            PosterTextTable table = new PosterTextTable();
            table.items = new List<PosterTextData>();
            List<PosterTextData> formatedList = new List<PosterTextData>();
            for (int i = 0; i < input.Length; i++)
            {
                var asset = input[i];
                if (!formatedList.Contains(asset))
                {
                    formatedList.Add(asset);
                }
            }
            table.items.AddRange(formatedList);
            return table;
        }
        public static void ReplaceJsonData(PosterObject from)
        {
            if (!PosterExists(from))
            {
                throw new Exception($"Poster {from.name} is not exists!");
            }

            string path = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "PosterFiles", from.name, "PosterData.json");
            PosterTextTable output = JsonUtility.FromJson<PosterTextTable>(File.ReadAllText(path));
            for (int i = 0; i < output.items.Count; i++)
            {
                var fromValue = from.textData[i];
                var toValue = output.items[i];
                fromValue.textKey = toValue.textKey;
                fromValue.position = toValue.position;
                fromValue.size = toValue.size;
                fromValue.fontSize = toValue.fontSize;
                fromValue.color = toValue.color;

            }
        }
        public static bool PosterExists(PosterObject currentPoster)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, "PosterFiles", currentPoster.name);
            return Directory.Exists(path);
        }
    }
}
