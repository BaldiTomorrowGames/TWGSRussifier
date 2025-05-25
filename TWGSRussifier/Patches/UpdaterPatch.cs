using HarmonyLib;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using TWGSRussifier.API;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch]
    internal class UpdaterPatch
    {
        private static bool updateChecked = false;
        private static readonly string UpdateLocalizationKey = "TWGS_Menu_UpdateAvailable";
        private static readonly string ReminderLocalizationKey = "TWGS_Menu_Reminder";
        private static GameObject dummyReminder = null;

        [HarmonyPatch(typeof(GameObject), "SetActive")]
        private static class SetActivePatch
        {
            [HarmonyPostfix]
            private static void Postfix(GameObject __instance, bool value)
            {
                if (__instance.name == "Menu" && value)
                {
                    if (!updateChecked)
                    {
                        CheckUpdatesAsync();
                        updateChecked = true;
                    }

                    Transform reminderTransform = __instance.transform.Find("Reminder");
                    if (reminderTransform != null)
                    {
                        ModifyReminderElement(reminderTransform.gameObject);
                    }
                }
            }
        }

        private static Transform GetDummyReminder(Transform parent)
        {
            if (dummyReminder == null)
            {
                dummyReminder = new GameObject("DummyReminder");
                dummyReminder.AddComponent<RectTransform>();
                
                TextMeshProUGUI textComponent = dummyReminder.AddComponent<TextMeshProUGUI>();
                textComponent.text = "";
                textComponent.enabled = false;
                
                dummyReminder.SetActive(false);
                
                GameObject.DontDestroyOnLoad(dummyReminder);
                
                //API.Logger.Info("Создан фиктивный объект DummyReminder для MTM101BaldAPI");
            }
            
            dummyReminder.transform.SetParent(parent, false);
            
            return dummyReminder.transform;
        }

        [HarmonyPatch(typeof(Transform), "Find")]
        private static class TransformFindPatch
        {
            [HarmonyPrefix]
            private static bool Prefix(Transform __instance, string n, ref Transform __result)
            {
                if (n == "Reminder")
                {
                    StackFrame[] stackFrames = new System.Diagnostics.StackTrace(true).GetFrames();
                    if (stackFrames != null)
                    {
                        foreach (var frame in stackFrames)
                        {
                            MethodBase method = frame.GetMethod();
                            if (method != null && method.DeclaringType != null)
                            {
                                string namespaceName = method.DeclaringType.Namespace ?? "";
                                
                                if (namespaceName.StartsWith("MTM101BaldAPI") || 
                                    namespaceName.Contains(".MTM101BaldAPI") ||
                                    method.DeclaringType.FullName.Contains("MTM101BaldAPI"))
                                {
                                    // API.Logger.Info($"Перехвачен доступ к Reminder из MTM101BaldAPI (метод: {method.DeclaringType.FullName}.{method.Name})");
                                    __result = GetDummyReminder(__instance);
                                    return false;
                                }
                            }
                        }
                    }
                }
                
                return true;
            }
        }


        [HarmonyPatch(typeof(Transform), "GetChild")]
        private static class TransformGetChildPatch
        {
            [HarmonyPostfix]
            private static void Postfix(Transform __result)
            {
                if (__result != null && __result.name == "Reminder")
                {
                    StackFrame[] stackFrames = new System.Diagnostics.StackTrace(true).GetFrames();
                    if (stackFrames != null)
                    {
                        foreach (var frame in stackFrames)
                        {
                            MethodBase method = frame.GetMethod();
                            if (method != null && method.DeclaringType != null)
                            {
                                string namespaceName = method.DeclaringType.Namespace ?? "";
                                
                                if (namespaceName.StartsWith("MTM101BaldAPI") || 
                                    namespaceName.Contains(".MTM101BaldAPI") ||
                                    method.DeclaringType.FullName.Contains("MTM101BaldAPI"))
                                {
                                    // API.Logger.Info($"Перехвачен доступ к Reminder через GetChild из MTM101BaldAPI (метод: {method.DeclaringType.FullName}.{method.Name})");
                                    
                                    Transform oldParent = __result.parent;
                                    
                                    __result = GetDummyReminder(oldParent);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static async void CheckUpdatesAsync()
        {
            await UpdateChecker.CheckForUpdates();
            
            GameObject menuObject = GameObject.Find("Menu");
            if (menuObject != null)
            {
                Transform reminderTransform = menuObject.transform.Find("Reminder");
                if (reminderTransform != null)
                {
                    ModifyReminderElement(reminderTransform.gameObject);
                }
            }
        }

        private static void ModifyReminderElement(GameObject reminderObject)
        {
            if (reminderObject == null) return;

            TextMeshProUGUI textComponent = reminderObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                TextLocalizer localizer = textComponent.GetComponent<TextLocalizer>();
                if (localizer == null)
                {
                    localizer = reminderObject.AddComponent<TextLocalizer>();
                }

                if (UpdateChecker.IsUpdateAvailable)
                {
                    localizer.key = UpdateLocalizationKey;
                    localizer.RefreshLocalization();

                    textComponent.raycastTarget = true;

                    StandardMenuButton button = reminderObject.GetComponent<StandardMenuButton>();
                    if (button == null)
                    {
                        button = reminderObject.AddComponent<StandardMenuButton>();
                        button.OnPress = new UnityEvent();
                        button.OnHighlight = new UnityEvent();
                        button.OnRelease = new UnityEvent();
                        button.OffHighlight = new UnityEvent();
                        button.underlineOnHigh = true;
                        button.text = textComponent;
                        button.gameObject.tag = "Button";

                        button.OnPress.AddListener(() => {
                            Application.OpenURL(UpdateChecker.GetReleasesPageUrl());
                        });
                    }
                }
                else
                {
                    localizer.key = ReminderLocalizationKey;
                    localizer.RefreshLocalization();

                    textComponent.raycastTarget = false;

                    StandardMenuButton button = reminderObject.GetComponent<StandardMenuButton>();
                    if (button != null)
                    {
                        GameObject.Destroy(button);
                    }
                }
            }
        }
    }
} 