using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Controller modules/top down player movement", 1)]
public class TDPlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float maxSpeed = 1f;
    private Rigidbody2D rb;
    private Transform mainCameraTransform;
    private Quaternion originalRotation;
    private bool spriteCanRotate;
    public bool isCrouching;
    public bool isSprinting;
    public movementState currentMovementState { get; private set; }
    
    
    public enum movementState{ 
        Idle,
        Walking,
        Sprinting
    }

    // Start is called before the first frame update
    void Awake()
    {
        
        mainCameraTransform = Camera.main.transform;
        originalRotation = transform.rotation;
        
    }

    private void Update()
    {
        
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalRotation = transform.rotation;
        UIController.Instance.OnShutDownReactorButton_Clicked += EnableZeroG;
    }
    private void OnDestroy()
    {
        UIController.Instance.OnShutDownReactorButton_Clicked -= EnableZeroG;
    }
    public void Move(Vector2 direction)
    {
        if (direction.x != 0 || direction.y != 0)
        {
            Vector3 cameraUp = mainCameraTransform.up;
            cameraUp.z = 0;
            cameraUp.Normalize();
            Vector3 cameraRight = mainCameraTransform.right;
            cameraRight.z = 0;
            cameraRight.Normalize();

            Vector3 movement = (cameraUp * direction.y + cameraRight * direction.x);
            movement = Vector3.ClampMagnitude(movement, 1f);

            float currentSpeed = currentMovementState == movementState.Sprinting ? sprintSpeed : moveSpeed;
            rb.AddForce(movement * currentSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed * currentSpeed);

        }
        if (!spriteCanRotate && transform.rotation.z != 0)
        {
            transform.rotation = originalRotation;
        }
        UpdateMovementState();
    }

    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
        switch (sprinting)
        {
            case true:
                currentMovementState = movementState.Sprinting; 
                break;
            case false: currentMovementState = movementState.Walking; 
                break;

        }
    }

    public void EnableZeroG()
    {
        rb.drag = 0;
        spriteCanRotate = true;
        GetComponent<AudioSource>().enabled = false; ;
    }

    public void UpdateMovementState()
    {
        if(rb.velocity.magnitude > 0.3  && !isSprinting)
        {
            currentMovementState = movementState.Walking;
        }
        else if (isSprinting )
        {
            currentMovementState = movementState.Sprinting;
        }
        else if(rb.velocity.magnitude <  0.2 && currentMovementState != movementState.Idle)
        {
            currentMovementState = movementState.Idle;
        }
    }
}
