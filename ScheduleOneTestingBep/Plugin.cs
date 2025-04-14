using System.Collections;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;

using HarmonyLib;
using UnityEngine;

using ScheduleOne;
using ScheduleOne.PlayerScripts;
using ScheduleOneTestingBep.Managers;
using ScheduleOneTestingBep.Scripts.Components.ObjectScripts;

namespace ScheduleOneTestingBep;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");

        new Harmony($"com.maxtorcoder.{MyPluginInfo.PLUGIN_NAME}")
            .PatchAll();

        StartCoroutine(InitializeData());
    }

    IEnumerator InitializeData()
    {
        yield return new WaitUntil(() => AssetManager.IsLoaded);

        // Add any custom models etc.
        RegistryManager.RetrieveDataFromRegistry(Registry.Instance);
        RegistryManager.RegisterItem<ToiletCustom>("toilet", "decoration/toilet/toilet_built_custom.prefab");
        RegistryManager.RegisterItem<ToiletCustom>("goldentoilet", "decoration/toilet/goldentoilet_built_custom.prefab");

        RegistryManager.RegisterItem<Object>("dumbell", "decoration/object/dumbell_built.prefab", "decoration/object/dumbell.asset");
    }
}
