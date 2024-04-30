using System;
using UnityEngine;

namespace Environment
{
    public class Door : MonoBehaviour
    {
        public Transform Axis;
        [SerializeField] private float interactionRadius;
        [SerializeField] private LayerMask playerMask;
        private bool isOpen;

        private void Update()
        {
            if (Physics.CheckSphere(transform.position, interactionRadius, playerMask))
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    if (isOpen)
                        Close();
                    else
                        Open();
                    isOpen = !isOpen;
                }
            }
        }
        
        

        public void Open() => Axis.Rotate(Vector3.up, 90);

        public void Close() => Axis.Rotate(Vector3.up, -90);
    }
}