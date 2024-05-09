using System;
using Environment;
using UnityEngine;

namespace Items
{
    public class DoorKey : MonoBehaviour, IPlacementItem
    {
        [SerializeField] private Door door;

        private void Start()
        {
            door.enabled = false;
        }

        public void OnPickUp()
        {
            door.enabled = true;
            Destroy(gameObject);
        }
        
        public void Place(PlacementManager manager)
        {
            var position = manager.GetPositionInUnity(manager.GetPosition());
            position.y = 1;
            transform.position = position;
        }
    }
}