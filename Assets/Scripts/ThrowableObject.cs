using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableObject : Interactable
{
    private Rigidbody2D rb;

    [SerializeField] private AudioClip impactSound;

   [SerializeField] private float impactSoundRadius;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void HandleImpact(Collision2D collision)
    {
        if (soundSource != null && impactSound != null)
        {
            soundSource.PlayOneShot(impactSound);
        } 

        if(player == null)
        {
            player = player = GameObject.FindGameObjectWithTag("Player");
        }
        player.GetComponent<PlayerController>().GenerateNoise(transform.position, impactSoundRadius,1f);
    }

    public override void Interact()
    {
        
        base.Interact();
        // Handle object pickup and add to player's inventory
        player.GetComponent<PlayerController>().AddToInventory(gameObject);
        
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else
        {
            HandleImpact(collision);
        }
    }

    public void Throw(Vector2 throwDirection, float throwForce, Vector3 throwPosition)
    {
        transform.position = throwPosition;
        gameObject.SetActive(true);
               
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
    }

    public void HandleChildCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
        else
        {
            HandleImpact(collision);
        }
    }
}


