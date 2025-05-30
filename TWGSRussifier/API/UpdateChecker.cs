using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TWGSRussifier.API
{
    public static class UpdateChecker
    {
        private const string RepoOwner = "BaldiTomorrowGames";
        private const string RepoName = "TWGSRussifier";
        private const string UpdateUrl = "https://gamebanana.com/mods/updates/597541"; 

        public static bool IsUpdateAvailable { get; private set; } = false;
        public static string LatestVersionString { get; private set; } = string.Empty;
        public static string CurrentVersionString { get; private set; } = RussifierTemp.ModVersion;

        public static string GetReleasesPageUrl()
        {
            return UpdateUrl;
        }

        public static async Task CheckForUpdates()
        {
            IsUpdateAvailable = false; 
            LatestVersionString = string.Empty;
            CurrentVersionString = RussifierTemp.ModVersion;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TWGSRussifierUpdateChecker", "1.0"));
                    
                    string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        
                        Regex tagRegex = new Regex("\"tag_name\"\\s*:\\s*\"([^\"]+)\"");
                        Match match = tagRegex.Match(jsonResponse);
                        
                        if (match.Success && match.Groups.Count >= 2)
                        {
                            string latestVersionTag = match.Groups[1].Value;
                            
                            if (!string.IsNullOrEmpty(latestVersionTag))
                            {
                              //  Logger.Info($"Последняя версия на GitHub ({RepoOwner}/{RepoName}): {latestVersionTag}");
                                Version currentModVersion = new Version(CurrentVersionString);
                                string sanitizedLatestVersion = latestVersionTag.StartsWith("v") ? latestVersionTag.Substring(1) : latestVersionTag;
                                
                                try
                                {
                                    Version latestGitHubVersion = new Version(sanitizedLatestVersion);

                                    if (latestGitHubVersion > currentModVersion)
                                    {
                                        Logger.Warning($"Доступна новая версия мода: {latestVersionTag}! Текущая версия: v{CurrentVersionString}");
                                        IsUpdateAvailable = true;
                                        LatestVersionString = latestVersionTag;
                                    }
                                    else
                                    {
                                        Logger.Info("Установлена последняя версия мода.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                  //  Logger.Error($"Ошибка при сравнении версий: {ex.Message}");
                                }
                            }
                            else
                            {
                              //  Logger.Error("Получен пустой тег версии из ответа GitHub API.");
                            }
                        }
                        else
                        {
                           // Logger.Error("Не удалось найти тег версии в ответе GitHub API.");
                        }
                    }
                    else
                    {
                       // Logger.Error($"Ошибка при запросе к GitHub API ({RepoOwner}/{RepoName}): {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Logger.Error($"Исключение при проверке обновлений: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
} 