using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Broccoline : CreatureBase
{
    public GameObject broccoliToThrow;

    private GameObject player;
    private Rigidbody rb;
    
    private const float ViewAngle = 110f;
    private const float ViewDistance = 20f;
    private const float MaxSpeed = 10f;
    private const float Cooldown = 5f;
    private const float LaunchAngle = 45f; // ”гол запуска
    private const float ProjectileSpeed = 5f; // —корость снар€да
    
    private float lastAbilityUse;
    private bool isGrounded;
    private bool isRunning;
    
    public Broccoline() : base(100, 20)
    {
    }
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody>();
        isGrounded = true;
    }

    void Update()
    {
        if (Time.time - lastAbilityUse > Cooldown && IsPlayerDetected())
        {
            SpeedUp();
        }
        /*else if (Time.time - lastAbilityUse > Cooldown 
            && Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            Jump();
        }
        else
        {
            PerformAttack();
        }*/
    }
    
    public override void PerformAttack()
    {
        //player.ConsumeDamage(Damage);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;
        
        if (isRunning)
        {
            //player.ConsumeDamage(Damage * 2);
            isRunning = false;
        }
        else if (!isGrounded)
        {
            //player.ConsumeDamage(Damage * 3);
            isGrounded = true;
        }
    }
    
    private bool IsPlayerDetected()
    {
        var dirToPlayer = player.transform.position - transform.position;
        var angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        if (!(angleToPlayer < ViewAngle / 2) || !(dirToPlayer.magnitude < ViewDistance)) 
            return false;

        if (!Physics.Raycast(transform.position, dirToPlayer.normalized, out var hit, ViewDistance)) 
            return false;
        
        return hit.transform == player.transform;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        isGrounded = false;
        lastAbilityUse = Time.time;
    }

    private void SpeedUp()
    {
        var vector = (player.transform.position - transform.position) * 10;
        rb.AddForce(vector, ForceMode.VelocityChange);
        isRunning = true;
        lastAbilityUse = Time.time;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ThrowBroccoli()
    {
        var playerDirection = player.transform.position - transform.position;
        playerDirection.y = 0; //не уверен что надо
        
        var projectile = Instantiate(broccoliToThrow, transform.position, Quaternion.identity);
        var rb = projectile.GetComponent<Rigidbody>();

        var velocity = playerDirection.normalized * ProjectileSpeed * Mathf.Pow(playerDirection.magnitude, 0.5f) + transform.up * ProjectileSpeed;
        rb.AddForce(velocity, ForceMode.Impulse);
        lastAbilityUse = Time.time;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ScatterBroccoli()
    {
        lastAbilityUse = Time.time;
        
        foreach (var angle in GetSequence(360, 8))
        {
            var projectile = Instantiate(broccoliToThrow, transform.position, Quaternion.identity);
            var rb = projectile.GetComponent<Rigidbody>();

            var x = transform.position.x + Mathf.Sin(angle * Mathf.Deg2Rad);
            var z = transform.position.z + Mathf.Cos(angle * Mathf.Deg2Rad);

            var velocity = new Vector3(x, transform.position.y, z).normalized * ProjectileSpeed
                           + transform.up * ProjectileSpeed;
            Debug.Log(velocity.x);
            rb.AddForce(velocity, ForceMode.Impulse);
        }
    }

    private IEnumerable<int> GetSequence(int total, int count)
    {
        var current = 0;
        var step = total / count;
        while (current <= total)
        {
            yield return current;
            current += step;
        }
    }
}
