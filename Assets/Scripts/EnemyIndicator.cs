using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicator : MonoBehaviour
{

    public GameObject player;
    public GameObject target;
    public float distanceFromPlayer = 2f;
    public float minDistanceToShow = 5f;
    public float maxDistanceToShow = 30f;
    public float rotationOffset = 135f;

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
        // Calculate the direction vector from the player to the target
        Vector3 directionToEnemy = (target.transform.position - player.transform.position).normalized;
        float distance = Vector3.Distance(player.transform.position, target.transform.position);

        // Set the position of the indicator to be 'distanceFromPlayer' units away from the player in the direction of the enemy
        transform.position = player.transform.position + directionToEnemy * distanceFromPlayer;

        // Rotate the indicator to face the enemy
        float angle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - rotationOffset); // Subtract 90 degrees if the sprite's default orientation is upwards

        return distance > minDistanceToShow && distance < maxDistanceToShow;

    }

    private void ShowIndicator()
    {
        // If the target is not null, update the indicator position and rotation
        if (target != null)
        {


            // Set the visibility of the indicator based on the distance between the player and the enemy
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
            // Hide the indicator if the enemy is null
            spriteRenderer.enabled = false;
        }
    }

    // Method to update the enemy reference (e.g. when the enemy is changed or becomes audible)
    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
    public void SetPlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
    
}
