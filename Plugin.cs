using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace UnrestrictedPlacement
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource StaticLogger;
        public static bool Enabled { get; set; } = true;
        public static ConfigEntry<KeyCode> SwitchKey { get; set; }
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            SwitchKey = Config.Bind("", "Switch key", KeyCode.F8, "The key with which to enable and disable the mod.");

            StaticLogger = Logger;
        }
        private void Update()
        {
            if(Input.GetKeyDown(SwitchKey.Value))
            {
                Enabled = !Enabled;
            }
        }
    }
    public static class UnrestrictedPlacementPatch
    {
        [HarmonyPatch(typeof(IPlacingMode), "OnTriggerEnter")]
        public static class IPlacingMode_OnTriggerEnter_Patch
        {
            public static void Postfix(IPlacingMode __instance)
            {
                if (!Plugin.Enabled) return;
                //Plugin.StaticLogger.LogError("Removing triggers");
                __instance.m_Triggers.Clear();
            }
        }
        [HarmonyPatch(typeof(FurniturePlacer), "PlacingRaycast")]
        public static class FurniturePlacer_PlacingRaycast_Patch
        {
            public static void Postfix(FurniturePlacer __instance)
            {
                if (!Plugin.Enabled) return;
                __instance.m_CurrentPlacingMode.PlacedOnCorrectSurface = true;
            }
        }
        [HarmonyPatch(typeof(IPlacingMode), "UpdateHologramColor")]
        public static class IPlacingMode_UpdateHologramColor_Patch
        {
            public static void Prefix(IPlacingMode __instance)
            {
                //Plugin.StaticLogger.LogWarning(__instance.m_Triggers.Count);
                //Plugin.StaticLogger.LogWarning(__instance.m_OverrideAvailability);
            }
            public static void Postfix(IPlacingMode __instance)
            {
                if (!Plugin.Enabled) return;
                __instance.HologramColor = Color.green;
            }
        }
    }
}
