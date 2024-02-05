using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSpriteModule : InteractionModule
{
    public Sprite initialImage;
    public Sprite finalImage;
    public Animator animator;
    public float animationDuration = 1f;
    public float waitTime = 1f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Interact()
    {
        if(animator != null)
        {
            TriggerNextAnimation();
            
            StartCoroutine(TriggerAnimationAfterTime());
        }
        

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            TriggerExitAnimation();
        }
    }

    private void TriggerExitAnimation()
    {
        if(animator != null)
        {
            TriggerNextAnimation();
            spriteRenderer.sortingOrder--;

        }
    }

    //private void Start()
    //{
    //    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
    //    if (spriteRenderer != null)
    //    {
    //        spriteRenderer.sprite = initialImage;
    //    }
    //    else
    //    {
    //        Debug.LogError("SpriteRenderer component not found!");
    //    }
    //}

    public IEnumerator TriggerAnimationAfterTime()
    {
        //todo, add timer, check if it is used and trigger next animation
        yield return new WaitForSeconds(animationDuration);
    }


    private void TriggerNextAnimation()
    {
        animator.SetTrigger("NextState");
    } 
   
}
