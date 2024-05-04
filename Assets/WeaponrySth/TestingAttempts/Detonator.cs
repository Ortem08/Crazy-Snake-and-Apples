using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detonator : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        //var exp = Instantiate(explosionPrefab);
        
    }
}
