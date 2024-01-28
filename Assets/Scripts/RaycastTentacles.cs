using UnityEngine;
using UnityEngine.AI;

public class RaycastTentacles : MonoBehaviour
{
    [SerializeField] private int numberOfRays = 5;
    [SerializeField] private float maxTentacleDistance = 10f;
    [SerializeField] private float angleBetweenRays = 15f;
    [SerializeField] private float maxAccelerationDistance = 15f;
    

    private LineRenderer[] lineRenderers;
    private Vector3[] targetPositions;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private float smoothFactor = 1f;
    [SerializeField] private Vector3 LastPosition;



    [SerializeField] private Material LineRendererMaterial;
    [SerializeField] private float minWidth;
    [SerializeField] private float maxWidth;

    float updateTimer;

    [SerializeField] NavMeshAgent _agent;
    
    void Start()
    {
        InitalSetup();
    }

    private void InitalSetup()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (lineRenderers == null)
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
            lr.startWidth = Random.Range(minWidth, maxWidth);
            lr.endWidth = Random.Range(minWidth, maxWidth);
            lr.useWorldSpace = true;
            lr.material = LineRendererMaterial;
            lr.startColor = Color.black;
            lr.endColor = Color.black;
            lr.sortingOrder = 1;

            lineRenderers[i] = lr;
        }

        CastNewTentacles(true);
    }

    void Update()
    {
        SyncTentacleOrigin();

      
        CastNewTentacles(false);
        MoveTentacleTowardsTarget();
        UpdateTentacleWidth();
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, maxTentacleDistance, TargetMask);

            if (hit.collider != null && (ShouldMoveTentacles(lineRenderers[i].GetPosition(lineRenderers[i].positionCount-1))|| overrideMovementCheck))
            {
                
                targetPositions[i] = hit.point;
                


            }
            else if(hit.collider == null && Vector2.Distance( lineRenderers[i].GetPosition(1), lineRenderers[i].GetPosition(0)) >= maxTentacleDistance) 
            {
                targetPositions[i] = transform.position;
                //lineRenderers[i].SetPosition(1, transform.position);
            }
            MoveTentacleTowardsTarget();
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
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            Vector3 currentPosition = lineRenderers[i].GetPosition(1);

            float distanceToTarget = Vector3.Distance(currentPosition, targetPositions[i]);
            float acceleration = Mathf.Clamp01(distanceToTarget / maxAccelerationDistance); // Adjust maxAccelerationDistance as needed

            // Use a quadratic acceleration function
            float accelerationFactor = acceleration * acceleration;

            // Move the tentacle towards the target with the calculated acceleration
            Vector3 newPosition = Vector3.Lerp(currentPosition, targetPositions[i], accelerationFactor);


            lineRenderers[i].SetPosition(1, newPosition);
        }

    }

    public void SpawnTentacles()
    {
        CastNewTentacles(true);
    }

    private void UpdateTentacleWidth()
    {
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPositions[i]);

            // Calculate the width based on the distance using linear interpolation
            float normalizedDistance = Mathf.Clamp01(distanceToTarget / maxTentacleDistance); // Adjust maxTentacleLength as needed
            float tentacleWidth = Mathf.Lerp(minWidth, maxWidth, 1 - normalizedDistance);

            // Set the calculated width to both start and end of the LineRenderer
            lineRenderers[i].startWidth = tentacleWidth;
            lineRenderers[i].endWidth = tentacleWidth;
        }
    }

}
