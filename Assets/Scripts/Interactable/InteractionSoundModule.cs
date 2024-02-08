using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSoundModule : InteractionModule
{
    protected AudioSource soundSource;
    [SerializeField] private List<string> SoundKeys;
    [SerializeField] private List<AudioClip> soundValues;

    private Dictionary<string, AudioClip> interactionSounds;
    [SerializeField] public float noiseRadius;

    // Start is called before the first frame update
    void Start()
    {
        soundSource = GetComponent<AudioSource>();
        interactionSounds = new Dictionary<string, AudioClip>();
        for (int i = 0; i < Mathf.Min(SoundKeys.Count, soundValues.Count); i++)
        {
            interactionSounds[SoundKeys[i]] = soundValues[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public override void Interact()
    {
        base.Interact();
        
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
}
