using CodeMonkey.Utils;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool godMode;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float crouchSpeed = .2f;
    [SerializeField] private float sprintSpeed = 3f;
    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float baseVisibility = 1f;

    [SerializeField] private State playerState;
    public bool isCrouching;
    public bool isSprinting;
    [SerializeField] private float fov =180;
    //[SerializeField] FieldOfView fov;
    [SerializeField] GameObject FOVLight;
    [SerializeField] GameObject aroundPlayerLight;

    [SerializeField] private LayerMask litLayer;
    public event Action OnDeath;

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
        
       // controls.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
       // controls.Player.Aim.performed += ctx => Aim(ctx.ReadValue<Vector2>());
        controls.Player.Interact.performed += _ => Interact();
        controls.Player.Interact.performed += _ => WakeUp();
        controls.Player.Restart.performed += _ => RestartScene();
        controls.Player.Run.performed += _ => ToggleSprinting();
        //controls.Player.Crouch.performed += _ => ToggleCrouching();
        controls.Player.Throw.performed += _ => ThrowObject();
        controls.Player.ToggleFlashlight.performed += _ => ToggleFlashlight();

    }

    // Start is called before the first frame update
    void Start()
    {
        playerState = State.Sleeping;
        
        rb= GetComponent<Rigidbody2D>();
        FOVLight = GameObject.FindGameObjectWithTag("FOV");

        FOVLight.SetActive(false);
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
        UIController.Instance.ShowGameOver(false);

    }

    // Update is called once per frame
    void Update()
    {
        
        
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
            playerState = State.Alive;
            UIController.Instance.HideGameOver();
            FOVLight.SetActive(true);
            aroundPlayerLight.SetActive(true);
        }

       
    }
    private void Aim(Vector2 input)
    {
        Vector3 aimDir;

        if (input != Vector2.zero) // Check if the input is from the controller joystick
        {
            aimDir = new Vector3(input.x, input.y).normalized;
        }
        else // If not, use the mouse position as the aim direction
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.localPosition.z));
            aimDir = (UtilsClass.GetMouseWorldPosition() - transform.position).normalized;
            
        }

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        FOVLight.gameObject.transform.rotation = Quaternion.AngleAxis(UtilsClass.GetAngleFromVector(aimDir) - fov / 2, Vector3.forward);
    }


    private void Move(Vector2 direction)
    {
        
        Vector3 movement = new Vector3(direction.x, direction.y, 0f);
        movement = Vector3.ClampMagnitude(movement, 1f);

        float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : moveSpeed);
        rb.AddForce(movement * currentSpeed);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed * currentSpeed);
        if(transform.rotation.z != 0)
        {
            transform.rotation = originalRotation;
        }

    }
    private void HandleInput()
    {

        
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetAxis("LeftTrigger") > 0.1f)
        {
            ToggleSprinting();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrouching();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("A_Button"))
        {
            Interact();
        }
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("X_Button")) // Left mouse click or X button
        {
            ThrowObject();
        }

        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Y_Button")) // F key or Y button
        {
            ToggleFlashlight();
        }



    }

    public void ChangeFOVStatus(bool status)
    {
        FOVLight.SetActive(status);
    }

    private void ToggleFlashlight()
    {
        flashlightOn = !flashlightOn;
        FOVLight.SetActive(flashlightOn);
    }

    public float GetVisibility()
    {
        float visibility = baseVisibility;

        // Check if the player is in a lit area
        bool isLit = Physics2D.OverlapCircle(transform.position, 5, litLayer);
        if (isLit)
        {
            visibility = 1f;
        }
        else
        {
            visibility = flashlightOn ? 0.5f : 0.2f;
        }

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

    private void ToggleCrouching()
    {
        isCrouching = !isCrouching;
        isSprinting = false;
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
        if (interactableObject != null)
        {
            interactableObject.Interact();
        }
    }
}
