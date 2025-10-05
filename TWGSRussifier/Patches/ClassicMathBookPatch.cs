using HarmonyLib;
using UnityEngine;
using TMPro;

namespace TWGSRussifier.Patches
{
    internal class ClassicMathBookPatch
    {
        [HarmonyPatch(typeof(ClickableSpecialFunctionTrigger), "Clicked")]
        private static class ClickableSpecialFunctionTriggerPatch
        {
            [HarmonyPostfix]
            private static void Postfix(ClickableSpecialFunctionTrigger __instance, int player)
            {
                if (__instance.name.Contains("BasementMathBook") || 
                    __instance.transform.parent?.name.Contains("BasementMathBook") == true ||
                    IsClassicBook(__instance.name) ||
                    (__instance.transform.parent != null && IsClassicBook(__instance.transform.parent.name)))
                {
                    ApplyBookLocalization(__instance.transform);
                }
            }
        }

        private static bool IsClassicBook(string name)
        {
            return name.Contains("ClassicBook_1") || name.Contains("ClassicBook_2") || 
                   name.Contains("ClassicBook_3") || name.Contains("ClassicBook_4") || 
                   name.Contains("ClassicBook_5") || name.Contains("ClassicBook_6") || 
                   name.Contains("ClassicBook_7") || name.Contains("ClassicBook_8") || 
                   name.Contains("ClassicBook_9") || name.Contains("ClassicBook_10") || 
                   name.Contains("ClassicBook_11") || name.Contains("ClassicBook_12") || 
                   name.Contains("ClassicBook_Kindergarten") || 
                   name.Contains("ClassicBook_Preschool") || 
                   name.Contains("ClassicBook_College");
        }

        private static void ApplyBookLocalization(Transform triggerTransform)
        {
            Transform? bookTransform = FindBookTransform(triggerTransform);
            
            if (bookTransform != null)
            {
                if (bookTransform.name.Contains("BasementMathBook"))
                {
                    Transform coverTextTransform = bookTransform.Find("Canvas/CoverText/TitleTMP");
                    if (coverTextTransform != null)
                    {
                        ApplyLocalizerToText(coverTextTransform, "TWGS_MathBook_Title");
                    }
                }
                else if (IsClassicBook(bookTransform.name))
                {
                    Transform titleTextTransform = bookTransform.Find("Canvas/CoverText/TitleTMP");
                    if (titleTextTransform != null)
                    {
                        ApplyLocalizerToText(titleTextTransform, "TWGS_ClassicBook_Title", true);
                    }
                    
                    Transform subTextTransform = bookTransform.Find("Canvas/CoverText/SubTMP");
                    if (subTextTransform != null)
                    {
                        string localizationKey = GetLocalizationKey(bookTransform.name);
                        ApplyLocalizerToText(subTextTransform, localizationKey);
                    }
                    
                    Transform authorTextTransform = bookTransform.Find("Canvas/CoverText/AuthorTMP");
                    if (authorTextTransform != null)
                    {
                        ApplyLocalizerToText(authorTextTransform, "TWGS_ClassicBook_Author");
                    }
                    
                    ApplyInsideTextLocalization(bookTransform);
                    
                    ApplySizeDeltaSettings(bookTransform);
                }
            }
        }

        private static void ApplyLocalizerToText(Transform textTransform, string localizationKey, bool applySpecialFontSize = false)
        {
            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TextLocalizer existingLocalizer = textComponent.GetComponent<TextLocalizer>();
                if (existingLocalizer == null)
                {
                    TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
                    localizer.key = localizationKey;
                    if (applySpecialFontSize)
                    {
                        textComponent.fontSize = 26;
                    }
                    localizer.RefreshLocalization();
                }
            }
        }

        private static void ApplyInsideTextLocalization(Transform bookTransform)
        {
            for (int i = 0; i <= 5; i++)
            {
                Transform textTransform = bookTransform.Find($"Canvas/InsideText/Text_{i}");
                if (textTransform != null)
                {
                    string localizationKey = $"TWGS_ClassicBook_Text_{i}";
                    ApplyLocalizerToText(textTransform, localizationKey);
                }
            }
        }

        private static void ApplySizeDeltaSettings(Transform bookTransform)
        {
            Transform text4Transform = bookTransform.Find("Canvas/InsideText/Text_4");
            if (text4Transform != null)
            {
                RectTransform rectTransform = text4Transform.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(199f, 100f);
                }
            }
            
            Transform text5Transform = bookTransform.Find("Canvas/InsideText/Text_5");
            if (text5Transform != null)
            {
                RectTransform rectTransform = text5Transform.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.sizeDelta = new Vector2(187f, 128f);
                }
            }
        }

        private static string GetBookIdentifier(string bookName)
        {
            if (bookName.Contains("ClassicBook_1")) return "1";
            if (bookName.Contains("ClassicBook_2")) return "2";
            if (bookName.Contains("ClassicBook_3")) return "3";
            if (bookName.Contains("ClassicBook_4")) return "4";
            if (bookName.Contains("ClassicBook_5")) return "5";
            if (bookName.Contains("ClassicBook_6")) return "6";
            if (bookName.Contains("ClassicBook_7")) return "7";
            if (bookName.Contains("ClassicBook_8")) return "8";
            if (bookName.Contains("ClassicBook_9")) return "9";
            if (bookName.Contains("ClassicBook_10")) return "10";
            if (bookName.Contains("ClassicBook_11")) return "11";
            if (bookName.Contains("ClassicBook_12")) return "12";
            if (bookName.Contains("ClassicBook_Kindergarten")) return "Kindergarten";
            if (bookName.Contains("ClassicBook_Preschool")) return "Preschool";
            if (bookName.Contains("ClassicBook_College")) return "College";
            
            return "Default";
        }
        private static string GetLocalizationKey(string bookName)
        {
            string bookIdentifier = GetBookIdentifier(bookName);
            return $"TWGS_ClassicBook_{bookIdentifier}";
        }

        private static Transform? FindBookTransform(Transform startTransform)
        {
            Transform current = startTransform;
            while (current != null)
            {
                if (current.name.Contains("BasementMathBook"))
                {
                    ClassicMathBook mathBook = current.GetComponent<ClassicMathBook>();
                    if (mathBook != null)
                    {
                        return current;
                    }
                }
                else if (IsClassicBook(current.name))
                {
                    if (current.GetComponent<MonoBehaviour>() != null)
                    {
                        return current;
                    }
                }
                current = current.parent;
            }

            ClassicMathBook[] mathBooks = startTransform.GetComponentsInChildren<ClassicMathBook>();
            if (mathBooks.Length > 0)
            {
                return mathBooks[0].transform;
            }

            Transform[] allChildren = startTransform.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (IsClassicBook(child.name))
                {
                    return child;
                }
            }

            ClassicMathBook? sceneBook = Object.FindObjectOfType<ClassicMathBook>();
            if (sceneBook != null)
            {
                return sceneBook.transform;
            }

            Transform[] allTransforms = Object.FindObjectsOfType<Transform>();
            foreach (Transform t in allTransforms)
            {
                if (IsClassicBook(t.name))
                {
                    return t;
                }
            }
            return null;
        }
    }
}