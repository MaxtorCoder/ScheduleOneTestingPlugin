using FishNet;

using HarmonyLib;

using ScheduleOne;
using ScheduleOneTestingBep.Managers;

namespace ScheduleOneTestingBep.Patches;

[HarmonyPatch(typeof(Registry))]
public class RegistryPatches
{
    [HarmonyPatch(nameof(Registry.Awake))]
    [HarmonyPrefix]
    public static void Awake_Prefix(Registry __instance)
    {
        AssetManager.LoadAssetBundle("custom_asset_bundle");
        AssetManager.Register(InstanceFinder.NetworkManager, AssetManager.LoadedAssetBundle);
    }
}
