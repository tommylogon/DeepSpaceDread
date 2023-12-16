using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] public string interactText = "Press E to interact";
    public List<string> messages;
    public List<string> messageKeys;
    public MessageDatabase messageDatabase;
    [SerializeField] protected bool closeEnoughToInteract = false;
    [SerializeField] public float noiseRadius;

    protected GameObject player;

    protected AudioSource soundSource;

    public AudioClip interactionClip;


    protected virtual void Start()
    {
        soundSource = GetComponent<AudioSource>();
        if (messageDatabase != null && messageKeys.Count > 0)
        {
            messages = messageDatabase.GetMessages(messageKeys);
        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeEnoughToInteract = true;
            UIController.Instance.ShowInteraction(interactText);
            player = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeEnoughToInteract = false;
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
