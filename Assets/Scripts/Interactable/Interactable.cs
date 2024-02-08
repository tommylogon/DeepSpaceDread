using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] public string interactText = "Press E to interact";

    [SerializeField] private InteractionModule[] interactionModules;

    
    [SerializeField] protected bool closeEnoughToInteract = false;
    
    

    protected PlayerController playerRef;

    





    protected virtual void Start()
    {
        

        

       
    }

 

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeEnoughToInteract = true;
            UIController.Instance.ShowInteraction(interactText);
            playerRef = other.GetComponent<PlayerController>();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeEnoughToInteract = false;
            UIController.Instance.HideInteraction();
            UIController.Instance.HideMessageAfterDelay(10f);
            playerRef = null;
        }
    }
  
    public void ShowInteractablePrompt()
    {
        if (!closeEnoughToInteract)
        {

            UIController.Instance.ShowInteraction(interactText+", Too Far away."  );
        }
        else
        {
            UIController.Instance.ShowInteraction(interactText);
        }
        
    }



    public virtual void Interact()
    {
        if(interactionModules.Length > 0)
        {
            for (int i = 0; i < interactionModules.Length; i++)
            {
                interactionModules[i].Interact();
            }
        }
        else
        {
            Debug.Log(gameObject.name + " at " + transform.position + " Does not have any interaction modules");
        }

    }
   

    
   

}


