using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.IK;

public class TentacleMover : MonoBehaviour
{

    public Transform tentacleTarget;

    public float moveDistance;
    public float raycastDistance = 0.2f;
    public LayerMask wallLayer;
    bool isMoving;
    public bool canAttachToWall;
    public float moveSpeed;
    public float distance;
    Vector3 newTargetPos;

    

    // Update is called once per frame
    void Update()
    {
        WallChecker();
        distance = Vector2.Distance(tentacleTarget.position, transform.position);
       
        
        if (distance > moveDistance  && !isMoving )
        {
            isMoving = true;   
            newTargetPos = transform.position;
        }
        if(isMoving)
        {
            float t = Time.deltaTime * moveSpeed;
            tentacleTarget.position = Vector3.Lerp(tentacleTarget.position, newTargetPos, t);
            //tentacleTarget.position = transform.position;
        }
        if(Vector2.Distance(tentacleTarget.position, newTargetPos) < 0.1 && isMoving)
        {
            isMoving = false;
        }
        
    }

    private void WallChecker()
    {
        // Define the direction of the raycast and the distance
         // You can change this to Vector3.right, Vector3.left, or Vector3.down as needed
        


        RaycastHit2D hitUp = CastRay(Vector2.up);
        RaycastHit2D hitDown = CastRay(Vector2.down);
        RaycastHit2D hitLeft = CastRay(Vector2.left);
        RaycastHit2D hitRight = CastRay(Vector2.right);

        // Store all hits in a list
        List<RaycastHit2D> hits = new List<RaycastHit2D> { hitUp, hitDown, hitLeft, hitRight };

        // Filter out the non-hits and find the closest hit
        RaycastHit2D closestHit = hits.Where(hit => hit.collider != null)
                                      .OrderBy(hit => hit.distance)
                                      .FirstOrDefault();

        if (closestHit.collider != null)
        {
            Vector3 point = closestHit.point;

            // Determine the direction of the hit relative to the tentacle's position
            Vector3 hitDirection = point - transform.position;

            // Adjust the position slightly away from the wall based on the hit direction
            if (Mathf.Abs(hitDirection.x) > Mathf.Abs(hitDirection.y))
            {
                // The hit is more horizontal (left or right)
                point.x += hitDirection.x > 0 ? -0.1f : 0.1f; // Adjust X position
            }
            else
            {
                // The hit is more vertical (up or down)
                point.y += hitDirection.y > 0 ? -0.1f : 0.1f; // Adjust Y position
            }

            // Update the GameObject's position
            if (canAttachToWall)
            {
                transform.position = point;
              
            }
            
            return;
        }
        //if(EffectorOrigin != null && attachedToWall)
        //{
        //    transform.position = EffectorOrigin.transform.position;
        //    attachedToWall = false;
        //}
            
    }
    private RaycastHit2D CastRay(Vector2 direction)
    {
        return Physics2D.Raycast(transform.position, direction, raycastDistance, wallLayer);
    }

}
