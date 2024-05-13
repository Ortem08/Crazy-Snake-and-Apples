using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] 
    private float deadBodyThresholdToRespawn = 0.8f;

    private int totalMobsAmount;
    private List<List<Vector3>> positions;
    private bool isFirstCall = true;

    public void Place(PlacementManager manager)
    {
        if (isFirstCall)
        {
            FirstPlace(manager);
            isFirstCall = false;
            return;
        }
        
        var deadBodyObjs = GameObject.FindGameObjectsWithTag("DeadBody").ToList();
        if (deadBodyObjs.Count < totalMobsAmount * deadBodyThresholdToRespawn)
            return;
        
        foreach (var obj in deadBodyObjs)
        {
            Destroy(obj);
        }

        for (int i = 0; i < records.Count; i++)
        {
            manager.Spawn(positions[i], records[i].prefab);
        }
    }

    private void FirstPlace(PlacementManager manager)
    {
        positions = new List<List<Vector3>>();
        for (int i = 0; i < records.Count; i++)
        {
            var record = records[i];
            positions.Add(new List<Vector3>());
            for (int j = 0; j < record.Amount; j++)
            {
                var position = manager.GetPosition(10);
                var transformPositions = manager.GetTransformPosition(position);
                Instantiate(record.prefab, transformPositions, Quaternion.identity);
                manager.AddOnPlacementMap(position, 3);
                positions[i].Add(transformPositions);
            }

            totalMobsAmount += record.Amount;
        }
    }
}
