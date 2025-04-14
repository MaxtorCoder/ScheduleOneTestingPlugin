using System.Collections.Generic;
using System.Linq;

using ScheduleOne;
using ScheduleOne.Building;
using ScheduleOne.EntityFramework;
using ScheduleOne.Equipping;
using ScheduleOne.ItemFramework;
using ScheduleOne.Storage;

using UnityEngine;

namespace ScheduleOneTestingBep.Managers;

public static class RegistryManager
{
    static readonly Dictionary<string, GameObject> _buildHandlerObjects = [];
    static readonly Dictionary<string, Equippable> _equippableObjects = [];
    static readonly Dictionary<string, StoredItem> _storedItemObjects = [];

    /// <summary>
    /// Register a <see cref="ItemDefinition"/> instance
    /// </summary>
    /// <param name="key"></param>
    /// <param name="prefabPath"></param>
    /// <param name="itemDefinitionPath"></param>
    public static void RegisterItem<T>(string key, string prefabPath, string itemDefinitionPath = "") where T : Object
    {
        prefabPath = $"assets/resources/{prefabPath}";

        if (!string.IsNullOrEmpty(itemDefinitionPath))
            itemDefinitionPath = $"assets/resources/{itemDefinitionPath}";

        var itemRegistry = Registry.Instance.ItemRegistry.FirstOrDefault(x => x.ID == key);
        switch (itemRegistry)
        {
            case null when !string.IsNullOrEmpty(itemDefinitionPath):
            {
                var itemDefinition = AssetManager.GetRegisteredScriptableObject(itemDefinitionPath);
                if (!itemDefinition)
                {
                    Plugin.Logger.LogError($"[AssetManager]: Failed to find definition for {itemDefinitionPath}");
                    return;
                }

                itemRegistry = new()
                {
                    Definition = (ItemDefinition)itemDefinition,
                    ID = key,
                    AssetPath = prefabPath
                };
                Registry.Instance.ItemRegistry.Add(itemRegistry);
                Registry.Instance.AddToItemDictionary(itemRegistry);

                break;
            }
            case null when string.IsNullOrEmpty(itemDefinitionPath):
            {
                Plugin.Logger.LogError($"[AssetManager]: Failed to create new ItemDefinition with key: {key}, itemDefinition argument is null");
                return;
            }
        }

        if (itemRegistry == null)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to retrieve itemRegistry for key: {key}");
            return;
        }

        var prefab = AssetManager.GetRegisteredGameObject(prefabPath);
        if (!prefab)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to find {prefabPath} in asset bundle");
            return;
        }

        if (_equippableObjects.TryGetValue(itemRegistry.Definition.Equippable.name, out var equippable))
            itemRegistry.Definition.Equippable = equippable;

        if (itemRegistry.Definition is StorableItemDefinition storableItemDefinition)
            if (_storedItemObjects.TryGetValue(storableItemDefinition.StoredItem.name, out var storedItem))
                storableItemDefinition.StoredItem = storedItem;

        if (itemRegistry.Definition is BuildableItemDefinition buildableItemDefinition)
        {
            var buildableItemComponent = prefab.GetComponent<BuildableItem>();
            if (!buildableItemComponent)
            {
                Plugin.Logger.LogError($"[AssetManager]: Failed to find \"BuildableItem\" component in {prefab}");
                return;
            }

            if (_buildHandlerObjects.TryGetValue(buildableItemDefinition.BuiltItem.BuildHandler.name, out var buildHandler))
                buildableItemComponent.buildHandler = buildHandler;

            // Override the build handler and the BuiltItem with the custom item
            buildableItemDefinition.BuiltItem = buildableItemComponent;
            Plugin.Logger.LogInfo($"[AssetManager]: Assigned {prefab}");
        }
    }

    /// <summary>
    /// Retrieve all <see cref="BuildStart_Base"/> and <see cref="BuildUpdate_Base"/> instances from already existing items in registry.
    /// </summary>
    /// <param name="registryInstance"></param>
    public static void RetrieveDataFromRegistry(Registry registryInstance)
    {
        foreach (var itemInRegistry in registryInstance.ItemRegistry)
        {
            if (itemInRegistry.Definition is BuildableItemDefinition buildableItemDefinition)
            {
                if (!buildableItemDefinition.BuiltItem)
                    continue;

                var buildHandler = buildableItemDefinition.BuiltItem.BuildHandler;
                _buildHandlerObjects.TryAdd(buildHandler.name, buildHandler);
            }

            if (itemInRegistry.Definition is StorableItemDefinition storableItemDefinition)
            {
                if (!storableItemDefinition.StoredItem)
                    continue;

                var storedItem = storableItemDefinition.StoredItem;
                _storedItemObjects.TryAdd(storedItem.name, storedItem);
            }

            if (itemInRegistry.Definition.Equippable)
                _equippableObjects.TryAdd(itemInRegistry.Definition.Equippable.name, itemInRegistry.Definition.Equippable);
        }

        Plugin.Logger.LogInfo($"[RegistryManager]: Loaded {_buildHandlerObjects.Count} build handler(s)");
        Plugin.Logger.LogInfo($"[RegistryManager]: Loaded {_storedItemObjects.Count} stored item(s)");
        Plugin.Logger.LogInfo($"[RegistryManager]: Loaded {_equippableObjects.Count} equippable(s)");
    }
}
