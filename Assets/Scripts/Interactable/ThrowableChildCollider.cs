using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableChildCollider : MonoBehaviour
{
    private ThrowableObject throwableObject;
    public int lightLayer;

    private void Awake()
    {
        throwableObject = GetComponentInParent<ThrowableObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (throwableObject != null)
        {
            throwableObject.HandleChildCollision(collision.collider);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (throwableObject != null && collider.gameObject.layer != lightLayer)
        {
            throwableObject.HandleChildCollision(collider);
        }
    }
}
