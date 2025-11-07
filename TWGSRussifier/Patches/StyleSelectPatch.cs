using HarmonyLib;
using UnityEngine;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    internal class StyleSelectPatch
    {
        private static bool shouldApplyLocalization = false;
        
        [HarmonyPatch(typeof(StandardMenuButton), "Highlight")]
        private static class HighlightPatch
        {
            [HarmonyPostfix]
            private static void Postfix(StandardMenuButton __instance)
            {
                if (__instance == null || __instance.transform == null) return;
                
                if (Singleton<PlayerFileManager>.Instance != null && 
                    Singleton<PlayerFileManager>.Instance.flags[4])
                {
                    return;
                }
                
                string objectPath = GetGameObjectPath(__instance.transform);
                
                if (objectPath.Contains("StyleSelect") && objectPath.Contains("Baldi"))
                {
                    ApplyLocalizationToDescription(__instance);
                }
            }
        }

        [HarmonyPatch(typeof(GameLoader), "SetStyle")]
        private static class SetStylePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameLoader __instance, int style)
            {
                if (style == 3)
                {
                    shouldApplyLocalization = true;
                }
            }
        }

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (value && shouldApplyLocalization && __instance.name == "ModeSelect")
                {
                   // API.Logger.Info("ModeSelect активирован, применяем локализацию");
                    ApplyLocalizationToCurrentStyle(__instance.transform);
                    shouldApplyLocalization = false;
                }
            }
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                shouldApplyLocalization = false;
            }
        }

        private static void ApplyLocalizationToDescription(StandardMenuButton button)
        {
            if (button == null) return;

            Transform? styleSelectTransform = FindStyleSelectParent(button.transform);
            if (styleSelectTransform == null) return;

            Transform? descriptionTransform = styleSelectTransform.Find("Description");
            if (descriptionTransform == null) return;

            TextMeshProUGUI? textComponent = descriptionTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null) return;

            TextLocalizer? existingLocalizer = textComponent.GetComponent<TextLocalizer>();
            if (existingLocalizer != null)
            {
                Object.DestroyImmediate(existingLocalizer);
            }

            TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
            localizer.key = "TWGS_NullStyle_Desc";
            localizer.RefreshLocalization();
        }

        private static void ApplyLocalizationToCurrentStyle(Transform rootTransform)
        {
            if (rootTransform == null)
            {
               // API.Logger.Warning("rootTransform is null");
                return;
            }

            try
            {
                Transform? currentStyleTransform = rootTransform.Find("CurrentStyle");
                if (currentStyleTransform == null)
                {
                   // API.Logger.Warning("CurrentStyle не найден в ModeSelect");
                    return;
                }

                Transform? textTransform = currentStyleTransform.Find("Text (TMP)");
                if (textTransform == null)
                {
                  // API.Logger.Warning("Text (TMP) не найден в CurrentStyle");
                    return;
                }

                TextMeshProUGUI? textComponent = textTransform.GetComponent<TextMeshProUGUI>();
                if (textComponent == null)
                {
                   // API.Logger.Warning("TextMeshProUGUI компонент не найден");
                    return;
                }

                TextLocalizer? existingLocalizer = textComponent.GetComponent<TextLocalizer>();
                if (existingLocalizer != null)
                {
                    Object.DestroyImmediate(existingLocalizer);
                }

                TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                localizer.key = "TWGS_Men_NullStyle";
                localizer.RefreshLocalization();

                // API.Logger.Info("Локализация успешно применена к CurrentStyle/Text");
            }
            catch (System.Exception)
            {
                // API.Logger.Error($"Ошибка при применении локализации к CurrentStyle: {ex.Message}");
            }
        }

        private static Transform? FindStyleSelectParent(Transform child)
        {
            Transform? current = child;
            
            while (current != null)
            {
                if (current.name == "StyleSelect")
                {
                    return current;
                }
                current = current.parent;
            }
            
            return null;
        }

        private static string GetGameObjectPath(Transform transform)
        {
            if (transform == null) return "";
            
            string path = transform.name;
            Transform current = transform.parent;
            
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }
            
            return path;
        }
    }
}

