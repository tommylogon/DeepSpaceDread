using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;

    private AudioSource audioSource;
    private float walkSpeed = 1.0f;
    private float runSpeed = 2.0f;
    private float jumpVolume = 1.0f;
    private float landVolume = 1.0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstepSound(bool isRunning)
    {

        AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.clip = footstepSound;
        if (!audioSource.isPlaying)
        {
            if (isRunning)
            {
                audioSource.pitch = runSpeed;
            }
            else
            {
                audioSource.pitch = walkSpeed;
            }

            audioSource.Play();
        }
        
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void PlayJumpSound()
    {
        audioSource.clip = jumpSound;
        audioSource.volume = jumpVolume;
        audioSource.Play();
    }

    public void PlayLandSound()
    {
        audioSource.clip = landSound;
        audioSource.volume = landVolume;
        audioSource.Play();
    }
}
