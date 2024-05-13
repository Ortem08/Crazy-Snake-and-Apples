using System.Collections.Generic;
using UnityEngine;

public class MicroApplesPlacer : MonoBehaviour, IPlacer
{
    private List<Vector3> spawnPositions;

    public void Place(PlacementManager manager)
    {
        if (spawnPositions.Count == 0)
        {
            spawnPositions = new List<Vector3>();
            for (int i = 0; i < 3; i++)
            {
                var position = manager.GetPosition(2);
                var transformPosition = manager.GetTransformPosition(position);
                manager.AddOnPlacementMap(position, 10);
                spawnPositions.Add(transformPosition);
            }
        }
        
        manager.Spawn(spawnPositions, gameObject);
    }
}