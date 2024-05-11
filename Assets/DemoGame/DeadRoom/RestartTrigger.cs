using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartTrigger : MonoBehaviour
{
    public event Action OnTriggerEvent;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }
        OnTriggerEvent?.Invoke();
    }
}
