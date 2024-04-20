using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour, IHurtable
{
    private Inventory inventory;

    [SerializeField]
    private float health = 100;

    public float Health => health;

    public void ConsumeDamage(float amount)
    {
        TakeDamage(new DamageInfo(amount));
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        health -= damageInfo.Amount;
        Debug.Log($"Player Hurt: health = {health}");
        if (health < 0)
        {
            Debug.Log("stop shooting! I`m already dead");
        }
    }

    private void Awake()
    {
        inventory = new Inventory();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
