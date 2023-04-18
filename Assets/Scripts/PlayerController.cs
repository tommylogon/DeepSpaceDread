using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool godMode;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float crouchSpeed = .2f;
    [SerializeField] private float sprintSpeed = 3f;
    [SerializeField] private float maxSpeed = 1f;

    [SerializeField] private State playerState;
    public bool isCrouching;
    public bool isSprinting;
    [SerializeField] private float fov =180;
    //[SerializeField] FieldOfView fov;
    [SerializeField] GameObject Light;

    public event Action OnDeath;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        playerState = State.Alive;
        
        rb= GetComponent<Rigidbody2D>();


    }

    private void PlayerController_OnDeath()
    {
        playerState = State.Dead;
        OnDeath?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == State.Alive)
        {
            HandleInput();
            HandleAim();
            if(transform.rotation.z != 0f)
            {
                transform.eulerAngles = new Vector3 (0,0,0);
            }
        }



    }

    private void HandleAim()
    {
        Vector3 aimDir = (UtilsClass.GetMouseWorldPosition() - transform.position ).normalized;
        //fov.SetOrigin(transform.position);
        //fov.SetAimDirection(aimDir);

       
            
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
           
            Light.gameObject.transform.rotation = Quaternion.AngleAxis(UtilsClass.GetAngleFromVector(aimDir) - fov / 2, Vector3.forward);
        
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        movement = Vector3.ClampMagnitude(movement, 1f);

        rb.AddForce(movement * moveSpeed);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);

        if (isSprinting)
        {
            rb.AddForce(movement * moveSpeed*sprintSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed*sprintSpeed);
        }
        if (isCrouching)
        {
            
            rb.AddForce(movement * moveSpeed * crouchSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed *crouchSpeed);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = !isSprinting;
            isCrouching = false;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            isSprinting = false;
        }
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            Debug.Log("Interact");
        }



    }
    



}
