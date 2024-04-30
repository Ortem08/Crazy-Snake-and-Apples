using System;
using Environment;
using UnityEngine;

namespace Items
{
    public class DoorKey : MonoBehaviour
    {
        [SerializeField] private Door door;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                MakeDoorActive();
                Destroy(gameObject);
            }
        }

        public void MakeDoorActive() => door.enabled = true;
    }
}