using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using HDLethalCompanyPatch.patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HDLethalCompanyPatch
{
    public enum QualitySetting
    {
        VeryLow,
        Low,
        Medium,
        High
    }

    public static class HDLCPatchProperties
    {
        public const string Name = "HDLCPatch";
        public const string GUID = "HDLCPatch";
        public const string Version = "1.0.2";

        //public static QualitySetting FogQuality { get; set; } = QualitySetting.VeryLow;
        //public static Module HDLCAssembly;
    }

    [BepInPlugin(HDLCPatchProperties.GUID, HDLCPatchProperties.Name, HDLCPatchProperties.Version)]
    [BepInDependency("HDLethalCompany", BepInDependency.DependencyFlags.HardDependency)]
    public class HDLCPatch : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static Harmony _harmony;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {HDLCPatchProperties.Name} {HDLCPatchProperties.Version} is loaded!");

            _harmony = new Harmony(HDLCPatchProperties.GUID);

            PatchHDLC();
        }

        public static void PatchHDLC()
        {
            Logger.LogInfo("Patching...");
            List<MethodBase> methods = (List<MethodBase>)Harmony.GetAllPatchedMethods();

            foreach (MethodBase method in methods)
            {
                PatchInfo patchi = PatchManager.GetPatchInfo(method);

                Patch[] prefixPatches = patchi.prefixes;

                foreach (var p in prefixPatches)
                {
                    if (p.PatchMethod.Name == "RoundPostFix" && p.owner == "HDLethalCompany")
                    {
                        Logger.LogInfo("Found problem patch from HDLethalCompany RoundPostFix, unpatching it...");
                        _harmony.Unpatch(method, p.PatchMethod);
                    }
                }
            }

            Logger.LogInfo("Finished patching");
        }

    }
}
