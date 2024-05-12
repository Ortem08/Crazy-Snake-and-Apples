using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDetector : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public float lifetime { get; set; } = 0.1f;

    public Vector3 ExpirationPos;

    public RaycastHit? HardHit;

    public float speed = 50.0f;
    public float tracerLength = 5.0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));   // may be use composition? nah, i`d win
        lineRenderer.material.color = Color.yellow;
    }

    /// <summary>
    /// beware: if notPierceable & Pierceable != 0 object that is contained in both may be returned twice
    /// </summary>
    public IEnumerable<RaycastHit> PerformAction(Vector3 origin, Vector3 direction, float range, LayerMask notPierceable,
        LayerMask pierceable, bool CanPierce = true, Vector3? visibleOrigin = null)
    {
        var actualRange = range;
        var expirationPos = origin + direction * range;

        LayerMask hardLayer = CanPierce ? notPierceable : notPierceable | pierceable;

        if (Physics.Raycast(origin, direction.normalized, out var hitOnHardSurface, range, hardLayer))
        {
            actualRange = hitOnHardSurface.distance;
            expirationPos = hitOnHardSurface.point;
            HardHit = hitOnHardSurface;
            yield return hitOnHardSurface;
            
        }
        ExpirationPos = expirationPos;
        StartCoroutine(ShowLineAndExpire(visibleOrigin == null ? origin : (Vector3)visibleOrigin, expirationPos));

        if (!CanPierce)
        {
            yield break;
        }

        foreach (var hitSoft in Physics.RaycastAll(
                    origin, direction.normalized, actualRange, pierceable))
        {
            yield return hitSoft;
        }
    }

    private IEnumerator ShowLineAndExpire(Vector3 start, Vector3 end)
    {
        float startTime = Time.time;
        Vector3 currentStart = start;

        lineRenderer.enabled = true;

        while (Vector3.Distance(currentStart, end) > tracerLength)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / Vector3.Distance(start, end);

            currentStart = Vector3.Lerp(start, end, fractionOfJourney);
            Vector3 currentEnd = currentStart + (end - start).normalized * tracerLength;

            lineRenderer.SetPosition(0, currentStart);
            lineRenderer.SetPosition(1, currentEnd);

            yield return null;
        }

        lineRenderer.enabled = false;
        Destroy(gameObject);
    }
}
