using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class GunShot : MonoBehaviour, IProjectile, IDamaging, IPierceable
{
    public event Action<IProjectileInfo> OnProjectileEvent;

    public DamageInfo DamageInfo { get; set; } = new DamageInfo(10);

    [DoNotSerialize]
    public bool CanPierce { get; set; } = false;

    private readonly float range = 100;

    private LineRenderer lineRenderer;

    private bool used = false;

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
        //FireNotPiercing(origin, direction);

        //FirePiercing(origin, direction);

        if (used)
        {
            throw new Exception("wtf maaan! its disposable");
        }
        used = true;

        var rangeLimit = range;
        var expirationPos = origin + direction.normalized * range;

        if (CanPierce)
        {
            if (Physics.Raycast(origin, direction.normalized, out var hitOnHardSurface, rangeLimit, LayersStorage.NotPierceableObstacles))
            {
                rangeLimit = hitOnHardSurface.distance;
                expirationPos = hitOnHardSurface.point;
                AttemptHurting(hitOnHardSurface.collider.gameObject);
            }

            Physics.RaycastAll(
                    origin, direction.normalized, rangeLimit, LayersStorage.Pierceable
                ).Select(hit => AttemptHurting(hit.collider.gameObject))
                .ToList();
        }
        else
        {
            if (Physics.Raycast(origin, direction.normalized, out var firstHit, rangeLimit))
            {
                AttemptHurting(firstHit.collider.gameObject);
                expirationPos = firstHit.point;
            }

        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, expirationPos);

        StartCoroutine(ShowLaserAndExpire(expirationPos, direction));
    }

    private bool AttemptHurting(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<IHurtable>(out var hurtable))
        {
            hurtable.TakeDamage(DamageInfo);
            return true;
        }
        return false;
    }

    private void FirePiercing(Vector3 origin, Vector3 direction)
    {

        var rangeLimit = range;
        var expirationPos = origin + direction.normalized * range;
        if (Physics.Raycast(origin, direction.normalized, out var hit, rangeLimit, LayersStorage.NotPierceableObstacles))
        {
            rangeLimit = hit.distance;
            expirationPos = hit.point;
        }

        foreach (var hitInfo in Physics.RaycastAll(
            origin, direction.normalized, rangeLimit, LayersStorage.Pierceable))
        {
            if (hitInfo.collider.gameObject.TryGetComponent<IHurtable>(out var hurtable))
            {
                hurtable.TakeDamage(DamageInfo);
            }
        }

        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, expirationPos);

        StartCoroutine(ShowLaserAndExpire(expirationPos, direction));
    }

    private void FireNotPiercing(Vector3 origin, Vector3 direction)
    {
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
