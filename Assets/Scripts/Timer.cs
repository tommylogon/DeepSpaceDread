using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public static Timer instance;

    public event EventHandler OnTimeOut;

    [SerializeField] private float initialTime = 60f;

    [SerializeField] private TextMeshProUGUI timerText = null;

    private static float timeLeft = 0f;
    private static bool isTimeOut = false;

    private static bool isPaused = false;

    private void Awake()
    {
        instance = this;

    }
    void Start()
    {


        isPaused=false; 
        isTimeOut=false;
        timeLeft = instance.initialTime;


    }


    void Update()
    {
        if (isTimeOut || isPaused)
        {
            return;
        }
        timeLeft -= Time.deltaTime;
        ;

        timerText.text = FormatTime();


        if (timeLeft <= 0)
        {
            isTimeOut = true;

            OnTimeOut?.Invoke(this, EventArgs.Empty);

        }


    }

    public string FormatTime()
    {
        return FormatTime(timeLeft);
    }

    public static string FormatTime(float timeToFormat)
    {
        int minutes = Mathf.FloorToInt(timeToFormat / 60);
        int seconds = Mathf.FloorToInt(timeToFormat % 60);
        int milliseconds = Mathf.FloorToInt((timeToFormat - Mathf.Floor(timeToFormat)) * 1000f);
        if(timeLeft <= 0)
        {
            return "00:00:000";
        }
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public void AddTime(float timeToAdd) 
    { 
        timeLeft += timeToAdd;
        timerText.text = FormatTime();
    }
    public void RemoveTime(float timeToRemove) { timeLeft -= timeToRemove; }

    public bool IsTimedOut() { return isTimeOut; }

    public void PauseTimer(bool state) { isPaused = state; }


    public float GetTimeLeft() { return timeLeft; }

    public void SetInitialTime(float time) { initialTime = time; }



}
