using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;

namespace TWGSRussifier
{
    internal class NameEntryPatch
    {
        private static bool fixesApplied = false;
        
        
        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class ClipboardScreenSetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && __instance.name == "ClipboardScreen")
                {
                    NameManager nameManager = NameManager.nm;
                    if (nameManager != null)
                    {
                        ApplyNewFileButtonFixes(nameManager);
                    }
                }
                
                if (__instance.name == "Menu" && value)
                {
                    fixesApplied = false;
                }
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Awake")]
        private static class NameManagerAwakePatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Load")]
        private static class LoadPatch
        {
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                ApplyNewFileButtonFixes(__instance);
            }
        }
        
        private static void ApplyNewFileButtonFixes(NameManager nameManager)
        {
            if (nameManager == null || fixesApplied) return;
            
            if (nameManager.newFileButton != null)
            {
                RectTransform buttonRect = nameManager.newFileButton.GetComponent<RectTransform>();
                if (buttonRect != null)
                {
                    buttonRect.sizeDelta = new Vector2(158f, 30f);
                    Debug.Log("[NameEntryPatch] Изменен размер кнопки NewFileButton");
                }
                
                Transform textComponent = nameManager.newFileButton.transform.Find("Text (TMP)");
                if (textComponent != null)
                {
                    RectTransform textRect = textComponent.GetComponent<RectTransform>();
                    if (textRect != null)
                    {
                        textRect.anchoredPosition = new Vector2(1f, 0f);
                        textRect.sizeDelta = new Vector2(150f, 32f);
                        
                        Debug.Log("[NameEntryPatch] Применены исправления для текста кнопки NewFileButton");
                        fixesApplied = true;
                    }
                }
            }
        }
        
        
        [HarmonyPatch(typeof(NameManager), "Update")]
        private static class UpdatePatch
        {
            private static int frameCount = 0;
            private static readonly int MAX_ATTEMPTS = 5;
            
            [HarmonyPostfix]
            private static void Postfix(NameManager __instance)
            {
                if (!fixesApplied && frameCount < MAX_ATTEMPTS)
                {
                    frameCount++;
                    ApplyNewFileButtonFixes(__instance);
                }
                
                if (fixesApplied)
                {
                    frameCount = 0;
                }
            }
        }
    }
} 