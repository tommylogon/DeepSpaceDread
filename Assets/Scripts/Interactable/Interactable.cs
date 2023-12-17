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
    [SerializeField] protected bool playRandomMessage;
    [SerializeField] public float noiseRadius;

    protected PlayerController player;

    protected AudioSource soundSource;
    [SerializeField] private List<string> SoundKeys;
    [SerializeField] private List<AudioClip> soundValues;

    [SerializeField] protected string questInfo = "";
    [SerializeField] private bool saves = false;

    private Dictionary<string, AudioClip> interactionSounds;


    protected virtual void Start()
    {
        soundSource = GetComponent<AudioSource>();
        if (messageDatabase != null && messageKeys.Count > 0)
        {
            messages = messageDatabase.GetMessages(messageKeys);
        }
        interactionSounds = new Dictionary<string, AudioClip>();
        for (int i = 0; i < Mathf.Min(SoundKeys.Count, soundValues.Count); i++)
        {
            interactionSounds[SoundKeys[i]] = soundValues[i];
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            closeEnoughToInteract = true;
            UIController.Instance.ShowInteraction(interactText);
            player = other.GetComponent<PlayerController>();
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
        PlaySound("DefaultSound");
    }
    protected void PlaySound(string soundKey)
    {
        if (interactionSounds.TryGetValue(soundKey, out AudioClip clip))
        {
            soundSource.PlayOneShot(clip);
            GameHandler.instance.GenerateNoise(transform.position, noiseRadius, 1);
        }
    }

    protected void PlayMessage(int messageIndex = 0, int messageRange = 1)
    {
        if (closeEnoughToInteract)
        {
            string selectedMessage = "It's Empty...";
            if (messages.Count > 0 && messages[0] != "")
            {
                if (playRandomMessage || messageRange>1)
                {
                    
                    if (messages.Count > 1)
                    {
                        selectedMessage = messages[Random.Range(0, messageRange)];
                        if(selectedMessage == "")
                        {
                            selectedMessage = "Ugh...";
                        }
                    }

                }
                else
                {
                    selectedMessage = messages[messageIndex];
                }
            }            
            UIController.Instance.ShowMessage(selectedMessage);

            if (saves)
            {
                UIController.Instance.SaveToMemory(questInfo);
            }
            
        }
    }
}


