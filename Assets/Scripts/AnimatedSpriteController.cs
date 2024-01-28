using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSpriteController : MonoBehaviour
{
    public Sprite initialImage;
    public Sprite finalImage;
    public float animationDuration = 1f;
    public float waitTime = 1f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = initialImage;
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found!");
        }
    }

    public void TriggerAnimation()
    {
        
    }

  

   
}
