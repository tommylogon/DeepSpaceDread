using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] public string interactText = "Press E to interact";
    [SerializeField] public string message = "";
    [SerializeField] public bool canInteract = false;
    [SerializeField] public float noiseRadius;

    protected GameObject player;

    protected AudioSource soundSource;

    public AudioClip interactionClip;


    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
       //
    }

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
        if (interactionClip != null)
        {
            soundSource.PlayOneShot(interactionClip);
            player.GetComponent<PlayerController>().GenerateNoise(transform.position, noiseRadius, 1);
        }
    }
}
