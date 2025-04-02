using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace TWGSRussifier.Patches
{
    public class TextLocalizer : MonoBehaviour
    {
        public string key;
        private TextMeshProUGUI textComponent;
        
        // Init TextMeshPro
        private void Awake()
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        // Init LocalizationManager
        private void Update()
        {
            if (textComponent != null && !string.IsNullOrEmpty(key) && Singleton<LocalizationManager>.Instance != null)
            {
                string localizedText = Singleton<LocalizationManager>.Instance.GetLocalizedText(key);
                if (!string.IsNullOrEmpty(localizedText) && textComponent.text != localizedText)
                {
                    textComponent.text = localizedText;
                }
            }
        }
    }

    public class TextFixes : MonoBehaviour
    {
        private static readonly Dictionary<string, Vector2> positionFixes = new Dictionary<string, Vector2>()
        {
            // RectTransform Position
            { "PickMode/Tutorial", new Vector2(120f, 4f) },
            { "PickMode/FieldTrips", new Vector2(-120f, 4f) },
            { "PickMode/TutorialPrompt/YesButton", new Vector2(90, -105) },
            { "PickMode/TutorialPrompt/NoButton/Text", new Vector2(0, -9) },
            { "Options/Graphics/ApplyButton", new Vector2(115f, -160f) },
            { "Options/General/FlashToggle/ToggleText", new Vector2(-8f, 12f) },
            { "Options/Data/Main/DeleteFileButton", new Vector2(0f, -123f) },
            { "Options/Data/Main/ResetTripScoresButton", new Vector2(0f, -65f) },
            { "Options/Data/Main/ResetEndlessScoresButton", new Vector2(0f, -10f) },
            { "Options/ControlsTemp/MapperButton/MapperButton", new Vector2(5f, 0f) },
            { "Options/ControlsTemp/SteamButton/SteamInputButton", new Vector2(4f, 0f) },
            { "NameEntry/ClipboardScreen/NewFileButton/Text (TMP)", new Vector2(1f, 0f) },
            { "Score/Rank", new Vector2(115, 13) }

        };

        private static readonly Dictionary<string, Vector2> sizeFixes = new Dictionary<string, Vector2>()
        {
            // Size Delta Position
            { "Menu/StartTest", new Vector2(210, 32) },
            { "Menu/StartTest_1", new Vector2(228, 32) },
            { "Options/ControlsTemp/MapperButton/MapperButton", new Vector2(400f, 32f) },
            { "Options/Graphics/ApplyButton/ApplyText", new Vector2(132f, 32f) },
            { "Options/ControlsTemp/SteamButton/SteamInputButton", new Vector2(360f, 32f) },
            { "Options/ControlsTemp/SteamButton/SteamDesc", new Vector2(340f, 128f) },
            { "HideSeekMenu/MainContinue", new Vector2(400f, 32f) },
            { "MinigameHUD/ScoreIndicatorBase/ScoreIndicator", new Vector2(103f, 32f) },
            { "PickEndlessMap/Random", new Vector2(134f, 100f) },
            { "NameEntry/ClipboardScreen/NewFileButton/Text (TMP)", new Vector2(150f, 32f) },
            { "PickMode/TutorialPrompt/NoButton/Text", new Vector2(155, 32) }
        };

        private static readonly Dictionary<string, Vector2> anchorMinFixes = new Dictionary<string, Vector2>()
        {
            // Anchored Position
            { "ElevatorTransission/BigScreen/GradeValue", new Vector2(0.56f, 0.5f) }
        };

        private static readonly Dictionary<string, string> localizationKeys = new Dictionary<string, string>()
        {
            // Menu Localization Keys
            { "PickEndlessMap/Text (TMP)", "TWGS_Menu_EndlessMapText" },
            { "Options/ControlsTemp/MapperButton/MapperButton", "TWGS_Menu_MapperButtonText" },
            { "Options/ControlsTemp/SteamButton/SteamInputButton", "TWGS_Menu_SteamInputText" },
            { "Options/ControlsTemp/SteamButton/SteamDesc", "TWGS_Menu_SteamDescText" },
            { "NameEntry/SaveError/Text (TMP)", "TWGS_Menu_SaveErrorText" },
            { "ElevatorTransission/Error", "TWGS_Menu_ErrorText" },
            { "Score/Text", "TWGS_Menu_ScoreText" },
            { "ChallengeWin/Canvas/Text (TMP)", "TWGS_Menu_WinChallengeText" },
            { "Menu/StartTest", "TWGS_Menu_TestMapText" },
            { "Menu/StartTest_1", "TWGS_Menu_TestMapText_1" },
            { "Score/Congrats", "TWGS_Menu_NewBestText" },

            // About Localization Keys
            { "About/DevUpdateTitle", "TWGS_About_DevUpdateTitle" },
            { "About/DevUpdateText", "TWGS_About_DevUpdateText" },
            { "About/Credits", "TWGS_About_CreditsText" },
            { "About/WebsiteButton", "TWGS_About_WebsiteButtonText" },
            { "About/DevlogsButton", "TWGS_About_DevlogsButtonText" },
            { "About/BugsButton", "TWGS_About_BugsButtonText" },
            { "About/SaveFolderButton", "TWGS_About_SaveFolderButtonText" },
            { "About/AnniversaryButton", "TWGS_About_AnniversaryButtonText" },
            { "About/RoadmapButton", "TWGS_About_RoadmapButtonText" },

            // Credits Localization Keys 
            { "Main Credits (5)/Text", "TWGS_Credits_ThankYouText" },
            { "Main Credits (3.5)/Text", "TWGS_Credits_SoundsFromText" },
            { "Main Credits (3.5)/TrademarkText", "TWGS_Credits_WarnerDisclaimerText" },
            { "Main Credits (2)/Text", "TWGS_Credits_TestingFeedbackText" },
            { "Main Credits (2)/Text (1)", "TWGS_Credits_TutorialsText" },
            { "Main Credits (2)/Text (2)", "TWGS_Credits_OtherTestersText" },
            { "Main Credits (1)/Text", "TWGS_Credits_VoicesText" },
            { "Main Credits (1)/Text (1)", "TWGS_Credits_ArtistsText" },
            { "Main Credits (4)/Text", "TWGS_Credits_MusicText" },
            { "Main Credits (4)/Text (1)", "TWGS_Credits_SpecialThanksText" },
            { "Main Credits (4)/Text (2)", "TWGS_Credits_BibleVerseText" },
            { "Main Credits (3)/Text", "TWGS_Credits_ToolsText" },
            { "Main Credits (3)/Text (1)", "TWGS_Credits_AssetsText" },
            { "Main Credits (3.75)/Text", "TWGS_Credits_OpenSourceText" },
            { "Main Credits (3.75)/LicenseText", "TWGS_Credits_LicenseText" },
            { "Main Credits/Text", "TWGS_Credits_MainTitleText" },
            { "Main Credits/TrademarkText", "TWGS_Credits_UnityDisclaimerText" },

            // Pause Localization Keys
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/ResumeButton", "TWGS_Pause_ResumeButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/OptionsButton", "TWGS_Pause_OptionsButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/QuitButton", "TWGS_Pause_QuitButton" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/PauseLabel", "TWGS_Pause_PauseLabel" },
            { "CoreGameManager(Clone)/PauseMenuScreens/PauseScreen/Main/SeedLabel", "TWGS_Pause_SeedButton" },

        };

        private bool isSaveErrorTextUpdated = false;

        // Init Method (Load on TPPlugin.cs)
        public static void Init()
        {
            if (GameObject.Find("TextLocationFixBehaviour") == null)
            {
                GameObject go = new GameObject("TextLocationFixBehaviour");
                go.AddComponent<TextFixes>();
                UnityEngine.Object.DontDestroyOnLoad(go);
                // Debug.Log("[TextLocationFix] Загрузка...");
            }
        }

        // Update Initialize 
        private void Update()
        {
            // Init RectTransform, Size Delta and Anchored Positions
            UpdatePositions();
            UpdateSizes();
            UpdateAnchorMins();
            // Init Text Replace
            UpdateTexts();
            
            // Elevator YTPs Replace
            FixTextReplace("ElevatorTransission/BigScreen/GradeBonusValue");
            FixTextReplace("ElevatorTransission/BigScreen/TimeBonusValue");
        }


        // Main Functions
        private void UpdatePositions()
        {
            foreach (var kvp in positionFixes)
            {
                FixAnchoredPosition(kvp.Key, kvp.Value);
            }
        }


        private string GetPathForObject(GameObject obj)
        {
            Stack<string> pathParts = new Stack<string>();
            Transform current = obj.transform;
            
            while (current != null)
            {
                pathParts.Push(current.name);
                current = current.parent;
            }
            
            return string.Join("/", pathParts);
        }

        private void UpdateSizes()
        {
            foreach (var kvp in sizeFixes)
            {
                FixSizeDelta(kvp.Key, kvp.Value);
            }
        }

        private void UpdateAnchorMins()
        {
            foreach (var kvp in anchorMinFixes)
            {
                FixAnchorMin(kvp.Key, kvp.Value);
            }
        }

        private void UpdateTexts()
        {
            TextMeshProUGUI[] texts = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
            foreach (var tmp in texts)
            {
                string fullPath = GetFullPathForGameObject(tmp.gameObject);
                if (localizationKeys.ContainsKey(fullPath))
                {
                    string localizationKey = localizationKeys[fullPath];
                    
                    TextLocalizer localizer = tmp.GetComponent<TextLocalizer>();
                    
                    if (localizer == null)
                    {
                        localizer = tmp.gameObject.AddComponent<TextLocalizer>();
                        localizer.key = localizationKey; 
                        // Debug.Log($"Added TextLocalizer component to {fullPath} with key {localizationKey}");
                    }
                    else
                    {
                        if (localizer.key != localizationKey)
                        {
                             localizer.key = localizationKey;
                             // Debug.Log($"Updated TextLocalizer key for {fullPath} to {localizationKey}");
                        }
                    }
                    
                    if (Singleton<LocalizationManager>.Instance != null)
                    {
                        string localizedText = Singleton<LocalizationManager>.Instance.GetLocalizedText(localizationKey);
                        
                        if (fullPath == "NameEntry/SaveError/Text (TMP)" && isSaveErrorTextUpdated)
                            continue;

                        if (!string.IsNullOrEmpty(localizedText) && tmp.text != localizedText)
                        {
                             tmp.text = localizedText;
                        }
                        
                        if (fullPath == "NameEntry/SaveError/Text (TMP)")
                            isSaveErrorTextUpdated = true;
                        
                        // Debug.Log($"TextFixes: Текст объекта {fullPath} обновлён по ключу {localizationKey}");
                    }
                }
            }
        }

        private string GetFullPathForGameObject(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;
            
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            
            return path;
            }

            private void FixAnchoredPosition(string fullPath, Vector2 targetPos)
            {
                RectTransform rt = FindRectTransform(fullPath);
                if (rt != null && rt.anchoredPosition != targetPos)
                {
                    rt.anchoredPosition = targetPos;
                    // Debug.Log($"Fixed {fullPath} anchoredPosition to {targetPos}");
                }
            }

            private void FixSizeDelta(string fullPath, Vector2 targetSize)
            {
                RectTransform rt = FindRectTransform(fullPath);
                if (rt != null && rt.sizeDelta != targetSize)
                {
                    rt.sizeDelta = targetSize;
                    // Debug.Log($"Fixed {fullPath} sizeDelta to {targetSize}");
                }
            }

            private void FixAnchorMin(string fullPath, Vector2 targetAnchorMin)
            {
                RectTransform rt = FindRectTransform(fullPath);
                if (rt != null && rt.anchorMin != targetAnchorMin)
                {
                    rt.anchorMin = targetAnchorMin;
                    // Debug.Log($"Fixed {fullPath} anchorMin to {targetAnchorMin}");
                }
            }

            private void FixText(string fullPath, string targetText, Action onFix = null)
            {
                TextMeshProUGUI tmp = FindComponent<TextMeshProUGUI>(fullPath);
                if (tmp != null && tmp.text != targetText)
                {
                    tmp.text = targetText;
                    // Debug.Log($"Fixed {fullPath} text to '{targetText}'");
                    onFix?.Invoke();
                }
            }

            private void FixTextReplace(string fullPath)
            {
                TextMeshProUGUI tmp = FindComponent<TextMeshProUGUI>(fullPath);
                if (tmp != null && tmp.text.Contains("YTPs"))
                {
                    string newText = tmp.text.Replace("YTPs", "ОТМ");
                    tmp.text = newText;
                
                TextLocalizer localizer = tmp.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = tmp.gameObject.AddComponent<TextLocalizer>();
                    if (fullPath == "ElevatorTransission/BigScreen/GradeBonusText")
                        localizer.key = "TWGS_GradeBonusText";
                    else if (fullPath == "ElevatorTransission/BigScreen/TimeBonusText")
                        localizer.key = "TWGS_TimeBonusText";
                }
                
                // Debug.Log($"Fixed {fullPath} text replacement: now '{newText}'");
            }
        }

            private RectTransform FindRectTransform(string fullPath)
            {
                GameObject go = GameObject.Find(fullPath);
                if (go != null)
                    return go.GetComponent<RectTransform>();
                return null;
            }

            private T FindComponent<T>(string fullPath) where T : Component
            {
                GameObject go = GameObject.Find(fullPath);
                if (go != null)
                    return go.GetComponent<T>();
                return null;
        }
    }
}
