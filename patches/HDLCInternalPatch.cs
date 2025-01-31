using HarmonyLib;
using System;
using System.Reflection;

namespace HDLethalCompanyPatch.patches
{
    [HarmonyPatch]
    internal class HDLCSetFogQualityPatch
    {
        static MethodBase TargetMethod()
        {
            Type t = AccessTools.TypeByName("HDLethalCompany.Patch.GraphicsPatch");
            return AccessTools.Method(t, "SetFogQuality");
        }

        public static void Prefix(object __instance)
        {

        }
    }
}
