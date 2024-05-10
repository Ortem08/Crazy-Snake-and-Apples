using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SnakeHitboxHandler : MonoBehaviour, IHurtable
{
    public float Health => snakeController.Health;

    public float MaxHealth => snakeController.MaxHealth;

    public UnityEvent<float, float> OnHealthDecrease => snakeController.OnHealthDecrease;

    public UnityEvent<float, float> OnHealthIncrease => snakeController.OnHealthIncrease;

    private SnakeController snakeController;

    public void ConsumeDamage(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        snakeController.TakeDamage(damageInfo);
    }

    private void Start()
    {
        snakeController = GetComponentInParent<SnakeController>();
    }
}
