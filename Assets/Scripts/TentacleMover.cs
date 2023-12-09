using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleMover : MonoBehaviour
{
    public Transform tentacleTarget;
    public float moveDistance;
    float raycastDistance = 5f;
    public Vector3 direction = Vector3.up;
    public LayerMask wallLayer;
    

    // Update is called once per frame
    void Update()
    {
        WallChecker();
        if(Vector2.Distance(tentacleTarget.position, transform.position) > moveDistance)
        {
            tentacleTarget.position = transform.position;
        }
        
    }

    private void WallChecker()
    {
        // Define the direction of the raycast and the distance
         // You can change this to Vector3.right, Vector3.left, or Vector3.down as needed
        

        // Perform the raycast
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, wallLayer);

        if (hit.collider != null)
        {
            Vector3 point = hit.point;

            // Adjust the position slightly away from the wall
            // Check if the raycast is vertical (up or down)
            if (direction == Vector3.up || direction == Vector3.down)
            {
                // Adjust Y position
                point.y += (direction == Vector3.up) ? -0.1f : 0.1f;
            }
            else
            {
                // For horizontal raycast (left or right), adjust X position
                point.x += (direction == Vector3.right) ? 0.1f : -0.1f;
            }

            // Update the GameObject's position
            transform.position = point;
        }
    }

}
