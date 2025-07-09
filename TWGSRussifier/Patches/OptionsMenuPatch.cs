using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier
{
    [HarmonyPatch(typeof(OptionsMenu), "Awake")]
    internal class OptionsMenuPatch
    {
        private static readonly List<KeyValuePair<string, Vector2>> AnchoredPositionTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("General/FlashToggle/ToggleText", new Vector2(-8f, 12f)),
            new KeyValuePair<string, Vector2>("Data/Main/DeleteFileButton", new Vector2(0f, -123f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetTripScoresButton", new Vector2(0f, -65f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetEndlessScoresButton", new Vector2(0f, -10f)),
            new KeyValuePair<string, Vector2>("Graphics/PixelFilterToggle/HotSpot", new Vector2(-14f, -20f)),

            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton/MapperButton", new Vector2(5f, 0f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamInputButton", new Vector2(4f, 0f)),
            
            new KeyValuePair<string, Vector2>("Graphics/FullScreenToggle/ToggleText", new Vector2(-6f, 0f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("General/FlashToggle/HotSpot", new Vector2(232f, 60f)),
            new KeyValuePair<string, Vector2>("Graphics/PixelFilterToggle/HotSpot", new Vector2(300f, 60f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton", new Vector2(370f, 32f)),
            new KeyValuePair<string, Vector2>("Graphics/FullScreenToggle/HotSpot", new Vector2(265f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/MapperButton/MapperButton", new Vector2(400f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamInputButton", new Vector2(360f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton", new Vector2(355f, 32f)),
            new KeyValuePair<string, Vector2>("ControlsTemp/SteamButton/SteamDesc", new Vector2(340f, 128f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMinTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("Data/Main/ResetEndlessScoresButton", new Vector2(-160f, -60f)),
            new KeyValuePair<string, Vector2>("Data/Main/ResetTripScoresButton", new Vector2(-160f, -110f))
        };

        private static readonly List<KeyValuePair<string, Vector2>> OffsetMaxTargets = new List<KeyValuePair<string, Vector2>>
        {
        };

        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "ControlsTemp/MapperButton/MapperButton", "TWGS_Menu_MapperButtonText" },
            { "ControlsTemp/SteamButton/SteamInputButton", "TWGS_Menu_SteamInputText" },
            { "ControlsTemp/SteamButton/SteamDesc", "TWGS_Menu_SteamDescText" },
        };

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenu), "Awake")]
        private static void Awake_Postfix(OptionsMenu __instance)
        {
            ApplyChanges(__instance);
            ApplyChangesToAllApplyButtons(__instance);
            
            ApplyLocalization(__instance);
        }

        private static void ApplyChangesToAllApplyButtons(OptionsMenu optionsMenuInstance)
        {
            if (optionsMenuInstance == null) return;
            
            Transform optionsTransform = optionsMenuInstance.transform;
            var children = optionsTransform.GetComponentsInChildren<Transform>(true);

            foreach (var child in children)
            {
                if (child.name == "ApplyButton")
                {
                    RectTransform rectTransform = child.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = new Vector2(115f, -160f);
                        rectTransform.sizeDelta = new Vector2(140f, 32f);

                        Transform applyTextTransform = child.Find("ApplyText");
                        if (applyTextTransform != null)
                        {
                            RectTransform applyTextRect = applyTextTransform.GetComponent<RectTransform>();
                            if (applyTextRect != null)
                            {
                                applyTextRect.sizeDelta = new Vector2(132f, 32f);
                            }
                        }
                    }
                }
            }
        }

        private static void ApplyChanges(OptionsMenu optionsMenuInstance)
        {
            Transform optionsTransform = optionsMenuInstance.transform;

            optionsTransform.SetAnchoredPositions(AnchoredPositionTargets);
            optionsTransform.SetSizeDeltas(SizeDeltaTargets);
            optionsTransform.SetOffsetMins(OffsetMinTargets);
            optionsTransform.SetOffsetMaxs(OffsetMaxTargets);
        }

        private static void ApplyLocalization(OptionsMenu optionsMenuInstance)
        {
            if (optionsMenuInstance == null) return;
            optionsMenuInstance.transform.ApplyLocalizations(LocalizationKeys);
        }
    }
} 