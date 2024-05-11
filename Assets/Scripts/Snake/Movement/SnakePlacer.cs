using UnityEngine;

public class SnakePlacer : MonoBehaviour, IPlacer
{
    public void Place(PlacementManager manager)
    {
        transform.position = manager.GetTransformPosition(manager.GetPosition(0));
        GetComponent<AINavigation>().enabled = true;
    }
}