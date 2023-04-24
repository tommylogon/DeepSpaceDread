using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactText = "Press E to interact";
    [SerializeField] internal string message = "";
    [SerializeField] protected bool canInteract = false;

    protected GameObject player;

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
            StartCoroutine(HideMessageAfterDelay(5));
            player = null;
        }
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (canInteract)
        {
            UIController.Instance.HideMessage();
        }
    }

public virtual void Interact()
    {
    }
}
