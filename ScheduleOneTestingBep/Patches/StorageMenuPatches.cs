using HarmonyLib;

using ScheduleOne.Storage;
using ScheduleOne.UI;

using ScheduleOneTestingBep.Scripts.Components.ObjectScripts;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ScheduleOneTestingBep.Patches;

// [HarmonyPatch(typeof(StorageMenu))]
// public class StorageMenuPatches
// {
//     [HarmonyPatch(nameof(StorageMenu.Open), typeof(StorageEntity))]
//     [HarmonyPostfix]
//     public static void Open_Postfix(StorageMenu __instance, StorageEntity entity)
//     {
//         var doneButtonGameObject = GameObject.Find("UI/StorageMenu/Container/DoneButton");
//         if (!doneButtonGameObject)
//         {
//             Plugin.Logger.LogError($"{doneButtonGameObject} is null or the name is not correct");
//             return;
//         }
//
//         var doneButton = doneButtonGameObject.GetComponent<Button>();
//
//         var textComponent = doneButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
//         if (!textComponent)
//         {
//             Plugin.Logger.LogError($"Cannot find text component on {doneButton}");
//             return;
//         }
//
//         var toiletCustomComponent = entity.GetComponent<ToiletCustom>();
//         if (toiletCustomComponent)
//         {
//             textComponent.text = "FLUSH";
//             doneButton.onClick.RemoveAllListeners();
//             doneButton.onClick.AddListener(toiletCustomComponent.FlushToilet);
//         }
//         else
//         {
//             textComponent.text = "DONE";
//             doneButton.onClick.RemoveAllListeners();
//             doneButton.onClick.AddListener(StorageMenu.Instance.CloseMenu);
//         }
//     }
// }
