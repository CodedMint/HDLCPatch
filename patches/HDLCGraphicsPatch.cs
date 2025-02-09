using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;


namespace HDLethalCompanyPatch.patches
{
    internal class HDLCGraphicsPatch
    {
        public static bool HasDefaults = false;
        public static bool CanChangeFog = false;
        public static bool FogEnabled = true;
        public static bool EnablePostProcessing = true;
        public static bool EnableFoliage = true;
        public static LayerMask DefaultCullingMask = 0;
        public static AssetBundle HDAssetBundle = null;
        public static bool EnableResolutionOverride = true;
        public static float ResolutionScale = 1;
        public static PlayerControllerB PlayerRef;
        public static bool MaskRemoved = false;
        public static int TextureQuality = 3;
        public static bool DisableFoliageSetting = false;
        public static bool EnableAA = false;
        public static bool EnablePatchOverride = false;
        public static AntiAliasingSetting AAMode = AntiAliasingSetting.FAA;
        public static int LODQuality;
        public static bool StartCalled = false;
        public static int RenderResolutionWidth = 1;
        public static int RenderResolutionHeight = 1;

        public static void SettingsChanged()
        {
            try
            {
                HDLCPatch.Logger.LogInfo("Applying settings changes...");

                //Store values from HDLethalCompany to use later if EnableHDPatchOverrideSettings is false
                if (!HasDefaults)
                {
                    HDLCPatch.DefaultResolutionScale = (float)HDLCPatchProperties.GraphicsPatch.GetField("multiplier").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultFogQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setFogQuality").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultLODQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setLOD").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultShadowQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setShadowQuality").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultEnableFog = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFog").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultEnablePostProcessing = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enablePostProcessing").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultEnableFoliage = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFoliage").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultEnableResolutionOverride = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableResolutionFix").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultEnableAntiAliasing = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableAntiAliasing").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                    HDLCPatch.DefaultTextureQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setTextureResolution").GetValue(HDLCPatchProperties.GraphicsPatchObj);

