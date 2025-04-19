using HarmonyLib;

using ScheduleOne;

using ScheduleOneTestingBep.Commands;

namespace ScheduleOneTestingBep.Patches;

[HarmonyPatch(typeof(Console))]
public class ConsolePatches
{
    [HarmonyPatch(nameof(Console.Awake))]
    [HarmonyPostfix]
    public static void Awake_Postfix(Console __instance)
    {
        Plugin.Logger.LogInfo("[ConsolePatches]: Adding custom commands");

        Console.commands.Add("addstore", new TestAddStoreCommand());
        Console.commands.Add("reloadterrain", new ReloadTerrainCommand());
        Console.commands.Add("freezetime", new FreezeTimeCommand());
    }
}
