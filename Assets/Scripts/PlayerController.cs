using CodeMonkey.Utils;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool godMode;
    private bool lastInputFromController;
    public bool isCrouching;
    public bool isSprinting;

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float baseVisibility = 1f;
    [SerializeField] private float visibility;
    [SerializeField] private float detectionRadius = 5;

    [SerializeField] private float fov = 180;


    [SerializeField] private State playerState;
    [SerializeField] private Animator animator;
    
    [SerializeField] GameObject FlashLight;
    [SerializeField] GameObject aroundPlayerLight;

    [SerializeField] private LayerMask litLayer;
    [SerializeField] private LayerMask wallLayer;

    

    Rigidbody2D rb;

    [SerializeField] private GameObject throwableObjectPrefab;
    [SerializeField] private float throwForce = 5f;
    private bool flashlightOn;

    public PlayerControls controls;
    public InputAction move;
    public InputAction aim;

    private Quaternion originalRotation;

    private IInteractable interactableObject;

    private void Awake()
    {
        controls = new PlayerControls();
        
        controls.Player.Interact.performed += _ => Interact();
        controls.Player.Interact.performed += _ => WakeUp();
        controls.Player.Restart.performed += _ => RestartScene();
        controls.Player.Run.performed += _ => ToggleSprinting();    
        controls.Player.Throw.performed += _ => ThrowObject();
        controls.Player.ToggleFlashlight.performed += _ => ToggleFlashlight();
        controls.Player.Escape.performed += _ => UIController.Instance.ToggleMenu();

    }

    void Start()
    {
        playerState = State.Sleeping;
        
        rb= GetComponent<Rigidbody2D>();
        FlashLight = GameObject.FindGameObjectWithTag("FOV");

        FlashLight.SetActive(false);
        aroundPlayerLight.SetActive(false);

        originalRotation = transform.rotation;

    }

    private void OnEnable()
    {
        controls.Enable();
        move = controls.Player.Move;
        aim = controls.Player.Aim;
        
    }

    private void OnDisable()
    {
        controls.Disable();
        move.Disable();

    }

    public void PlayerDied()
    {
        playerState = State.Dead;
        animator.SetBool("Dead", true);
        UIController.Instance.ShowGameOver(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        UpdateVisibilityAndLight();
        if (playerState == State.Alive)
        {
            Move(move.ReadValue<Vector2>());
            //HandleInput();
            Aim(aim.ReadValue<Vector2>());
            HandleMovementSounds();
        }
        else if (playerState == State.Dead)
        {
            StopMovingAndPlayingSounds();
        }



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
    


    private void Move(Vector2 direction)
    {
        if(direction.x != 0 || direction.y != 0) 
        {
            Vector3 movement = new Vector3(direction.x, direction.y, 0f);
            movement = Vector3.ClampMagnitude(movement, 1f);

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            rb.AddForce(movement * currentSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed * currentSpeed);
           
        }
        if (transform.rotation.z != 0)
        {
            transform.rotation = originalRotation;
        }



    }

    public void ChangeFOVStatus(bool status)
    {
        FlashLight.SetActive(status);
    }

    private void ToggleFlashlight()
    {
        if(playerState == State.Alive)
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

    public bool CanHearPlayerRunning()
    {
        if(isSprinting && rb.velocity.magnitude > 0.5)
        {
            return true;
        }
        return false;
    }

    private void RestartScene()
    {
        if (playerState == State.Dead)
        {
            SceneManager.LoadScene("SampleScene");
        }
        
    }

    private void ToggleSprinting()
    {
        isSprinting = !isSprinting;
        isCrouching = false;
    }



    private void HandleMovementSounds()
    {
        if (rb.velocity.magnitude > 0.2)
        {
            GetComponent<PlayerSounds>().PlayFootstepSound(isSprinting);
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
        if (throwableObjectPrefab)
        {
            GameObject throwableObject = Instantiate(throwableObjectPrefab, transform.position, Quaternion.identity);
            Rigidbody2D throwableRb = throwableObject.GetComponent<Rigidbody2D>();
            Vector2 throwDirection = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
            throwableRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
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
    }
}
