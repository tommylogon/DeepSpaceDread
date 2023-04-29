using System;
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
    [SerializeField] private AudioClip alienEmergeSound;

    [SerializeField] private bool soundIsPlaying;
    [SerializeField] private bool walkingIsActive;

    [SerializeField] private AudioSource audioSourceWalking;
    [SerializeField] private AudioSource audioSourceAction;

    public void PlayPlayerSpottedSound()
    {
        if (!audioSourceAction.isPlaying)
        {
            soundIsPlaying = true;
            
            audioSourceAction.PlayOneShot(playerSpottedSound);
        }
    }

    public void ChangeActionVolume(float volume)
    {
        audioSourceAction.volume = Mathf.Clamp01(volume);

    }

    public void ChangeWalkingVolume(float volumeModifier)
    {
        audioSourceWalking.volume = Mathf.Clamp01(volumeModifier);
        if (walkingIsActive && !audioSourceWalking.isPlaying)
        {
            audioSourceWalking.Play();
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
            soundIsPlaying = true;
            audioSourceAction.PlayOneShot(eatingSound);
        }
    }

    public void StopEatingSound()
    {
        audioSourceAction.Stop();
        soundIsPlaying = false;
    }

    public void PlayAttackSound()
    {
        if (!soundIsPlaying)
        {
            soundIsPlaying = true;
            audioSourceAction.PlayOneShot(attackSound);
        }
    }

    public void PlayAlienEmergeSound()
    {
        if (!soundIsPlaying)
        {
            soundIsPlaying = true;
            audioSourceAction.PlayOneShot(alienEmergeSound);
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

    internal void StartWalkingSound()
    {
        if (!walkingIsActive)
        {
            walkingIsActive = true;
            audioSourceWalking.Play();
        }
    }

    internal void StopWalingSound()
    {
        walkingIsActive = false;
        audioSourceWalking.Stop();
    }
}