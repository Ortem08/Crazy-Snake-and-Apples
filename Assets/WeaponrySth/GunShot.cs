using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GunShot : MonoBehaviour, IProjectile, IDamaging
{
    public event Action<IProjectileInfo> OnProjectileEvent;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    private readonly float range = 100;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? nah, i`d win
        lineRenderer.material.color = Color.yellow;
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        // Debug.Log("nieeeeaaayy!!!");
        RaycastHit hit;
        Vector3 shootDirection = direction.normalized;
        Vector3 startPosition = origin;

        var expirationPos = startPosition + shootDirection * range;

        if (Physics.Raycast(startPosition, shootDirection, out hit, range))
        {
            //Debug.Log(hit.rigidbody);

            if (hit.collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(DamageInfo);

            }

            expirationPos = hit.point;

            lineRenderer.SetPosition(0, startPosition);     // why ???
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            // why different?
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, startPosition + shootDirection * range);
        }
        
        

        StartCoroutine(ShowLaserAndExpire(expirationPos, direction));
    }

    private IEnumerator ShowLaserAndExpire(Vector3 expirationPos, Vector3 direction)
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.1f); // ”меньшенное врем€ видимости дл€ имитации вспышки
        lineRenderer.enabled = false;

        OnProjectileEvent?.Invoke(
                new CompositionBasedProjectileInfo(new object[] { 
                    new LocationInfo(
                            expirationPos,
                            direction
                        ),
                })
            );
        Destroy(gameObject);
    }

    public bool TryGetModificationInterface<T>(out T modifiable)
        where T : class
    {
        modifiable = this as T;
        if (modifiable != null)
        {
            return true;
        }
        return false;
    }
}
