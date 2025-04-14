using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class InputManagerExtensions
    {
        private static readonly MethodInfo GetInputButtonNameOriginal = AccessTools.Method(
            typeof(InputManager), 
            "GetInputButtonName", 
            new System.Type[] { typeof(string), typeof(string) });
            
        public static string GetInputButtonName(this InputManager instance, string buttonName, string mapName, bool displayPrompt)
        {
            return (string)GetInputButtonNameOriginal.Invoke(instance, new object[] { buttonName, mapName });
        }
    }
} 