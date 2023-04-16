using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager 
{

    public static float volume;
    public enum Sound
    {
        PlayerMove,
        Interact,
        AlienMove,
        AlienAttack,
        PlayerDeath,
        ButtonClick,
        ButtonHover,


    }

    public static void PlaySound(Sound sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        AudioClip clip = GetAudioClip(sound);
        audioSource.PlayOneShot(clip,volume);
        if(GameHandler.instance != null)
        {
            MonoBehaviour monoBehaviour = GameHandler.instance;
            monoBehaviour.StartCoroutine(DestroySoundAfterTime(soundGameObject, clip.length));
        }
        
    }

    private static IEnumerator DestroySoundAfterTime(GameObject soundGameObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Object.Destroy(soundGameObject);
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach(GameAssets.SoundAudioClip soundAudioClip in GameAssets.instance.soundAudioClipArray)
        {
            if(soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        Debug.LogError("Sound " + sound + " not found in assets!");
        return null;
    }

    public static void AddButtonSounds(this Button_UI buttonUI)
    {
        buttonUI.MouseOverOnceFunc += () => PlaySound(Sound.ButtonHover);
        buttonUI.ClickFunc += () => PlaySound(Sound.ButtonClick);
    }
}
