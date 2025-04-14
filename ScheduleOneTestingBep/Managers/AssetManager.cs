using System.Collections.Generic;
//#if !DEBUG
using System.Reflection;
//#endif

using FishNet.Managing;
using FishNet.Managing.Object;
using FishNet.Object;

using ScheduleOneTestingBep.Utils;

using UnityEngine;

namespace ScheduleOneTestingBep.Managers;

public static class AssetManager
{
    public static AssetBundle LoadedAssetBundle;
    public static bool IsLoaded;

    static readonly Dictionary<string, GameObject> _loadedGameObjects = [];
    static readonly Dictionary<string, ScriptableObject> _loadedScriptableObjects = [];

    /// <summary>
    /// Load a <see cref="UnityEngine.AssetBundle"/> instance from the provided <see cref="assetBundleName"/>
    /// </summary>
    /// <param name="assetBundleName"></param>
    public static void LoadAssetBundle(string assetBundleName)
    {
        if (LoadedAssetBundle)
            return;

// #if DEBUG
//         var assetBundle = AssetBundle.LoadFromFile($"{Application.streamingAssetsPath}\\{assetBundleName}");
// #else
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ScheduleOneTestingBep.Assets.{assetBundleName}");
        if (stream == null)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to load stream: {assetBundleName}");
            return;
        }

        var assetBundle = AssetBundle.LoadFromStream(stream);
//#endif
        if (assetBundle == null)
        {
            Plugin.Logger.LogError($"[AssetManager]: Failed to load AssetBundle: {assetBundleName}");
            return;
        }

        var assetNames = assetBundle.GetAllAssetNames();
        Plugin.Logger.LogInfo($"[AssetManager]: Loaded AssetBundle: {assetBundleName} with: {assetNames.Length} asset(s)");

        LoadedAssetBundle = assetBundle;
    }

    /// <summary>
    /// Register the <see cref="AssetBundle"/>
    /// </summary>
    /// <param name="networkManager"></param>
    /// <param name="assetBundle"></param>
    public static void Register(NetworkManager networkManager, AssetBundle assetBundle)
    {
        if (!assetBundle)
            return;

        var assetBundleHash = assetBundle.Get16BitHash();
        var netPrefabs = networkManager.GetPrefabObjects<SinglePrefabObjects>(assetBundleHash, createIfMissing: true);

        foreach (var assetName in assetBundle.GetAllAssetNames())
        {
            var gameObject = assetBundle.LoadAsset<GameObject>(assetName);
            if (gameObject && !_loadedGameObjects.ContainsKey(assetName))
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

            var scriptableObject = assetBundle.LoadAsset<ScriptableObject>(assetName);
            if (scriptableObject && !_loadedScriptableObjects.ContainsKey(assetName))
            {
                Plugin.Logger.LogInfo($"[AssetManager]:     -> Loaded ScriptableObject {assetName}");
                _loadedScriptableObjects.TryAdd(assetName, scriptableObject);
            }
        }

        IsLoaded = true;
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
}
