using HarmonyLib;
using System;
using Object = UnityEngine.Object;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Timeline;
using TMPro;
using TWGSRussifier.Runtime;

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
                    ApplyAdditionalSizeDeltaSettings(bookTransform);
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
            SetRectTransformSize(bookTransform, "Canvas/InsideText/Text_4", 199f, 100f);
            SetRectTransformSize(bookTransform, "Canvas/InsideText/Text_5", 187f, 128f);
        }

        private static void ApplyAdditionalSizeDeltaSettings(Transform bookTransform)
        {
            SetRectTransformSize(bookTransform, "Canvas/InsideText1/Text_1", 183f, 300f);
            SetRectTransformSize(bookTransform, "Canvas/InsideText2/Text_0", 191f, 300f);
            SetRectTransformSize(bookTransform, "Canvas/InsideText2/Text_1", 188f, 300f);
            SetRectTransformSize(bookTransform, "Canvas/InsideText3/Text_0", 200f, 300f);
        }

        private static void SetRectTransformSize(Transform parent, string path, float width, float height)
        {
            Transform target = parent.Find(path);
            if (target == null) return;

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null) return;

            rectTransform.sizeDelta = new Vector2(width, height);
        }

        [HarmonyPatch(typeof(ClassicFinaleManager), "AwakeFunction")]
        private static class ClassicFinaleAwakePatch
        {
            [HarmonyPostfix]
            private static void Postfix(ClassicFinaleManager __instance)
            {
                ClassicMathBook? bookPre = AccessTools.Field(typeof(ClassicFinaleManager), "bookPre")
                    ?.GetValue(__instance) as ClassicMathBook;
                if (bookPre != null)
                    ApplyFinalBookAdjustments(bookPre.transform);

                UnityEngine.Playables.PlayableDirector? director =
                    AccessTools.Field(typeof(ClassicFinaleManager), "director")
                    ?.GetValue(__instance) as UnityEngine.Playables.PlayableDirector;
                if (director?.playableAsset is TimelineAsset timeline)
                    ReplaceTimelineAudio(timeline);
            }
        }

        private static void ReplaceTimelineAudio(TimelineAsset timeline)
        {
            if (LanguageManager.instance == null) return;

            foreach (TrackAsset track in timeline.GetOutputTracks())
            {
                AudioTrack? audioTrack = track as AudioTrack;
                if (audioTrack == null) continue;
                foreach (TimelineClip clip in audioTrack.GetClips())
                {
                    AudioPlayableAsset? audioAsset = clip.asset as AudioPlayableAsset;
                    if (audioAsset == null || audioAsset.clip == null) continue;

                    AudioClip? replacement = LanguageManager.instance.GetClip(audioAsset.clip.name);
                    if (replacement != null)
                        audioAsset.clip = replacement;
                }
            }
        }

        [HarmonyPatch(typeof(UnityEngine.Object), "Instantiate", new Type[] { typeof(UnityEngine.Object) })]
        private static class BookInstantiatePatch
        {
            [HarmonyPostfix]
            private static void Postfix(UnityEngine.Object __result)
            {
                if (__result == null) return;

                GameObject? gameObject = null;
                if (__result is GameObject go)
                    gameObject = go;
                else if (__result is Component component)
                    gameObject = component.gameObject;

                if (gameObject == null) return;

                if (IsClassicBook(gameObject.name))
                {
                    ApplySizeDeltaSettings(gameObject.transform);
                    ApplyAdditionalSizeDeltaSettings(gameObject.transform);
                }
            }
        }

        private static void ApplyFinalBookAdjustments(Transform root)
        {
            if (root == null) return;

            ApplyFontSizeFixed(root, "Canvas/CoverText/TitleTMP", 31f);
            ApplySizeDeltaFixed(root, "Canvas/InsideText1/Text_1", 183f, 300f);
            ApplySizeDeltaFixed(root, "Canvas/InsideText2/Text_0", 191f, 300f);
            ApplySizeDeltaFixed(root, "Canvas/InsideText2/Text_1", 188f, 300f);
            ApplySizeDeltaFixed(root, "Canvas/InsideText3/Text_0", 200f, 300f);
        }

        private static void ApplyFontSizeFixed(Transform parent, string path, float fontSize)
        {
            Transform target = parent.Find(path);
            if (target == null) return;

            ContentSizeFitter csf = target.GetComponent<ContentSizeFitter>();
            if (csf != null) csf.enabled = false;

            TMP_Text tmp = target.GetComponent<TMP_Text>();
            if (tmp == null) return;
            tmp.enableAutoSizing = false;
            tmp.fontSize = fontSize;
        }

        private static void ApplySizeDeltaFixed(Transform parent, string path, float width, float height)
        {
            Transform target = parent.Find(path);
            if (target == null) return;

            ContentSizeFitter csf = target.GetComponent<ContentSizeFitter>();
            if (csf != null) csf.enabled = false;

            RectTransform rt = target.GetComponent<RectTransform>();
            if (rt != null) rt.sizeDelta = new Vector2(width, height);
        }

        private static string GetBookIdentifier(string bookName)
        {
            if (bookName.Contains("ClassicBook_Kindergarten")) return "Kindergarten";
            if (bookName.Contains("ClassicBook_Preschool")) return "Preschool";
            if (bookName.Contains("ClassicBook_College")) return "College";
            if (bookName.Contains("ClassicBook_10")) return "10";
            if (bookName.Contains("ClassicBook_11")) return "11";
            if (bookName.Contains("ClassicBook_12")) return "12";
            if (bookName.Contains("ClassicBook_1")) return "1";
            if (bookName.Contains("ClassicBook_2")) return "2";
            if (bookName.Contains("ClassicBook_3")) return "3";
            if (bookName.Contains("ClassicBook_4")) return "4";
            if (bookName.Contains("ClassicBook_5")) return "5";
            if (bookName.Contains("ClassicBook_6")) return "6";
            if (bookName.Contains("ClassicBook_7")) return "7";
            if (bookName.Contains("ClassicBook_8")) return "8";
            if (bookName.Contains("ClassicBook_9")) return "9";
            
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