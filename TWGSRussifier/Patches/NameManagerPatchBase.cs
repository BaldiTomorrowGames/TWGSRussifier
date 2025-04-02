using HarmonyLib;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(NameManager)), HarmonyPatch("Awake"), HarmonyPatch(typeof(NameManager)), HarmonyPatch("Awake")]
    public static class NameManagerPatchBase
    {
    }
}