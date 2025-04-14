using System.Collections.Generic;
using System.Linq;

using ScheduleOne;
using ScheduleOne.Equipping;
using ScheduleOne.ItemFramework;
using ScheduleOne.PlayerScripts;
using ScheduleOne.UI.Shop;

using ScheduleOneTestingBep.Constants;

namespace ScheduleOneTestingBep.Managers;

public static class ShopManager
{
    static readonly Dictionary<ShopCode, List<ShopListing>> _customShopListings = [];

    /// <summary>
    /// Adds an item to the <see cref="ShopCode"/>
    /// </summary>
    /// <param name="shopCode"></param>
    /// <param name="itemCode"></param>
    /// <param name="price"></param>
    public static void AddItemToShop(ShopCode shopCode, string itemCode, float? price = null)
    {
        var itemRegistry = Registry.Instance.ItemRegistry.FirstOrDefault(x => x.ID == itemCode);
        if (itemRegistry is not { Definition: StorableItemDefinition storableItemDefinition })
            return;

        var shopListing = new ShopListing();
        var basePrice = storableItemDefinition.BasePurchasePrice;
        if (price is not null)
        {
            shopListing.OverridePrice = true;
            shopListing.OverriddenPrice = price.Value;
            basePrice = price.Value;
        }

        var categoryString = storableItemDefinition.ShopCategories.Aggregate("", (current, categoryInstance) => current + $"{categoryInstance.Category}, ");
        shopListing.name = $"{storableItemDefinition.Name} ({basePrice}) ({categoryString})";
        shopListing.Item = storableItemDefinition;

        if (!_customShopListings.ContainsKey(shopCode))
            _customShopListings.Add(shopCode, []);

        _customShopListings[shopCode].Add(shopListing);

        Plugin.Logger.LogInfo($"[ShopManager]: Added {shopListing.name} to {shopCode}");
    }

    /// <summary>
    /// Retrieve a <see cref="List{T}"/> container instance of <see cref="ShopListing"/> instances via provided <see cref="ShopCode"/>
    /// </summary>
    /// <param name="shopCode"></param>
    /// <returns></returns>
    public static List<ShopListing> GetShopListings(ShopCode shopCode)
    {
        if (_customShopListings.TryGetValue(shopCode, out var shopListing))
            return shopListing;

        Plugin.Logger.LogError($"[ShopManager]: Could not find any custom listings for {shopCode}");
        return null;
    }
}
