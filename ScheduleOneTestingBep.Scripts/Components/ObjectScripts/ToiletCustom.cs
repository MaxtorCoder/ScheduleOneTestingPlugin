using System.Collections;

using FishNet.Object;

using UnityEngine.Events;

using ScheduleOne.Interaction;
using ScheduleOne.Storage;
using UnityEngine;

namespace ScheduleOneTestingBep.Scripts.Components.ObjectScripts
{
    public class ToiletCustom : NetworkBehaviour
    {
        public StorageEntity storageEntity;
        public UnityEvent onFlush;
        public InteractableObject interactableObject;

        bool _isFlushing;

        public void Interacted()
        {
            _isFlushing = true;
            ServerFlush();
        }

        public void Hovered()
        {
            if (!_isFlushing)
            {
                interactableObject.SetInteractableState(InteractableObject.EInteractableState.Default);
                interactableObject.SetMessage("Flush");
            }
            else
                interactableObject.SetInteractableState(InteractableObject.EInteractableState.Disabled);
        }

        [ServerRpc(RequireOwnership = false)]
        void ServerFlush() => ClientFlush();

        [ObserversRpc]
        void ClientFlush()
        {
            _isFlushing = true;
            StartCoroutine(FlushCoroutine());
        }

        IEnumerator FlushCoroutine()
        {
            onFlush?.Invoke();

            yield return new WaitForSeconds(0.5f);
            storageEntity.ClearContents();
            yield return new WaitForSeconds(0.5f);

            _isFlushing = false;
        }
    }
}
