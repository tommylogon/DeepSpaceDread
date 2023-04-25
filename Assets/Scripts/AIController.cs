
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IController
{
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private AlienSounds alienSounds;
    [SerializeField] private float moveRadius = 15f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 360f;
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float PlayerVisible = 0f;
    [SerializeField] private float hunger = 0f;
    [SerializeField] private float hungerRate = 0.1f;
    [SerializeField] private float hungerThreshold = 0.8f;

    [SerializeField] private GameObject playerRef;
    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private bool moveRandomLocation = false;
    [SerializeField] private bool lookForTargets = true;
    [SerializeField] private bool alienHasScreamed;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask corpseLayer;
    [SerializeField] private float eatingTime = 5f;
    private bool isEating = false;

    // Start is called before the first frame update
    void Start()
    {

        playerRef = GameObject.FindGameObjectWithTag("Player");

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = walkingSpeed;

        alienSounds = GetComponent<AlienSounds>();

        StartCoroutine(FOVCheck());

    }

    private void Update()
    {
        if(isEating) { return; 
        }

        hunger += hungerRate * Time.deltaTime;
        hunger = Mathf.Clamp01(hunger);


        See();
        Attack();
        if (hunger >= hungerThreshold)
        {
            EatCorpse();
        }
        Move();
        RotateSprite();
        


    }

    private void RotateSprite()
    {
        float angle = Mathf.Atan2(navAgent.velocity.y, navAgent.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    private void See()
    {
        if (canSeePlayer && PlayerVisible >= 1 || playerRef.GetComponent<PlayerController>().CanHearPlayerRunning() )
        {
            canSeePlayer = true;
            PlayerVisible += playerRef.GetComponent<PlayerController>().GetVisibility();

            if (!alienHasScreamed)
            {
                alienHasScreamed = true;
                alienSounds.PlayPlayerSpottedSound();
            }
             
                navAgent.SetDestination(playerRef.transform.position);
            
            
            
        }
        else
        {
            alienHasScreamed = false;
            canSeePlayer = false;
            PlayerVisible -= Time.deltaTime / 2;
            PlayerVisible = Mathf.Clamp01(PlayerVisible);
            alienSounds.StopAllSound();
        }
    }
    private void Move()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance > 1f)
        {

            float distance = Vector3.Distance(playerRef.transform.position, transform.position);
            float maxDistance = 10;

            if (distance > maxDistance)
            {

                alienSounds.ChangeWalkingSound(0f);
            }
            else
            {

                alienSounds.ChangeWalkingSound(1f - (distance / maxDistance));
            }
            

        }

        if (moveRandomLocation && !navAgent.pathPending && navAgent.remainingDistance < 0.5f && !canSeePlayer)
        {
            if(!alienSounds.IsPlaying()) alienSounds.PlayStopSound();

            Vector2 randomPoint = (UnityEngine.Random.insideUnitCircle * moveRadius) + (Vector2)transform.position;

            // Set the destination of the NavMeshAgent to the random point
            navAgent.SetDestination(randomPoint);
        }
    }
    private IEnumerator FOVCheck()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (lookForTargets)
        {
            yield return wait;
            FOV();
        }

        
    }

    private void FOV()
    {
        Collider2D[] rangeCheck = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetLayer);

        if (rangeCheck.Length > 0)
        {
            Transform target = rangeCheck[0].transform;
            Vector2 directionToTarget = (target.position - transform.position).normalized;

            if(Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if(!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
                {
                    PlayerVisible += target.GetComponent<PlayerController>().GetVisibility(); 
                    if(PlayerVisible >= 1f)
                    {
                        canSeePlayer = true;
                        return;
                    }
                    
                }
            }
        }
        canSeePlayer = false;
        return;
    }

    private void Attack()
    {
        if (canSeePlayer && Vector2.Distance(transform.position, playerRef.transform.position) < 0.5f)
        {

            if (!alienSounds.IsPlaying())
            {
                alienSounds.PlayAttackSound();
                
            }
            playerRef.GetComponent<PlayerController>().PlayerDied();
        }
    }
    private void EatCorpse()
    {
        Collider2D[] corpsesInRange = Physics2D.OverlapCircleAll(transform.position, 1f, corpseLayer);

        if (corpsesInRange.Length > 0)
        {
            InteractObject corpse = corpsesInRange[0].GetComponent<InteractObject>();
            if (corpse != null)
            {
                StartCoroutine(StartEating(corpse));
            }
        }
    }

    private IEnumerator StartEating(InteractObject corpse)
    {
        isEating = true;
        navAgent.isStopped = true;
        alienSounds.PlayEatingSound();

        yield return new WaitForSeconds(eatingTime);

        hunger = 0f;

        isEating = false;
        navAgent.isStopped = false;
        alienSounds.StopEatingSound();
    }
}
