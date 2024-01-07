using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIPerception : MonoBehaviour
{

    /*
     this scipt should handle perception. aka  visual or audiotory detection of target objects and announce if it has found a suitable target.

     */
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 360f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask hiddenLayer;

    [SerializeField] private float targetVisible;
    [SerializeField] public bool canSeePlayer { get; private set; }
    [SerializeField] private bool lookForTargets = true;
    private bool inView;

    [SerializeField] private float minDistanceFromNoise =5;
    [SerializeField] private float maxDistanceFromNoise=25;

    private Vector3 lastKnownTargetPosition;


    private Transform target;

    public Action<GameObject> OnTargetInAttackRange;
    public Action<Vector3> OnTargetFound;
    public Action<Vector3> OnTargetLost;

    // Start is called before the first frame update
    void Start()
    {
        GameHandler.instance.OnNoiseGenerated += HearNoise;
        StartCoroutine(RunCheckTargetInSight());

    }

    private void OnDestroy()
    {
        GameHandler.instance.OnNoiseGenerated -= HearNoise;

    }

    private void Update()
    {
        UpdateSight();

        
    }

    

    public void CheckIfAnyTargetInFOV()
    {
        Collider2D[] targetsInRange = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetLayer);
        if (targetsInRange.Length > 0)
        {
            target = targetsInRange[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if (Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
                {
                    targetVisible += target.GetComponent<PlayerController>().GetVisibility(); //this should go away
                    inView = true;
                    if (targetVisible >= 1f)
                    {
                        canSeePlayer = true;
                        return;
                    }
                }
                else
                {
                    inView = false;
                }
               
            }
        }
        if (target != null && target.gameObject.layer == LayerMask.NameToLayer("BehindMask") && lastKnownTargetPosition != null)
        {
                OnTargetLost?.Invoke(lastKnownTargetPosition);
                target=null;
                inView = false;
                canSeePlayer = false;
                
        }
        if (canSeePlayer && Vector2.Distance(transform.position, target.transform.position) < 0.5f /*&& playCon.CheckIfPlayerIsAlive()*/)
        {
            OnTargetInAttackRange?.Invoke(target.gameObject);
        }
    }

    public void HearNoise(Vector2 noiseOrigin, float noiseRadius)
    {
        if (canSeePlayer && targetVisible >= 1) { return; }

        float distanceToNoise = Vector2.Distance(noiseOrigin, transform.position);

        if (distanceToNoise <= noiseRadius)
        {
            float distanceFromNoise = Random.Range(minDistanceFromNoise, maxDistanceFromNoise);
            float angle = Random.Range(0, 360);
            Vector2 randomOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distanceFromNoise;
            Vector2 targetPosition = noiseOrigin + randomOffset;
            OnTargetFound?.Invoke(targetPosition);
            
        }
    }
    private void UpdateSight()
    {
        if(target != null && canSeePlayer && targetVisible >= 1 && target.GetComponent<PlayerController>().CheckIfPlayerIsAlive())
        {
            targetVisible += target.GetComponent<PlayerController>().GetVisibility();
            OnTargetFound?.Invoke(target.position);

        }
        else if(!inView)
        {
            canSeePlayer = false;
            targetVisible -= Time.deltaTime / 2;
           
            

        }
        targetVisible = Mathf.Clamp01(targetVisible);
        if(targetVisible <= 0 && target != null)
        {
            canSeePlayer = false;
            OnTargetLost?.Invoke(target.position);
            target = null;
        }

        if (canSeePlayer)
        {
            lastKnownTargetPosition = target.position;
        }
    }

    private IEnumerator RunCheckTargetInSight()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (lookForTargets)
        {
            yield return wait;
            CheckIfAnyTargetInFOV();
        }


    }

    public GameObject FindNearestLocker(Vector3 searchPosition)
    {
        Locker nearestLocker = null;
        float minDistance = Mathf.Infinity;

        foreach (var locker in FindObjectsOfType<Locker>()) // Assuming Locker is the component on hiding spots
        {
            float distance = Vector3.Distance(searchPosition, locker.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestLocker = locker;
            }
        }

        return nearestLocker.gameObject;
    }
}
