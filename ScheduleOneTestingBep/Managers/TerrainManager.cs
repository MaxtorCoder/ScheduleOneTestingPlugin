using FishNet;
using UnityEngine;

namespace ScheduleOneTestingBep.Managers;

public static class TerrainManager
{
    static GameObject _mapContainerObject;

    /// <summary>
    /// Intialize the <see cref="TerrainManager"/> instance.
    /// </summary>
    public static void Initialize()
    {
        _mapContainerObject = GameObject.Find("Map/Container/");
        if (!_mapContainerObject)
        {
            Plugin.Logger.LogError("[TerrainManager]: Could not find Main Terrain Container!");
            return;
        }

        OverrideTerrain();
    }

    /// <summary>
    /// Override the terrain data
    /// </summary>
    public static void OverrideTerrain()
    {
        AssetManager.ReloadAssetBundle("custom_asset_bundle", InstanceFinder.NetworkManager);

        foreach (var (_, terrainPlacement) in AssetManager.GetLoadedTerrains())
        {
            Plugin.Logger.LogInfo($"[TerrainManager]: Processing terrain fragment: {terrainPlacement.TerrainName} at {terrainPlacement.Position}");

            Terrain terrainComponent = null;
            TerrainCollider terrainColliderComponent = null;

            var terrainObject = GameObject.Find($"Map/Container/{terrainPlacement.TerrainName}");
            if (!terrainObject)
            {
                // Create the terrain object
                Plugin.Logger.LogInfo($"[TerrainManager]: Could not find terrain with name: \"Map/Container/{terrainPlacement.TerrainName}\"");
                terrainObject = new(terrainPlacement.TerrainName)
                {
                    transform =
                    {
                        position = terrainPlacement.Position,
                        parent = _mapContainerObject.transform
                    },
                };
                terrainComponent = terrainObject.AddComponent<Terrain>();
                terrainColliderComponent = terrainObject.AddComponent<TerrainCollider>();
            }

            terrainComponent ??= terrainObject.GetComponent<Terrain>();
            terrainColliderComponent ??= terrainObject.GetComponent<TerrainCollider>();
            if (!terrainComponent && !terrainColliderComponent)
            {
                Plugin.Logger.LogError($"[TerrainManager]: Failed to retrieve Terrain(Collider) component: {terrainComponent} {terrainColliderComponent}");
                continue;
            }

            terrainComponent.materialTemplate = terrainPlacement.TerrainMaterial;
            terrainComponent.terrainData = terrainPlacement.TerrainData;
            terrainColliderComponent.terrainData = terrainPlacement.TerrainData;
            Plugin.Logger.LogInfo($"[TerrainManager]: Finished processing terrain fragment: {terrainPlacement.TerrainName} at {terrainPlacement.Position}");
        }
    }
}
