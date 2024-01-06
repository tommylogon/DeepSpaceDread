
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour, IController
{
    [SerializeField] private AIPerception perception;

    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private AlienSounds alienSounds;
    [SerializeField] private float moveRadius = 15f;
    [SerializeField] private float stunDuration = 2f;
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float fleeDistance = 5f;
    private float stunEndTime;

    [SerializeField] private float hunger = 0f;
    [SerializeField] private float hungerRate = 0.1f;
    [SerializeField] private float hungerThreshold = 0.8f;
    private float screamDelay = 0f;
    [SerializeField] private float corpseSearchRadius = 5f;

    [SerializeField] private State currentState = State.Idle;

    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject enemyIndicatorPrefab;
    [SerializeField] private GameObject enemyIndicator;
    [SerializeField] private GameObject alienPrefab;
    private GameObject foundTarget;

    [SerializeField] private bool moveRandomLocation = false;

    [SerializeField] private bool alienHasScreamed;

    [SerializeField] private LayerMask corpseLayer;
    [SerializeField] private float eatingTime = 5f;
    public float rotationOffset = 135f;

    private Vector3 lastKnownLocation;



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
        perception = GetComponent<AIPerception>();


        enemyIndicator = Instantiate(enemyIndicatorPrefab);
        enemyIndicator.GetComponent<EnemyIndicator>().SetEnemy(gameObject);
        enemyIndicator.GetComponent<EnemyIndicator>().SetPlayer(playerRef);

        perception.OnTargetFound += TargetFound;
        perception.OnTargetLost += TargetLost;
        perception.OnTargetInAttackRange += AttackTarget;

        currentState = State.Hunting;

    }

    private void FixedUpdate()
    {
        if (currentState == State.Hunting)
        {
            UpdateHunger();

        }
        if (currentState == State.Hungry)
        {
            EatCorpse();
        }
        if (currentState == State.Chasing)
        {
            UpdateMove();
            RotateSprite();
        }

        if (currentState == State.Attacking)
        {
            AttackTarget(foundTarget);
        }


        if (currentState == State.Idle)
        {

        }

        if (currentState == State.Stunned)
        {
            UpdateStunned();

        }
        if (currentState == State.Investigating)
        {
            InvestigateArea();
        }

    }

  

    private void UpdateStunned()
    {
        if (Time.time >= stunEndTime)
        {
            currentState = State.Hunting;
            navAgent.isStopped = false;
        }

    }

    private void TargetFound(Vector3 targetPos)
    {
        if (currentState != State.Chasing)
        {
            currentState = State.Chasing;
            navAgent.SetDestination(targetPos);
            AlienScream();
        }

    }
    private void TargetLost(Vector3 lastKnownPos)
    {
        if (currentState == State.Chasing)
        {
            currentState = State.Investigating;
            lastKnownLocation = lastKnownPos;

        }
    }

    private void InvestigateArea()
    {
        if (currentState == State.Investigating && lastKnownLocation != Vector3.zero)
        {
            navAgent.SetDestination(lastKnownLocation);
            if (navAgent.remainingDistance < 1)
            {
                GameObject foundLocker = perception.FindNearestLocker(lastKnownLocation);
                if (foundLocker != null)
                {
                    AttackTarget(foundLocker);
                    

                }
                currentState = State.Hunting;
                lastKnownLocation = Vector3.zero;
            }
        }
    }

    private void RotateSprite()
    {
        float angle = Mathf.Atan2(navAgent.velocity.y, navAgent.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - rotationOffset));

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
    private void UpdateHunger()
    {
        hunger += hungerRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, hungerThreshold);
        if (hunger >= hungerThreshold)
        {
            currentState = State.Hungry;
        }
    }
    private void UpdateMove()
    {

        if (navAgent != null && navAgent.isOnNavMesh)
        {
            if (!navAgent.pathPending && navAgent.remainingDistance > 1f)
            {
                SetVolumeByDistance();
            }

            if (moveRandomLocation && !navAgent.pathPending && navAgent.remainingDistance < 0.5f && !perception.canSeePlayer)
            {
                Vector2 randomPoint = (Random.insideUnitCircle * moveRadius) + (Vector2)transform.position;
                navAgent.SetDestination(randomPoint);
            }
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
    private void AttackTarget(GameObject foundTarget)
    {
        
        if (perception.canSeePlayer && Vector2.Distance(transform.position, foundTarget.transform.position) < 0.5f /*&& playCon.CheckIfPlayerIsAlive()*/)
        {

            if (!alienSounds.IsPlaying())
            {
                alienSounds.PlayAttackSound();

            }
             foundTarget.GetComponent<IDamage>().TakeDamage(1);
            SetStunnedState();

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
        if (currentState == State.Hungry)
        {
            navAgent.isStopped = true;
            alienSounds.PlayEatingSound();

            yield return new WaitForSeconds(eatingTime);

            hunger = 0f;

            currentState = State.Hunting;
            navAgent.isStopped = false;
            alienSounds.StopEatingSound();
            currentState = State.Hunting;
            if (corpse != null)
            {
                Instantiate(alienPrefab, Vector2.zero, Quaternion.identity);
                yield return new WaitForSeconds(1f);
                Destroy(corpse.gameObject);
                alienSounds.PlayAlienEmergeSound();
            }
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


    private void SetStunnedState()
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
            else if (targetState == State.Stunned)
            {
                SetStunnedState();
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
