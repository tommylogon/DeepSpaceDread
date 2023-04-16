using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    [SerializeField] private  bool RunGame;
   



    private void Awake()
    {


        instance = this;
     




    }

   

    void Start()
    {
        Debug.Log("Gamehandler.start");


       

    }

    private void Update()
    {
        
    }

    
}