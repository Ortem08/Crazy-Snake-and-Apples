using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to store layers - in case layers will be changed, they can be changed in one place
/// try to access layers masks from here pls
/// </summary>
public static class LayersStorage
{
    public static LayerMask Pierceable = LayerMask.GetMask("PierceableHurtables");

    public static LayerMask NotPierceableObstacles = LayerMask.GetMask("Default", "MazeWalls");
}
