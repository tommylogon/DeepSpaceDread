using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableChildCollider : MonoBehaviour
{
    private ThrowableObject throwableObject;

    private void Awake()
    {
        throwableObject = GetComponentInParent<ThrowableObject>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (throwableObject != null)
        {
            throwableObject.HandleChildCollision(collision);
        }
    }
}