                    HasDefaults = true;
                }

                //Set values
                if (HDLCPatch.EnableHDPatchOverrideSettings.Value)
                {
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setShadowQuality").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.ShadowQuality.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setLOD").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.LODQuality.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setFogQuality").SetValue(HDLCPatchProperties.GraphicsPatchObj, (int)HDLCPatch.FogQuality.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_widthResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.ResolutionScale.Value * 860);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_heightResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.ResolutionScale.Value * 520);
                    HDLCPatchProperties.GraphicsPatch.GetField("multiplier").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.ResolutionScale.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableFog").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.EnableFog.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enablePostProcessing").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.EnablePostProcessing.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableFoliage").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.EnableFoliage.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableResolutionFix").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.EnableResolutionOverride.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setTextureResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.TextureQuality.Value);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableAntiAliasing").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.EnableAntiAliasing.Value);
                }
                else
                {
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setShadowQuality").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultShadowQuality);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setLOD").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultLODQuality);
                    HDLCPatchProperties.GraphicsPatch.GetField("multiplier").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultResolutionScale);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_widthResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultResolutionScale * 860);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_heightResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultResolutionScale * 520);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setFogQuality").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultFogQuality);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableFog").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultEnableFog);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enablePostProcessing").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultEnablePostProcessing);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableFoliage").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultEnableFoliage);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableResolutionFix").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultEnableResolutionOverride);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_setTextureResolution").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultTextureQuality);
                    HDLCPatchProperties.GraphicsPatch.GetField("m_enableAntiAliasing").SetValue(HDLCPatchProperties.GraphicsPatchObj, HDLCPatch.DefaultEnableAntiAliasing);
                }

                //Keep a local reference of some values
                FogEnabled = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFog").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                EnablePostProcessing = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enablePostProcessing").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                EnableFoliage = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFoliage").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                EnableResolutionOverride = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableResolutionFix").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                ResolutionScale = (float)HDLCPatchProperties.GraphicsPatch.GetField("multiplier").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                TextureQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setTextureResolution").GetValue(HDLCPatchProperties.GraphicsPatchObj);
                DisableFoliageSetting = HDLCPatch.DisableFoliageConfig.Value;
                EnablePatchOverride = HDLCPatch.EnableHDPatchOverrideSettings.Value;
                AAMode = HDLCPatch.AASetting.Value;
                LODQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setLOD").GetValue(HDLCPatchProperties.GraphicsPatchObj);

                //Make sure the player reference is not null before setting some settings
                if (PlayerRef != null)
                {
                    try { SetCameraData(PlayerRef); } catch(Exception e) { HDLCPatch.Logger.LogError($"Failed to set camera related settings\n{e.ToString()}"); }
                    try { SetResolution(PlayerRef); } catch(Exception e) { HDLCPatch.Logger.LogError($"Failed to set resolution\n{e.ToString()}"); }
                }

                try { SetTextureQuality(); } catch(Exception e) { HDLCPatch.Logger.LogError($"Failed to set texture quality\n{e.ToString()}"); }

                HDLCPatch.Logger.LogInfo("Settings applied");
            }
            catch(Exception ex)
            {
                HDLCPatch.Logger.LogError("Failed to change settings!");
                HDLCPatch.Logger.LogError(ex.ToString());
                HDLCPatch.Logger.LogInfo("Attempting to try reset internal methods");

                try
                {
                    Assembly assembly = HDLCPatch.HarmonyGetAssembly("HDLethalCompany");
                    HDLCPatch.SetupInternalMethods(assembly);
                    HDLCPatch.Logger.LogInfo("Reset may have succeded. Try changing settings again");
                }
                catch(Exception e) 
                {
                    HDLCPatch.Logger.LogFatal("Failed to setup internal method references");
                    HDLCPatch.Logger.LogError(e.ToString());
                }
            }
        }

        public static void SetCameraData(PlayerControllerB player)
        {
            HDAdditionalCameraData[] cameras = Resources.FindObjectsOfTypeAll<HDAdditionalCameraData>();

            foreach(HDAdditionalCameraData cam in cameras)
            {
                if (cam.gameObject.name == "MapCamera") continue;

                cam.customRenderingSettings = true;

                SetFogQuality(cam);
                TogglePostProcessing(cam);
                HDLCPatchProperties.SetLevelOfDetail.Invoke(HDLCPatchProperties.GraphicsPatchObj, [cam]);
                HDLCPatchProperties.SetShadowQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, [HDAssetBundle, cam]);

                if (!DisableFoliageSetting)
                {
                    if (!EnableFoliage)
                    {
                        LayerMask mask = cam.GetComponent<Camera>().cullingMask;
                        mask &= ~(0x1 << 10);
                        cam.GetComponent<Camera>().cullingMask = mask;
                        MaskRemoved = true;
                    }
                    else
                    {
                        LayerMask mask = cam.GetComponent<Camera>().cullingMask;
                        mask |= 0x1 << 10;
                        cam.GetComponent<Camera>().cullingMask = mask;
                        MaskRemoved = false;
                    }
                }

                if (cam.gameObject.name == "SecurityCamera" || cam.gameObject.name == "ShipCamera") continue;

                SetAntiAliasing(cam);
            }     
        }

        public static void SetResolution(PlayerControllerB player)
        {
            int resolutionWidth = 860;
            int resolutionHeight = 520;

            if (HDLCPatch.ResolutionMethod.Value == ResolutionSettingMethod.ScaleSlider)
            {
                resolutionWidth = (int)Math.Round(860 * ResolutionScale, 0);
                resolutionHeight = (int)Math.Round(520 * ResolutionScale, 0);
            }
            else if(HDLCPatch.ResolutionMethod.Value == ResolutionSettingMethod.Presets)
            {
                switch(HDLCPatch.ResolutionPresetValue.Value)
                {
                    case ResolutionPreset.R640x480:
                        resolutionWidth = 640;
                        resolutionHeight = 480;
                        break;

                    case ResolutionPreset.R1280x720:
                        resolutionHeight = 720;
                        resolutionWidth = 1280;
                        break;

                    case ResolutionPreset.R1920x1080:
                        resolutionWidth = 1920;
                        resolutionHeight = 1080;
                        break;

                    case ResolutionPreset.R2560x1440:
                        resolutionWidth = 2560;
                        resolutionHeight = 1440;
                        break;

                    case ResolutionPreset.R3840x2160:
                        resolutionHeight = 2160;
                        resolutionWidth = 3840;
                        break;
                }
            }
            else if(HDLCPatch.ResolutionMethod.Value == ResolutionSettingMethod.Custom)
            {
                resolutionWidth = HDLCPatch.ResolutionWidth.Value;
                resolutionHeight = HDLCPatch.ResolutionHeight.Value;
            }

            if (!EnableResolutionOverride)
            {
                resolutionHeight = 520;
                resolutionWidth = 860;
            }

            RenderResolutionHeight = resolutionHeight;
            RenderResolutionWidth = resolutionWidth;

            //UnityEngine.Vector2 aspectRatio = GetAspectRatio(resolutionWidth, resolutionHeight);
            //UnityEngine.Vector2 normalizedRatio = GetNormalizedAspectRatio(resolutionWidth, resolutionHeight);

            //HDLCPatch.Logger.LogInfo("Resolution " + $"{resolutionWidth}x{resolutionHeight} Aspect Ratio: {aspectRatio.x}:{aspectRatio.y}");

            player.gameplayCamera.targetTexture.Release();
            //player.gameplayCamera.rect = new Rect(default, normalizedRatio) { center = new UnityEngine.Vector2(0.5f, 0.5f)};
            player.gameplayCamera.targetTexture.width = resolutionWidth;
            player.gameplayCamera.targetTexture.height = resolutionHeight;
            player.gameplayCamera.targetTexture.Create();
        }

        public static UnityEngine.Vector2 GetNormalizedAspectRatio(int width, int height)
        {
            UnityEngine.Vector2 aspectRatio = GetAspectRatio(width, height);
            UnityEngine.Vector2 resolution = new UnityEngine.Vector2(width, height);

            return aspectRatio / resolution;
        }

        public static UnityEngine.Vector2 GetAspectRatio(int width, int height)
        {
            int gcd = GetGCD(width, height);
            return new UnityEngine.Vector2(width / gcd, height / gcd);
        }

        public static int GetGCD(int a, int b)
        {
            while( a != 0 && b != 0 )
            {
                if(a > b)
                {
                    a %= b;
                }
                else
                {
                    b %= a;
                }
            }

            return a | b;
        }

        public static void TogglePostProcessing(HDAdditionalCameraData camera)
        {
            camera.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.CustomPass] = true;
            camera.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass, HDLCPatch.EnablePostProcessing.Value);
        }

        public static void SetFogQuality(HDAdditionalCameraData camera)
        {
            if (!CanChangeFog)
                return;

            camera.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.Volumetrics] = true;
            camera.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.Volumetrics, HDLCPatch.EnableFog.Value);

            try
            {
                Volume[] volumes = UnityEngine.Object.FindObjectsByType<Volume>(FindObjectsSortMode.None);
                BoolParameter fogEnabled = new BoolParameter(HDLCPatch.EnableFog.Value);

                for (int i = 0; i < volumes.Length; i++)
                {
                    if (volumes[i].sharedProfile.TryGet<Fog>(out Fog fog))
                    {
                        fog.enabled = fogEnabled;

                        fog.quality.Override(3);

                        switch(HDLCPatch.FogQuality.Value)
                        {
                            case QualitySetting.VeryLow:
                                fog.volumetricFogBudget = 0.025f;
                                fog.resolutionDepthRatio = 0.2f;
                                break;

                            case QualitySetting.Low:
                                fog.volumetricFogBudget = 0.05f;
                                fog.resolutionDepthRatio = 0.5f;
                                break;

                            case QualitySetting.Medium:
                                fog.volumetricFogBudget = 0.65f;
                                fog.resolutionDepthRatio = 0.6f;
                                break;

                            case QualitySetting.High:
                                fog.volumetricFogBudget = 0.75f;
                                fog.resolutionDepthRatio = 0.7f;
                                break;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                HDLCPatch.Logger.LogError($"Failed to set fog!\n{e.ToString()}");
            }
        }

        public static void SetTextureQuality()
        {
            int quality = 3 - TextureQuality;
            QualitySettings.globalTextureMipmapLimit = quality;
        }

        public static void SetAntiAliasing(HDAdditionalCameraData camera)
        {
            if(EnableAA)
            {
                if (EnablePatchOverride)
                {
                    switch (AAMode)
                    {
                        case AntiAliasingSetting.FAA:
                            camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing;
                            break;
                        case AntiAliasingSetting.TAA:
                            camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
                            break;
                        case AntiAliasingSetting.SMAA:
                            camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                            break;
                        default:
                            camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                            break;
                    }
                }
                else
                {
                    camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                }
            }
            else
            {
                camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
            }
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.GetTextureFromImage))]
        [HarmonyPostfix]
        public static Texture2D GetTextureFromImagePostfix(Texture2D __result)
        {
            __result.ignoreMipmapLimit = true;
            return __result;
        }

        [HarmonyPatch(typeof(HUDManager), "UpdateScanNodes")]
        [HarmonyPostfix]
        public static void UpdateScanNodesPostfix(PlayerControllerB playerScript, HUDManager __instance, Dictionary<RectTransform, ScanNodeProperties> ___scanNodes)
        {
            for(int i = 0; i < __instance.scanElements.Length; i++)
            {
                if (___scanNodes.TryGetValue(__instance.scanElements[i], out ScanNodeProperties scanNode))
                {
                    UnityEngine.Vector3 elementPos = playerScript.gameplayCamera.WorldToViewportPoint(scanNode.transform.position);
                    __instance.scanElements[i].anchoredPosition = new UnityEngine.Vector2(-450.65f + 901.3f * elementPos.x, -261.575f + 523.15f * elementPos.y);
                }   
            }
        }

        [HarmonyPatch(typeof(MenuManager), "SetLoadingScreen")]
        [HarmonyPrefix]
        public static void SetLoadingScreenPrefix()
        {
            StartCalled = false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPrefix]
        public static void StartPrefix(PlayerControllerB __instance)
        {
            if (!StartCalled)
            {
                CanChangeFog = true;
                PlayerRef = __instance;
                StartCalled = true;

                SettingsChanged();
            }
        }


        [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
        [HarmonyPrefix]
        public static void RoundPrefix(PlayerControllerB __instance)
        {
            CanChangeFog = false;
        }

        [HarmonyPatch(typeof(RoundManager), "GenerateNewFloor")]
        [HarmonyPostfix]
        public static void RoundPostfix(PlayerControllerB __instance)
        {
            CanChangeFog = true;

            SettingsChanged();

            if (LODQuality != 0) return;
            HDLCPatch.Logger.LogInfo("Removing catwalk stairs lod");
            HDLCPatchProperties.RemoveLodFromGameObject.Invoke(HDLCPatchProperties.GraphicsPatchObj, ["CatwalkStairs"]);
        }

        
    }
}
