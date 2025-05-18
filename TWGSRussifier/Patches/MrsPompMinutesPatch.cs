using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    [System.Serializable]
    public class MinutesElement
    {
        public int num;
        public string key;

        public MinutesElement(int n, string k)
        {
            num = n;
            key = k;
        }
    }

    [HarmonyPatch(typeof(NoLateTeacher))]
    [HarmonyPatch("UpdateTimer")]
    public static class MrsPompMinutesPatch
    {
        static void Postfix(NoLateTeacher __instance, float time)
        {
            MinutesElement[] minutes = new MinutesElement[] {
                new MinutesElement(0, "Vfx_NoL_MinutesLeft"),
                new MinutesElement(1, "Vfx_NoL_MinutesLeft_Additional"),
                new MinutesElement(2, "Vfx_NoL_MinutesLeft")
            };

            FieldInfo audMinutesField = AccessTools.Field(typeof(NoLateTeacher), "audMinutesLeft");
            FieldInfo timerField = AccessTools.Field(typeof(NoLateTeacher), "currentDisplayTime");

            int currentDisplayTime = (int)timerField.GetValue(__instance);
            SoundObject sound = (SoundObject)audMinutesField.GetValue(__instance);

            foreach (var minute in minutes)
            {
                if (currentDisplayTime / 60 == minute.num)
                {
                    if (sound != null)
                    {
                        sound.soundKey = minute.key;
                        audMinutesField.SetValue(__instance, sound);
                       // Debug.Log($"Установлен ключ звука: {minute.key} для минуты {minute.num}");
                    }
                    else
                    {
                       // Debug.LogWarning("SoundObject не найден!");
                    }

                    break;
                }
            }
        }
    }
}
