using System;
using Environment;
using UnityEngine;

namespace Items
{
    public class DoorKey : MonoBehaviour, IPlacer
    {
        [SerializeField] private Door door;

        [SerializeField] private NotificationManager notificationManager;

        private void Start()
        {
            door.enabled = false;
        }

        public void OnPickUp()
        {
            door.enabled = true;
            notificationManager?.Notify("Теперь можно открыть дверь");
            Destroy(gameObject);
        }
        
        public void Place(PlacementManager manager)
        {
            var position = manager.GetTransformPosition(manager.GetPosition());
            position.y = 0;
            transform.position = position;
        }
    }
}