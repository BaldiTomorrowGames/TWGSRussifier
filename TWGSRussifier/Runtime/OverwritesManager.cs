using TWGSRussifier.API;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

namespace TWGSRussifier.Runtime
{
    [Serializable]
    public class OverwritesObject
    {
        public string MainVal;
        public string NewVal;
        public OverwritesObject(string mainv, string newv)
        {
            MainVal = mainv;
            NewVal = newv;
        }
    }

    public class OverwritesManager : MonoBehaviour
    {
        public static OverwritesManager instance;
        public string overwritesPath;
        public OverwritesObject[] overwrites;
        private PosterObject[] loadedPosters = new PosterObject[0];
        private bool PostersFound = false;

        public OverwritesStruct overwritesStruct;
        public Dictionary<string, SceneObject> scenes = new Dictionary<string, SceneObject>();

        public void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            overwritesStruct = new OverwritesStruct();

            overwritesPath = Path.Combine(Application.streamingAssetsPath, "Modded", RussifierTemp.ModGUID, RussifierTemp.OverwritesFile);

            string directoryPath = Path.GetDirectoryName(overwritesPath);
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                    Debug.Log("Создана директория: " + directoryPath);
                }
                catch (Exception e)
                {
                    Debug.LogError("Ошибка при создании директории: " + directoryPath);
                    Debug.LogException(e);
                }
            }

            Load();
            ModSceneManager.instance.onMenuSceneLoadOnce += Overwrite;
            ModSceneManager.instance.onMenuSceneLoadOnce += OverwritePosters;
            AssetsReloader.Instance.onAssetsReload += ReloadAssets;
        }

        public void ReloadAssets()
        {
            overwritesStruct = new OverwritesStruct();
            Load();
            Overwrite();
            OverwritePosters();
        }

        private void OverwritePosters()
        {
            if (!PostersFound)
            {
                PostersFound = true;
                loadedPosters = Resources.FindObjectsOfTypeAll<PosterObject>();
            }

            foreach (PosterObject currentPoster in loadedPosters)
            {
                if (PosterDumper.PosterExists(currentPoster))
                {
                    PosterDumper.ReplaceJsonData(currentPoster);
                }
                else
                {
                    PosterDumper.DumpPoster(currentPoster);
                }
            }
        }

        private void Overwrite()
        {
            SceneObject[] levels = Resources.FindObjectsOfTypeAll<SceneObject>();

            foreach (SceneObject level in levels)
            {
                Debug.Log($"Level Name: {level.name}");

                foreach (var ob in overwrites)
                {
                    if (level.name == ob.MainVal)
                    {
                        level.levelTitle = ob.NewVal;
                    }
                }
            }
        }

        private void OverwriteBigScreenUI()
        {
            GameObject bigScreen = GameObject.Find("ElevatorTransission/BigScreen");
            if (bigScreen == null)
                return;

            foreach (var info in overwritesStruct.bigScreenReplacements)
            {
                Transform elementTransform = bigScreen.transform.Find(info.elementName);
                if (elementTransform == null)
                {
                    continue;
                }
                TextMeshProUGUI tmp = elementTransform.GetComponent<TextMeshProUGUI>();
                if (tmp != null && !string.IsNullOrEmpty(info.newText))
                {
                    if (tmp.text != info.newText)
                    {
                        tmp.text = info.newText;
                    }
                }
                RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    if (info.anchoredPosition != Vector2.zero && rectTransform.anchoredPosition != info.anchoredPosition)
                    {
                        rectTransform.anchoredPosition = info.anchoredPosition;
                    }
                    if (info.sizeDelta != Vector2.zero && rectTransform.sizeDelta != info.sizeDelta)
                    {
                        rectTransform.sizeDelta = info.sizeDelta;
                    }
                }
            }
        }

        private void Update()
        {
            OverwriteBigScreenUI();
        }

        private void Load()
        {
            try
            {
                if (File.Exists(overwritesPath))
                {
                    string jsonContent = File.ReadAllText(overwritesPath);
                    overwritesStruct = JsonUtility.FromJson<OverwritesStruct>(jsonContent);
                }
                else
                {
                    string jsonContent = JsonUtility.ToJson(overwritesStruct, true);
                    File.WriteAllText(overwritesPath, jsonContent);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            overwrites = new OverwritesObject[]
            {
                new OverwritesObject("MainLevel_1", overwritesStruct.floors_one),
                new OverwritesObject("MainLevel_2", overwritesStruct.floors_two),
                new OverwritesObject("MainLevel_3", overwritesStruct.floors_three),
                new OverwritesObject("PlaceholderEnding", overwritesStruct.floors_win),
                new OverwritesObject("Pitstop", overwritesStruct.floors_store),
                new OverwritesObject("Tutorial", overwritesStruct.floors_tutorial),
                new OverwritesObject("Farm", overwritesStruct.trips_farm),
                new OverwritesObject("Camping", overwritesStruct.trips_camp),
                new OverwritesObject("EndlessRandomMedium", overwritesStruct.floors_end),
                new OverwritesObject("EndlessPremadeMedium", overwritesStruct.floors_end),
                new OverwritesObject("StealthyChallenge", overwritesStruct.challanges_stealthy),
                new OverwritesObject("GrappleChallenge", overwritesStruct.challanges__grapple),
                new OverwritesObject("SpeedyChallenge", overwritesStruct.challanges_speedy),
            };
        }
    }
}
