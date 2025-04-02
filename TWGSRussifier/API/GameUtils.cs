using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class GameUtils
    {
        public static string GetFileFrom(string[] paths, string fileName = "Subtitles_Russian.json")
        {
            foreach (var path in paths)
            {
                if (path.Contains(fileName))
                {
                  return path;
                }
            }
            return null;
        }
        public static T GetAssetFromResources<T>(string includedName = "ObjectName") where T : Component
        {
            var foundList = Resources.FindObjectsOfTypeAll<T>();

            foreach (var obj in foundList)
            {
                if (obj.name  == includedName)
                {
                    return obj;
                }
            }
            return foundList.First();
        }
        public static void InsertDirectory(string mainPath)
        {
            if (!Directory.Exists(mainPath))
            {
                Directory.CreateDirectory(mainPath);
            }
        }
        public static void CreateInstance<T>() where T : MonoBehaviour
        {
            if (GameObject.FindObjectOfType<T>(true) == null)
            {
                T newInstance = GameObject.Instantiate(new GameObject()).AddComponent<T>();
                newInstance.name = typeof(T).Name;
                Debug.Log($"Created {newInstance.name}");
            }
            else
            {
                throw new System.Exception($"Class {typeof(T).Name} is already exists!");
            }
           
        }
        public static T CreateInstanceI<T>() where T : MonoBehaviour
        {
            if (GameObject.FindObjectOfType<T>(true) == null)
            {
                T newInstance = GameObject.Instantiate(new GameObject()).AddComponent<T>();
                newInstance.name = typeof(T).Name;
                Debug.Log($"Created {newInstance.name}");
                return newInstance;
            }
            else
            {
                throw new System.Exception($"Class {typeof(T).Name} is already exists!");
            }
            return null;
        }
    }
}
