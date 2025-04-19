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

            var terrainObject = GameObject.Find($"Map/Container/{terrainPlacement.TerrainName}");
            if (!terrainObject)
                terrainObject = GameObject.Find($"Map/Container/{terrainPlacement.TerrainName}(Clone)");

            // Destroy the terrain object and then recreate it (why not)
            if (terrainObject)
                Object.Destroy(terrainObject);

            // Instantiate the terrain object
            terrainObject = Object.Instantiate(terrainPlacement.GameObject, _mapContainerObject.transform);
            terrainObject.transform.position = terrainPlacement.Position;

            var terrainComponent = terrainObject.GetComponent<Terrain>();
            var terrainColliderComponent = terrainObject.GetComponent<TerrainCollider>();
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
