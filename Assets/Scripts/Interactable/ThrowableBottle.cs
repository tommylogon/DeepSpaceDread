using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBottle : ThrowableObject
{
    [SerializeField] private GameObject[] brokenBottlePrefab;
    [SerializeField] private bool isSetToDestroy = false;
    private float timer;

    private void Update()
    {
       if(isSetToDestroy)
        {
            if(GetComponent<SpriteRenderer>().enabled)
            GetComponent<SpriteRenderer>().enabled = false;

            if(timer > 4)
            {
                Destroy(gameObject);
            }
            timer += Time.deltaTime;
        }
    }
    public override void HandleImpact(Collider2D collider)
    {
        if (!isSetToDestroy)
        {
            base.HandleImpact(collider);
            // Specific behavior for the bottle
            BreakBottle();

            if (collider.gameObject.CompareTag("Enemy"))
            {
                HandleChildCollision(collider);
            }
        }
        
    }

    private void BreakBottle()
    {
        // Instantiate broken bottle prefab
        if (brokenBottlePrefab != null && brokenBottlePrefab.Length > 0)
        {
            for (int i = 0; i < brokenBottlePrefab.Length; i++)
            {
                GameObject brokenBottle = Instantiate(brokenBottlePrefab[i], transform.position, Quaternion.identity);

                // Apply random force and torque to the broken piece
                Rigidbody2D brokenPieceRb = brokenBottle.GetComponent<Rigidbody2D>();
                if (brokenPieceRb != null)
                {
                    float force = Random.Range(2f, 5f); // Adjust the range based on desired scattering strength
                    Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    brokenPieceRb.AddForce(randomDirection * force, ForceMode2D.Impulse);
                    brokenPieceRb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            Debug.Log("Still missing the broken bottles here m8");
        }


        // Destroy original bottle game object
        isSetToDestroy = true;
    }
}
