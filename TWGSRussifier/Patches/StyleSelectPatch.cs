using HarmonyLib;
using UnityEngine;
using TMPro;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(StandardMenuButton), "Highlight")]
    internal class StyleSelectPatch
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
            localizer.key = "TWGS_Men_NullStyleDesc";
            localizer.RefreshLocalization();
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

