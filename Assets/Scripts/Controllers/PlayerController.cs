using CodeMonkey.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    #region Fields and Properties
    

    [SerializeField] private bool godMode;
    private bool lastInputFromController;
   
    private bool flashlightOn;

    public bool IsInsideLocker;
    public ParticleSystem deathEffect;


    [SerializeField] private float baseVisibility = 1f;
    [SerializeField] private float visibility;
    [SerializeField] private float detectionRadius = 5;
    [SerializeField] private float throwForce = 5f;

    [SerializeField] private float fov = 180;

    [SerializeField] private State playerState;
    [SerializeField] private Animator animator;

    [SerializeField] GameObject FlashLight;
    [SerializeField] GameObject aroundPlayerLight;
    [SerializeField] private Transform holdingPoint;

    private Transform mainCameraTransform;

    [SerializeField] private LayerMask litLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask alienLayer;

    public delegate void NoiseGeneratedEventHandler(Vector2 noiseOrigin, float noiseRadius);
    public event NoiseGeneratedEventHandler OnNoiseGenerated;

    Rigidbody2D rb;


    [SerializeField] private GameObject throwableObjectInventory;

    public PlayerControls controls;
    private TDPlayerMovement playerMovement;
    private Resource playerHealth;
    
    public InputAction move;
    public InputAction aim;


    private IInteractable interactableObject;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        controls = new PlayerControls();
        playerMovement = GetComponent<TDPlayerMovement>();
        playerHealth = GetComponent<Resource>();
    }

    void Start()
    {
        playerState = State.Sleeping;
        rb = GetComponent<Rigidbody2D>();
        FlashLight = GameObject.FindGameObjectWithTag("FOV");

        FlashLight.SetActive(false);
        aroundPlayerLight.SetActive(false);

        
    }

    private void OnEnable()
    {
        controls.Enable();
        move = controls.Player.Move;
        aim = controls.Player.Aim;

        controls.Player.Interact.performed += _ => Interact();
        controls.Player.Interact.performed += _ => WakeUp();
        controls.Player.Restart.performed += _ => RestartScene();
        controls.Player.Run.started += _ => Sprint(true);
        controls.Player.Run.canceled += _ => Sprint(false);
        controls.Player.Throw.performed += _ => ThrowObject();
        controls.Player.ToggleFlashlight.performed += _ => ToggleFlashlight();
        controls.Player.Escape.performed += _ => EscapeKeyPressed();

        playerHealth.OnEmpty += PlayerDied;
    }

    private void OnDisable()
    {
        controls.Disable();
        move.Disable();

        controls.Player.Interact.performed -= _ => Interact();
        controls.Player.Interact.performed -= _ => WakeUp();
        controls.Player.Restart.performed -= _ => RestartScene();
        controls.Player.Run.started -= _ => playerMovement.SetSprinting(true);
        controls.Player.Run.canceled -= _ => playerMovement.SetSprinting(false);
        controls.Player.Throw.performed -= _ => ThrowObject();
        controls.Player.ToggleFlashlight.performed -= _ => ToggleFlashlight();
        controls.Player.Escape.performed -= _ => EscapeKeyPressed();

        playerHealth.OnEmpty -= PlayerDied;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateVisibilityAndLight();

        if (playerState == State.Alive)
        {
            playerMovement.Move(move.ReadValue<Vector2>());
            Aim(aim.ReadValue<Vector2>());
            HandleMovementSounds();
        }
        else if (playerState == State.Dead)
        {
            StopMovingAndPlayingSounds();
        }
        if (throwableObjectInventory != null)
        {
            throwableObjectInventory.transform.position = holdingPoint.position;
            throwableObjectInventory.transform.rotation = holdingPoint.rotation;
        }
    }

    #endregion
    #region Custom Methods

    private void EscapeKeyPressed()
    {
        if (!UIController.Instance.IsReactorShowing())
        {
            UIController.Instance.ToggleMenu();
        }
        else
        {
            UIController.Instance.HideReactorPanel();
        }
        
        
    }

    public void TakeDamage(int damage)
    {
        playerHealth.ReduceRecource(damage);
    }

    public void PlayerDied()
    {
        if (godMode)
        {
            return; // Don't do anything if godMode is enabled
        }
        playerState = State.Dead;
        animator.SetBool("Dead", true);
        
        if (deathEffect != null)
        {
            deathEffect.Play();
        }

        UIController.Instance.ShowGameOver(false);

        

    }


    private void WakeUp()
    {
        if (playerState == State.Sleeping)
        {
            animator.SetBool("Sleeping", false);
            playerState = State.Alive;
            UIController.Instance.HideGameOver();
            ToggleFlashlight();
            aroundPlayerLight.SetActive(true);
        }


    }
    private void Aim(Vector2 controllerInput)
    {
        Vector3 aimDir;

        if (controllerInput != Vector2.zero)
        {
            lastInputFromController = true;
            aimDir = new Vector3(controllerInput.x, controllerInput.y).normalized;
        }
        else
        {
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                lastInputFromController = false;
            }

            if (!lastInputFromController)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.localPosition.z));
                aimDir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;

            }
            else
            {
                return;
            }
        }

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        FlashLight.gameObject.transform.rotation = Quaternion.AngleAxis(UtilsClass.GetAngleFromVector(aimDir) - fov / 2, Vector3.forward);
    }


    public void Sprint(bool sprinting)
    {
        playerMovement.SetSprinting(sprinting);
        
            animator.speed = sprinting ? 1.5f : 1;
    }


    public void ChangeFlashlighStatus(bool status)
    {
        flashlightOn = status;
        FlashLight.SetActive(status);
    }

    private void ToggleFlashlight()
    {
        if (playerState == State.Alive)
        {
            flashlightOn = !flashlightOn;
            FlashLight.SetActive(flashlightOn);
        }

    }
    private void UpdateVisibilityAndLight()
    {
        visibility = baseVisibility;

        // Check if the player is in a lit area
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, litLayer);

        foreach (Collider2D collider in colliders)
        {
            Light2D light2D = collider.GetComponent<Light2D>();
            if (light2D != null)
            {
                Vector2 directionToLight = (collider.transform.position - transform.position).normalized;
                float distanceToLight = Vector2.Distance(transform.position, collider.transform.position);

                if (collider.CompareTag("IgnoreRaycast"))
                {
                    // Ignore raycast and enable/disable light based on distance
                    if (distanceToLight <= detectionRadius && !light2D.enabled)
                    {
                        light2D.enabled = true;
                    }
                    else if (distanceToLight > detectionRadius && light2D.enabled)
                    {
                        light2D.enabled = false;
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToLight, distanceToLight, wallLayer);

                    if (hit.collider == null)
                    {
                        // No wall is blocking the light
                        if (distanceToLight <= detectionRadius && !light2D.enabled)
                        {
                            light2D.enabled = true;
                        }
                    }
                    else
                    {
                        // A wall is blocking the light
                        if (light2D.enabled)
                        {
                            light2D.enabled = false;
                        }
                    }
                }
            }
        }

        if (colliders.Length > 0)
        {
            visibility = 1f;
        }
        else
        {
            visibility = flashlightOn ? 0.5f : 0.2f;
        }
    }

    public float GetVisibility()
    {


        return Mathf.Clamp01(visibility);
    }

    private bool CanHearPlayerRunning()
    {
        if (playerMovement.isSprinting && rb.velocity.magnitude > 0.5)
        {
            return true;
        }
        return false;
    }

    private void RestartScene()
    {
        if (playerState == State.Dead)
        {
            SceneManager.LoadScene("GameScene");
        }

    }

    



    private void HandleMovementSounds()
    {
        if (rb.velocity.magnitude > 0.2)
        {
            GetComponent<PlayerSounds>().PlayFootstepSound(playerMovement.isSprinting);
        }
        else if (rb.velocity.magnitude < 0.1)
        {
            GetComponent<PlayerSounds>().StopSound();
        }
    }

    private void StopMovingAndPlayingSounds()
    {
        rb.velocity = Vector2.zero;
        GetComponent<PlayerSounds>().StopSound();
    }

    private void ThrowObject()
    {
        if (throwableObjectInventory && !IsInsideLocker)
        {

            // Unparent the object
            throwableObjectInventory.transform.SetParent(null);

            throwableObjectInventory.transform.position = transform.position;
            Vector2 throwDirection = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
            throwableObjectInventory.GetComponent<ThrowableObject>().Throw(throwDirection, throwForce, transform.position);

            throwableObjectInventory = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactableObject = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable) && interactable == interactableObject)
        {
            interactableObject = null;
        }
    }

    private void Interact()
    {
        if (interactableObject != null && playerState == State.Alive)
        {
            interactableObject.Interact();
        }
        if(IsInsideLocker && interactableObject == null)
        {
            Collider2D lockerCollider = Physics2D.OverlapCircle(transform.position, 2);
            OnTriggerEnter2D (lockerCollider);
        }
    }

    public void GenerateNoise(Vector2 noiseOrigin, float noiseRadius, float hearingChance)
    {
        float randomChance = Random.Range(0f, 1f);
        if (randomChance <= hearingChance)
        {
            OnNoiseGenerated?.Invoke(noiseOrigin, noiseRadius);
        }
    }

    public bool CheckIfPlayerIsAlive()
    {
        if (playerState == State.Alive)
        {
            return true;
        }
        else
        {
            return false;
        }



    }

    public void AddToInventory(GameObject pickupItem)
    {
        if(pickupItem != null)
        {
            if(throwableObjectInventory != null)
            {
                throwableObjectInventory.SetActive(true);
                
                
            }
            throwableObjectInventory = pickupItem;

        }
    }
    public Transform GetHoldingPoint()
    {
        return holdingPoint;
    }
    #endregion
}
