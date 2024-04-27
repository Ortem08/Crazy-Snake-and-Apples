using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        var exp = Instantiate(explosionPrefab);
        exp.transform.position = transform.position;
        exp.GetComponent<Explosion>().Explode(1, 2, 0.2f, 3);
        Destroy(gameObject);
    }
}
