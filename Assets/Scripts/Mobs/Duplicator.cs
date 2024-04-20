using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplicator : MonoBehaviour, IHurtable
{
    public float Health => 1;

    public void ConsumeDamage(float amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        Instantiate(gameObject).name = "duplicator clone";
    }
}
