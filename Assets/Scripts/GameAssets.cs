using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
   //This is the asset manager, it holds sprites, sounds, and other assets that are used in the game.
     

    
    public static GameAssets instance;
    public Sprite[] BackgroundSpriteArray; 
    public Sprite[] spaceshipSpriteArray;
    public Sprite[] asteroidSpritesArray;
    public Sprite[] stationSpriteArray; 
    public Sprite uncompletedSprite;


    public GameObject[] ShipsPrefabs;
    public GameObject PlayerFollowPrefab;

    public SoundAudioClip[] soundAudioClipArray;

    
    public float volume;
  




    private void Awake()
    {
        instance = this;

        
    }

    private void Update()
    {
        SoundManager.volume = volume;
    }





    [Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }

    


    

}
