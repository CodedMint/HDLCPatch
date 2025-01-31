using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using HDLethalCompanyPatch.config;
using HDLethalCompanyPatch.patches;
using System;
using System.Collections.Generic;
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
        public const string Version = "1.1.0";

        public static Assembly HDAssembly;
        public static object GraphicsPatchObj;
        public static Type GraphicsPatch;

        public static MethodInfo SetFogQuality;
        public static MethodInfo RemoveLodFromGameObject;
        public static MethodInfo ToggleCustomPass;
        public static MethodInfo SetLevelOfDetail;
        public static MethodInfo ToggleVolumetricFog;
        public static MethodInfo SetShadowQuality;
        public static MethodInfo SetAntiAliasing;
        public static MethodInfo SetTextureQuality;
    }

    [BepInPlugin(HDLCPatchProperties.GUID, HDLCPatchProperties.Name, HDLCPatchProperties.Version)]
    [BepInDependency("HDLethalCompany", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("LethalConfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class HDLCPatch : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static Harmony _harmony;

        public static ConfigEntry<bool> EnableHDPatchOverrideSettings;
        public static ConfigEntry<float> ResolutionScale;
        public static ConfigEntry<QualitySetting> FogQuality;
        public static ConfigEntry<bool> EnableFog;
        public static ConfigEntry<QualitySetting> ShadowQuality;
        public static ConfigEntry<QualitySetting> LODQuality;
        public static ConfigEntry<QualitySetting> TextureQuality;
        public static ConfigEntry<bool> EnablePostProcessing;
        public static ConfigEntry<bool> EnableFoliage;
        public static ConfigEntry<bool> EnableResolutionOverride;
        public static ConfigEntry<bool> EnableAntiAliasing;
        public static ConfigEntry<bool> DisableFoliageConfig;

        public static float DefaultResolutionScale = 0;
        public static int DefaultFogQuality = 0;
        public static int DefaultShadowQuality = 0;
        public static int DefaultLODQuality = 0;
        public static bool DefaultEnableFog = true;
        public static bool DefaultEnablePostProcessing = true;
        public static bool DefaultEnableFoliage = true;
        public static bool DefaultEnableResolutionOverride = true;
        public static int DefaultTextureQuality = 0;
        public static bool DefaultEnableAntiAliasing = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {HDLCPatchProperties.Name} {HDLCPatchProperties.Version} is loaded!");

            _harmony = new Harmony(HDLCPatchProperties.GUID);

            PatchHDLC();
            //SetupInternalMethods();
            SetupConfig();
        }

        public static void PatchHDLC()
        {
            Logger.LogInfo("Patching...");
            List<MethodBase> methods = (List<MethodBase>)Harmony.GetAllPatchedMethods();
            Assembly assembly = null;

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
                        assembly = p.PatchMethod.Module.Assembly;
                    }

                    if(p.PatchMethod.Name == "StartPrefix" && p.owner == "HDLethalCompany")
                    {
                        Logger.LogInfo("Unpatching StartPrefix to use custom patch instead");
                        _harmony.Unpatch(method, p.PatchMethod);
                    }
                }
            }

            _harmony.PatchAll(typeof(HDLCGraphicsPatch));
            SetupInternalMethods(assembly);
            Logger.LogInfo("Finished patching");
        }

        public Assembly GetAssembly(string assemblyName)
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.GetName().Name.Contains(assemblyName))
                {
                    return assembly;
                }
            }

            return null;
        }

        private static void SetupInternalMethods(Assembly assembly)
        {
            Logger.LogInfo("Creating references to internal methods...");

            //Assembly assembly = GetAssembly("HDLethalCompany");

            Logger.LogInfo(assembly.GetName().Name);

            HDLCPatchProperties.HDAssembly = assembly;
            HDLCPatchProperties.GraphicsPatchObj = assembly.CreateInstance("HDLethalCompany.Patch.GraphicsPatch");

            Type t = HDLCPatchProperties.GraphicsPatchObj.GetType();
            HDLCPatchProperties.GraphicsPatch = t;

            HDLCPatchProperties.SetFogQuality = t.GetMethod("SetFogQuality");
            HDLCPatchProperties.RemoveLodFromGameObject = t.GetMethod("RemoveLodFromGameObject");
            HDLCPatchProperties.SetLevelOfDetail = t.GetMethod("SetLevelOfDetail");
            HDLCPatchProperties.SetAntiAliasing = t.GetMethod("SetAntiAliasing");
            HDLCPatchProperties.SetShadowQuality = t.GetMethod("SetShadowQuality");
            HDLCPatchProperties.ToggleCustomPass = t.GetMethod("ToggleCustomPass");
            HDLCPatchProperties.ToggleVolumetricFog = t.GetMethod("ToggleVolumetricFog");
            HDLCPatchProperties.SetTextureQuality = t.GetMethod("SetTextureQuality");

            HDLCGraphicsPatch.HDAssetBundle = (AssetBundle)HDLCPatchProperties.GraphicsPatch.GetField("assetBundle").GetValue(HDLCPatchProperties.GraphicsPatchObj);

            Logger.LogInfo("References created!");
        }

        private void SetupConfig()
        {
            Logger.LogInfo("Setting up config...");

            EnableHDPatchOverrideSettings = Config.Bind("Override", "EnableHDPatchOverrideSettings", false, "Toggle to true to use settings from HDLCPatch instead of HDLethalCompany. This allows updating settings in game without restart.");
            ResolutionScale = Config.Bind("Resolution", "ResolutionScale", 2.233f, "Resolution Scale Multiplier | 1.000 = 860x520p | 2.233 =~ 1920x1080p | 2.977 = 2560x1440p | 4.465 = 3840x2060p");
            EnableFog = Config.Bind("QualitySettings", "EnableFog", true, "Toggles fog on or off");
            FogQuality = Config.Bind("QualitySettings", "FogQuality", QualitySetting.Low, "Adjusts the fog quality. Lower values will reduce GPU load");
            ShadowQuality = Config.Bind("QualitySettings", "ShadowQuality", QualitySetting.High, "Asjusts the shadow resolution. Lower values reduce GPU load");
            LODQuality = Config.Bind("QualitySettings", "LODQuality", QualitySetting.High, "Adjusts the lod distance. Low values reduce GPU load.");
            TextureQuality = Config.Bind("QualitySettings", "TextureQuality", QualitySetting.High, "Changes texture resolution");
            EnablePostProcessing = Config.Bind("QualitySettings", "EnablePostProcessing", true, "Turns on a color grading post process effect");
            EnableFoliage = Config.Bind("QualitySettings", "EnableFoliage", true, "Toggles foliage on or off");
            EnableResolutionOverride = Config.Bind("Resolution", "EnableResolutionOverride", true, "Toggles off or on overriding the vanilla resolution");
            EnableAntiAliasing = Config.Bind("QualitySettings", "EnableAntiAilasing", false, "Toggles anti-aliasing");
            DisableFoliageConfig = Config.Bind("Compatability", "DisableFoliageConfig", false, "Disables foliage setting to prevent an issue with certain mods");


            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for(int i = 0; i < assemblies.Length; i++)
            {
                if (assemblies[i].FullName.Contains("LethalConfig"))
                {
                    Logger.LogInfo("LethalConfig found. Setting up variables...");
                    SetupLethalConfig();
                    break;
                }
            }

            Logger.LogInfo("Config setup complete!");
        }

        private void SetupLethalConfig()
        {
            LCHDPatchConfigSettings.Setup();

            EnableHDPatchOverrideSettings.SettingChanged += SettingsChanged;
            ResolutionScale.SettingChanged += SettingsChanged;
            FogQuality.SettingChanged += SettingsChanged;
            EnableFog.SettingChanged += SettingsChanged;
            LODQuality.SettingChanged += SettingsChanged;
            ShadowQuality.SettingChanged += SettingsChanged;
            EnablePostProcessing.SettingChanged += SettingsChanged;
            EnableFoliage.SettingChanged += SettingsChanged;
            EnableResolutionOverride.SettingChanged += SettingsChanged;
            TextureQuality.SettingChanged += SettingsChanged;
            EnableAntiAliasing.SettingChanged += SettingsChanged;

            Logger.LogInfo("Finished setting variables for Lethal Config");
        }

        private void SettingsChanged(object sender, EventArgs args)
        {
            HDLCGraphicsPatch.SettingsChanged();
        }
    }
}
