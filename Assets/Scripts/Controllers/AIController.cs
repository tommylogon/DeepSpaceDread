
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


        SetIndicator();

        perception.OnTargetFound += TargetFound;
        perception.OnTargetLost += TargetLost;
        perception.OnTargetInAttackRange += AttackTarget;

        currentState = State.Hunting;

    }

    private void SetIndicator()
    {
        enemyIndicator = Instantiate(enemyIndicatorPrefab);
        enemyIndicator.GetComponent<DirectionIndicator>().SetTarget(gameObject);
        enemyIndicator.GetComponent<DirectionIndicator>().SetPlayer(playerRef);
    }

    private void FixedUpdate()
    {
        if (currentState == State.Hunting)
        {
            UpdateHunger();
            Move();

        }
        else if (currentState == State.Hungry)
        {
            EatCorpse();
        }
        else if (currentState == State.Chasing)
        {
            CheckIfCloseToTarget();
           
        }

        else if (currentState == State.Attacking)
        {
            AttackTarget(foundTarget);
        }


        else if (currentState == State.Idle)
        {

        }

        else if (currentState == State.Stunned)
        {
            UpdateStunned();

        }
        else if (currentState == State.Investigating)
        {
            InvestigateArea();
        }
        RotateSprite();
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
        if (currentState == State.Hunting)
        {
            if (navAgent.isStopped)
            {
                navAgent.isStopped = false;
            }
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
                lastKnownLocation = Vector3.zero;
            }
        }
    }

    private void RotateSprite()
    {
        Vector3 velocity = navAgent.velocity;
        if (velocity.magnitude > 0.1f) // Check if the agent is moving to avoid division by zero
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - rotationOffset));

            // Smoothly interpolate between the current rotation and the target rotation
            float rotationSpeed = navAgent.angularSpeed; // Adjust the speed as needed
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

    }

    private void CheckIfCloseToTarget()
    {
        if (navAgent.remainingDistance < 0.2 || navAgent.remainingDistance > 50000)
        {
            currentState = State.Hunting;
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
    private void UpdateHunger()
    {
        hunger += hungerRate * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0, hungerThreshold);
        if (hunger >= hungerThreshold)
        {
            currentState = State.Hungry;
        }
    }
    private void Move()
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

        //if (perception.canSeePlayer && Vector2.Distance(transform.position, foundTarget.transform.position) < 0.5f /*&& playCon.CheckIfPlayerIsAlive()*/)
        //{

        if (!alienSounds.IsPlaying())
        {
            alienSounds.PlayAttackSound();

        }
        foundTarget.GetComponent<IDamage>().TakeDamage(1);
        SetStunnedState();

        //}


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
        else
        {
            hungerThreshold--;
            currentState = State.Hunting;
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
            currentState = State.Eating;
            navAgent.isStopped = true;
            alienSounds.PlayEatingSound();

            yield return new WaitForSeconds(eatingTime);

            hunger = 0f;


            navAgent.isStopped = false;
            alienSounds.StopEatingSound();

            if (corpse != null)
            {
                Instantiate(alienPrefab, Vector2.zero, Quaternion.identity);
                yield return new WaitForSeconds(1f);
                Destroy(corpse.gameObject);
                alienSounds.PlayAlienEmergeSound();
            }
            currentState = State.Hunting;
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
