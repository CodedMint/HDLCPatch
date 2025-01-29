using HarmonyLib;
using Mono.Cecil;
using System.Reflection;


namespace HDLethalCompanyPatch.patches
{
    internal class HDLCGraphicsPatch
    {
        [HarmonyPatch("", "SetFogQuality")]
        [HarmonyPrefix]
        public static void ChangeFogQuality(object __instance)
        {
           // __instance.GetType().GetField("m_setFogQuality", BindingFlags.Static).SetValue(__instance, (int)HDLCPatchProperties.FogQuality);
        }
    }
}
