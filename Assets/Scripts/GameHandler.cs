using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    [SerializeField] private  bool RunGame;
    public delegate void NoiseGeneratedEventHandler(Vector2 noiseOrigin, float noiseRadius);
    public event NoiseGeneratedEventHandler OnNoiseGenerated;

    public enum GameState
    {
        Menu,
        Game, 
        GameOver,
        Finish
    }

    private void Awake()
    {


        instance = this;
     




    }

   

    void Start()
    {
        Debug.Log("Gamehandler.start");


       

    }

    public void GenerateNoise(Vector2 noiseOrigin, float noiseRadius, float hearingChance)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);
        if (randomChance <= hearingChance)
        {
            OnNoiseGenerated?.Invoke(noiseOrigin, noiseRadius);
        }
    }

}