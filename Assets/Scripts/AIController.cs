using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIController : MonoBehaviour, IController
{
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField]private float moveRadius = 15f;
    [SerializeField]private float viewRadius = 10f;
    [SerializeField]private float viewAngle = 120f;
    [SerializeField]private float speed = 3f;

    [SerializeField]private Transform player;
    private NavMeshAgent agent;
    [SerializeField] private bool isPlayerVisible = false;
    [SerializeField] private bool moveRandomLocation = false;
    [SerializeField] private Mask playerMask;

    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player").transform;

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

    }

    private void Update()
    {
        if (IsPlayerInSight())
        {
            isPlayerVisible = true;
            agent.SetDestination(player.position);
        }
        else
        {
            isPlayerVisible = false;
        }

        if (moveRandomLocation && !navAgent.pathPending && navAgent.remainingDistance < 0.5f)
        {
            // Calculate a random point within the move radius
            Vector2 randomPoint = (UnityEngine.Random.insideUnitCircle * moveRadius) + (Vector2)transform.position;

            // Set the destination of the NavMeshAgent to the random point
            navAgent.SetDestination(randomPoint);
        }
    }
    private bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        if (directionToPlayer.magnitude > viewRadius) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer > viewAngle / 2f) return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, viewRadius );
        if (hit.collider != null && hit.collider.CompareTag("Player")) return false;
        
        if (hit.collider.gameObject.CompareTag("Player"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsPlayerVisible()
    {
        return isPlayerVisible;
    }

}
