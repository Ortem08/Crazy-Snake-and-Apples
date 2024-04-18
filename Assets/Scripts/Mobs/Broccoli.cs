using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Broccoli : CreatureBase
{
    private const float BoomRadius = 4;
    private const float InAirHeight = 0.05f;
    private bool wasInAir;
    
    public Broccoli() : base(2, 100)
    {
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        if (GetDistanceToGround(gameObject) > InAirHeight)
        {
            wasInAir = true;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void PerformAttack()
    {
        var collidersArray = new Collider[50];
        var thisCollider = gameObject.GetComponent<Collider>();
        Physics.OverlapSphereNonAlloc(transform.position, BoomRadius, collidersArray);
        
        var creatureEntities = collidersArray
            .Where(x => x && x != thisCollider && x.CompareTag("Creature"))
            .Select(x => x.gameObject.GetComponent<CreatureBase>());
        
        foreach (var creature in creatureEntities)
        {
            creature.ConsumeDamage(Damage);
        }
    }
    
    private float GetDistanceToGround(GameObject obj)
    {
        const float groundLevel = 0f;
        var distance = obj.transform.position.y - groundLevel - gameObject.transform.localScale.y / 2;
        
        return distance;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (wasInAir)
        {
            Debug.Log("boom");
            PerformAttack();
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log("Broccoli DIED");
    }
}
