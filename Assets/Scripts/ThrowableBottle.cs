using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBottle : ThrowableObject
{
    [SerializeField] private GameObject brokenBottlePrefab;

    public override void HandleImpact(Collision2D collision)
    {
        base.HandleImpact(collision);
        // Specific behavior for the bottle
        BreakBottle();
    }

    private void BreakBottle()
    {
        // Instantiate broken bottle prefab
        if(brokenBottlePrefab != null)
        {
            GameObject brokenBottle = Instantiate(brokenBottlePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Still missing the broken bottles here m8");
        }
        
        // Destroy original bottle game object
        Destroy(gameObject);
    }
}
