﻿

using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AINavigation : MonoBehaviour
{
    [SerializeField] private float randomPointRadius = 50;
    [SerializeField] private float stoppingDistance = 0f;
    [SerializeField] private Transform player;
    [SerializeField] private float fieldOfViewAngle;
    [SerializeField] private LayerMask obstructionMask;

    private NavMeshAgent agent;
    private NavMeshPath path;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        path = new NavMeshPath();
    }

    private void Update()
    {
        //UpdateDestination();
    }

    private void UpdateDestination()
    {
        var check = CheckCanSeePlayer();
        Debug.Log(check);
        if (check)
        {
            agent.stoppingDistance = 2;
            agent.SetDestination(player.position);
        }
        else
            agent.stoppingDistance = 0;

        if (!agent.hasPath || (agent.hasPath && !check))
            TrySetNewDestination();
    }

    private bool CheckCanSeePlayer()
    {
        var directionToTarget = (player.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfViewAngle / 2)
        {
            var distanceToTarget = Vector3.Distance(transform.position, player.position);
            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                return true;
            }
        }

        return false;
    }

    private void TrySetNewDestination()
    {
        var randomPoint = transform.position + Random.insideUnitSphere * randomPointRadius;
        NavMesh.SamplePosition(randomPoint, out var hit, randomPointRadius, 1);
        var destination = hit.position;
        agent.CalculatePath(destination, path);
        if (path.status == NavMeshPathStatus.PathComplete)
            agent.SetPath(path);
    }
}