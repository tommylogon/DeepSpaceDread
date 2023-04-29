using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactText = "Press E to interact";
    [SerializeField] internal string message = "";
    [SerializeField] protected bool canInteract = false;

    protected GameObject player;

    // Declare a dictionary to store coroutines for each instance
    private static Dictionary<Interactable, Coroutine> hideMessageCoroutines = new Dictionary<Interactable, Coroutine>();

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            UIController.Instance.ShowInteraction(interactText);
            player = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            UIController.Instance.HideInteraction();
            UIController.Instance.HideMessageAfterDelay(10f);
            player = null;
        }
    }

   

public virtual void Interact()
    {
    }
}
