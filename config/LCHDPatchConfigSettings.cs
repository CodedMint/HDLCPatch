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
        public static EnumDropDownConfigItem<QualitySetting> FogQualityItem;
        public static EnumDropDownConfigItem<AntiAliasingSetting> AASettingItem;
        public static FloatSliderConfigItem ResolutionScaleItem;
        public static BoolCheckBoxConfigItem EnableHDPatchOverrideSettingsItem;
        public static BoolCheckBoxConfigItem EnableFogItem;
        public static BoolCheckBoxConfigItem EnablePostProcessingItem;
        public static BoolCheckBoxConfigItem EnableFoliageItem;
        public static BoolCheckBoxConfigItem EnableResolutionOverrideItem;
        public static BoolCheckBoxConfigItem EnableAntiAliasingItem;
        public static BoolCheckBoxConfigItem DisableFoliageConfigItem;

        public static void Setup()
        {
            LethalConfigManager.SetModDescription("A patch mod for HDLethalCompany that also includes runtime configs through LethalConfig");

            EnableHDPatchOverrideSettingsItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableHDPatchOverrideSettings, false);
            ResolutionScaleItem = new FloatSliderConfigItem(HDLCPatch.ResolutionScale, new FloatSliderOptions { Max = 4.465f, Min = 1, RequiresRestart = false });
            EnableFogItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableFog, false);
            FogQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.FogQuality, false);
            ShadowQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.ShadowQuality, false);
            LODQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.LODQuality, false);
            EnablePostProcessingItem = new BoolCheckBoxConfigItem(HDLCPatch.EnablePostProcessing, false);
            EnableFoliageItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableFoliage, false);
            EnableResolutionOverrideItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableResolutionOverride, false);
            EnableAntiAliasingItem = new BoolCheckBoxConfigItem(HDLCPatch.EnableAntiAliasing, false);
            TextureQualityItem = new EnumDropDownConfigItem<QualitySetting>(HDLCPatch.TextureQuality, false);
            DisableFoliageConfigItem = new BoolCheckBoxConfigItem(HDLCPatch.DisableFoliageConfig, true);
            AASettingItem = new EnumDropDownConfigItem<AntiAliasingSetting>(HDLCPatch.AASetting, false);

            LethalConfigManager.AddConfigItem(EnableHDPatchOverrideSettingsItem);
            LethalConfigManager.AddConfigItem(ResolutionScaleItem);
            LethalConfigManager.AddConfigItem(FogQualityItem);
            LethalConfigManager.AddConfigItem(ShadowQualityItem);
            LethalConfigManager.AddConfigItem(LODQualityItem);
            LethalConfigManager.AddConfigItem(TextureQualityItem);
            LethalConfigManager.AddConfigItem(EnablePostProcessingItem);
            LethalConfigManager.AddConfigItem(EnableFogItem);
            LethalConfigManager.AddConfigItem(EnableFoliageItem);
            LethalConfigManager.AddConfigItem(EnableResolutionOverrideItem);
            LethalConfigManager.AddConfigItem(EnableAntiAliasingItem);
            LethalConfigManager.AddConfigItem(DisableFoliageConfigItem);
            LethalConfigManager.AddConfigItem(AASettingItem);
        }
    }
}
