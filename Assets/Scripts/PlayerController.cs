using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static bool godMode;
    
    public State playerState;

    public event Action OnDeath;

    // Start is called before the first frame update
    void Start()
    {
        playerState = State.Alive;
        
        
        
    }

    private void PlayerController_OnDeath()
    {
        playerState = State.Dead;
        OnDeath?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == State.Alive)
        {

            
        }



    }

    



}
