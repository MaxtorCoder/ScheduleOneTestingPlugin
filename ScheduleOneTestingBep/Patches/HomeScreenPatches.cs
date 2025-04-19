using HarmonyLib;
using ScheduleOne.UI.Phone;
using ScheduleOneTestingBep.Managers;

namespace ScheduleOneTestingBep.Patches;

[HarmonyPatch(typeof(HomeScreen))]
public class HomeScreenPatches
{
    [HarmonyPatch(nameof(HomeScreen.Awake))]
    [HarmonyPostfix]
    public static void Awake_Postfix(HomeScreen __instance)
    {
        AppManager.Initialize(__instance);
    }
}
