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
    public enum FogSettingMethod
    {
        Sliders,
        Presets
    }

    public enum FogQualitySetting
    {
        Minimal,
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh,
        Extreme
    }

    public enum QualitySetting
    {
        VeryLow,
        Low,
        Medium,
        High
    }

    public enum AntiAliasingSetting
    {
        FAA,
        TAA,
        SMAA
    }

    public enum ResolutionSettingMethod
    {
        ScaleSlider,
        Presets,
        Custom
    }

    public enum ResolutionPreset
    {
        R640x480,
        R1280x720,
        R1920x1080,
        R2560x1440,
        R3840x2160
    }

    public static class HDLCPatchProperties
    {
        public const string Name = "HDLCPatch";
        public const string GUID = "HDLCPatch";
        public const string Version = "1.3.0";

        public static Assembly HDAssembly;
        public static object GraphicsPatchObj;
        public static Type GraphicsPatch;

        public static MethodInfo RemoveLodFromGameObject;
        public static MethodInfo SetShadowQuality;
    }

    [BepInPlugin(HDLCPatchProperties.GUID, HDLCPatchProperties.Name, HDLCPatchProperties.Version)]
    [BepInDependency("HDLethalCompany", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("LethalConfig", BepInDependency.DependencyFlags.SoftDependency)]
    public class HDLCPatch : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private static Harmony _harmony;

        public static ConfigEntry<QualitySetting> ShadowQuality;
        public static ConfigEntry<QualitySetting> LODQuality;
        public static ConfigEntry<QualitySetting> TextureQuality;
        public static ConfigEntry<FogQualitySetting> FogQuality;
        public static ConfigEntry<AntiAliasingSetting> AASetting;
        public static ConfigEntry<ResolutionSettingMethod> ResolutionMethod;
        public static ConfigEntry<ResolutionPreset> ResolutionPresetValue;
        public static ConfigEntry<FogSettingMethod> FogQualityMethod;
        public static ConfigEntry<float> VolumetricFogBudget;
        public static ConfigEntry<float> FogResolutionDepthRatio;
        public static ConfigEntry<float> ResolutionScale;
        public static ConfigEntry<int> ResolutionWidth;
        public static ConfigEntry<int> ResolutionHeight;
        public static ConfigEntry<bool> EnableFog;
        public static ConfigEntry<bool> EnablePostProcessing;
        public static ConfigEntry<bool> EnableFoliage;
        public static ConfigEntry<bool> EnableResolutionOverride;
        public static ConfigEntry<bool> EnableAntiAliasing;
        public static ConfigEntry<bool> DisableFoliageConfig;

        public static Assembly HDLethal;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {HDLCPatchProperties.Name} {HDLCPatchProperties.Version} is loaded!");

            _harmony = new Harmony(HDLCPatchProperties.GUID);

            PatchHDLC();
            SetupConfig();
        }

        //Remove some HDLethalCompany patches and replace them with new ones
        public static void PatchHDLC()
        {
            Logger.LogInfo("Patching...");
            List<MethodBase> methods = (List<MethodBase>)Harmony.GetAllPatchedMethods();
            bool roundPostfixSuccess = false;
            bool startPrefixSuccess = false;
            bool internalRefsSuccess = true;

            foreach (MethodBase method in methods)
            {
                PatchInfo patchi = PatchManager.GetPatchInfo(method);

                Patch[] prefixPatches = patchi.prefixes;
                Patch[] postfixPatches = patchi.postfixes;

                foreach (var p in prefixPatches)
                {
                    if (p.PatchMethod.Name == "RoundPostFix")
                    {
                        if (p.owner == "HDLethalCompany" || p.owner == "HDLethalCompanyRemake")
                        {
                            Logger.LogInfo("Attempting to unpatch HDLethalCompany RoundPostFix");
                            _harmony.Unpatch(method, p.PatchMethod);
                            HDLethal = p.PatchMethod.Module.Assembly;
                            roundPostfixSuccess = true;
                        }
                    }

                    if(p.PatchMethod.Name == "StartPrefix")
                    {
                        if (p.owner == "HDLethalCompany" || p.owner == "HDLethalCompanyRemake")
                        {
                            Logger.LogInfo("Attempting to unpatch HDLethalCompany RoundPostFix");
                            _harmony.Unpatch(method, p.PatchMethod);
                            startPrefixSuccess = true;
                        }
                    }
                }

                foreach(var p in postfixPatches)
                {
                    if (p.PatchMethod.Name == "UpdateScanNodesPostfix")
                    {
                        if (p.owner == "HDLethalCompany" || p.owner == "HDLethalCompanyRemake")
                        {
                            Logger.LogInfo("Attempting to unpatch HDLethalCompany UpdateScanNodesPostfix");
                            _harmony.Unpatch(method, p.PatchMethod);
                        }
                    }
                }

            }

            if (roundPostfixSuccess)
            {
                Logger.LogInfo("Unpatch RoundPostfix success!");
            }
            else
            {
                Logger.LogFatal("Failed to unpatch RoundPostfix");
            }

            if (startPrefixSuccess)
            {
                Logger.LogInfo("Unpatch StartPrefix success!");
            }
            else
            {
                Logger.LogFatal("Failed to unpatch StartPrefix");
            }

            _harmony.PatchAll(typeof(HDLCGraphicsPatch));

            try
            {
                SetupInternalMethods(HDLethal);
            }
            catch(Exception e)
            {
                Logger.LogWarning("Failed to set internal method references \n" + e.ToString());
                Logger.LogInfo("Attempting to get assembly with a different method...");

                try
                {
                    HDLethal = HarmonyGetAssembly("HDLethalCompany");
                    SetupInternalMethods(HDLethal);
                }
                catch (Exception e2)
                {
                    Logger.LogError("Backup method for getting assembly failed! \n" + e2.ToString());
                    Logger.LogInfo("Attempting 3rd method to get internal references...");

                    try
                    {
                        HDLethal = GetAssembly("HDLethalCompany");
                        SetupInternalMethods(HDLethal);
                    }
                    catch(Exception e3)
                    {
                        Logger.LogFatal("All methods to get HDLethalCompany internal methods failed!");
                        Logger.LogError(e3.ToString());
                        internalRefsSuccess = false;
                    }
                }
            }

            if (internalRefsSuccess)
            {
                Logger.LogInfo("Finished patching process");
            }
            else
            {
                Logger.LogFatal("Failed to set internal references to HDLethalCompany methods. Things might not work right!");
            }
        }

        //This might not work due to some mods changing assembly names!!!
        public static Assembly GetAssembly(string assemblyName)
        {
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if(assembly.GetName().Name.Contains(assemblyName) && !assembly.GetName().Name.Contains("HDLethalCompanyPatch"))
                {
                    Logger.LogInfo($"Got assembly: " + assembly.GetName().Name);
                    return assembly;
                }
            }

            Logger.LogError("Failed to get assembly");
            Logger.LogInfo("Currently available assemblies from AppDomain.CurrentDomain: ");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Logger.LogInfo($"{assembly.GetName().Name}");
            }

            return null;
        }

        public static Assembly HarmonyGetAssembly(string assemblyName)
        {
            foreach (Assembly assembly in AccessTools.AllAssemblies())
            {
                if (assembly.GetName().Name.Contains(assemblyName) && !assembly.GetName().Name.Contains("HDLethalCompanyPatch"))
                {
                    Logger.LogInfo($"Got assembly: " + assembly.GetName().Name);
                    return assembly;
                }
            }

            Logger.LogError("Failed to get assembly");
            Logger.LogInfo("Currently available assemblies from AccessTools: ");

            foreach(Assembly assembly in AccessTools.AllAssemblies())
            {
                Logger.LogInfo($"{assembly.GetName().Name}");
            }

            return null;
        }

        //Grab references to HDLethalCompany internal methods to call later
        public static void SetupInternalMethods(Assembly assembly)
        {
            Logger.LogInfo("Creating references to internal methods...");

            try { HDLCPatchProperties.HDAssembly = assembly; Logger.LogInfo("Got assembly of " + assembly.GetName().Name); } catch(Exception e) { Logger.LogError("Failed to get assembly reference\n" + e.ToString()); }
            try { HDLCPatchProperties.GraphicsPatchObj = assembly.CreateInstance("HDLethalCompany.Patch.GraphicsPatch"); } catch (Exception e) { Logger.LogError("Failed to get reference to GraphicsPatch\n" + e.ToString()); }

            Type t = HDLCPatchProperties.GraphicsPatchObj.GetType();
            HDLCPatchProperties.GraphicsPatch = t;

            HDLCPatchProperties.RemoveLodFromGameObject = t.GetMethod("RemoveLodFromGameObject");
            HDLCPatchProperties.SetShadowQuality = t.GetMethod("SetShadowQuality");

            HDLCGraphicsPatch.HDAssetBundle = (AssetBundle)HDLCPatchProperties.GraphicsPatch.GetField("assetBundle").GetValue(HDLCPatchProperties.GraphicsPatchObj);

            Logger.LogInfo("References created!");
        }

        private void SetupConfig()
        {
            Logger.LogInfo("Setting up config...");

            ResolutionScale = Config.Bind("Resolution", "ResolutionScale", 2.233f, "Resolution Scale Multiplier | 1.000 = 860x520p | 2.233 =~ 1920x1080p | 2.977 = 2560x1440p | 4.465 = 3840x2060p");
            EnableFog = Config.Bind("QualitySettings", "EnableFog", true, "Toggles fog on or off");
            FogQualityMethod = Config.Bind("QualitySettings", "FogQualitySettingMethod", FogSettingMethod.Presets, "Changes the method used to set fog quality.");
            FogQuality = Config.Bind("QualitySettings", "FogQuality", FogQualitySetting.Low, "Adjusts the fog quality. Lower values will reduce GPU load.\nFogQualitySettingMethod must be set to Presets.");
            FogResolutionDepthRatio = Config.Bind("QualitySettings", "FogResolutionDepthRatio", 0.3f, "Affects resolution of fog.\nFogQualitySettingMethod must be set to Sliders.");
            VolumetricFogBudget = Config.Bind("QualitySettings", "VolumetricFogBudget", 0.3f, "Affects volumetric density of fog.\nFogQualitySettingMethod must be set to Sliders.");
            ShadowQuality = Config.Bind("QualitySettings", "ShadowQuality", QualitySetting.High, "Adjusts the shadow resolution. Lower values reduce GPU load");
            LODQuality = Config.Bind("QualitySettings", "LODQuality", QualitySetting.High, "Adjusts the lod distance. Low values reduce GPU load.");
            TextureQuality = Config.Bind("QualitySettings", "TextureQuality", QualitySetting.High, "Changes texture resolution");
            EnablePostProcessing = Config.Bind("QualitySettings", "EnablePostProcessing", true, "Turns on a color grading post process effect");
            EnableFoliage = Config.Bind("QualitySettings", "EnableFoliage", true, "Toggles foliage on or off");
            EnableResolutionOverride = Config.Bind("Resolution", "EnableResolutionOverride", true, "Toggles off or on overriding the vanilla resolution");
            EnableAntiAliasing = Config.Bind("AntiAliasing", "EnableAntiAilasing", false, "Toggles anti-aliasing");
            DisableFoliageConfig = Config.Bind("Compatability", "DisableFoliageConfig", false, "Disables foliage setting to prevent an issue with certain mods");
            AASetting = Config.Bind("AntiAliasing", "AAMode", AntiAliasingSetting.FAA, "Changes the type of anti-aliasing used");
            ResolutionMethod = Config.Bind("Resolution", "ResolutionMethod", ResolutionSettingMethod.ScaleSlider, "Changes how resolution should be set.");
            ResolutionPresetValue = Config.Bind("Resolution", "ResolutionPreset", ResolutionPreset.R1920x1080, "If ResolutionMethod is set to Preset, this setting will be used.");
            ResolutionWidth = Config.Bind("Resolution", "ResolutionWidth", 1920, "If ResolutionMethod is set to Custom, this value will be used for resolution width.");
            ResolutionHeight = Config.Bind("Resolution", "ResolutionHeight", 1080, "If ResolutionMethod is set to Custom, this value will be used for resolution height.");

            //Check if LethalConfig is installed
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

        //Setup LethalConfig stuff and events to allow runtime changes
        private void SetupLethalConfig()
        {
            LCHDPatchConfigSettings.Setup();

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
            AASetting.SettingChanged += SettingsChanged;
            ResolutionPresetValue.SettingChanged += SettingsChanged;
            ResolutionMethod.SettingChanged += SettingsChanged;
            ResolutionHeight.SettingChanged += SettingsChanged;
            ResolutionWidth.SettingChanged += SettingsChanged;
            FogQualityMethod.SettingChanged += SettingsChanged;
            FogResolutionDepthRatio.SettingChanged += SettingsChanged;
            VolumetricFogBudget.SettingChanged += SettingsChanged;

            Logger.LogInfo("Finished setting variables for Lethal Config and setting up events");
        }

        private void SettingsChanged(object sender, EventArgs args)
        {
            Logger.LogInfo("Detected settings change");
            HDLCGraphicsPatch.SettingsChanged();
        }
    }
}
