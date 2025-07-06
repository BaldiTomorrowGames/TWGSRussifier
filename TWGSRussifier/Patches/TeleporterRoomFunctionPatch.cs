using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace TWGSRussifier
{
    public class TeleporterTextLocalizer : MonoBehaviour
    {
        public string key = null!;
        private TextMeshPro textComponent = null!;
        
        private void Awake()
        {
            textComponent = GetComponent<TextMeshPro>();
            RefreshLocalization();
        }
        
        private void OnEnable()
        {
            RefreshLocalization();
        }
        
        public void RefreshLocalization()
        {
            if (textComponent == null || string.IsNullOrEmpty(key)) return;
            
            string localizedText = LocalizationManager.Instance.GetLocalizedText(key);
            if (!string.IsNullOrEmpty(localizedText))
            {
                textComponent.text = localizedText;
            }
        }
    }

    [HarmonyPatch(typeof(TeleporterRoomFunction), "Initialize")]
    internal class TeleporterRoomFunctionPatch
    {
        private static Transform? FindInChildrenIncludingInactive(Transform parent, string name)
        {
            if (parent == null) return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                    return child;
                
                Transform? found = FindInChildrenIncludingInactive(child, name);
                if (found != null)
                    return found;
            }
            
            return null;
        }

        [HarmonyPostfix]
        private static void Initialize_Postfix(TeleporterRoomFunction __instance, RoomController room)
        {
            if (room == null || room.ec == null) return;
            
            if (room.ec.name.Contains("Laboratory_Lvl4") || room.ec.name.Contains("Laboratory_Lvl5"))
            {
                Transform? functionBase = FindInChildrenIncludingInactive(room.transform, "TeleporterRoomFunctionObjectBase");
                
                if (functionBase == null) return;
                
                List<Transform> labelObjects = new List<Transform>();
                for (int i = 0; i < 4; i++)
                {
                    Transform? label = FindInChildrenIncludingInactive(functionBase, "RoomLabels_" + i);
                    if (label != null)
                    {
                        labelObjects.Add(label);
                    }
                }
                
                if (labelObjects.Count == 0) return;
                
                for (int i = 0; i < labelObjects.Count; i++)
                {
                    Transform? textTransform = FindInChildrenIncludingInactive(labelObjects[i], "Text (TMP)");
                    
                    if (textTransform != null)
                    {
                        TextMeshPro? textComponent = textTransform.GetComponent<TextMeshPro>();
                        if (textComponent != null)
                        {
                            string localizationKey = "TWGS_RoomLabel_" + i;
                            
                            TeleporterTextLocalizer? localizer = textComponent.GetComponent<TeleporterTextLocalizer>();
                            if (localizer == null)
                            {
                                TextLocalizer? oldLocalizer = textComponent.GetComponent<TextLocalizer>();
                                if (oldLocalizer != null)
                                {
                                    Object.Destroy(oldLocalizer);
                                }
                                
                                localizer = textComponent.gameObject.AddComponent<TeleporterTextLocalizer>();
                                localizer.key = localizationKey;
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
} 