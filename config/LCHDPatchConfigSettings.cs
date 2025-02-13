using HDLethalCompanyPatch.patches;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;

namespace HDLethalCompanyPatch.config
{
    public static class LCHDPatchConfigSettings
    {
        public static EnumDropDownConfigItem<QualitySetting> ShadowQualityItem;
        public static EnumDropDownConfigItem<QualitySetting> LODQualityItem;
        public static EnumDropDownConfigItem<QualitySetting> TextureQualityItem;
        public static EnumDropDownConfigItem<FogQualitySetting> FogQualityItem;
        public static EnumDropDownConfigItem<AntiAliasingSetting> AASettingItem;
        public static EnumDropDownConfigItem<ResolutionSettingMethod> ResolutionMethodItem;
        public static EnumDropDownConfigItem<ResolutionPreset> ResolutionPresetItem;
        public static EnumDropDownConfigItem<FogSettingMethod> FogSettingMethodItem;
        public static FloatSliderConfigItem ResolutionScaleItem;
        public static FloatSliderConfigItem VolumetricFogBudgetItem;
        public static FloatSliderConfigItem FogResolutionDepthRatioItem;
        public static IntInputFieldConfigItem ResolutionWidthItem;
        public static IntInputFieldConfigItem ResolutionHeightItem;
        public static BoolCheckBoxConfigItem EnableFogItem;
        public static BoolCheckBoxConfigItem EnablePostProcessingItem;
        public static BoolCheckBoxConfigItem EnableFoliageItem;
        public static BoolCheckBoxConfigItem EnableResolutionOverrideItem;
        public static BoolCheckBoxConfigItem EnableAntiAliasingItem;
        public static BoolCheckBoxConfigItem DisableFoliageConfigItem;

        public static void Setup()
        {
            LethalConfigManager.SetModDescription("A patch mod for HDLethalCompany that also includes runtime configs through LethalConfig");

            ResolutionScaleItem = new FloatSliderConfigItem(HDLCPatch.ResolutionScale, new FloatSliderOptions { Max = 4.465f, Min = 0.25f, RequiresRestart = false });
            EnableFogItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableFog, false);
            FogSettingMethodItem = new EnumDropDownConfigItem<FogSettingMethod>(HDLCPatch.FogQualityMethod, false);
            FogQualityItem = new EnumDropDownConfigItem<FogQualitySetting>(HDLCPatch.FogQuality, false);
            FogResolutionDepthRatioItem = new FloatSliderConfigItem(HDLCPatch.FogResolutionDepthRatio, new FloatSliderOptions { Max = 1.00f, Min = 0.01f, RequiresRestart = false });
            VolumetricFogBudgetItem = new FloatSliderConfigItem(HDLCPatch.VolumetricFogBudget, new FloatSliderOptions { Min = 0.01f, Max = 1.00f, RequiresRestart = false });
            ShadowQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.ShadowQuality, false);
            LODQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.LODQuality, false);
            EnablePostProcessingItem = new BoolCheckBoxConfigItem(HDLCPatch.EnablePostProcessing, false);
            EnableFoliageItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableFoliage, false);
            EnableResolutionOverrideItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableResolutionOverride, false);
            EnableAntiAliasingItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableAntiAliasing, false);
            TextureQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.TextureQuality, false);
            DisableFoliageConfigItem = new BoolCheckBoxConfigItem(HDLCPatch.DisableFoliageConfig, true);
            AASettingItem = new EnumDropDownConfigItem<AntiAliasingSetting>(HDLCPatch.AASetting, false);
            ResolutionMethodItem = new EnumDropDownConfigItem<ResolutionSettingMethod>(HDLCPatch.ResolutionMethod, false);
            ResolutionPresetItem = new EnumDropDownConfigItem<ResolutionPreset>(HDLCPatch.ResolutionPresetValue, false);
            ResolutionHeightItem = new IntInputFieldConfigItem(HDLCPatch.ResolutionHeight, false);
            ResolutionWidthItem = new IntInputFieldConfigItem(HDLCPatch.ResolutionWidth, false);

            LethalConfigManager.AddConfigItem(EnableResolutionOverrideItem);
            LethalConfigManager.AddConfigItem(ResolutionMethodItem);
            LethalConfigManager.AddConfigItem(ResolutionScaleItem);
            LethalConfigManager.AddConfigItem(ResolutionPresetItem);
            LethalConfigManager.AddConfigItem(ResolutionWidthItem);
            LethalConfigManager.AddConfigItem(ResolutionHeightItem);
            LethalConfigManager.AddConfigItem(FogSettingMethodItem);
            LethalConfigManager.AddConfigItem(FogQualityItem);
            LethalConfigManager.AddConfigItem(FogResolutionDepthRatioItem);
            LethalConfigManager.AddConfigItem(VolumetricFogBudgetItem);
            LethalConfigManager.AddConfigItem(ShadowQualityItem);
            LethalConfigManager.AddConfigItem(LODQualityItem);
            LethalConfigManager.AddConfigItem(TextureQualityItem);
            LethalConfigManager.AddConfigItem(EnablePostProcessingItem);
            LethalConfigManager.AddConfigItem(EnableFogItem);
            LethalConfigManager.AddConfigItem(EnableFoliageItem);
            LethalConfigManager.AddConfigItem(EnableAntiAliasingItem);
            LethalConfigManager.AddConfigItem(DisableFoliageConfigItem);
            LethalConfigManager.AddConfigItem(AASettingItem);
            LethalConfigManager.AddConfigItem(new GenericButtonConfigItem("Commands", "ForceSettingsChange", "Makes a call to HDLC to run a settings change event. \nUse this if internal references failed to set on first change", "Force Change Settings", ()=>
            {
                HDLCGraphicsPatch.SettingsChanged();
            }));
        }
    }
}
