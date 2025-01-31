using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


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
        public static PlayerControllerB playerRef;
        public static RenderTexture renderTexture;
        public static bool SetRenderResolution = true;
        public static bool MaskRemoved = false;
        public static int TextureQuality = 3;
        public static bool DisableFoliageSetting = false;
        //public static int ResolutionWidth = 1920;
        //public static int ResolutionHeight = 1080;

        public static void SettingsChanged()
        {
            if(!HasDefaults)
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

            FogEnabled = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFog").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            EnablePostProcessing = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enablePostProcessing").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            EnableFoliage = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableFoliage").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            EnableResolutionOverride = (bool)HDLCPatchProperties.GraphicsPatch.GetField("m_enableResolutionFix").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            ResolutionScale = (float)HDLCPatchProperties.GraphicsPatch.GetField("multiplier").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            TextureQuality = (int)HDLCPatchProperties.GraphicsPatch.GetField("m_setTextureResolution").GetValue(HDLCPatchProperties.GraphicsPatchObj);
            DisableFoliageSetting = HDLCPatch.DisableFoliageConfig.Value;

            if (CanChangeFog)
            {
                HDLCPatchProperties.SetFogQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, null);
            }

            if (playerRef != null)
            {
                SetCameraData(playerRef);
            }
        }

        public static void SetCameraData(PlayerControllerB player)
        {
            HDAdditionalCameraData[] cameras = Resources.FindObjectsOfTypeAll<HDAdditionalCameraData>();

            foreach(HDAdditionalCameraData cam in cameras)
            {
                if (cam.gameObject.name == "MapCamera") continue;

                cam.customRenderingSettings = true;

                HDLCPatchProperties.ToggleCustomPass.Invoke(HDLCPatchProperties.GraphicsPatchObj, [cam, EnablePostProcessing]);
                HDLCPatchProperties.SetLevelOfDetail.Invoke(HDLCPatchProperties.GraphicsPatchObj, [cam]);
                HDLCPatchProperties.ToggleVolumetricFog.Invoke(HDLCPatchProperties.GraphicsPatchObj, [cam, FogEnabled]);

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

                HDLCPatchProperties.SetShadowQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, [HDAssetBundle, cam]);

                if (cam.gameObject.name == "SecurityCamera" || cam.gameObject.name == "ShipCamera") continue;

                SetAntiAliasing(cam);
            }

            SetTextureQuality();

            if (CanChangeFog)
            {
                HDLCPatchProperties.SetFogQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, null);
            }

            int resolutionWidth = (int)Math.Round(860 * ResolutionScale, 0);
            int resolutionHeight = (int)Math.Round(520 * ResolutionScale, 0);

            if (!EnableResolutionOverride)
            {
                resolutionHeight = 520;
                resolutionWidth = 860;
            }

            HDLCPatch.Logger.LogInfo("Resolution " + $"{resolutionWidth}x{resolutionHeight} Scale {ResolutionScale}");

            player.gameplayCamera.targetTexture.Release();
            player.gameplayCamera.targetTexture.width = resolutionWidth;
            player.gameplayCamera.targetTexture.height = resolutionHeight;
            player.gameplayCamera.targetTexture.Create();

            SetTextureQuality();
        }

        public static void SetTextureQuality()
        {
            int quality = 3 - TextureQuality;
            QualitySettings.globalTextureMipmapLimit = quality;
        }

        public static void SetAntiAliasing(HDAdditionalCameraData camera)
        {
            if (HDLCPatch.EnableAntiAliasing.Value)
            {
                camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            }
            else
            {
                camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPrefix]
        public static void StartPrefix(PlayerControllerB __instance)
        {
            CanChangeFog = true;

            SettingsChanged();
            SetCameraData(__instance);

            if (EnableResolutionOverride && SetRenderResolution)
            {
                int resolutionWidth = (int)Math.Round(860 * ResolutionScale, 0);
                int resolutionHeight = (int)Math.Round(520 * ResolutionScale, 0);

                HDLCPatch.Logger.LogInfo("Resolution " + $"{resolutionWidth}x{resolutionHeight} Scale {ResolutionScale}");

                __instance.gameplayCamera.targetTexture.width = resolutionWidth;
                __instance.gameplayCamera.targetTexture.height = resolutionHeight;

                SetRenderResolution = false;
            }

            playerRef = __instance;
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

            HDLCPatch.Logger.LogInfo("Setting fog");
            HDLCPatchProperties.SetFogQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, null);
            HDLCPatch.Logger.LogInfo("Fog set");

            HDLCPatch.Logger.LogInfo("Getting LOD value");
            int lodQuality = (int)HDLCPatchProperties.GraphicsPatch.GetRuntimeField("m_setLOD").GetValue(HDLCPatchProperties.GraphicsPatchObj);

            if (lodQuality != 0) return;
            HDLCPatch.Logger.LogInfo("Removing catwalk stairs lod");
            HDLCPatchProperties.RemoveLodFromGameObject.Invoke(HDLCPatchProperties.GraphicsPatchObj, ["CatwalkStairs"]);
        }

        
    }
}
