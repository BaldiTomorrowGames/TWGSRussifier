using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TWGSRussifier.Patches
{
    internal class ArrowPositioner : MonoBehaviour
    {
        private TextMeshProUGUI textComponent;
        private RectTransform arrowRect;

        public void Initialize(TextMeshProUGUI text, RectTransform arrow)
        {
            textComponent = text;
            arrowRect = arrow;
        }

        void LateUpdate()
        {
            if (textComponent != null && arrowRect != null)
            {
                textComponent.ForceMeshUpdate();
                var textInfo = textComponent.textInfo;

                if (textInfo.characterCount > 0)
                {
                    var lastVisibleCharInfo = textInfo.characterInfo[textInfo.characterCount - 1];
                    float textEdgeX = lastVisibleCharInfo.topRight.x;

                    arrowRect.anchorMin = new Vector2(0.5f, 0.5f);
                    arrowRect.anchorMax = new Vector2(0.5f, 0.5f);
                    arrowRect.pivot = new Vector2(0.5f, 0.5f);
                    arrowRect.anchoredPosition = new Vector2(textEdgeX + 5f, 2.4f);
                }
            }
        }
    }

    [HarmonyPatch] 
    internal class MainMenuPatch
    {
        private static readonly Dictionary<string, string> LocalizationKeys = new Dictionary<string, string>()
        {
            { "StartTest", "TWGS_Menu_TestMapText" },     
            { "StartTest_1", "TWGS_Menu_TestMapText_1" },
            { "Reminder", "TWGS_Menu_Reminder" },
            { "ModInfo", "TWGS_Menu_ModInfo" }
        };
        
        private static readonly List<SocialMediaInfo> SocialMediaLinks = new List<SocialMediaInfo>()
        {
            new SocialMediaInfo("TelegramButton", "TWGS_Menu_Telegram", "https://t.me/translate_balda"),
            new SocialMediaInfo("DiscordButton", "TWGS_Menu_Discord", "https://discord.gg/cGmxrmp5Tj"),
            new SocialMediaInfo("GameBananaButton", "TWGS_Menu_GameBanana", "https://gamebanana.com/mods/597541"),
            new SocialMediaInfo("YouTubeButton", "TWGS_Menu_YouTube", "https://www.youtube.com/@DragTors"),
            new SocialMediaInfo("GithubButton", "TWGS_Menu_Github", "https://github.com/BaldiTomorrowGames/TWGSRussifier")
        };
        
        private class SocialMediaInfo
        {
            public string ButtonName { get; private set; }
            public string LocalizationKey { get; private set; }
            public string Url { get; private set; }
            
            public SocialMediaInfo(string buttonName, string localizationKey, string url)
            {
                ButtonName = buttonName;
                LocalizationKey = localizationKey;
                Url = url;
            }
        }

        private static readonly List<KeyValuePair<string, Vector2>> SizeDeltaTargets = new List<KeyValuePair<string, Vector2>>
        {
            new KeyValuePair<string, Vector2>("StartTest", new Vector2(210f, 32f)),
            new KeyValuePair<string, Vector2>("StartTest_1", new Vector2(228f, 32f))
        };

        private static Transform FindInChildrenIncludingInactive(Transform parent, string path)
        {
            foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == path)
                {
                    return child;
                }
            }
            
             var children = parent.GetComponentsInChildren<Transform>(true); 
            foreach (var child in children)
            {
                if (child == parent) continue;
                if (DoesPathMatch(parent, child, path)) 
                {
                    return child;
                }
            }
            return null;
        }

        private static bool DoesPathMatch(Transform parent, Transform target, string expectedPath)
        {
            if (target == null || parent == null || target == parent) return false;
            StringBuilder pathBuilder = new StringBuilder();
            Transform current = target;
            while (current != null && current != parent)
            {
                if (pathBuilder.Length > 0)
                    pathBuilder.Insert(0, "/");
                pathBuilder.Insert(0, current.name);
                current = current.parent;
            }
            if (current != parent) return false;
            return pathBuilder.ToString() == expectedPath;
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "Menu" && value)
                {
                    ApplySizeChanges(__instance.transform);
                    ApplyLocalization(__instance.transform);
                    CreateModInfoButton(__instance.transform);
                }
            }
        }

        private static GameObject socialLinksPanel = null;
        private static bool dropdownVisible = false;
        private static RectTransform dropdownArrow = null;
        
        private static void CreateModInfoButton(Transform rootTransform)
        {
            if (rootTransform == null) return;

            Transform reminderTransform = rootTransform.Find("Reminder");
            if (reminderTransform == null) return;

            Transform existingModInfo = rootTransform.Find("ModInfo");
            if (existingModInfo != null) return;

            GameObject modInfo = GameObject.Instantiate(reminderTransform.gameObject, rootTransform);
            modInfo.name = "ModInfo";

            modInfo.transform.localPosition = new Vector3(-180f, 155f, 0f);
            modInfo.transform.SetSiblingIndex(15);
            
            RectTransform rectTransform = modInfo.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(118f, 50f);
                rectTransform.offsetMin = new Vector2(-239f, 160f);
            }

            TextMeshProUGUI textComponent = modInfo.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.raycastTarget = true;

                TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                }
                
                GameObject arrowObject = new GameObject("DropdownArrow", typeof(RectTransform));
                arrowObject.transform.SetParent(modInfo.transform, false);
                
                RectTransform arrowRect = arrowObject.GetComponent<RectTransform>();
                // arrowRect.anchorMin = new Vector2(1, 0.5f);
                // arrowRect.anchorMax = new Vector2(1, 0.5f);
                // arrowRect.pivot = new Vector2(0.5f, 0.5f);
                // arrowRect.anchoredPosition = new Vector2(2, 2.4f); 
                arrowRect.sizeDelta = new Vector2(8f, 8f);
                
                modInfo.AddComponent<ArrowPositioner>().Initialize(textComponent, arrowRect);
                
                GameObject triangle = new GameObject("Triangle", typeof(RectTransform));
                triangle.transform.SetParent(arrowObject.transform, false);
                
                RectTransform triangleRect = triangle.GetComponent<RectTransform>();
                triangleRect.anchorMin = Vector2.zero;
                triangleRect.anchorMax = Vector2.one;
                triangleRect.sizeDelta = Vector2.zero;
                
                TriangleImage triangleUI = triangle.AddComponent<TriangleImage>();
                triangleUI.color = new Color(0.5f, 0.5f, 0.5f, 1f);     
                
                dropdownArrow = arrowRect;
                
                localizer.key = "TWGS_Menu_ModInfo";
                localizer.RefreshLocalization();

                StandardMenuButton button = textComponent.gameObject.AddComponent<StandardMenuButton>();
                button.OnPress = new UnityEvent();
                button.OnHighlight = new UnityEvent();
                button.OnRelease = new UnityEvent();
                button.OffHighlight = new UnityEvent();
                button.underlineOnHigh = true;
                button.text = textComponent;
                button.gameObject.tag = "Button";

                button.OnPress.AddListener(() => { ToggleSocialLinksDropdown(rootTransform, modInfo); });
            }
            
            CreateSocialLinksPanel(rootTransform, modInfo);
        }

        private static void CreateSocialLinksPanel(Transform rootTransform, GameObject modInfoButton)
        {
            GameObject panel = new GameObject("SocialLinksPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(rootTransform, false);
            
            panel.transform.SetSiblingIndex(16);
            
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            
            panelRect.anchoredPosition = new Vector2(145, -116);
            
            float buttonHeight = 35f;
            float topBottomPadding = 20f;
            float panelHeight = (SocialMediaLinks.Count * buttonHeight) + topBottomPadding;
            
            panelRect.sizeDelta = new Vector2(130f, panelHeight);
            
            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            for (int i = 0; i < SocialMediaLinks.Count; i++)
            {
                SocialMediaInfo info = SocialMediaLinks[i];
                CreateSocialButton(panel, info.ButtonName, info.Url, i);
            }

            panel.SetActive(false);
            socialLinksPanel = panel;
        }
        
        private static void CreateSocialButton(GameObject parent, string buttonName, string url, int index)
        {
            GameObject buttonObj = new GameObject(buttonName, typeof(RectTransform), typeof(TextMeshProUGUI));
            buttonObj.transform.SetParent(parent.transform, false);
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(0.5f, 1);
            buttonRect.anchoredPosition = new Vector3(0, -10 - (index * 35), 0);
            buttonRect.sizeDelta = new Vector2(0, 30);
            
            TextMeshProUGUI textComponent = buttonObj.GetComponent<TextMeshProUGUI>();
            textComponent.fontSize = 16;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.raycastTarget = true;
            
            TextLocalizer localizer = buttonObj.AddComponent<TextLocalizer>();
            
            string localizationKey = SocialMediaLinks.Find(x => x.ButtonName == buttonName)?.LocalizationKey;
            if (string.IsNullOrEmpty(localizationKey))
            {
                localizationKey = buttonName;
            }
            
            localizer.key = localizationKey;
            localizer.RefreshLocalization();
            
            StandardMenuButton button = buttonObj.AddComponent<StandardMenuButton>();
            button.OnPress = new UnityEvent();
            button.OnHighlight = new UnityEvent();
            button.OnRelease = new UnityEvent();
            button.OffHighlight = new UnityEvent();
            button.underlineOnHigh = true;
            button.text = textComponent;
            button.gameObject.tag = "Button";
            
            button.OnPress.AddListener(() => { 
                Application.OpenURL(url);
                ToggleSocialLinksDropdown(null, null); 
            });
        }
        
        private static void ToggleSocialLinksDropdown(Transform rootTransform, GameObject modInfoButton)
        {
            if (socialLinksPanel == null && rootTransform != null && modInfoButton != null)
            {
                CreateSocialLinksPanel(rootTransform, modInfoButton);
            }
            
            if (socialLinksPanel != null)
            {
                dropdownVisible = !dropdownVisible;
                socialLinksPanel.SetActive(dropdownVisible);
                
                if (dropdownArrow != null)
                {
                    dropdownArrow.rotation = Quaternion.Euler(0, 0, dropdownVisible ? -90f : 0f);
                    
                    TriangleImage triangleUI = dropdownArrow.GetComponentInChildren<TriangleImage>();
                    if (triangleUI != null)
                    {
                        triangleUI.SetVerticesDirty();
                    }
                }
            }
        }

        private static void ApplySizeChanges(Transform rootTransform)
        {
            if (rootTransform == null) return;
            
            foreach (var target in SizeDeltaTargets)
            {
                Transform elementTransform = FindInChildrenIncludingInactive(rootTransform, target.Key); 
                if (elementTransform != null)
                {
                    RectTransform rectTransform = elementTransform.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        if (rectTransform.sizeDelta != target.Value)
                            rectTransform.sizeDelta = target.Value;
                    }
                }
            }
        }

        private class TriangleImage : UnityEngine.UI.Graphic
        {
            protected override void OnPopulateMesh(VertexHelper vh)
            {
                vh.Clear();
                
                Vector2 center = rectTransform.rect.center;
                float width = rectTransform.rect.width;
                float height = rectTransform.rect.height;
                
                UIVertex vert = UIVertex.simpleVert;
                vert.color = color;
                
                vert.position = new Vector3(center.x + width/2, center.y, 0);
                vh.AddVert(vert);
                
                vert.position = new Vector3(center.x - width/2, center.y + height/2, 0);
                vh.AddVert(vert);
                
                vert.position = new Vector3(center.x - width/2, center.y - height/2, 0);
                vh.AddVert(vert);
                
                vh.AddTriangle(0, 1, 2);
            }
        }
        
        private static void ApplyLocalization(Transform rootTransform)
        {
             if (rootTransform == null) return;
            
            foreach (var entry in LocalizationKeys)
            {
                string relativePath = entry.Key; 
                string localizationKey = entry.Value;
                
                Transform targetTransform = FindInChildrenIncludingInactive(rootTransform, relativePath); 
                if (targetTransform != null)
                {
                    TextMeshProUGUI textComponent = targetTransform.GetComponent<TextMeshProUGUI>();
                    
                    if (textComponent == null) 
                    {
                        Transform textChild = targetTransform.Find("Text (TMP)");
                        if (textChild != null)
                        {
                             textComponent = textChild.GetComponent<TextMeshProUGUI>();
                        }
                    }

                    if (textComponent != null)
                    {
                        TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                        if (localizer == null)
                        {
                            localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                            localizer.key = localizationKey;
                            localizer.RefreshLocalization(); 
                        }
                        else if (localizer.key != localizationKey)
                        {
                             localizer.key = localizationKey;
                             localizer.RefreshLocalization();
                        }
                    }
                }
            }
        }
    }
} 