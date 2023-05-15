
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IController
{
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private AlienSounds alienSounds;
    [SerializeField] private float moveRadius = 15f;
    [SerializeField] private float viewRadius = 10f;
    [SerializeField] private float viewAngle = 360f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float fleeDistance = 5f;
    private float stunEndTime;
    public float PlayerVisible { get; private set; }
    [SerializeField] private float hunger = 0f;
    [SerializeField] private float hungerRate = 0.1f;
    [SerializeField] private float hungerThreshold = 0.8f;
    private float screamDelay = 0f;
    [SerializeField] private float corpseSearchRadius = 5f;
    [SerializeField] private float flipInterval = 2f;
    [SerializeField] private State currentState = State.Idle;

    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject enemyIndicatorPrefab;
    [SerializeField] private GameObject enemyIndicator;
    [SerializeField] private GameObject alienPrefab;

    [SerializeField] private bool canSeePlayer = false;
    [SerializeField] private bool moveRandomLocation = false;
    [SerializeField] private bool lookForTargets = true;
    [SerializeField] private bool alienHasScreamed;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask corpseLayer;
    [SerializeField] private float eatingTime = 5f;
    public float rotationOffset = 135f;
    private bool isEating = false;
    [SerializeField] private float minDistanceFromNoise;
    [SerializeField] private float maxDistanceFromNoise;


    void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {

        

        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;
        navAgent.speed = walkingSpeed;

        alienSounds = GetComponent<AlienSounds>();

        StartCoroutine(FOVCheck());

        enemyIndicator = Instantiate(enemyIndicatorPrefab);
        enemyIndicator.GetComponent<EnemyIndicator>().SetEnemy(gameObject);
        enemyIndicator.GetComponent<EnemyIndicator>().SetPlayer(playerRef);
        playerRef.GetComponent<PlayerController>().OnNoiseGenerated += HearNoise; ;

        StartCoroutine(FlipSprite());

    }

    private void FixedUpdate()
    {
        if (isEating)
        {
            return;
        }

        hunger += hungerRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, hungerThreshold);


        UpdateSight();
        UpdateAttack();
        if (hunger >= hungerThreshold)
        {
            EatCorpse();
           
        }
        UpdateMove();
        RotateSprite();



    }

    private void OnDestroy()
    {
        if (playerRef != null)
        {
            playerRef.GetComponent<PlayerController>().OnNoiseGenerated -= HearNoise;
        }

    }


    private void RotateSprite()
    {
        float angle = Mathf.Atan2(navAgent.velocity.y, navAgent.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - rotationOffset));

    }

    private IEnumerator FlipSprite()
    {
        while (true)
        {
            yield return new WaitForSeconds(flipInterval);
            Vector3 currentScale = transform.localScale;
            currentScale.x *= -1; // Flip the sprite along the Y-axis
            transform.localScale = currentScale;
        }
    }

    private void AlienScream()
    {
        screamDelay += Time.deltaTime;
        if (screamDelay > 3f)
        {
            alienHasScreamed = false;
           
        }
        if (!alienHasScreamed)
        {
            alienHasScreamed = true;
            screamDelay = 0;
            alienSounds.PlayPlayerSpottedSound();
        }
    }
    private void UpdateSight()
    {
        if (canSeePlayer && PlayerVisible >= 1 && playerRef.GetComponent<PlayerController>().CheckIfPlayerIsAlive())
        {
            canSeePlayer = true;
            PlayerVisible += playerRef.GetComponent<PlayerController>().GetVisibility();

            AlienScream();


            navAgent.SetDestination(playerRef.transform.position);



        }
        else
        {


            canSeePlayer = false;
            PlayerVisible -= Time.deltaTime / 2;
            PlayerVisible = Mathf.Clamp01(PlayerVisible);

        }
    }


    public void HearNoise(Vector2 noiseOrigin, float noiseRadius)
    {
        if(canSeePlayer && PlayerVisible >= 1) { return; }

        float distanceToPlayer = Vector2.Distance(noiseOrigin, transform.position);
        if (distanceToPlayer <= noiseRadius)
        {
            float distanceFromNoise = Random.Range(minDistanceFromNoise, maxDistanceFromNoise);
            float angle = Random.Range(0, 360);
            Vector2 randomOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distanceFromNoise;
            Vector2 targetPosition = noiseOrigin + randomOffset;
            navAgent.SetDestination(targetPosition);
            AlienScream();
        }
    }

    private void UpdateMove()
    {
        if (IsStunned())
        {
            if (Time.time >= stunEndTime)
            {
                currentState = State.Idle;
                navAgent.isStopped = false;
            }
            return;
        }

        if (!navAgent.pathPending && navAgent.remainingDistance > 1f)
        {
            SetVolumeByDistance();
        }

        if (moveRandomLocation && !navAgent.pathPending && navAgent.remainingDistance < 0.5f && !canSeePlayer)
        {
            Vector2 randomPoint = (Random.insideUnitCircle * moveRadius) + (Vector2)transform.position;
            navAgent.SetDestination(randomPoint);
        }

        if (navAgent.velocity.magnitude > 0.1)
        {
            alienSounds.StartWalkingSound();
        }
        else
        {
            alienSounds.StopWalkingSound();
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

            if (Vector2.Angle(transform.up, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector2.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleLayer))
                {
                    PlayerVisible += target.GetComponent<PlayerController>().GetVisibility();
                    if (PlayerVisible >= 1f)
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

    private void UpdateAttack()
    {
        if (canSeePlayer && Vector2.Distance(transform.position, playerRef.transform.position) < 0.5f && playerRef.GetComponent<PlayerController>().CheckIfPlayerIsAlive())
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
        GameObject corpse = FindClosestCorpse();

        if (corpse != null)
        {
            navAgent.SetDestination(corpse.transform.position);

            if (Vector2.Distance(transform.position, corpse.transform.position) <= 1f)
            {
                InteractObject interactableCorpse = corpse.GetComponent<InteractObject>();
                if (interactableCorpse != null)
                {

                    StartCoroutine(StartEating(interactableCorpse));
                    hungerThreshold *= 2;
                }
            }
        }
    }
    private GameObject FindClosestCorpse()
    {
        Collider2D[] corpsesInRange = Physics2D.OverlapCircleAll(transform.position, corpseSearchRadius, corpseLayer);
        GameObject closestCorpse = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D corpseCollider in corpsesInRange)
        {
            float distance = Vector2.Distance(transform.position, corpseCollider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCorpse = corpseCollider.gameObject;
            }
        }

        return closestCorpse;
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
        if (corpse != null)
        {
            Instantiate(alienPrefab, corpse.gameObject.transform.position, Quaternion.identity);
            Destroy(corpse.gameObject);
            alienSounds.PlayAlienEmergeSound();
        }

    }

    private void SetVolumeByDistance()
    {
        float distance = Vector3.Distance(playerRef.transform.position, transform.position);
        float maxDistance = 10;
        if (distance > maxDistance)
        {

            alienSounds.ChangeWalkingVolume(0f);
            alienSounds.ChangeActionVolume(0f);
        }
        else
        {

            alienSounds.ChangeWalkingVolume(1f - (distance / maxDistance));
            alienSounds.ChangeActionVolume(1f - (distance / maxDistance));
        }
    }

    private bool IsStunned()
    {
        return currentState == State.Stunned;
    }

    private void StunEnemy()
    {
        currentState = State.Stunned;
        stunEndTime = Time.time + stunDuration;
        navAgent.isStopped = true;
    }

    public void HandleThrowableObjectHit(Collider2D collider, State targetState)
    {
        if (collider.gameObject.CompareTag("Enemy") && collider.gameObject == gameObject)
        {
            if (targetState == State.Fleeing)
            {
                Flee();
            }
            else if(targetState == State.Stunned)
            {
                StunEnemy();
            }
        }
    }
    private void Flee()
    {
        currentState = State.Fleeing;

        Vector2 fleeDirection = (transform.position - playerRef.transform.position).normalized;
        Vector2 fleeTarget = (Vector2)transform.position + fleeDirection * fleeDistance;

        navAgent.SetDestination(fleeTarget);
    }
}
