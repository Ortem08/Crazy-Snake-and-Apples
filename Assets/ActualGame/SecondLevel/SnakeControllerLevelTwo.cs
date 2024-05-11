using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerLevelTwo : SnakeController
{
    protected override void AfterDefeatAction()
    {
        ExplodeWithFruts();
    }

    private void ExplodeWithFruts()
    {
        var amount = 60;
        for (int i = 0; i < amount; i++)
        {
            var obj = Instantiate(ApplePrefab, transform.position + UnityEngine.Random.insideUnitSphere, UnityEngine.Random.rotation);
            var rb = obj.GetComponent<Rigidbody>();

            rb.AddExplosionForce(100, transform.position, 5, 3);
        }
        Destroy(gameObject);
    }
}
