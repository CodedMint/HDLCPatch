using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;


namespace HDLethalCompanyPatch.patches
{
    internal class HDLCGraphicsPatch
    {
        public static bool CanChangeFog = false;
        public static LayerMask DefaultCullingMask = 0;
        public static AssetBundle HDAssetBundle = null;
        public static PlayerControllerB PlayerRef;
        public static bool MaskRemoved = false;
        public static bool StartCalled = false;
        public static int RenderResolutionWidth = 1;
        public static int RenderResolutionHeight = 1;
        public static UnityEngine.Vector2 GameResolution = new Vector2(860, 520);
        private const int _DefaultFontSize = 30;
        public static Vector2 OriginalScale = new Vector2();
        public static bool OriginalScaleSet = false;
        public static float FontScale = 1;
        public static int CurrentFontWidth = 0;

        public static void SettingsChanged()
        {
            HDLCPatch.Logger.LogInfo("Applying settings changes...");

            //Make sure the player reference is not null before setting some settings
            if (PlayerRef != null)
            {
                try { SetCameraData(PlayerRef); } catch (Exception e) { HDLCPatch.Logger.LogError($"Failed to set camera related settings\n{e.ToString()}"); }
                try { SetResolution(PlayerRef); } catch (Exception e) { HDLCPatch.Logger.LogError($"Failed to set resolution\n{e.ToString()}"); }
            }

            try { SetTextureQuality(); } catch (Exception e) { HDLCPatch.Logger.LogError($"Failed to set texture quality\n{e.ToString()}"); }

            HDLCPatch.Logger.LogInfo("Settings applied");
        }

        #region Quality Settings

        #region Set Camera Data
        public static void SetCameraData(PlayerControllerB player)
        {
            HDAdditionalCameraData[] cameras = Resources.FindObjectsOfTypeAll<HDAdditionalCameraData>();

            foreach(HDAdditionalCameraData cam in cameras)
            {
                if (cam.gameObject.name == "MapCamera") continue;

                cam.customRenderingSettings = true;

                SetFogQuality(cam);
                TogglePostProcessing(cam);
                SetLODQuality(cam);

                SetShadowQuality(cam);

                ToggleFoliage(cam);

                if (cam.gameObject.name == "SecurityCamera" || cam.gameObject.name == "ShipCamera") continue;

                SetAntiAliasing(cam);
            }     
        }
        #endregion

        #region Toggle Foliage
        public static void ToggleFoliage(HDAdditionalCameraData cam)
        {
            if (HDLCPatch.DisableFoliageConfig.Value)
                return;

            if (!HDLCPatch.EnableFoliage.Value)
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
        #endregion

        #region Set LOD Quality
        //Do not change far clip plane here, keep that for a different setting such as view distance
        public static void SetLODQuality(HDAdditionalCameraData camera)
        {
            if (HDLCPatch.DisableLODConfig.Value)
                return;

            camera.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.LODBiasMode] = true;
            camera.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.LODBias] = true;

