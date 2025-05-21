using System;
using System.Diagnostics;
using BepInEx;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class Logger
    {
        private static BepInEx.Logging.ManualLogSource _bepLogger;
        private static readonly string ModuleName = RussifierTemp.ModName;

        public static void Init(BepInEx.Logging.ManualLogSource logger)
        {
            _bepLogger = logger;
        }

        private static string FormatMessage(string level, string message)
        {
            var frame = new StackFrame(2, false);
            var method = frame.GetMethod();
            string className = method?.DeclaringType?.Name ?? "UnknownClass";
            string methodName = method?.Name ?? "UnknownMethod";
            string time = DateTime.Now.ToString("HH:mm:ss.fff");
            return $"[{time}] [{level}] [{className}.{methodName}] {message}";
        }

        public static void Debug(string message)
        {
            if (!IsLoggingEnabled()) return;
            
            string msg = FormatMessage("DEBUG", message);
            if (_bepLogger != null)
                _bepLogger.Log(BepInEx.Logging.LogLevel.Debug, msg);
            else
                UnityEngine.Debug.Log(msg);
        }

        public static void Info(string message)
        {
            if (!IsLoggingEnabled()) return;
            
            string msg = FormatMessage("INFO", message);
            if (_bepLogger != null)
                _bepLogger.Log(BepInEx.Logging.LogLevel.Info, msg);
            else
                UnityEngine.Debug.Log(msg);
        }

        public static void Warning(string message)
        {
            if (!IsLoggingEnabled()) return;
            
            string msg = FormatMessage("WARNING", message);
            if (_bepLogger != null)
                _bepLogger.Log(BepInEx.Logging.LogLevel.Warning, msg);
            else
                UnityEngine.Debug.LogWarning(msg);
        }

        public static void Error(string message)
        {
            if (!IsLoggingEnabled()) return;
            
            string msg = FormatMessage("ERROR", message);
            if (_bepLogger != null)
                _bepLogger.Log(BepInEx.Logging.LogLevel.Error, msg);
            else
                UnityEngine.Debug.LogError(msg);
        }
        
        private static bool IsLoggingEnabled()
        {
            if (ConfigManager.EnableLogging == null) return true;
            
            return ConfigManager.IsLoggingEnabled();
        }
    }
} 