using HarmonyLib;
using TMPro;
using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace TWGSRussifier
{
    public class TeleporterTextLocalizer : MonoBehaviour
    {
        public string key;
        private TextMeshPro textComponent;
        private string originalText;
        
        private void Awake()
        {
            textComponent = GetComponent<TextMeshPro>();
            if (textComponent != null)
            {
                originalText = textComponent.text;
                RefreshLocalization();
            }
        }
        
        private void OnEnable()
        {
            RefreshLocalization();
        }
        
        public void RefreshLocalization()
        {
            if (textComponent == null) return;
            
            if (Runtime.LanguageManager.instance != null && !string.IsNullOrEmpty(key))
            {
                string localizedText = Runtime.LanguageManager.instance.GetKeyData(key);
                if (!string.IsNullOrEmpty(localizedText))
                {
                    textComponent.text = localizedText;
                }
            }
        }
        
        private void Update()
        {
            if (textComponent != null && textComponent.text != originalText)
            {
                RefreshLocalization();
            }
        }
    }

    [HarmonyPatch(typeof(TeleporterRoomFunction), "Initialize")]
    internal class TeleporterRoomFunctionPatch
    {
        private static Transform FindInChildrenIncludingInactive(Transform parent, string name)
        {
            if (parent == null) return null;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                    return child;
                
                Transform found = FindInChildrenIncludingInactive(child, name);
                if (found != null)
                    return found;
            }
            
            return null;
        }
        
        private static List<Transform> FindAllChildrenWithName(Transform parent, string contains)
        {
            List<Transform> result = new List<Transform>();
            if (parent == null) return result;
            
            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(true);
            
            foreach (Transform child in allChildren)
            {
                if (child.name.Contains(contains))
                {
                    result.Add(child);
                }
            }
            
            return result;
        }

        [HarmonyPostfix]
        private static void Initialize_Postfix(TeleporterRoomFunction __instance, RoomController room)
        {
            if (room == null || room.ec == null) return;
            
            if (room.ec.name.Contains("Laboratory_Lvl4") || room.ec.name.Contains("Laboratory_Lvl5"))
            {
                Transform functionBase = FindInChildrenIncludingInactive(room.transform, "TeleporterRoomFunctionObjectBase");
                
                if (functionBase == null) return;
                
                List<Transform> labelObjects = new List<Transform>();
                for (int i = 0; i < 4; i++)
                {
                    Transform label = FindInChildrenIncludingInactive(functionBase, "RoomLabels_" + i);
                    if (label != null)
                    {
                        labelObjects.Add(label);
                    }
                }
                
                if (labelObjects.Count == 0) return;
                
                for (int i = 0; i < labelObjects.Count; i++)
                {
                    Transform textTransform = FindInChildrenIncludingInactive(labelObjects[i], "Text (TMP)");
                    
                    if (textTransform != null)
                    {
                        TextMeshPro textComponent = textTransform.GetComponent<TextMeshPro>();
                        if (textComponent != null)
                        {
                            string localizationKey = "TWGS_RoomLabel_" + i;
                            
                            TeleporterTextLocalizer localizer = textComponent.GetComponent<TeleporterTextLocalizer>();
                            if (localizer == null)
                            {
                                TextLocalizer oldLocalizer = textComponent.GetComponent<TextLocalizer>();
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
    
    public static class TransformExtensions
    {
        public static string GetPath(this Transform transform)
        {
            StringBuilder sb = new StringBuilder();
            GetPathRecursive(transform, sb);
            return sb.ToString();
        }
        
        private static void GetPathRecursive(Transform transform, StringBuilder sb)
        {
            if (transform.parent == null)
            {
                sb.Append(transform.name);
            }
            else
            {
                GetPathRecursive(transform.parent, sb);
                sb.Append("/");
                sb.Append(transform.name);
            }
        }
    }
} 