            camera.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.LODBiasMode, true);
            camera.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.LODBias, true);

            camera.renderingPathCustomFrameSettings.lodBiasMode = LODBiasMode.OverrideQualitySettings;
            
            switch(HDLCPatch.LODQuality.Value)
            {
                case QualitySetting.VeryLow:
                    camera.renderingPathCustomFrameSettings.lodBias = 0.6f;
                    break;

                case QualitySetting.Low:
                    camera.renderingPathCustomFrameSettings.lodBias = 0.9f;
                    break;

                case QualitySetting.Medium:
                    camera.renderingPathCustomFrameSettings.lodBias = 1.6f;
                    break;

                case QualitySetting.High:
                    camera.renderingPathCustomFrameSettings.lodBias = 2.3f;
                    break;
            }
        }
        #endregion

        #region Set Resolution
        public static void SetResolution(PlayerControllerB player)
        {
            if(HDLCPatch.DisableResolutionConfig.Value) 
                return;

            int resolutionWidth = 860;
            int resolutionHeight = 520;

            if (HDLCPatch.ResolutionMethod.Value == ResolutionSettingMethod.ScaleSlider)
            {
                resolutionWidth = (int)Math.Round(860 * HDLCPatch.ResolutionScale.Value, 0);
                resolutionHeight = (int)Math.Round(520 * HDLCPatch.ResolutionScale.Value, 0);
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

            if (!HDLCPatch.EnableResolutionOverride.Value)
            {
                resolutionHeight = 520;
                resolutionWidth = 860;
            }

            RenderResolutionHeight = resolutionHeight;
            RenderResolutionWidth = resolutionWidth;

            GameResolution = new UnityEngine.Vector2(resolutionWidth, resolutionHeight);

            //UnityEngine.Vector2 aspectRatio = GetAspectRatio(resolutionWidth, resolutionHeight);
            //UnityEngine.Vector2 normalizedRatio = GetNormalizedAspectRatio(resolutionWidth, resolutionHeight);

            //HDLCPatch.Logger.LogInfo("Resolution " + $"{resolutionWidth}x{resolutionHeight} Aspect Ratio: {aspectRatio.x}:{aspectRatio.y}");

            player.gameplayCamera.aspect = resolutionWidth / resolutionHeight;
            //player.playerScreen.canvas.renderingDisplaySize;

            player.gameplayCamera.targetTexture.Release();
            //player.gameplayCamera.rect = new Rect(default, normalizedRatio) { center = new UnityEngine.Vector2(0.5f, 0.5f)};
            player.gameplayCamera.targetTexture.width = resolutionWidth;
            player.gameplayCamera.targetTexture.height = resolutionHeight;
            player.gameplayCamera.targetTexture.Create();
        }
        #endregion

        #region Get Normalized Aspect Ratio
        public static UnityEngine.Vector2 GetNormalizedAspectRatio(int width, int height)
        {
            UnityEngine.Vector2 aspectRatio = GetAspectRatio(width, height);
            UnityEngine.Vector2 resolution = new UnityEngine.Vector2(width, height);

            return aspectRatio / resolution;
        }
        #endregion

        #region Get Aspect Ratio
        public static UnityEngine.Vector2 GetAspectRatio(int width, int height)
        {
            int gcd = GetGCD(width, height);
            return new UnityEngine.Vector2(width / gcd, height / gcd);
        }
        #endregion

        #region Greatest Common Denominator
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
        #endregion

        #region Set Shadow Quality
        public static void SetShadowQuality(HDAdditionalCameraData cam)
        {
            if (HDLCPatch.DisableShadowConfig.Value)
                return;

            if (!HDLCPatch.HDLethalCompanyFound)
                return;

            try
            {
                HDLCPatchProperties.SetShadowQuality.Invoke(HDLCPatchProperties.GraphicsPatchObj, [HDAssetBundle, cam]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to set shadow quality HDLC reference missing...\n" + e.ToString());
            }

            //HDRenderPipelineAsset pipelineAsset = (HDRenderPipelineAsset)QualitySettings.renderPipeline;
            //RenderPipelineSettings pipelineSettings = pipelineAsset.currentPlatformRenderPipelineSettings;
            //HDShadowInitParameters shadow = pipelineSettings.hdShadowInitParams;

            /*HDLCPatch.Logger.LogInfo("----Shadow Settings----\n" +
                $"AllowDirectionalMixedCachedShadows: {shadow.allowDirectionalMixedCachedShadows}\n" +
                $"AreaShadowFilteringQuality: {shadow.areaShadowFilteringQuality}\n" +
                $"DirectionalShadowsDepthBits: {shadow.directionalShadowsDepthBits}\n" +
                $"MaxAreaShadowMapResolution: {shadow.maxAreaShadowMapResolution}\n" +
                $"MaxDirectionalShadowMapResolution: {shadow.maxDirectionalShadowMapResolution}\n" +
                $"MaxPunctualShadowMapResolution: {shadow.maxPunctualShadowMapResolution}\n" +
                $"ShadowResolutionDirectional: {shadow.shadowResolutionDirectional}\n" +
                $"MaxShadowRequests: {shadow.maxShadowRequests}\n" +
                $"ScreenSpaceShadowBufferFormat: {shadow.screenSpaceShadowBufferFormat}\n" +
                $"SupportScreenSpaceShadows: {shadow.supportScreenSpaceShadows}\n" +
                $"ShadowResolutionPunctual: {shadow.shadowResolutionPunctual[0]},{shadow.shadowResolutionPunctual[1]},{shadow.shadowResolutionPunctual[2]},{shadow.shadowResolutionPunctual[3]}\n" +
                $"ShadowResolutionArea: {shadow.shadowResolutionArea}\n" +
                $"CachedPunctualLightShadowAtlas: {shadow.cachedPunctualLightShadowAtlas}\n" +
                $"CachedAreaLightShadowAtlas: {shadow.cachedAreaLightShadowAtlas}\n" +
                $"AreaLightShadowAtlas.ShadowAtlasResolution: {shadow.areaLightShadowAtlas.shadowAtlasResolution}\n" +
                $"AreaLightShadowAtlas.UseDynamicViewportRescale: {shadow.areaLightShadowAtlas.useDynamicViewportRescale}\n" +
                $"-----------------------");*/
        }
        #endregion

        #region Toggle Post Processing
        public static void TogglePostProcessing(HDAdditionalCameraData camera)
        {
            if (HDLCPatch.DisablePostProcessConfig.Value)
                return;

            camera.renderingPathCustomFrameSettingsOverrideMask.mask[(int)FrameSettingsField.CustomPass] = true;
            camera.renderingPathCustomFrameSettings.SetEnabled(FrameSettingsField.CustomPass, HDLCPatch.EnablePostProcessing.Value);
        }
        #endregion

        #region Set Fog Quality
        public static void SetFogQuality(HDAdditionalCameraData camera)
        {
            if (HDLCPatch.DisableFogConfig.Value)
                return;

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

                        if (HDLCPatch.FogQualityMethod.Value == FogSettingMethod.Presets)
                        {
                            switch (HDLCPatch.FogQuality.Value)
                            {
                                case FogQualitySetting.Minimal:
                                    fog.volumetricFogBudget = 0.01f;
                                    fog.resolutionDepthRatio = 0.1f;
                                    break;
                                case FogQualitySetting.VeryLow:
                                    fog.volumetricFogBudget = 0.025f;
                                    fog.resolutionDepthRatio = 0.2f;
                                    break;

                                case FogQualitySetting.Low:
                                    fog.volumetricFogBudget = 0.1f;
                                    fog.resolutionDepthRatio = 0.45f;
                                    break;

                                case FogQualitySetting.Medium:
                                    fog.volumetricFogBudget = 0.35f;
                                    fog.resolutionDepthRatio = 0.6f;
                                    break;

                                case FogQualitySetting.High:
                                    fog.volumetricFogBudget = 0.55f;
                                    fog.resolutionDepthRatio = 0.7f;
                                    break;
                                case FogQualitySetting.Extreme:
                                    fog.volumetricFogBudget = 0.75f;
                                    fog.resolutionDepthRatio = 0.8f;
                                    break;
                            }
                        }
                        else
                        {
                            fog.volumetricFogBudget = HDLCPatch.VolumetricFogBudget.Value;
                            fog.resolutionDepthRatio = HDLCPatch.FogResolutionDepthRatio.Value;
                        }
                    }

                }

            }
            catch(Exception e)
            {
                HDLCPatch.Logger.LogError($"Failed to set fog!\n{e.ToString()}");
            }
        }
        #endregion

        #region Set Texture Quality
        public static void SetTextureQuality()
        {
            if (HDLCPatch.DisableTextureConfig.Value)
                return;

            int quality = 3 - (int)HDLCPatch.TextureQuality.Value;
            QualitySettings.globalTextureMipmapLimit = quality;
        }
        #endregion

        #region Set Anti Aliasing
        public static void SetAntiAliasing(HDAdditionalCameraData camera)
        {
            if(HDLCPatch.EnableAntiAliasing.Value)
            {
                    switch (HDLCPatch.AASetting.Value)
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
                camera.antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
            }
        }
        #endregion
        #endregion

        #region Patches
        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.GetTextureFromImage))]
        [HarmonyPostfix]
        public static Texture2D GetTextureFromImagePostfix(Texture2D __result)
        {
            if(HDLCPatch.EnableSteamProfileImageFix.Value)
                __result.ignoreMipmapLimit = true;
            return __result;
        }

        [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
        [HarmonyPostfix]
        public static string TextPostProcessPostfix(string __result, Terminal __instance)
        {
            if (HDLCPatch.DisableResolutionConfig.Value || HDLCPatch.DisableTerminalResolutionFix.Value)
                return __result;

            if(!OriginalScaleSet)
            {
                OriginalScale = new Vector2(__instance.playerScreenTexHighRes.width, __instance.playerScreenTexHighRes.height);
                HDLCPatch.Logger.LogInfo("SCALE: " + OriginalScale.x + "|" + OriginalScale.y);
                OriginalScaleSet = true;
            }

            CurrentFontWidth = RenderResolutionWidth;

            if (__instance.playerScreenTexHighRes.width != CurrentFontWidth)
            {
                __instance.playerScreenTexHighRes.Release();
                __instance.playerScreenTexHighRes.height = RenderResolutionHeight;
                __instance.playerScreenTexHighRes.width = RenderResolutionWidth;
                __instance.playerScreenTexHighRes.Create();
            }

            return __result;
        }

        [HarmonyPatch(typeof(HUDManager), "UpdateScanNodes")]
        [HarmonyPostfix]
        public static void UpdateScanNodesPostfix(PlayerControllerB playerScript, HUDManager __instance, Dictionary<RectTransform, ScanNodeProperties> ___scanNodes)
        {
            if (HDLCPatch.DisableResolutionConfig.Value)
                return;

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

            if (HDLCPatch.LODQuality.Value != 0) return;
            if (HDLCPatch.DisableCatwalkRemoval.Value) return;

            HDLCPatch.Logger.LogInfo("Removing catwalk stairs lod");
            RemoveLOD("CatwalkStairs");
        }
        #endregion

        public static void RemoveLOD(string name)
        {
            UnityEngine.Object[] groups = Resources.FindObjectsOfTypeAll(typeof(LODGroup));

            foreach (LODGroup group in groups)
            {
                if (group.gameObject.name == name) group.enabled = false;
            }
        }


    }
}
