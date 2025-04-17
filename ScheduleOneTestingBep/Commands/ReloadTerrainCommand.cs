using System.Collections.Generic;
using ScheduleOne;
using ScheduleOneTestingBep.Managers;

namespace ScheduleOneTestingBep.Commands;

public class ReloadTerrainCommand : Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        TerrainManager.OverrideTerrain();
    }

    public override string CommandWord => "reloadterrain";
    public override string CommandDescription => "reloadterrain";
    public override string ExampleUsage => "reloadterrain";
}
