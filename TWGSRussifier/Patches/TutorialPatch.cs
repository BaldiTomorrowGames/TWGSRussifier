using HarmonyLib;
using UnityEngine;
using TMPro;
using System.Collections;
// using TWGSRussifier.Runtime;
using TWGSRussifier.API;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(TutorialGameManager), "BeginPlay")]
    public static class TutorialPatch
    {
        [HarmonyPostfix]
        public static void Postfix(TutorialGameManager __instance)
        {
            __instance.StartCoroutine(ApplyChangesWithDelay());
        }

        private static IEnumerator ApplyChangesWithDelay()
        {
            yield return new WaitForSeconds(0.5f);

            if (ConfigManager.AreSoundsEnabled())
            {
                SoundObject[] allSounds = Resources.FindObjectsOfTypeAll<SoundObject>();
                foreach (SoundObject soundObject in allSounds)
                {
                    if (TPPlugin.AllClips.TryGetValue(soundObject.name, out AudioClip newClip))
                    {
                        if (soundObject.soundClip != newClip)
                        {
                            soundObject.soundClip = newClip;
                        }
                    }
                }
            }
            
            GameObject tutorialManager = GameObject.Find("TutorialGameManager(Clone)");
            if (tutorialManager != null)
            {
                Transform textTransform = tutorialManager.transform.Find("DefaultCanvas/Text");
                if (textTransform != null)
                {
                    TextLocalizer existingLocalizer = textTransform.GetComponent<TextLocalizer>();
                    if (existingLocalizer == null)
                    {
                        TextLocalizer localizer = textTransform.gameObject.AddComponent<TextLocalizer>();
                        localizer.key = "TWGS_Tutorial_DefaultCanvas";
                    }
                }
            }
        }
    }
} 