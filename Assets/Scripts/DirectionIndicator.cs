using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{

    private GameObject player;
    private GameObject target;
    private float distanceFromPlayer = 2f;
    private float minDistanceToShow = 5f;
    private float maxDistanceToShow = 30f;
    private float rotationOffset = 135f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ShowIndicator();
    }

    private bool IsWithinActivationDistance()
    {
        if (target == null) return false;
        if(player == null) return false;
        // Calculate the direction vector from the player to the target
        Vector3 directionToTarget = (target.transform.position - player.transform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, target.transform.position);

        MoveIndicator(directionToTarget);

        return distance > minDistanceToShow && distance < maxDistanceToShow;

    }

    private void MoveIndicator(Vector3 directionToTarget)
    {
        // Set the position of the indicator to be 'distanceFromPlayer' units away from the player in the direction of the enemy
        transform.position = player.transform.position + directionToTarget * distanceFromPlayer;

        // Rotate the indicator to face the enemy
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - rotationOffset); // Subtract 90 degrees if the sprite's default orientation is upwards
    }

    private void ShowIndicator()
    {
        if (target != null)
        {

            if (IsWithinActivationDistance())
            {
                spriteRenderer.enabled = true;
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }

    public void SetDistances(float minDistance, float maxDistance)
    {
        minDistanceToShow = minDistance;
        maxDistanceToShow = maxDistance;
    }
    
}
