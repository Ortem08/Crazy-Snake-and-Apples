using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUser
{
    public Transform HandTransform { get; }

    public Transform CameraTransform { get; }

    public Transform SelfTransform { get; }

    public Vector3 Velocity { get; }
}
