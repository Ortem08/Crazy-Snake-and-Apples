using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnerRecord
{
    public GameObject prefab;
    public int Amount;
}

public class MobPlacer : MonoBehaviour, IPlacer
{
    [SerializeField]
    private List<SpawnerRecord> records;

    public void Place(PlacementManager manager)
    {
        foreach (var record in records)
        {
            for (int i = 0; i < record.Amount; i++)
            {
                var position = manager.GetPosition(10);
                Instantiate(record.prefab, manager.GetTransformPosition(position), Quaternion.identity);
                manager.AddOnPlacementMap(position, 3);
            }
        }
    }
}
