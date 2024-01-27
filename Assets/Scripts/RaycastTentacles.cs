using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RaycastTentacles : MonoBehaviour
{
    [SerializeField] private int numberOfRays = 5;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float angleBetweenRays = 15f;
    private LineRenderer[] lineRenderers;
    private Vector3[] targetPositions;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private float smoothFactor = 1f;
    [SerializeField] private Vector3 LastPosition;



    [SerializeField] private Material LineRendererMaterial;
    [SerializeField] private float StartWidth;
    [SerializeField] private float endWidth;

    float updateTimer;

    [SerializeField] NavMeshAgent _agent;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        if(lineRenderers == null)
        {
            lineRenderers = new LineRenderer[numberOfRays];
        }
        
        targetPositions = new Vector3[numberOfRays];
        for (int i = 0; i < numberOfRays; i++)
        {
            GameObject lineRendererObject = new GameObject("TentacleRay" + i);
            lineRendererObject.transform.parent = this.transform;

            LineRenderer lr = lineRendererObject.AddComponent<LineRenderer>();
            lr.positionCount = 2; // Start and end points
            lr.startWidth = StartWidth;
            lr.endWidth = endWidth;
            lr.useWorldSpace = true; 
            lr.material = LineRendererMaterial; // Use a basic sprite material
            lr.startColor = Color.black;
            lr.endColor = Color.black;

            lineRenderers[i] = lr;
        }
       
        CastNewTentacles(true);
    }

    // Update is called once per frame
    void Update()
    {
        SyncTentacleOrigin();

      
        CastNewTentacles(false);
        MoveTentacleTowardsTarget();
    }

    private void SyncTentacleOrigin()
    {
        

        for (int i = 0; i < numberOfRays; i++)
        {
            
                lineRenderers[i].SetPosition(0, transform.position);
                
        }
    }

    private void CastNewTentacles(bool overrideMovementCheck)
    {
        Vector2 forwardDirection = transform.up * -1;
        float startAngle = -angleBetweenRays * (numberOfRays - 1) / 2;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + angleBetweenRays * i;
            Vector2 rayDirection = Quaternion.Euler(0, 0, angle) * forwardDirection;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, rayDistance, TargetMask);

            if (hit.collider != null && (ShouldMoveTentacles(lineRenderers[i].GetPosition(lineRenderers[i].positionCount-1))|| overrideMovementCheck))
            {
                
                targetPositions[i] = hit.point;
                MoveTentacleTowardsTarget();


            }
            else if(hit.collider == null)
            {
                //lineRenderers[i].SetPosition(1, transform.position);
            }
            
        }
    }

    private bool ShouldMoveTentacles(Vector3 tentacleEndPoint)
    {
        Vector2 creatureDirection = _agent.velocity.normalized;
        Vector2 relativePosition = tentacleEndPoint - transform.position;

        if (Vector2.Dot(creatureDirection, relativePosition) < 0)
        {
            return true;
        }
        return false;
    }

    private void MoveTentacleTowardsTarget()
    {
        for(int i=0;  i < lineRenderers.Length; i++) 
        {
            Vector3 currentPosition = lineRenderers[i].GetPosition(1);
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPositions[i], Time.deltaTime * smoothFactor); // smoothFactor is a speed control variable

            if(newPosition != targetPositions[i])
            {
                lineRenderers[i].SetPosition(1, newPosition);
            }
            
        }
        
    }

    public void SpawnTentacles()
    {
        CastNewTentacles(true);
    }
}
