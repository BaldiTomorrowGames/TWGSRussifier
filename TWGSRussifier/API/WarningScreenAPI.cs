using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TWGSRussifier.API
{
    public static class WarningScreenAPI
    {
        private static List<string> nonCriticalScreens = new List<string>();
        private static List<string> criticalScreens = new List<string>();
        private static int currentPage = 0;
        private static string pressAny = "";
        
        private static (string, bool)[] Screens => nonCriticalScreens
            .Select(x => (x, false))
            .ToArray()
            .AddRangeToArray(criticalScreens.Select(x => (x, true)).ToArray())
            .ToArray();

        public static void AddWarningScreen(string text, bool fatal)
        {
            if (fatal)
            {
                criticalScreens.Add(text);
            }
            else
            {
                nonCriticalScreens.Add(text);
            }
        }
        
        private static T[] AddRangeToArray<T>(this T[] array, T[] toAdd)
        {
            T[] newArray = new T[array.Length + toAdd.Length];
            Array.Copy(array, newArray, array.Length);
            Array.Copy(toAdd, 0, newArray, array.Length, toAdd.Length);
            return newArray;
        }
    }
} 