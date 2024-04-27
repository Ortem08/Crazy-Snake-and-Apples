using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationInfo : ILocationInfo
{
    public LocationInfo(Vector3 posion, Vector3 direction)
    {
        Posion = posion;
        Direction = direction;
    }

    public Vector3 Posion { get; set; }

    public Vector3 Direction { get; set; }
}
