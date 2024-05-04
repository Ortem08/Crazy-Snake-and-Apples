using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : CreatureBase
{
    public GameObject LightningHolderCanvas;
    public GameObject LighningPrefab;
    public GameObject ProjectilePrefab;

    private int bossStage = 1;
    private GameObject player;
    private const int numberOfLightnings = 3;
    private const float attackDelay = 2f;
    private const float growSpeed = 0.2f;

    private const float jumpForce = 2.5f;
    private const float projectileSpeed = 5f;

    private int lastAbility;


    public Boss() : base(1000, 100)
    {
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //FireProjectiles();
        StartCoroutine(SelectAbility());
    }

    private IEnumerator PerformLightningAttack()
    {
        var bossPosition = transform.position;
        var angleStep = 360.0f / numberOfLightnings;

        for (var i = 0; i < numberOfLightnings; i++)
        {
            var angle = i * angleStep;
            var sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            var cos = Mathf.Cos(angle * Mathf.Deg2Rad);

            var direction = new Vector3(bossPosition.x + sin * 3, 0.01f, bossPosition.z + cos * 3);

            var lightning = Instantiate(LighningPrefab, LightningHolderCanvas.transform, worldPositionStays: true);
            lightning.transform.position = direction;
            lightning.transform.Rotate(0f, 0f, angle);


            StartCoroutine(GrowLightning(lightning, direction));
        }

        yield return new WaitForSeconds(attackDelay);

        // Тут код для нанесения урона
    }

    private IEnumerator GrowLightning(GameObject lightning, Vector3 direction)
    {
        var scale = 0.1f;
        while (scale < 1f)
        {
            //lightning.transform.position += direction * growSpeed;
            //lightning.transform.localScale = new Vector3(scale, scale, scale);
            scale += growSpeed;
            yield return null;
        }
    }

    private IEnumerator SelectAbility()
    {
        while (true)
        {
            Debug.Log(lastAbility);
            var ability = GetRandomAbility();
            switch (ability)
            {
                case 0:
                    StartCoroutine(JumpAttack());
                    break;
                case 1:
                    StartCoroutine(ChargeAttack());
                    break;
                case 2:
                    FireProjectiles();
                    break;
                case 3:
                    StartCoroutine(SpinAndShoot());
                    break;
            }
            yield return new WaitForSeconds(3.0f); // Wait time between abilities
        }
    }

    private int GetRandomAbility()
    {
        int ability;
        do
        {
            ability = Random.Range(0, 4);
        } while (ability == lastAbility);

        lastAbility = ability;
        return ability;
    }

    private IEnumerator JumpAttack()
    {
        for (var i = 0; i < 3; i++)
        {
            var playerDirection = player.transform.position - transform.position;
            playerDirection.y = player.transform.position.y * 10;
            GetComponent<Rigidbody>()
                .AddForce(playerDirection.normalized * (jumpForce * Mathf.Sqrt(playerDirection.magnitude)),
                    ForceMode.Impulse);

            Debug.Log(i == 2 ? "Last jump" : "Jump");

            yield return new WaitForSeconds(2.0f);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator ChargeAttack()
    {
        yield return new WaitForSeconds(1.5f);

        var direction = player.transform.position - transform.position;
        GetComponent<Rigidbody>().AddForce(3 * direction, ForceMode.Impulse);
    }

    private void FireProjectiles()
    {
        var angles = new [] { -30f, 0f, 30f };
        foreach (var angle in angles)
        {
            var projectile = Instantiate(ProjectilePrefab, transform.position + transform.forward * 2, Quaternion.identity);
            projectile.transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
            projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void FireSingleProjectile()
    {
        var projectile = Instantiate(ProjectilePrefab, transform.position + transform.forward * 2, Quaternion.identity);
        projectile.transform.forward = transform.forward;
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * projectileSpeed;
    }

    private IEnumerator SpinAndShoot()
    {
        const int desiredPuffs = 20;
        
        var donePuffs = 0;
        while (donePuffs < desiredPuffs)
        {
            transform.Rotate(0, 360f / desiredPuffs, 0);
            FireSingleProjectile();
            donePuffs++;
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void PerformAttack()
    {
    }
}