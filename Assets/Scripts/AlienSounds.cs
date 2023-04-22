using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSounds : MonoBehaviour
{
    [SerializeField] private AudioClip playerSpottedSound;
    [SerializeField] private AudioClip walkingSound;
    [SerializeField] private AudioClip stoppingSound;
    [SerializeField] private AudioClip eatingSound;
    [SerializeField] private AudioClip attackSound;

    [SerializeField] private bool soundIsPlaying;
    [SerializeField] private bool walkingSoundIsPlaying;

    [SerializeField] private AudioSource audioSourceWalking;
    [SerializeField] private AudioSource audioSourceAction;


    public void PlayPlayerSpottedSound()
    {
        if(!audioSourceAction.isPlaying)
        {
            audioSourceAction.PlayOneShot(playerSpottedSound);

        }
        
           
    }

    public void ChangeWalkingSound(float volumeModifier)
    {

        audioSourceWalking.volume = Mathf.Clamp01(volumeModifier);
            if (!audioSourceWalking.isPlaying)
            {

               // audioSourceWalking.Play();
            }
        
        
    }

    public void StopAllSound()
    {
        audioSourceAction.Stop();
        soundIsPlaying = false;
    }

    public void PlayEatingSound()
    {
        if (!soundIsPlaying)
        {

            audioSourceAction.PlayOneShot(eatingSound);
        }
        
    }

    public void PlayAttackSound()
    {
        if (!soundIsPlaying)
        {
            soundIsPlaying = true;
            audioSourceAction.PlayOneShot(attackSound);
        }
        
        

    }
    public void PlayStopSound()
    {
        if (!soundIsPlaying)
        {
            soundIsPlaying = true;
            audioSourceAction.PlayOneShot(stoppingSound);
        }
        
    }

    public bool IsPlaying() { return soundIsPlaying; }
}
