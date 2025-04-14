using System;
using System.Linq;

using HarmonyLib;

using ScheduleOne.UI.Shop;

using ScheduleOneTestingBep.Constants;
using ScheduleOneTestingBep.Utils;

namespace ScheduleOneTestingBep.Patches;

[HarmonyPatch(typeof(ShopInterface))]
public class ShopInterfacePatches
{
    [HarmonyPatch(nameof(ShopInterface.Awake))]
    [HarmonyPrefix]
    public static void Awake_Prefix(ShopInterface __instance)
    {
        if (__instance.ShopCode == "shop" || __instance.Listings == null)
            return;

        var shopCode = (ShopCode)Enum.Parse(typeof(ShopCode), __instance.ShopCode.ToPascalCase());
        var customShopListings = Managers.ShopManager.GetShopListings(shopCode);
        if (customShopListings == null)
            return;

        foreach (var customShopListing in customShopListings)
        {
            if (__instance.Listings.Any(x => x.name == customShopListing.name))
            {
                Plugin.Logger.LogError($"[ShopManager]: Listing {customShopListing.name} already exists in {shopCode}.");
                continue;
            }

            __instance.Listings.Add(customShopListing);
            Plugin.Logger.LogInfo($"[ShopManager]: Listed {customShopListing.name} to {shopCode}.");
        }
    }
}
