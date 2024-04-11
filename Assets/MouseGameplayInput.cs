/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseGameplayInput : MonoBehaviour
{
    public event Action<Vector2> RotationInputReceived;

    public MouseGameplayInput(InputMap map)
    {
        map.KeyboardAndMouse.deltaMouse.performed += context =>
        {
            var mouseDelta = context.ReadValue<Vector2>();
            RotationInputReceived?.Invoke(mouseDelta);
        };
    }
}
*/