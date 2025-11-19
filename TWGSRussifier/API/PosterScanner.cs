using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TWGSRussifier.API
{
    public static class PosterScanner
    {
        /// <param name="modPath">Путь к папке мода</param>
        public static void ScanAndExportNewPosters(string modPath)
        {
            if (!ConfigManager.IsDevModeEnabled())
            {
                Logger.Debug("Режим разработчика отключен, сканирование постеров пропущено.");
                return;
            }

            string postersPath = Path.Combine(modPath, "PosterFiles");
            if (!Directory.Exists(postersPath))
            {
                Directory.CreateDirectory(postersPath);
                Logger.ForceInfo($"[DEV MODE] Создана папка для постеров: {postersPath}");
            }

            Logger.ForceInfo("=== НАЧАЛО СКАНИРОВАНИЯ ПОСТЕРОВ (DEV MODE) ===");
            Logger.ForceInfo($"Сканирование игровых ресурсов...");
            
            PosterObject[] allPosters = Resources.FindObjectsOfTypeAll<PosterObject>();
            Logger.ForceInfo($"Найдено постеров в игре: {allPosters.Length}");
            Logger.ForceInfo($"Проверка папки: {postersPath}");
            Logger.ForceInfo("---");

            int newPostersCount = 0;
            int existingPostersCount = 0;
            int errorCount = 0;

            foreach (PosterObject poster in allPosters)
            {
                try
                {
                    string posterFolderPath = Path.Combine(postersPath, poster.name);
                    string posterDataPath = Path.Combine(posterFolderPath, "PosterData.json");

                    Logger.ForceInfo($"Проверка постера: {poster.name}");
                    
                    if (File.Exists(posterDataPath))
                    {
                        existingPostersCount++;
                        Logger.ForceInfo($"  └─ Статус: УЖЕ СУЩЕСТВУЕТ");
                        continue;
                    }

                    newPostersCount++;
                    Logger.ForceInfo($"  └─ Статус: [НОВЫЙ ПОСТЕР]");
                    
                    if (!Directory.Exists(posterFolderPath))
                    {
                        Directory.CreateDirectory(posterFolderPath);
                        Logger.ForceInfo($"  └─ [СОЗДАНА ПАПКА]: {posterFolderPath}");
                    }
                    else
                    {
                        Logger.ForceInfo($"  └─ Папка уже существует: {posterFolderPath}");
                    }

                    ExportPosterData(poster, posterDataPath);
                    LogPosterDetails(poster);
                    Logger.ForceInfo("---");
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Logger.ForceWarning($"  └─ [ОШИБКА] при обработке постера '{poster.name}': {ex.Message}");
                    Logger.ForceInfo("---");
                }
            }

            Logger.ForceInfo("=== СКАНИРОВАНИЕ ПОСТЕРОВ ЗАВЕРШЕНО ===");
            Logger.ForceInfo($"Итоги сканирования:");
            Logger.ForceInfo($"  Всего постеров в игре: {allPosters.Length}");
            Logger.ForceInfo($"  Уже существующих: {existingPostersCount}");
            Logger.ForceInfo($"  Новых найдено: {newPostersCount}");
            
            if (errorCount > 0)
            {
                Logger.ForceWarning($"  Ошибок при обработке: {errorCount}");
            }

            if (newPostersCount > 0)
            {
                Logger.ForceInfo("");
                Logger.ForceWarning("!!! ВНИМАНИЕ !!!");
                Logger.ForceWarning($"Найдено НОВЫХ постеров: {newPostersCount}");
                Logger.ForceWarning("Проверьте папку PosterFiles и добавьте переводы!");
                Logger.ForceWarning("После добавления переводов отключите Dev Mode в конфиге!");
            }
            else
            {
                Logger.ForceInfo("Новых постеров не обнаружено. Все постеры уже добавлены.");
            }
        }

        private static void ExportPosterData(PosterObject poster, string outputPath)
        {
            PosterTextTable posterTable = new PosterTextTable();

            if (poster.textData != null && poster.textData.Length > 0)
            {
                Logger.ForceInfo($"  └─ Экспорт данных постера...");
                foreach (var textData in poster.textData)
                {
                    PosterTextData exportData = new PosterTextData
                    {
                        textKey = textData.textKey,
                        position = new IntVector2(textData.position.x, textData.position.z),
                        size = new IntVector2(textData.size.x, textData.size.z),
                        fontSize = textData.fontSize,
                        color = textData.color
                    };
                    
                    posterTable.items.Add(exportData);
                }
            }

            string json = JsonUtility.ToJson(posterTable, true);
            File.WriteAllText(outputPath, json);
            
            Logger.ForceInfo($"  └─ [СОЗДАН JSON]: PosterData.json");
            Logger.ForceInfo($"     Текстовых элементов: {posterTable.items.Count}");
            Logger.ForceInfo($"     Полный путь: {outputPath}");
        }

        private static void LogPosterDetails(PosterObject poster)
        {
            Logger.ForceInfo($"  └─ Детали постера:");
            Logger.ForceInfo($"     Текстовых элементов: {poster.textData?.Length ?? 0}");
            
            if (poster.textData != null && poster.textData.Length > 0)
            {
                for (int i = 0; i < poster.textData.Length; i++)
                {
                    var textData = poster.textData[i];
                    Logger.Debug($"     Элемент {i}: Key='{textData.textKey}', FontSize={textData.fontSize}, Pos=({textData.position.x}, {textData.position.z})");
                }
            }
        }
    }
}

