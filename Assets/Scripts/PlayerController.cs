using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    // Start is called before the first frame update
    void Start()
    {
        playerState = State.Sleeping;
        
        rb= GetComponent<Rigidbody2D>();
        FOVLight = GameObject.FindGameObjectWithTag("FOV");

        FOVLight.SetActive(false);
        aroundPlayerLight.SetActive(false);



    }

    public void PlayerDied()
    {
        playerState = State.Dead;
        UIController.Instance.ShowGameOver(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(playerState == State.Sleeping && Input.GetKeyDown(KeyCode.E))
        {
            
                playerState = State.Alive;
                UIController.Instance.HideGameOver();
            FOVLight.SetActive(true);
            aroundPlayerLight.SetActive(true);
            
        }
        if(playerState == State.Dead && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene("SampleScene");
        }
        if (playerState == State.Alive)
        {
            HandleInput();
            HandleAim();
            if(transform.rotation.z != 0f)
            {
                transform.eulerAngles = new Vector3 (0,0,0);
            }
            if(rb.velocity.magnitude > 0.1)
            {
                GetComponent<PlayerSounds>().PlayFootstepSound(isSprinting);
            }
            else if(rb.velocity.magnitude < 0.1)
            {
                GetComponent<PlayerSounds>().StopSound();
            }
        }
        else if(playerState == State.Dead)
        {
            rb.velocity = Vector2.zero;
            GetComponent<PlayerSounds>().StopSound();
        }



    }

    private void HandleAim()
    {
        Vector3 aimDir = (UtilsClass.GetMouseWorldPosition() - transform.position ).normalized;
       
            
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
           
            FOVLight.gameObject.transform.rotation = Quaternion.AngleAxis(UtilsClass.GetAngleFromVector(aimDir) - fov / 2, Vector3.forward);
        
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        

        Vector3 movement = new Vector3(horizontal, vertical, 0f);
        movement = Vector3.ClampMagnitude(movement, 1f);

        if (!isSprinting)
        {
            rb.AddForce(movement * moveSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);


        }
        else if (isSprinting)
        {
            rb.AddForce(movement * moveSpeed*sprintSpeed);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed*sprintSpeed);
        }
         else if (isCrouching)
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

    public void ChangeFOVStatus(bool status)
    {
        FOVLight.SetActive(status);
    }

    public float GetVisibility()
    {
        float visibility = baseVisibility;

        // Check if the player is in a lit area
        bool isLit = Physics2D.OverlapCircle(transform.position, 5 , litLayer);
        if (isLit)
        {
            visibility =1f;
        }
        else
        {
            visibility = 0.5f;
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
}
