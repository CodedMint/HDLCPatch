using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace HDLethalCompanyPatch
{
    [BepInPlugin("HDLCPatch", "HDLCPatch", "1.0.0")]
    [BepInDependency("HDLethalCompany")]
    public class HDLCPatch : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private Harmony _harmony;

        private void Awake()
        {
            _harmony = new Harmony("HDLCPatch");

            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin HDLCPatch is loaded!");

            List<MethodBase> methods = (List<MethodBase>)Harmony.GetAllPatchedMethods();

            foreach (MethodBase method in methods)
            {
                Logger.LogInfo("Got method: " + method.Name + " from Module: " + method.Module);

                if(method.Module.ToString() == "HDLethalCompanyRemake.dll" && method.Name == "RoundPostFix")
                {
                    Logger.LogInfo("Found RoundPostFix from HDLethalCompany attempting to remove problem patch...");
                    _harmony.Unpatch(method, HarmonyPatchType.Prefix);
                }
            }
        }
    }
}
