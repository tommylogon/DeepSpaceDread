using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThrowableObject : Interactable
{
    private Rigidbody2D rb;

    [SerializeField] private AudioClip impactSound;

   [SerializeField] private float impactSoundRadius;
    [SerializeField] private bool disableOnHit;

    

    public State targetState;

    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void HandleImpact(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            AIController aiController;
            if (collider.gameObject.TryGetComponent<AIController>(out aiController))
            {
                aiController.HandleThrowableObjectHit(collider, targetState);
            }
            Vector2 bounceDirection = (transform.position - collider.transform.position).normalized;
            float bounceForce = 2f; 
            rb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);

        }
        if (soundSource != null && impactSound != null)
        {
            soundSource.PlayOneShot(impactSound);
        }         
        
        GameHandler.instance.GenerateNoise(transform.position, impactSoundRadius,1f);
    }

    public override void Interact() 
    {
        
        base.Interact();
       
        playerRef.GetComponent<PlayerController>().AddToInventory(gameObject);

        transform.SetParent(playerRef.GetComponent<PlayerController>().GetHoldingPoint());
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        spriteRenderer.sortingOrder = 4;

    
       }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else
        {
            HandleImpact(collision.collider);
        }
        DisableOnHit();
    }

    private void DisableOnHit()
    {
        if(disableOnHit) 
        { 
            gameObject.SetActive(false);
        }
    }

    public void Throw(Vector2 throwDirection, float throwForce, Vector3 throwPosition)
    {
        transform.position = throwPosition;
        gameObject.SetActive(true);
               
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        spriteRenderer.sortingOrder = 0;
    }

    public void HandleChildCollision(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>());
        }
        else
        {
            HandleImpact(collider);

            
           

        }
    }

}


