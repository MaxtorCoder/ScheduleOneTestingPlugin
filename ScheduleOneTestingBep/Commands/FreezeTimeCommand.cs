using System.Collections.Generic;
using ScheduleOne;
using ScheduleOne.GameTime;

namespace ScheduleOneTestingBep.Commands;

public class FreezeTimeCommand : Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        TimeManager.Instance.TimeProgressionMultiplier = 0f;
    }

    public override string CommandWord => "freezetime";
    public override string CommandDescription => "freezetime";
    public override string ExampleUsage => "freezetime";
}
