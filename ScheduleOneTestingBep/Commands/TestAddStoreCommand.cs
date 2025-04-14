using System.Collections.Generic;
using ScheduleOne;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI.Shop;

namespace ScheduleOneTestingBep.Commands;

public class TestAddStoreCommand : Console.ConsoleCommand
{
    public override void Execute(List<string> args)
    {
        if (args.Count == 0)
            return;

        var itemCode = args[0];
        ShopManager.Instance.SetStock(Player.Local.Connection, "gas_mart_central", itemCode, 100);
    }

    public override string CommandWord => "addstore";
    public override string CommandDescription => "Adds a test item to a store";
    public override string ExampleUsage => "addstore <itemCode>";
}
