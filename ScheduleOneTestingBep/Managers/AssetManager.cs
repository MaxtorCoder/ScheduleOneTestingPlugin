using System.Collections.Generic;
using System.IO;
#if !DEBUG
using System.Reflection;
#endif

using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;
using ScheduleOneTestingBep.Models;
using ScheduleOneTestingBep.Utils;

using UnityEngine;

namespace ScheduleOneTestingBep.Managers;

public static class AssetManager
{
    public static AssetBundle LoadedAssetBundle;
    public static bool IsLoaded;

    static readonly Dictionary<string, GameObject> _loadedGameObjects = [];
    static readonly Dictionary<string, ScriptableObject> _loadedScriptableObjects = [];
    static readonly Dictionary<string, TerrainPlacement> _loadedTerrains = [];

    /// <summary>
    /// Load a <see cref="UnityEngine.AssetBundle"/> instance from the provided <see cref="assetBundleName"/>
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="forceReload"></param>
    public static void LoadAssetBundle(string assetBundleName, bool forceReload = false)
    {
        if (LoadedAssetBundle)
        {
            if (forceReload)
            {
                Plugin.Logger.LogInfo($"[AssetManager]: Reloading asset bundle {assetBundleName}");

                LoadedAssetBundle.Unload(true);
                LoadedAssetBundle = null;
            }
            else
            {
                Plugin.Logger.LogInfo($"[AssetManager]: AssetBundle '{assetBundleName}' already loaded. Skipping reload.");
                return;
            }
        }

#if DEBUG
        var path = $"{Application.streamingAssetsPath}\\{assetBundleName}";
        if (!File.Exists(path))
            return;

        var assetBundle = AssetBundle.LoadFromFile(path);
#else
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ScheduleOneTestingBep.Assets.{assetBundleName}");
        if (stream == null)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to load stream: {assetBundleName}");
            return;
        }

        var assetBundle = AssetBundle.LoadFromStream(stream);
#endif
        if (assetBundle == null)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to load AssetBundle: {assetBundleName}");
            return;
        }

        var assetNames = assetBundle.GetAllAssetNames();
        Plugin.Logger.LogInfo($"[AssetManager]: Loaded AssetBundle: {assetBundleName} with: {assetNames.Length} asset(s)");

        foreach (var assetName in assetNames)
            Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded Asset: {assetName}");

        LoadedAssetBundle = assetBundle;
    }

    /// <summary>
    /// Register the <see cref="AssetBundle"/>
    /// </summary>
    /// <param name="networkManager"></param>
    public static void Register(NetworkManager networkManager)
    {
        if (!LoadedAssetBundle)
            return;

        var assetBundleHash = LoadedAssetBundle.Get16BitHash();
        var netPrefabs = networkManager.GetPrefabObjects<SinglePrefabObjects>(assetBundleHash, createIfMissing: true);

        foreach (var assetName in LoadedAssetBundle.GetAllAssetNames())
        {
            var gameObject = LoadedAssetBundle.LoadAsset<GameObject>(assetName);
            if (gameObject && !_loadedGameObjects.ContainsKey(assetName))
            {
                var terrainComponent = gameObject.GetComponent<Terrain>();
                if (terrainComponent && !_loadedTerrains.ContainsKey(assetName))
                {
                    var terrainPlacement = new TerrainPlacement
                    {
                        TerrainName = assetName.Split('/')[^1]
                            .Replace(".prefab", "")
                            .CapitalizeEachWord(),
                        TerrainData = terrainComponent.terrainData,
                        TerrainMaterial = terrainComponent.materialTemplate,
                        Position = gameObject.transform.position
                    };
                    _loadedTerrains.Add(assetName, terrainPlacement);

                    Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded Terrain {assetName} ({terrainPlacement.TerrainName}) at position {gameObject.transform.position}");
                }
                else
                {
                    // Register if a NetworkObject is attached to the GameObject
                    var networkObject = gameObject.GetComponent<NetworkObject>();
                    if (networkObject)
                    {
                        Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded GameObject {assetName} with NetworkedObject");
                        netPrefabs.AddObject(networkObject, checkForDuplicates: true);
                    }
                    else
                        Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded GameObject {assetName}");

                    _loadedGameObjects.Add(assetName, gameObject);
                }
            }

            var scriptableObject = LoadedAssetBundle.LoadAsset<ScriptableObject>(assetName);
            if (scriptableObject && !_loadedScriptableObjects.ContainsKey(assetName))
            {
                Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded ScriptableObject {assetName}");
                _loadedScriptableObjects.TryAdd(assetName, scriptableObject);
            }
        }

        IsLoaded = true;
    }

    /// <summary>
    /// Reloads the <see cref="LoadedAssetBundle"/> instance and register the objects
    /// </summary>
    /// <param name="assetBundleName"></param>
    /// <param name="networkManager"></param>
    public static void ReloadAssetBundle(string assetBundleName, NetworkManager networkManager)
    {
        _loadedGameObjects.Clear();
        _loadedScriptableObjects.Clear();
        _loadedTerrains.Clear();

        LoadAssetBundle(assetBundleName, forceReload: true);
        Register(networkManager);
    }

    /// <summary>
    /// Retrieve a <see cref="GameObject"/> instance from stored <see cref="_loadedGameObjects"/> container
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public static GameObject GetRegisteredGameObject(string objectName)
    {
        if (_loadedGameObjects.TryGetValue(objectName, out var gameObject))
            return gameObject;

        Plugin.Logger.LogError($"[AssetManager]: Could not find loaded GameObject with name {objectName}");
        return null;
    }

    /// <summary>
    /// Retrieve a <see cref="ScriptableObject"/> instance from stored <see cref="_loadedScriptableObjects"/> container
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public static ScriptableObject GetRegisteredScriptableObject(string objectName)
    {
        if (_loadedScriptableObjects.TryGetValue(objectName, out var scriptableObject))
            return scriptableObject;

        Plugin.Logger.LogError($"[AssetManager]: Could not find loaded ScriptableObject with name {objectName}");
        return null;
    }

    /// <summary>
    /// Retrieve all <see cref="TerrainPlacement"/> instances.
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, TerrainPlacement> GetLoadedTerrains() => _loadedTerrains;
}
